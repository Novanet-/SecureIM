using System;
using System.Collections.Generic;
using System.Diagnostics;
using PCSC;
using PCSC.Iso7816;
using SecureIM.Desktop.model;

namespace SecureIM.Desktop.controller
{
    class SmartcardController
    {
        #region Private Fields

        // ReSharper disable once InconsistentNaming
        private static readonly byte[] SECUREIMCARD_AID = {0xA0, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x10, 0x01};

        private readonly SCardProtocol _activeProtocol;
        private readonly IntPtr _pioSendPci;

        #endregion Private Fields

        #region Public Constructors

        public SmartcardController()
        {
            using (var context = new SCardContext())
            {
                using (SCardReader reader = EstablishCardConnection(context))
                {
                    _activeProtocol = reader.ActiveProtocol;

                    switch (_activeProtocol)
                    {
                        case SCardProtocol.T0:
                            _pioSendPci = SCardPCI.T0;
                            break;
                        case SCardProtocol.T1:
                            _pioSendPci = SCardPCI.T1;
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
                            throw new PCSCException(SCardError.ProtocolMismatch,
                                "Protocol not supported: " + reader.ActiveProtocol);
                    }

                    byte[] response = SelectApplet();
                    if (response.Length > 0)
                    {
                        Debug.WriteLine("SCIM applet loaded\n");
                    }

//                    Debug.WriteLine("response: ");

//                    foreach (byte t in response)
//                        Debug.Write($"{t:X2} ");

//                    Debug.WriteLine("");
                }
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public byte[] SendCommand(SecureIMCardInstructions command, byte? p1 = 0x00, byte? p2 = 0x00, byte[] data = null,
            byte? le = 0x00)
        {
            var response = new byte[] {};
            switch (command)
            {
                case SecureIMCardInstructions.RSA_GENERATE_KEY_PAIR:
                    GenerateKeyPair();
                    break;
                case SecureIMCardInstructions.RSA_GET_PUBLIC_KEY:
                    GetPublicKey();
                    break;
                case SecureIMCardInstructions.RSA_GET_PRIVATE_KEY:
                    GetPrivateKey();
                    break;
                case SecureIMCardInstructions.RSA_SET_PUBLIC_KEY:
                    SetPublicKey();
                    break;
                case SecureIMCardInstructions.RSA_SET_PRIVATE_KEY:
                    SetPrivateKey();
                    break;
                case SecureIMCardInstructions.RSA_SIGN:
                    SignCertificate();
                    break;
                case SecureIMCardInstructions.RSA_VERIFY:
                    VerifyCertificate();
                    break;
                case SecureIMCardInstructions.RSA_DO_CIPHER:
                    DoCipher();
                    break;
                case SecureIMCardInstructions.CARD_RESET:
                    GenerateKeyPair();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }

            return response;
        }

        private void DoCipher()
        {
            throw new NotImplementedException();
        }

        private void VerifyCertificate()
        {
            throw new NotImplementedException();
        }

        private void SignCertificate()
        {
            throw new NotImplementedException();
        }

        private void SetPrivateKey()
        {
            throw new NotImplementedException();
        }

        private void SetPublicKey()
        {
            throw new NotImplementedException();
        }

        private void GetPrivateKey()
        {
            throw new NotImplementedException();
        }

        private void GetPublicKey()
        {
            throw new NotImplementedException();
        }

        private void GenerateKeyPair()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        private static void CheckErr(SCardError err)
        {
            if (err != SCardError.Success)
                throw new PCSCException(err, SCardHelper.StringifyError(err));
        }

        private static string ChooseCard(ISCardContext context)
        {
            context.Establish(SCardScope.System);

            string[] readerNames = context.GetReaders();
            if (readerNames == null || readerNames.Length < 1)
            {
                Debug.WriteLine("You need at least one reader in order to run this example.");
            }

            string readerName = ChooseReader(readerNames);
            if (readerName == null)
            {
                throw new Exception("No readers found/Could not connect to reader");
            }
            return readerName;
        }

        private static string ChooseReader(IReadOnlyList<string> readerNames)
        {
            // Show available readers.
//            Debug.WriteLine("Available readers: ");
//            for (var i = 0; i < readerNames.Count; i++)
//            {
//                Debug.WriteLine("[" + i + "] " + readerNames[i]);
//            }

            return readerNames[0];
        }

        private static SCardReader ConnectToCard(ISCardContext context, string readerName)
        {
            var reader = new SCardReader(context);

            SCardError sc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
            if (sc == SCardError.Success) return reader;

            Debug.WriteLine("Could not connect to reader {0}:\n{1}", readerName, SCardHelper.StringifyError(sc));
            return null;
        }

        private static SCardReader EstablishCardConnection(ISCardContext context)
        {
            string readerName = ChooseCard(context);
            SCardReader reader = ConnectToCard(context, readerName);
            return reader;
        }

        private void DecryptMessage(byte[] encryptedMessage)
        {
            throw new NotImplementedException();
        }

        private void EncryptMessage(byte[] plaintextMessage)
        {
            throw new NotImplementedException();
        }

        private byte[] EstablishConnectionAndTransmitAPDU(CommandApdu apdu)
        {
            using (var context = new SCardContext())
            {
                using (SCardReader reader = EstablishCardConnection(context))
                {
                    byte[] response = TransmitAPDU(apdu, reader);
                    return response;
                }
            }
        }


        private byte[] IssueCard()
        {
            Debug.WriteLine("Creating ISSUE APDU \n");
            CommandApdu apdu = APDUFactory.RSA_GENERATE_KEY_PAIR(_activeProtocol);
            Debug.WriteLine("Sending ISSUE APDU \n");

            return EstablishConnectionAndTransmitAPDU(apdu);
        }

        private byte[] SelectApplet(byte[] aid = null)
        {
            aid = aid ?? SECUREIMCARD_AID;
            Debug.WriteLine("Creating ISSUE APDU \n");
            CommandApdu apdu = APDUFactory.SELECT(aid, _activeProtocol);
            Debug.WriteLine("Sending SELECT APDU \n");

            return EstablishConnectionAndTransmitAPDU(apdu);
        }

        private byte[] TransmitAPDU(CommandApdu apdu, SCardReader reader)
        {
            var pbRecvBuffer = new byte[256];

            SCardError err = reader.Transmit(_pioSendPci, apdu.ToArray(), ref pbRecvBuffer);
            CheckErr(err);

            reader.Dispose();

            return pbRecvBuffer;
        }

        #endregion Private Methods
    }
}