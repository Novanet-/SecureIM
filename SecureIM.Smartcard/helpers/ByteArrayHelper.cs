using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SecureIM.Smartcard.helpers
{
    class ByteArrayHelper
    {
        /// <summary>
        /// To the hexadecimal string.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns></returns>
        [ItemNotNull]
        [Pure]
        [NotNull]
        public static string ToHexString([NotNull] byte[] hex)
        {
            if (hex.Length == 0) return String.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
                s.Append($"{b:X2} ");

            return s.ToString();
        }
    }
}
