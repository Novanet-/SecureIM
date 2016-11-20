using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureIM.Desktop.model.abstractions;

namespace SecureIM.Desktop.controller
{
    class Crypto : AbstractCrypto
    {
        public override string DecryptAes()
        {
            throw new NotImplementedException();
        }

        public override string DecryptRsa()
        {
            throw new NotImplementedException();
        }

        public override string DecryptSha()
        {
            throw new NotImplementedException();
        }

        public override string EncryptAes()
        {
            throw new NotImplementedException();
        }

        public override string EncryptRsa()
        {
            throw new NotImplementedException();
        }

        public override string EncryptSha()
        {
            throw new NotImplementedException();
        }

        public static string PadString(string inputString, int padToLength)
        {
            padToLength = 16;
            int inputLength = inputString.Length;
            int padLength = (padToLength - (inputLength%padToLength)%padToLength);
            byte bytePadLength = Convert.ToByte(padLength);
            StringBuilder sb = new StringBuilder(inputString);
            for (byte i = 0; i < bytePadLength; i++)
            {
                sb.Append(System.Text.Encoding.ASCII.GetString(new[] {bytePadLength}));
            }
        }
    }
}