using System;
using System.Diagnostics;
using JetBrains.Annotations;
using PCSC;
using PCSC.Iso7816;
using SecureIM.Smartcard.helpers;
using SecureIM.Smartcard.model.smartcard;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.controller.smartcard
{
    /// <summary>
    /// SmartcardController
    /// </summary>
    public class SmartcardController
    {
        #region Private Properties

        // ReSharper disable once InconsistentNaming
        private static byte[] SECUREIMCARD_AID { get; } = {0xA0, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x10, 0x01};

        private SCardProtocol ActiveProtocol { get; set; }
        private SCardContext CardContext { get; }
        private SCardReader CardReader { get; set; }
        private IntPtr PioSendPci { get; set; }
        private bool ReaderConnected { get; set; }

        #endregion Private Properties

        #region Internal Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartcardController" /> class.
        /// </summary>
        internal SmartcardController()
        {
            CardContext = new SCardContext();
        }

        #endregion Internal Constructors

        #region Public Methods

        /// <summary>
        /// Connects to s card reader.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void ConnectToSCardReader([NotNull] string readerName)
        {
            CardReader = SmartcardControllerBuilder.ConnectToReader(CardContext, readerName);
            try
            {
                ActiveProtocol = CardReader.ActiveProtocol;

                switch (ActiveProtocol)
                {
                    case SCardProtocol.T0:
                        PioSendPci = SCardPCI.T0;
                        break;

                    case SCardProtocol.T1:
                        PioSendPci = SCardPCI.T1;
                        break;

                    case SCardProtocol.Unset:
                        break;

                    case SCardProtocol.Raw:
                        break;

                    case SCardProtocol.T15:
                        break;

                    case SCardProtocol.Any:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            ReaderConnected = true;
        }

        /// <summary>
        /// Gets the s card readers.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public string[] GetSCardReaders() => SmartcardControllerBuilder.GetSmartcardReaders(CardContext);

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="data">The data.</param>
        /// <param name="le">The le.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException">Condition.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">command - null</exception>
        /// <exception cref="OverflowException">The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue" /> elements.</exception>
        [NotNull]
        internal byte[] SendCommand(SecureIMCardInstructions command, byte p1 = 0x00, byte p2 = 0x00, [CanBeNull] byte[] data = null, byte le = 0x00)
        {
            if (!ReaderConnected) throw new SmartcardException(SmartcardException.NoReadersError);

            byte[] response = {};
            try
            {
                string dataString = data != null && data.Length != 0 ? ByteArrayHelper.ToHexString(data) : "None";

                Debug.WriteLine($"Creating and sending {command} with P1 = {p1}, P2 = {p2} and Data = {dataString}");
                response = SendCommandTransmitter(command, data, le);
                Debug.WriteLine($"{command} sent with Response = {ByteArrayHelper.ToHexString(response)}");
                Debug.WriteLine("");
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.ToString());
            }
            return response;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Checks the error.
        /// </summary>
        /// <param name="err">The error.</param>
        /// <exception cref="PCSC.PCSCException"></exception>
        /// <exception cref="PCSCException"></exception>
        private static void CheckErr(SCardError err)
        {
            if (err != SCardError.Success) throw new PCSCException(err, SCardHelper.StringifyError(err));
        }

        /// <summary>
        /// Sends the command transmitter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="data">The data.</param>
        /// <param name="le">The le.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">command - null</exception>
        [NotNull]
        private byte[] SendCommandTransmitter(SecureIMCardInstructions command, [CanBeNull] byte[] data, byte le)
        {
            TransmitAPDU(APDUFactory.SELECT(SECUREIMCARD_AID));
            switch (command)
            {
                case SecureIMCardInstructions.INS_SELECT_SCIM:
                    return TransmitAPDU(APDUFactory.SELECT(SECUREIMCARD_AID));

                case SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR:
                    return TransmitAPDU(APDUFactory.ECC_GEN_KEYPAIR());

                case SecureIMCardInstructions.INS_ECC_GET_PRI_KEY:
                    return TransmitAPDU(APDUFactory.GET_PRI_KEY());

                case SecureIMCardInstructions.INS_ECC_GET_PUB_KEY:
                    return TransmitAPDU(APDUFactory.GET_PUB_KEY());

                case SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY:
                    if (data != null) return TransmitAPDU(APDUFactory.SET_GUEST_PUB_KEY(data));
                    break;

                case SecureIMCardInstructions.INS_ECC_GEN_SECRET:
                    return TransmitAPDU(APDUFactory.GEN_SECRET());

                case SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY:
                    return TransmitAPDU(APDUFactory.GEN_DES_KEY());

                case SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT:
                    if (data != null) return TransmitAPDU(APDUFactory.SET_INPUT_TEXT(data));
                    break;

                case SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT:
                    return TransmitAPDU(APDUFactory.DO_CIPHER(false));

                case SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT_GET_RESPONSE:
                    return TransmitAPDU(APDUFactory.DO_CIPHER(false, le));

                case SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT:
                    return TransmitAPDU(APDUFactory.DO_CIPHER(true));

                case SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT_GET_RESPONSE:
                    return TransmitAPDU(APDUFactory.DO_CIPHER(true, le));

                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }

            throw new SmartcardException("Data was null when ti should have existed");
        }

        /// <summary>
        /// Transmits the apdu.
        /// </summary>
        /// <param name="apdu">The apdu.</param>
        /// <returns></returns>
        [NotNull]
        private byte[] TransmitAPDU([NotNull] Apdu apdu)
        {
            var pbRecvBuffer = new byte[256];

            SCardError err = CardReader.Transmit(PioSendPci, apdu.ToArray(), ref pbRecvBuffer);
            CheckErr(err);

            //            CardReader.Dispose();

            return pbRecvBuffer;
        }

        #endregion Private Methods
    }
}