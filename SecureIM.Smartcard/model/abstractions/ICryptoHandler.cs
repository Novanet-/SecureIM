using JetBrains.Annotations;

namespace SecureIM.Smartcard.model.abstractions
{
    /// <summary>
    /// ICryptoHandler
    /// </summary>
    public interface ICryptoHandler
    {
        #region Public Methods

        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyBytes">The key bytes.</param>
        /// <returns></returns>
        [NotNull]
        string Decrypt(string data, byte[] keyBytes);

        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyBytes">The key bytes.</param>
        /// <returns></returns>
        [NotNull]
        string Encrypt(string data, byte[] keyBytes);

        /// <summary>
        /// Generates the asymmetric key pair.
        /// </summary>
        void GenerateAsymmetricKeyPair();

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        byte[] GetPublicKey();

        #endregion Public Methods
    }
}