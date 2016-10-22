namespace SecureIM.Desktop.model.abstractions
{
    internal interface ISmartcard
    {

        #region Public Methods

        string Decrypt();
        string Encrypt();
        bool EraseSmartcard();
        bool RegisterSmartcard();
        bool UnlockCertificate();

        #endregion Public Methods

    }

    internal abstract class AbstractSmartcard : ISmartcard
    {

        #region Public Fields

        public bool Online;
        public bool Registered;

        #endregion Public Fields

        #region Public Methods

        public abstract string Decrypt();
        public abstract string Encrypt();
        public abstract bool EraseSmartcard();
        public abstract bool Ping();
        public abstract bool RegisterSmartcard();
        public abstract bool UnlockCertificate();

        #endregion Public Methods

    }
}