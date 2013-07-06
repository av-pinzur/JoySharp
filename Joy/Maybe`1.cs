﻿using System;

namespace AvP.Joy
{
    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly bool hasValue;
        private readonly T value;

        public static Maybe<T> Some(T value) { return new Maybe<T>(true, value); }
        public static Maybe<T> None { get { return new Maybe<T>(false, default(T)); } }
        public static Maybe<T> If(bool condition, Func<T> valueGetter) { return condition ? Some(valueGetter()) : None; }

        public static implicit operator Maybe<T>(T value) { return Some(value); }

        // Note: this struct has an implicit parameterless constructor as follows:
        //public Maybe()
        //{
        //    this.hasValue = false;
        //    this.value = default(T);
        //}

        private Maybe(bool hasValue, T value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        public bool HasValue { get { return hasValue; } }
        public T Value { get { if (!hasValue) throw new InvalidOperationException("The current Maybe has no value."); return value; } }
        public T ValueOrDefault(T fallback = default(T)) { return hasValue ? value : fallback; }

        public bool Equals(Maybe<T> other)
        {
            return Equals(this.hasValue, other.hasValue)
                && Equals(this.value, other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is Maybe<T> && Equals((Maybe<T>)obj);
        }

        public override int GetHashCode()
        {
            return hasValue.GetHashCode() 
                ^ (value == null ? 0 : value.GetHashCode());
        }

        public override string ToString()
        {
            return hasValue ? value.ToString() : "{none}";
        }
    }
}