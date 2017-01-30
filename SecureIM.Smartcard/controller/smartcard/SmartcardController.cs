using System;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;
using PCSC;
using PCSC.Iso7816;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.controller.smartcard
{
    public class SmartcardController
    {
        #region Public Properties

        // ReSharper disable once InconsistentNaming
        public static byte[] SECUREIMCARD_AID { get; } = {0xA0, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x10, 0x01};

        public SCardProtocol ActiveProtocol { get; }
        public SCardContext CardContext { get; } = new SCardContext();
        public SCardReader CardReader { get; }
        public IntPtr PioSendPci { get; }
        public SmartcardControllerBuilder ScControllerBuilder { get; } = new SmartcardControllerBuilder();

        #endregion Public Properties


        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartcardController" /> class.
        /// </summary>
        /// <exception cref="PCSCException">Protocol not supported: " + CardReader.ActiveProtocol</exception>
        public SmartcardController()
        {
            CardReader = ScControllerBuilder.EstablishCardConnection(CardContext);
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
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion Public Constructors


        #region Public Methods



        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="data">The data.</param>
        /// <param name="le">The le.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">command - null</exception>
        [NotNull]
        public byte[] SendCommand(SecureIMCardInstructions command, byte p1 = 0x00, byte p2 = 0x00, byte[] data = null, byte le = 0x00)
        {
            string dataString = data != null && data.Length != 0 ? ToHexString(data) : "None";
            Debug.WriteLine($"Creating and sending {command} with P1 = {p1}, P2 = {p2} and Data = {dataString}");
            byte[] response = SendCommandTransmitter(command, p1, p2, data, le);
//            var responseDataString = new StringBuilder();
//
//            foreach (byte dataByte in response) { responseDataString.Append($"{dataByte:X2} "); }

            Debug.WriteLine($"{command} sent with Response = {ToHexString(response)}");
            Debug.WriteLine("");
            return response;
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Checks the error.
        /// </summary>
        /// <param name="err">The error.</param>
        /// <exception cref="PCSCException"></exception>
        private static void CheckErr(SCardError err)
        {
            if (err != SCardError.Success) throw new PCSCException(err, SCardHelper.StringifyError(err));
        }

        /// <summary>
        /// Sends the command transmitter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="data">The data.</param>
        /// <param name="le">The le.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">command - null</exception>
        [NotNull]
        private byte[] SendCommandTransmitter(SecureIMCardInstructions command, byte p1, byte p2, byte[] data, byte le)
        {
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
                    return TransmitAPDU(APDUFactory.SET_GUEST_PUB_KEY(data));
                case SecureIMCardInstructions.INS_ECC_GEN_SECRET:
                    return TransmitAPDU(APDUFactory.GEN_SECRET());
                case SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY:
                    return TransmitAPDU(APDUFactory.GEN_DES_KEY());
                case SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT:
                    return TransmitAPDU(APDUFactory.SET_INPUT_TEXT(data));
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

        /// <summary>
        /// To the hexadecimal string.
        /// </summary>
        /// <param name="hex">The hexadecimal.</param>
        /// <returns></returns>
        [NotNull]
        public static string ToHexString([NotNull] byte[] hex)
        {
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex) { s.Append($"{b:X2} "); }

            return s.ToString();
        }

        #endregion Private Methods
    }
}