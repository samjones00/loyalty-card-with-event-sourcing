using System;

namespace LoyaltyCard.Domain.Extensions
{
    public static class Guards
    {
        public static T ThrowIfNull<T>(this T source, string paramName) where T: class
        {
            return source is null ? throw new ArgumentNullException(paramName) : source;
        }

        public static string ThrowIfNullOrEmpty(this string source, string paramName)
        {
            return string.IsNullOrEmpty(source) ? throw new ArgumentNullException(paramName) : source;
        }
    }
}
