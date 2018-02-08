using System.Data.JsonRpc.Resources;
using System.Globalization;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC message identifier.</summary>
    public readonly struct JsonRpcId : IEquatable<JsonRpcId>, IComparable<JsonRpcId>
    {
        /// <summary>Gets an empty identifier.</summary>
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

        /// <summary>Compares the current <see cref="JsonRpcId" /> with another <see cref="JsonRpcId" /> and returns an integer that indicates whether the current <see cref="JsonRpcId" /> precedes, follows, or occurs in the same position in the sort order as the other <see cref="JsonRpcId" />.</summary>
        /// <param name="other">A <see cref="JsonRpcId" /> to compare with the current <see cref="JsonRpcId" />.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(JsonRpcId other)
        {
            switch (other.Type)
            {
                case JsonRpcIdType.Number:
                    {
                        switch (Type)
                        {
                            case JsonRpcIdType.Number:
                                {
                                    return _valueNumber.CompareTo(other._valueNumber);
                                }
                            case JsonRpcIdType.String:
                                {
                                    return +1;
                                }
                            default:
                                {
                                    return -1;
                                }
                        }
                    }
                case JsonRpcIdType.String:
                    {
                        switch (Type)
                        {
                            case JsonRpcIdType.Number:
                                {
                                    return -1;
                                }
                            case JsonRpcIdType.String:
                                {
                                    return _valueString.CompareTo(other._valueString);
                                }
                            default:
                                {
                                    return -1;
                                }
                        }
                    }
                default:
                    {
                        switch (Type)
                        {
                            case JsonRpcIdType.None:
                                {
                                    return +0;
                                }
                            default:
                                {
                                    return +1;
                                }
                        }
                    }
            }
        }

        /// <summary>Indicates whether the current <see cref="JsonRpcId" /> is equal to another <see cref="JsonRpcId" />.</summary>
        /// <param name="other">A <see cref="JsonRpcId" /> to compare with the current <see cref="JsonRpcId" />.</param>
        /// <returns><see langword="true" /> if the current <see cref="JsonRpcId" /> is equal to the other <see cref="JsonRpcId" />; otherwise, <see langword="false" />.</returns>
        public bool Equals(JsonRpcId other)
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

        /// <summary>Indicates whether the current <see cref="JsonRpcId" /> is equal to the specified object.</summary>
        /// <param name="obj">The object to compare with the current <see cref="JsonRpcId" />.</param>
        /// <returns><see langword="true" /> if the current <see cref="JsonRpcId" /> is equal to the specified object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case JsonRpcId other:
                    {
                        return Equals(other);
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

        /// <summary>Returns the hash code for the current <see cref="JsonRpcId" />.</summary>
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

        /// <summary>Converts the current <see cref="JsonRpcId" /> to its equivalent string representation.</summary>
        /// <returns>The string representation of the current <see cref="JsonRpcId" />.</returns>
        public override string ToString()
        {
            switch (Type)
            {
                case JsonRpcIdType.Number:
                    {
                        return _valueNumber.ToString(CultureInfo.InvariantCulture);
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

        /// <summary>Indicates whether the left <see cref="JsonRpcId" /> is equal to the right <see cref="JsonRpcId" />.</summary>
        /// <param name="obj1">The left <see cref="JsonRpcId" /> operand.</param>
        /// <param name="obj2">The right <see cref="JsonRpcId" /> operand.</param>
        /// <returns><see langword="true" /> if the left <see cref="JsonRpcId" /> is equal to the right <see cref="JsonRpcId" />; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(JsonRpcId obj1, JsonRpcId obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>Indicates whether the left <see cref="JsonRpcId" /> is not equal to the right <see cref="JsonRpcId" />.</summary>
        /// <param name="obj1">The left <see cref="JsonRpcId" /> operand.</param>
        /// <param name="obj2">The right <see cref="JsonRpcId" /> operand.</param>
        /// <returns><see langword="true" /> if the left <see cref="JsonRpcId" /> is not equal to the right <see cref="JsonRpcId" />; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(JsonRpcId obj1, JsonRpcId obj2)
        {
            return !obj1.Equals(obj2);
        }

        /// <summary>Performs an implicit conversion from <see cref="ulong" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" /> from.</param>
        public static implicit operator JsonRpcId(long value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" /> from.</param>
        public static implicit operator JsonRpcId(string value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="JsonRpcId" /> to <see cref="long" />.</summary>
        /// <param name="value">The identifier to get a <see cref="long" /> value from.</param>
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
        /// <param name="value">The identifier to get a <see cref="string" /> value from.</param>
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