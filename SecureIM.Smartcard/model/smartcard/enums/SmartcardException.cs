using System;
using System.Runtime.Serialization;

namespace SecureIM.Smartcard.model.smartcard.enums
{
    class SmartcardException : Exception
    {
        public static string ConnectionError { get; } = "Could not connect to reader {0}:\n{1}";
        public static string NoReadersError { get; } = "You need at least one reader in order to run this example.";
        public static string ReaderNotSelectedError { get; } = "Reader not selected";

        public SmartcardException(string message) : base(message) { }

        public SmartcardException(string message, Exception innerException) : base(message, innerException) { }

        protected SmartcardException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public static string FormatErrorMessage(string message, string readerName, string cardError) => string.Format(message, readerName, cardError);
    }
}