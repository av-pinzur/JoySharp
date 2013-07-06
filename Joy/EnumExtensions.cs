﻿using System;
using System.ComponentModel;
using System.Linq;

namespace AvP.Joy
{
    public static class EnumExtensions
    {
        public static bool IsDefined(this Enum value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return Enum.IsDefined(value.GetType(), value);
        }

        public static string GetDescription(this Enum value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (!value.IsDefined()) throw new ArgumentOutOfRangeException("value", value, "Parameter must be a member of the {0} enum.");

            var valueName = value.ToString();
            var displayNameAttribute = value.GetType().GetMember(valueName).Single().GetCustomAttribute<DescriptionAttribute>(false);
            return displayNameAttribute == null ? valueName : displayNameAttribute.Description;
        }
    }
}