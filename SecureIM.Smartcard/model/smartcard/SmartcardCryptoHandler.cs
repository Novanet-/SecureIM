using System;
using SecureIM.Smartcard.model.abstractions;

namespace SecureIM.Smartcard.model.smartcard
{
    class SmartcardCryptoHandler : ICryptoHandler
    {
        #region Public Methods

        public string Decrypt(string data, byte[] keyBytes = null) { throw new NotImplementedException(); }

        public string Encrypt(string data, byte[] keyBytes) { throw new NotImplementedException(); }

        public bool GenerateAsymmetricKeyPair() { throw new NotImplementedException(); }

        public byte[] GetPublicKey() { throw new NotImplementedException(); }

        #endregion Public Methods
    }
}