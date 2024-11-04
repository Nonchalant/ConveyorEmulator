using System;
using System.ComponentModel;
using System.Linq;

namespace Helper
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T value) where T : Enum
        {
            string str = value.ToString();

            var description = typeof(T)
                .GetField(str)
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .FirstOrDefault()
                ?.Description;

            return description ?? str;
        }
    }
}