using System.Data.JsonRpc.Resources;
using System.Globalization;

namespace System.Data.JsonRpc
{
    /// <summary>Represents RPC message identifier.</summary>
    public readonly struct JsonRpcId : IEquatable<JsonRpcId>, IComparable<JsonRpcId>
    {
        private readonly string _valueString;
        private readonly long _valueInteger;
        private readonly double _valueFloat;

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public JsonRpcId(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _valueString = value;
            _valueInteger = default;
            _valueFloat = default;

            Type = JsonRpcIdType.String;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        public JsonRpcId(long value)
        {
            _valueString = default;
            _valueInteger = value;
            _valueFloat = default;

            Type = JsonRpcIdType.Integer;
        }

        /// <summary>Initializes a new instance of the <see cref="JsonRpcId" /> structure.</summary>
        /// <param name="value">The identifier value.</param>
        public JsonRpcId(double value)
        {
            _valueString = default;
            _valueInteger = default;
            _valueFloat = value;

            Type = JsonRpcIdType.Float;
        }

        bool IEquatable<JsonRpcId>.Equals(JsonRpcId other)
        {
            return Equals(in other);
        }

        int IComparable<JsonRpcId>.CompareTo(JsonRpcId other)
        {
            return CompareTo(in other);
        }

        /// <summary>Compares the current <see cref="JsonRpcId" /> with another <see cref="JsonRpcId" /> and returns an integer that indicates whether the current <see cref="JsonRpcId" /> precedes, follows, or occurs in the same position in the sort order as the other <see cref="JsonRpcId" />.</summary>
        /// <param name="other">A <see cref="JsonRpcId" /> to compare with the current <see cref="JsonRpcId" />.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(in JsonRpcId other)
        {
            switch (other.Type)
            {
                case JsonRpcIdType.String:
                    {
                        switch (Type)
                        {
                            case JsonRpcIdType.String:
                                {
                                    return _valueString.CompareTo(other._valueString);
                                }
                            case JsonRpcIdType.Integer:
                                {
                                    return +1;
                                }
                            case JsonRpcIdType.Float:
                                {
                                    return +1;
                                }
                            default:
                                {
                                    return -1;
                                }
                        }
                    }
                case JsonRpcIdType.Integer:
                    {
                        switch (Type)
                        {
                            case JsonRpcIdType.String:
                                {
                                    return -1;
                                }
                            case JsonRpcIdType.Integer:
                                {
                                    return _valueInteger.CompareTo(other._valueInteger);
                                }
                            case JsonRpcIdType.Float:
                                {
                                    return +1;
                                }
                            default:
                                {
                                    return -1;
                                }
                        }
                    }
                case JsonRpcIdType.Float:
                    {
                        switch (Type)
                        {
                            case JsonRpcIdType.String:
                                {
                                    return -1;
                                }
                            case JsonRpcIdType.Integer:
                                {
                                    return -1;
                                }
                            case JsonRpcIdType.Float:
                                {
                                    return _valueFloat.CompareTo(other._valueFloat);
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
        public bool Equals(in JsonRpcId other)
        {
            switch (other.Type)
            {
                case JsonRpcIdType.String:
                    {
                        return (Type == JsonRpcIdType.String) && _valueString.Equals(other._valueString);
                    }
                case JsonRpcIdType.Integer:
                    {
                        return (Type == JsonRpcIdType.Integer) && _valueInteger.Equals(other._valueInteger);
                    }
                case JsonRpcIdType.Float:
                    {
                        return (Type == JsonRpcIdType.Float) && _valueFloat.Equals(other._valueFloat);
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
                        return Equals(in other);
                    }
                case string other:
                    {
                        return (Type == JsonRpcIdType.String) && _valueString.Equals(other);
                    }
                case long other:
                    {
                        return (Type == JsonRpcIdType.Integer) && _valueInteger.Equals(other);
                    }
                case double other:
                    {
                        return (Type == JsonRpcIdType.Float) && _valueFloat.Equals(other);
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
                    case JsonRpcIdType.String:
                        {
                            result = (result * 16777619) ^ _valueString.GetHashCode();
                        }
                        break;
                    case JsonRpcIdType.Integer:
                        {
                            result = (result * 16777619) ^ _valueInteger.GetHashCode();
                        }
                        break;
                    case JsonRpcIdType.Float:
                        {
                            result = (result * 16777619) ^ _valueFloat.GetHashCode();
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
                case JsonRpcIdType.String:
                    {
                        return _valueString;
                    }
                case JsonRpcIdType.Integer:
                    {
                        return _valueInteger.ToString(CultureInfo.InvariantCulture);
                    }
                case JsonRpcIdType.Float:
                    {
                        // Writes at least 1 precision digit out of 16 possible

                        return _valueFloat.ToString("0.0###############", CultureInfo.InvariantCulture);
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
        public static bool operator ==(in JsonRpcId obj1, in JsonRpcId obj2)
        {
            return obj1.Equals(in obj2);
        }

        /// <summary>Indicates whether the left <see cref="JsonRpcId" /> is not equal to the right <see cref="JsonRpcId" />.</summary>
        /// <param name="obj1">The left <see cref="JsonRpcId" /> operand.</param>
        /// <param name="obj2">The right <see cref="JsonRpcId" /> operand.</param>
        /// <returns><see langword="true" /> if the left <see cref="JsonRpcId" /> is not equal to the right <see cref="JsonRpcId" />; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(in JsonRpcId obj1, in JsonRpcId obj2)
        {
            return !obj1.Equals(in obj2);
        }

        /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" /> from.</param>
        public static implicit operator JsonRpcId(string value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="ulong" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" /> from.</param>
        public static implicit operator JsonRpcId(long value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="double" /> to <see cref="JsonRpcId" />.</summary>
        /// <param name="value">The value to create a <see cref="JsonRpcId" /> from.</param>
        public static implicit operator JsonRpcId(double value)
        {
            return new JsonRpcId(value);
        }

        /// <summary>Performs an implicit conversion from <see cref="JsonRpcId" /> to <see cref="string" />.</summary>
        /// <param name="value">The identifier to get a <see cref="string" /> value from.</param>
        /// <exception cref="InvalidCastException">The underlying value is not of type <see cref="string" />.</exception>
        public static explicit operator string(in JsonRpcId value)
        {
            if (value.Type != JsonRpcIdType.String)
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("id.invalid_cast"), typeof(JsonRpcId), typeof(string)));
            }

            return value._valueString;
        }

        /// <summary>Performs an implicit conversion from <see cref="JsonRpcId" /> to <see cref="long" />.</summary>
        /// <param name="value">The identifier to get a <see cref="long" /> value from.</param>
        /// <exception cref="InvalidCastException">The underlying value is not of type <see cref="long" />.</exception>
        public static explicit operator long(in JsonRpcId value)
        {
            if (value.Type != JsonRpcIdType.Integer)
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("id.invalid_cast"), typeof(JsonRpcId), typeof(long)));
            }

            return value._valueInteger;
        }

        /// <summary>Performs an implicit conversion from <see cref="JsonRpcId" /> to <see cref="double" />.</summary>
        /// <param name="value">The identifier to get a <see cref="double" /> value from.</param>
        /// <exception cref="InvalidCastException">The underlying value is not of type <see cref="double" />.</exception>
        public static explicit operator double(in JsonRpcId value)
        {
            if (value.Type != JsonRpcIdType.Float)
            {
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("id.invalid_cast"), typeof(JsonRpcId), typeof(double)));
            }

            return value._valueFloat;
        }

        /// <summary>Gets the identifier type.</summary>
        public JsonRpcIdType Type
        {
            get;
        }
    }
}