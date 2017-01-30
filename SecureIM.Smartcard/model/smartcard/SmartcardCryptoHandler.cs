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

            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

            byte[] decryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT, 0x01);

            return Encoding.ASCII.GetString(decryptedBytes);
        }

        public string Encrypt([NotNull] string data, [NotNull] byte[] keyBytes)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

            byte[] encryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT);
            if (encryptedBytes[0] == 0x6C)
            {
                byte le = encryptedBytes[1];
                SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT_GET_RESPONSE, 0, 0, null, le);
            }

            return Encoding.ASCII.GetString(encryptedBytes);
        }

        public void GenerateAsymmetricKeyPair() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR);

        public byte[] GetPublicKey() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PUB_KEY);
        public byte[] GetPrivKey() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PUB_KEY);
        public byte[] GetPrivateKey() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PRI_KEY);

        #endregion Public Methods
    }
}