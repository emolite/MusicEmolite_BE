using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MS_Application.Helpers
{
    public static class EnumHelper
    {
        public static string GetDisplayName(Enum value)
        {
            var field = value
                .GetType()
                .GetField(value.ToString());

            var attribute = field?
                .GetCustomAttribute<DisplayAttribute>();

            return attribute?.Name ?? value.ToString();
        }
    }
}