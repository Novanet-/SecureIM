using System;
using System.Text;
using SecureIM.Smartcard.controller.abstractions;

namespace SecureIM.Smartcard.controller.crypto
{
    internal class Crypto : AbstractCrypto
    {
        #region Public Methods


        /// <summary>
        ///     Pads the string.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="padToLength">Length of the pad to.</param>
        /// <returns></returns>
        public static string PadString(string inputString, int padToLength)
        {
            padToLength = 16;
            int inputLength = inputString.Length;
            int padLength = padToLength - inputLength % padToLength % padToLength;
            byte bytePadLength = Convert.ToByte(padLength);
            var sb = new StringBuilder(inputString);
            for (byte i = 0; i < bytePadLength; i++) sb.Append(Encoding.ASCII.GetString(new[] {bytePadLength}));
            return sb.ToString();
        }

        public override string DecryptAes() { throw new NotImplementedException(); }

        public override string DecryptRsa() { throw new NotImplementedException(); }

        public override string DecryptSha() { throw new NotImplementedException(); }

        public override string EncryptAes() { throw new NotImplementedException(); }

        public override string EncryptRsa() { throw new NotImplementedException(); }

        public override string EncryptSha() { throw new NotImplementedException(); }


        #endregion Public Methods
    }
}