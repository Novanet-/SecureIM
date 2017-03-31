using System;
using System.Text;
using JetBrains.Annotations;

namespace SecureIM.Smartcard.helpers
{
    static class ByteArrayHelper
    {
        /// <summary>
        /// To the hexadecimal string.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns></returns>
        [Pure]
        [NotNull]
        public static string ToHexString([NotNull] byte[] hex)
        {
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
                s.Append($"{b:X2} ");

            return s.ToString();
        }

        /// <summary>
        /// Trims the array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="trimByAmount">The trim by amount.</param>
        /// <returns></returns>
        [NotNull]
        internal static byte[] TrimArray(byte[] array, int trimByAmount)
        {
            if (array.Length > trimByAmount)
                Array.Resize(ref array, array.Length - trimByAmount);
            return array;
        }
    }
}
