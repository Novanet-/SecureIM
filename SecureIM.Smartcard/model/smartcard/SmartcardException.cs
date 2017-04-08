using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace SecureIM.Smartcard.model.smartcard
{
    /// <summary>
    ///SmartcardException
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class SmartcardException : Exception
    {
        #region Public Properties

        public static string ConnectionError { get; } = "Could not connect to reader {0}:\n{1}";
        public static string NoReadersError { get; } = "You need at least one reader in order to run this example.";
        public static string ReaderNotSelectedError { get; } = "Reader not selected";

        #endregion Public Properties

        #region Public Constructors

        public SmartcardException([NotNull] string message) : base(message)
        {
        }

        public SmartcardException([NotNull] string message, [NotNull] Exception innerException) : base(message, innerException)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected SmartcardException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors

        #region Public Methods

        [NotNull]
        public static string FormatErrorMessage([NotNull] string message, [NotNull] string readerName, [NotNull] string cardError)
            => string.Format(message, readerName, cardError);

        #endregion Public Methods
    }
}