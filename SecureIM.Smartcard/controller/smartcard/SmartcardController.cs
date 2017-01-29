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
        ///     Initializes a new instance of the <see cref="SmartcardController" /> class.
        /// </summary>
        /// <exception cref="PCSCException">Protocol not supported: " + CardReader.ActiveProtocol</exception>
        public SmartcardController()
        {
            CardReader = ScControllerBuilder.EstablishCardConnection(CardContext);
            try
            {
                ActiveProtocol = CardReader.ActiveProtocol;

                if (ActiveProtocol == SCardProtocol.T0) { PioSendPci = SCardPCI.T0; }
                else if (ActiveProtocol == SCardProtocol.T1) { PioSendPci = SCardPCI.T1; }
                else if (ActiveProtocol == SCardProtocol.Unset) { }
                else if (ActiveProtocol == SCardProtocol.Raw) { }
                else if (ActiveProtocol == SCardProtocol.T15) { }
                else if (ActiveProtocol == SCardProtocol.Any) { }
                else
                {
                    throw new PCSCException(SCardError.ProtocolMismatch,
                                            "Protocol not supported: " + CardReader.ActiveProtocol);
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion Public Constructors


        #region Public Methods

        /// <summary>
        ///     Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="data">The data.</param>
        /// <param name="le">The le.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">command - null</exception>
        [NotNull]
        public byte[] SendCommand(SecureIMCardInstructions command, byte? p1 = 0x00, byte? p2 = 0x00, [CanBeNull] byte[] data = null, byte? le = 0x00)
        {
            string dataString = data != null && data.Length != 0 ? data.ToString() : "None";
            Debug.WriteLine($"Creating and sending {command} with P1 = {p1}, P2 = {p2} and Data = {dataString}");

            byte[] response = SendCommandTransmitter(command);

            var responseDataString = new StringBuilder();
            foreach (byte dataByte in response) { responseDataString.Append($"{dataByte:X2} "); }

            Debug.WriteLine($"{command} sent with Response = {responseDataString}");

            return response;
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        ///     Checks the error.
        /// </summary>
        /// <param name="err">The error.</param>
        /// <exception cref="PCSCException"></exception>
        private static void CheckErr(SCardError err)
        {
            if (err != SCardError.Success) throw new PCSCException(err, SCardHelper.StringifyError(err));
        }

        private byte[] DoDesCipher() { throw new NotImplementedException(); }

        private void GenerateDesKey() { throw new NotImplementedException(); }

        private void GenerateSecret() { throw new NotImplementedException(); }

        private byte[] GetPublicKey() { throw new NotImplementedException(); }

        /// <summary>
        ///     Selects the applet.
        /// </summary>
        /// <param name="aid">The aid.</param>
        /// <returns></returns>
        [NotNull]
        private byte[] SelectApplet(byte[] aid = null)
        {
            aid = aid ?? SECUREIMCARD_AID;
            CommandApdu apdu = APDUFactory.SELECT(aid);
            return TransmitAPDU(apdu);
        }

        [NotNull]
        private byte[] SendCommandTransmitter(SecureIMCardInstructions command)
        {
            var response = new byte[] {};
            switch (command)
            {
                case SecureIMCardInstructions.INS_SELECT_SCIM:
                    return SelectApplet();
                case SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR:
                    return TransmitAPDU(APDUFactory.ECC_GEN_KEYPAIR());
                case SecureIMCardInstructions.INS_ECC_GET_S:
                    return TransmitAPDU(APDUFactory.GET_PRI_KEY());
                case SecureIMCardInstructions.INS_ECC_GET_W:
                    return GetPublicKey();
                case SecureIMCardInstructions.INS_ECC_SET_GUEST_W:
                    SetPublicKey();
                    break;
                case SecureIMCardInstructions.INS_ECC_GEN_SECRET:
                    GenerateSecret();
                    break;
                case SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY:
                    GenerateDesKey();
                    break;
                case SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT:
                    SetInputText();
                    break;
                case SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER:
                    return DoDesCipher();
                case SecureIMCardInstructions.INS_ECC_SET_S:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }

            return response;
        }

        private void SetInputText() { throw new NotImplementedException(); }

        private void SetPublicKey() { throw new NotImplementedException(); }

        /// <summary>
        ///     Transmits the apdu.
        /// </summary>
        /// <param name="apdu">The apdu.</param>
        /// <param name="reader">The reader.</param>
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