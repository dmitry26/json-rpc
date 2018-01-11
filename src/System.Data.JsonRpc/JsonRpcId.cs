using System.Data.JsonRpc.Resources;
using System.Globalization;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC identifier.</summary>
    public readonly struct JsonRpcId
    {
        /// <summary>Represents not specified identifier.</summary>
        public static readonly JsonRpcId None = default;

        private readonly long _valueNumber;
        private readonly string _valueString;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        public JsonRpcId(long value)
        {
            _valueNumber = value;
            _valueString = default;

            Type = JsonRpcIdType.Number;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public JsonRpcId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _valueNumber = default;
            _valueString = value;

            Type = JsonRpcIdType.String;
        }

        /// <summary>Gets the identifier type.</summary>
        public JsonRpcIdType Type
        {
            get;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = (int)2166136261;

                switch (Type)
                {
                    case JsonRpcIdType.Number:
                        {
                            result = (result * 16777619) ^ _valueNumber.GetHashCode();
                        }
                        break;
                    case JsonRpcIdType.String:
                        {
                            result = (result * 16777619) ^ _valueString.GetHashCode();
                        }
                        break;
                }

                return result;
            }
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case JsonRpcId other:
                    {
                        switch (other.Type)
                        {
                            case JsonRpcIdType.Number:
                                {
                                    return (Type == JsonRpcIdType.Number) && _valueNumber.Equals(other._valueNumber);
                                }
                            case JsonRpcIdType.String:
                                {
                                    return (Type == JsonRpcIdType.String) && _valueString.Equals(other._valueString);
                                }
                            default:
                                {
                                    return (Type == JsonRpcIdType.None);
                                }
                        }
                    }
                case long other:
                    {
                        return (Type == JsonRpcIdType.Number) && _valueNumber.Equals(other);
                    }
                case string other:
                    {
                        return (Type == JsonRpcIdType.String) && _valueString.Equals(other);
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>Converts the value of this instance to its equivalent string representation.</summary>
        /// <returns>The string representation of the value of this instance.</returns>
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
                        return string.Empty;
                    }
            }
        }

        /// <summary>Overloads == operator.</summary>
        /// <param name="obj1">The left <see cref="JsonRpcId" /> operand.</param>
        /// <param name="obj2">The right <see cref="JsonRpcId" /> operand.</param>
        /// <returns>The result of == operation.</returns>
        public static bool operator ==(JsonRpcId obj1, JsonRpcId obj2)
        {
            return object.Equals(obj1, obj2);
        }

        /// <summary>Overloads != operator.</summary>
        /// <param name="obj1">The left <see cref="JsonRpcId" /> operand.</param>
        /// <param name="obj2">The right <see cref="JsonRpcId" /> operand.</param>
        /// <returns>The result of != operation.</returns>
        public static bool operator !=(JsonRpcId obj1, JsonRpcId obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>Performs an implicit conversion from <see cref="ulong" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" />.</param>
        public static implicit operator JsonRpcId(long value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" />.</param>
        public static implicit operator JsonRpcId(string value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="JsonRpcId" /> to <see cref="long" />.</summary>
        /// <param name="value">The value to create a <see cref="long" />.</param>
        /// <exception cref="InvalidCastException">The underlying value is not of type <see cref="long" />.</exception>
        public static explicit operator long(JsonRpcId value)
        {
            if (value.Type != JsonRpcIdType.Number)
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("id.invalid_cast"), typeof(JsonRpcId), typeof(long)));
            }

            return value._valueNumber;
        }

        /// <summary>Performs an implicit conversion from <see cref="JsonRpcId" /> to <see cref="string" />.</summary>
        /// <param name="value">The value to create a <see cref="string" />.</param>
        /// <exception cref="InvalidCastException">The underlying value is not of type <see cref="string" />.</exception>
        public static explicit operator string(JsonRpcId value)
        {
            if (value.Type != JsonRpcIdType.String)
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("id.invalid_cast"), typeof(JsonRpcId), typeof(string)));
            }

            return value._valueString;
        }
    }
}