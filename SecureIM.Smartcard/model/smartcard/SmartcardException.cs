using System;
using System.Runtime.Serialization;

namespace SecureIM.Smartcard.model.smartcard
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    internal class SmartcardException : Exception
    {
        #region Public Properties

        public static string ConnectionError { get; } = "Could not connect to reader {0}:\n{1}";
        public static string NoReadersError { get; } = "You need at least one reader in order to run this example.";
        public static string ReaderNotSelectedError { get; } = "Reader not selected";

        #endregion Public Properties


        #region Public Constructors

        public SmartcardException(string message) : base(message) { }

        public SmartcardException(string message, Exception innerException) : base(message, innerException) { }

        #endregion Public Constructors


        #region Protected Constructors

        protected SmartcardException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion Protected Constructors


        #region Public Methods

        public static string FormatErrorMessage(string message, string readerName, string cardError) => string.Format(message, readerName, cardError);

        #endregion Public Methods
    }
}