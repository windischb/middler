using System;

namespace middler.Scripting.HttpCommand
{
    internal class ReadonlyHttpHandlerOptions : IEquatable<ReadonlyHttpHandlerOptions>
    {
        public string DestinationHost { get; }
        public string ProxyHost { get; }
        public bool IgnoreProxy { get; }

        public ReadonlyHttpHandlerOptions(HttpHandlerOptions httpHandlerOptions)
        {
            DestinationHost = httpHandlerOptions.RequestUri?.Host;
            ProxyHost = httpHandlerOptions.Proxy?.Address?.Host;
            IgnoreProxy = httpHandlerOptions.IgnoreProxy;
        }


        public static bool operator ==(ReadonlyHttpHandlerOptions left, ReadonlyHttpHandlerOptions right) => Equals(left, right);

        public static bool operator !=(ReadonlyHttpHandlerOptions left, ReadonlyHttpHandlerOptions right) => !Equals(left, right);

        public bool Equals(ReadonlyHttpHandlerOptions other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(DestinationHost, other.DestinationHost) &&
                   Equals(ProxyHost, other.ProxyHost) &&
                   Equals(IgnoreProxy, other.IgnoreProxy);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReadonlyHttpHandlerOptions)obj);
        }

        public override int GetHashCode()
        {
            return (DestinationHost, ProxyHost, IgnoreProxy).GetHashCode();
        }
    }
}