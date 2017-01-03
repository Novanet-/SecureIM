using System;
using System.Text;
using SecureIM.Smartcard.controller.smartcard;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.model.smartcard
{
    class SmartcardCryptoHandler : ICryptoHandler
    {
        #region Public Properties

        public SmartcardController SmartcardController { get; set; }

        #endregion Public Properties


        #region Public Constructors

        public SmartcardCryptoHandler()
        {
            SmartcardController = new SmartcardController();
            throw new NotImplementedException();
        }

        #endregion Public Constructors


        #region Public Methods

        public string Decrypt(string data, byte[] keyBytes = null)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);


            throw new NotImplementedException();

/*            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_W, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);
            return SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER, 0x01).ToString();*/
        }

        public string Encrypt(string data, byte[] keyBytes)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            throw new NotImplementedException();

/*            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_W, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);
            return SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER).ToString();*/
        }

        public void GenerateAsymmetricKeyPair()
            => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR);

        public byte[] GetPublicKey() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_W);

        #endregion Public Methods
    }
}