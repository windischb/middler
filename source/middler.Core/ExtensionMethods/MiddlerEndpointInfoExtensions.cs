using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using middler.Common;
using middler.Common.SharedModels.Enums;
using middler.Common.SharedModels.Models;
using middler.Core.IPHelper;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace middler.Core.ExtensionMethods
{
    public static class MiddlerRuleExtensions
    {
       

        public static AccessMode? AccessAllowed(this MiddlerRule middlerMiddler,  IMiddlerRequestContext middlerRequestContext) {
            return AccessAllowed(middlerMiddler.Permissions, middlerRequestContext);
        }

        public static AccessMode? AccessAllowed(this IEnumerable<MiddlerRulePermission> permissionRules, IMiddlerRequestContext middlerRequestContext) {

            

            var roles = middlerRequestContext.Principal.Claims.Where(c => c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                            .Select(c => c.Value)
                            .ToList();

            var hasUser = !String.IsNullOrWhiteSpace(middlerRequestContext.Principal.Identity.Name);

            foreach (var permissionRule in permissionRules) {
                var inRange = SourceIpAddressIsInRange(middlerRequestContext.SourceIPAddress, permissionRule.SourceAddress);
                var isClient = IsCurrentClient(middlerRequestContext.Principal, permissionRule.Client);


                switch (permissionRule.Type) {
                    case PrincipalType.Everyone: {
                        if (inRange && isClient) {
                            return permissionRule.AccessMode;
                        }

                        break;
                    }
                    case PrincipalType.Authenticated: {
                        if (inRange && isClient && hasUser) {
                            return permissionRule.AccessMode;
                        }

                        break;
                    }
                    case PrincipalType.User: {

                        if (inRange && isClient && Wildcard.Match(middlerRequestContext.Principal.Identity.Name, permissionRule.PrincipalName.ToNull() ?? "")) {
                            return permissionRule.AccessMode;
                        }

                        break;
                    }
                    case PrincipalType.Role: {
                        if (isClient && inRange) {
                            foreach (var role in roles) {
                                if (Wildcard.Match(role, permissionRule.PrincipalName.ToNull() ?? "")) {
                                    return permissionRule.AccessMode;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return null;
        }


        private static bool SourceIpAddressIsInRange(this IPAddress sourceIp, string range) {

            if (String.IsNullOrWhiteSpace(range))
                range = "*";

            try {


                var ips = Regex.Split(range, @"[,|;]");

                foreach (var ip in ips) {

                    if (ip.Contains("*") || ip.Contains("?")) {
                        if (Wildcard.Match(sourceIp.ToString(), ip))
                            return true;

                    }

                    if (ip.Contains("/") || ip.Contains("-")) {
                        var net = IPAddressRange.Parse(ip);
                        if (net.Contains(sourceIp))
                            return true;
                    } else {
                        if (IPAddress.Parse(ip).Equals(sourceIp))
                            return true;
                    }
                }

                return false;
            } catch (Exception) {
                
                return false;
            }
        }

        private static bool IsCurrentClient(ClaimsPrincipal principal, string clientRange) {

            if (String.IsNullOrWhiteSpace(clientRange))
                return true;

            var currentClient = principal.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value;

            if (String.IsNullOrWhiteSpace(currentClient))
                return false;

            var clients = Regex.Split(clientRange, @"[,|;]").Select(c => c.Trim());
            foreach (var client in clients) {

                if (String.IsNullOrWhiteSpace(client))
                    return true;

                if (Wildcard.Match(currentClient, client, true))
                    return true;
            }

            return false;
        }
    }

    public class AccessAllowedResult {

        public bool AccessAllowed { get; }

        public string[] AdditionalRoles { get; } = new string[0];

        public AccessAllowedResult(bool accessAllowed, string additionalRoles = null) {
            AccessAllowed = accessAllowed;

            if (additionalRoles != null) {
                AdditionalRoles = additionalRoles
                    .Split(new[] {"\r\n", "\n", ";"}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim()).ToArray();
            }

        }

        public AccessAllowedResult(AccessMode accessMode, string additionalRoles = null) {
            AccessAllowed = accessMode == AccessMode.Allow;
            
            if (additionalRoles != null) {
                AdditionalRoles = additionalRoles
                    .Split(new [] { "\r\n", "\n", ";" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim()).ToArray();
            }

        }

        public AccessAllowedResult(MiddlerRulePermission rule) {
            AccessAllowed = rule.AccessMode == AccessMode.Allow;
        }

        public static implicit operator Boolean(AccessAllowedResult accessAllowedResult) {
            return accessAllowedResult.AccessAllowed;
        }
    }
}
