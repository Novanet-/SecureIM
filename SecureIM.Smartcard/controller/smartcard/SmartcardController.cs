using System;
using System.Diagnostics;
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
        /// Initializes a new instance of the <see cref="SmartcardController"/> class.
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
        public byte[] SendCommand(SecureIMCardInstructions command, byte? p1 = 0x00, byte? p2 = 0x00,
                                  [CanBeNull] byte[] data = null,
                                  byte? le = 0x00)
        {
            var response = new byte[] {};
            if (command == SecureIMCardInstructions.INS_SELECT_SCIM) { response = SelectApplet(); }
            else if (command == SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR) { GenerateKeyPair(); }
            else if (command == SecureIMCardInstructions.INS_ECC_GET_S) { response = GetPrivateKey(); }
            else if (command == SecureIMCardInstructions.INS_ECC_GET_W) { response = GetPublicKey(); }
            else if (command == SecureIMCardInstructions.INS_ECC_SET_GUEST_W) { SetPublicKey(); }
            else if (command == SecureIMCardInstructions.INS_ECC_GEN_SECRET) { GenerateSecret(); }
            else if (command == SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY) { GenerateDesKey(); }
            else if (command == SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT) { SetInputText(); }
            else if (command == SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER) { response = DoDesCipher(); }
            else if (command == SecureIMCardInstructions.INS_ECC_SET_S) { }
            else
            { throw new ArgumentOutOfRangeException(nameof(command), command, null); }

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

        private void GenerateKeyPair() { throw new NotImplementedException(); }

        private void GenerateSecret() { throw new NotImplementedException(); }

        private byte[] GetPrivateKey() { throw new NotImplementedException(); }

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
            Debug.WriteLine("Creating ISSUE APDU \n");
            CommandApdu apdu = APDUFactory.SELECT(ActiveProtocol, aid);
            Debug.WriteLine("Sending SELECT APDU \n");

            return TransmitAPDU(apdu, CardReader);
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
        private byte[] TransmitAPDU([NotNull] Apdu apdu, [NotNull] ISCardReader reader)
        {
            var pbRecvBuffer = new byte[256];

            SCardError err = reader.Transmit(PioSendPci, apdu.ToArray(), ref pbRecvBuffer);
            CheckErr(err);

            reader.Dispose();

            return pbRecvBuffer;
        }

        #endregion Private Methods
    }
}