using JetBrains.Annotations;

namespace SecureIM.Smartcard.model.abstractions
{
    public interface ICryptoHandler
    {
        #region Public Methods

        [NotNull]
        string Decrypt(string data, byte[] keyBytes = null);

        [NotNull]
        string Encrypt(string data, byte[] keyBytes);

        void GenerateAsymmetricKeyPair();

        [NotNull]
        byte[] GetPublicKey();
        byte[] GetPrivateKey();

        #endregion Public Methods
    }
}