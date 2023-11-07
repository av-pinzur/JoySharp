using System.ComponentModel;

namespace AvP.Joy;

public static class EnumExtensions
{
    public static bool IsDefined(this Enum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return Enum.IsDefined(value.GetType(), value);
    }

    public static string GetDescription(this Enum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (!value.IsDefined()) throw new ArgumentOutOfRangeException(nameof(value), value, "Parameter must be a member of the {0} enum.");

        var valueName = value.ToString();
        var descriptionAttribute = value.GetType().GetMember(valueName).Single().GetCustomAttribute<DescriptionAttribute>(false);
        return descriptionAttribute == null ? valueName : descriptionAttribute.Description;
    }
}
