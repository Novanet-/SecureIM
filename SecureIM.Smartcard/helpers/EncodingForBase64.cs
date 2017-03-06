using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace SecureIM.Smartcard.helpers
{
    public static class EncodingForBase64
    {
        /// <summary>
        /// Encodes the base64.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        [CanBeNull]
        public static string EncodeBase64([NotNull] this Encoding encoding, [NotNull] string text)
        {
            try
            {
                byte[] textAsBytes = encoding.GetBytes(text);
                return Convert.ToBase64String(textAsBytes);
            }
            catch (FormatException e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Decodes the base64.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="encodedText">The encoded text.</param>
        /// <returns></returns>
        [CanBeNull]
        public static string DecodeBase64([NotNull] this Encoding encoding, [NotNull] string encodedText)
        {
            try
            {
                byte[] textAsBytes = Convert.FromBase64String(encodedText);
                return encoding.GetString(textAsBytes);
            }
            catch (FormatException e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
        }
    }
}