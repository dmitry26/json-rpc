#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC identifier.</summary>
    public struct JsonRpcId
    {
        /// <summary>Represents not specified identifier.</summary>
        public static readonly JsonRpcId None = default(JsonRpcId);

        private readonly long _valueNumber;
        private readonly string _valueString;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        public JsonRpcId(long value)
        {
            _valueNumber = value;
            _valueString = null;

            Type = JsonRpcIdType.Number;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        public JsonRpcId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length == 0)
            {
                throw new ArgumentException("Value is an empty string", nameof(value));
            }

            _valueNumber = default(long);
            _valueString = value;

            Type = JsonRpcIdType.String;
        }

        public override int GetHashCode()
        {
            switch (Type)
            {
                case JsonRpcIdType.Number:
                    {
                        return _valueNumber.GetHashCode();
                    }
                case JsonRpcIdType.String:
                    {
                        return _valueString.GetHashCode();
                    }
                default:
                    {
                        return base.GetHashCode();
                    }
            }
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case JsonRpcId other:
                    {
                        if (Type != other.Type)
                        {
                            return false;
                        }

                        switch (Type)
                        {
                            case JsonRpcIdType.Number:
                                {
                                    return object.Equals(_valueNumber, other._valueNumber);
                                }
                            case JsonRpcIdType.String:
                                {
                                    return object.Equals(_valueString, other._valueString);
                                }
                            default:
                                {
                                    return true;
                                }
                        }
                    }
                case long other:
                    {
                        return object.Equals(this, new JsonRpcId(other));
                    }
                case string other:
                    {
                        return object.Equals(this, new JsonRpcId(other));
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case JsonRpcIdType.Number:
                    {
                        return _valueNumber.ToString();
                    }
                case JsonRpcIdType.String:
                    {
                        return _valueString;
                    }
                default:
                    {
                        return Type.ToString();
                    }
            }
        }

        public static bool operator ==(JsonRpcId obj1, JsonRpcId obj2)
        {
            return object.Equals(obj1, obj2);
        }

        public static bool operator !=(JsonRpcId obj1, JsonRpcId obj2)
        {
            return !(obj1 == obj2);
        }

        public static implicit operator JsonRpcId(long value) =>
            new JsonRpcId(value);

        public static implicit operator JsonRpcId(string value) =>
            new JsonRpcId(value);

        public static explicit operator long(JsonRpcId value) =>
            value.Type == JsonRpcIdType.Number ? value._valueNumber : throw new InvalidOperationException("Value is not a number");

        public static explicit operator string(JsonRpcId value) =>
            value.Type == JsonRpcIdType.String ? value._valueString : throw new InvalidOperationException("Value is not a string");

        /// <summary>Gets the identifier type.</summary>
        public JsonRpcIdType Type { get; }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member