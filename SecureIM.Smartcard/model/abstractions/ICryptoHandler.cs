namespace SecureIM.Smartcard.model.abstractions
{
    interface ICryptoHandler
    {
        #region Public Methods

        string Decrypt(string data, byte[] keyBytes = null);

        string Encrypt(string data, byte[] keyBytes);

        void GenerateAsymmetricKeyPair();

        byte[] GetPublicKey();

        #endregion Public Methods
    }
}