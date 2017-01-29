using System.Text;
using JetBrains.Annotations;
using SecureIM.Smartcard.controller.smartcard;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.model.smartcard
{
    public class SmartcardCryptoHandler : ICryptoHandler
    {
        #region Public Properties

        public SmartcardController SmartcardController { get; }

        #endregion Public Properties


        #region Public Constructors

        public SmartcardCryptoHandler() { SmartcardController = new SmartcardController(); }

        #endregion Public Constructors


        #region Public Methods

        public string Decrypt([NotNull] string data, [CanBeNull] byte[] keyBytes = null)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_W, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

            byte[] decryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER, 0x01);

            return Encoding.ASCII.GetString(decryptedBytes);
        }

        public string Encrypt([NotNull] string data, [NotNull] byte[] keyBytes)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_W, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

            byte[] encryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER);

            return Encoding.ASCII.GetString(encryptedBytes);
        }

        public void GenerateAsymmetricKeyPair()
        {
//            SmartcardController.SendCommand(SecureIMCardInstructions.INS_SELECT_SCIM);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR);
        }

        public byte[] GetPublicKey() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_W);

        #endregion Public Methods
    }
}