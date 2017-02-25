﻿using System.Text;
using SecureIM.Smartcard.helpers;

namespace SecureIM.ChatBackend.helpers
{
    internal class BackendHelper
    {
        #region Internal Methods

        internal static byte[] DecodeToByteArrayBase64(string data)
        {
            byte[] pubKeyBytes = Encoding.Default.GetBytes(Encoding.Default.DecodeBase64(data));
            return pubKeyBytes;
        }

        internal static string EncodeFromByteArrayBase64(byte[] dataBytes)
        {
            string pubKeyB64 = Encoding.Default.EncodeBase64(Encoding.Default.GetString(dataBytes));
            return pubKeyB64;
        }

        #endregion Internal Methods
    }
}