using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace SecureIM.Smartcard.helpers
{
    public static class EncodingForBase64
    {
        [CanBeNull]
        public static string EncodeBase64([NotNull] this System.Text.Encoding encoding, [NotNull] string text)
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

        [CanBeNull]
        public static string DecodeBase64([NotNull] this System.Text.Encoding encoding, [NotNull] string encodedText)
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