using System.Diagnostics;
using JetBrains.Annotations;
using PCSC;
using SecureIM.Smartcard.model.smartcard;

namespace SecureIM.Smartcard.controller.smartcard
{
    /// <summary>
    /// SmartcardControllerBuilder
    /// </summary>
    internal static class SmartcardControllerBuilder
    {
        #region Internal Methods

        /// <summary>
        /// Gets the smartcard readers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException"></exception>
        [NotNull]
        internal static string[] GetSmartcardReaders([NotNull] ISCardContext context)
        {
            context.Establish(SCardScope.System);
            string[] readerNames = context.GetReaders();
            if (readerNames.Length < 1) throw new SmartcardException(SmartcardException.NoReadersError);

            return readerNames;
        }

        /// <summary>
        /// Connects to reader.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="readerName">Name of the reader.</param>
        /// <returns></returns>
        [NotNull]
        internal static SCardReader ConnectToReader([NotNull] ISCardContext context, [NotNull] string readerName)
        {
            SCardReader reader = ConnectToCard(context, readerName);
            return reader;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Connects to card.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="readerName">Name of the reader.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException"></exception>
        [NotNull]
        private static SCardReader ConnectToCard([NotNull] ISCardContext context, [NotNull] string readerName)
        {
            var reader = new SCardReader(context);
            try
            {
                SCardError sc = reader.Connect(readerName, SCardShareMode.Exclusive, SCardProtocol.Any);
                if (sc != SCardError.Success)
                {
                    string formatErrorMessage =
                        SmartcardException.FormatErrorMessage("Could not connect to reader {0}:\n{1}",
                            readerName, SCardHelper.StringifyError(sc));

                    throw new SmartcardException(formatErrorMessage);
                }
            }
            catch (SmartcardException e)
            {
                Debug.WriteLine(e.ToString());
            }

            return reader;
        }

        #endregion Private Methods
    }
}