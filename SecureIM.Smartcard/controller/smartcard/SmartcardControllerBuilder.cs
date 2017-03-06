using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using PCSC;
using SecureIM.Smartcard.model.smartcard;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.controller.smartcard
{
    /// <summary>
    /// SmartcardControllerBuilder
    /// </summary>
    public class SmartcardControllerBuilder
    {
        #region Internal Methods

        /// <summary>
        /// Gets the smartcard readers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException"></exception>
        internal string[] GetSmartcardReaders([NotNull] ISCardContext context)
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
        internal SCardReader ConnectToReader(ISCardContext context, string readerName)
        {
            SCardReader reader = ConnectToCard(context, readerName);
            return reader;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Chooses the card.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException"></exception>
        [NotNull]
        private static string ChooseCard([NotNull] ISCardContext context)
        {
            context.Establish(SCardScope.System);
            string[] readerNames = context.GetReaders();
            if (readerNames.Length < 1) throw new SmartcardException(SmartcardException.NoReadersError);

            return ChooseReader(readerNames);
        }

        /// <summary>
        /// Chooses the reader.
        /// </summary>
        /// <param name="readerNames">The reader names.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException">Reader not selected</exception>
        [NotNull]
        private static string ChooseReader([NotNull] IReadOnlyList<string> readerNames)
        {
            //             Show available readers.
            Debug.WriteLine("Available readers: ");
            for (var i = 0; i < readerNames.Count; i++) { Debug.WriteLine("[" + i + "] " + readerNames[i]); }

            if (readerNames[0] == null) throw new SmartcardException("Reader not selected");

            return readerNames[0];
        }

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