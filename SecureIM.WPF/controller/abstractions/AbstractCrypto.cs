namespace SecureIM.WPF.controller.abstractions
{
    internal interface ICrypto
    {
        #region Public Methods

        string DecryptAes();

        string DecryptRsa();

        string DecryptSha();

        string EncryptAes();

        string EncryptRsa();

        string EncryptSha();

        #endregion Public Methods
    }

    internal abstract class AbstractCrypto : ICrypto
    {
        #region Public Methods

        public abstract string DecryptAes();

        public abstract string DecryptRsa();

        public abstract string DecryptSha();

        public abstract string EncryptAes();

        public abstract string EncryptRsa();

        public abstract string EncryptSha();

        #endregion Public Methods
    }
}