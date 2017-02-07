using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using PCSC;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.controller.smartcard
{
    public class SmartcardControllerBuilder
    {
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
                SCardError sc = reader.Connect(readerName, SCardShareMode.Exclusive, SCardProtocol.T0);
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

        /// <summary>
        /// Establishes the card connection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException">Condition.</exception>
        [NotNull]
        internal SCardReader EstablishCardConnection([NotNull] ISCardContext context)
        {
            string readerName = ChooseCard(context);
            SCardReader reader = ConnectToCard(context, readerName);
            return reader;
        }
    }
}