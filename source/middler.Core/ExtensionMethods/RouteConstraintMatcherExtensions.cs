using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace middler.Core.ExtensionMethods
{
    public static class MiddlerRouteConstraintMatcher
    {

        public static bool Match(
            IDictionary<string, IRouteConstraint> constraints,
            RouteValueDictionary routeValues,
            IRouter route,
            RouteDirection routeDirection,
            ILogger logger)
        {
            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (constraints == null || constraints.Count == 0)
            {
                return true;
            }

            foreach (var kvp in constraints)
            {
                var constraint = kvp.Value;
                if (!constraint.Match(null, route, kvp.Key, routeValues, routeDirection))
                {
                    if (routeDirection.Equals(RouteDirection.IncomingRequest))
                    {
                        routeValues.TryGetValue(kvp.Key, out var routeValue);

                        //logger.ConstraintNotMatched(routeValue, kvp.Key, kvp.Value);
                    }

                    return false;
                }
            }

            return true;
        }
    }
    
}
