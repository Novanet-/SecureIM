using System;
using System.Text;
using JetBrains.Annotations;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard.enums;

namespace SecureIM.Smartcard.controller.smartcard
{
    public class SmartcardCryptoHandler : ICryptoHandler
    {
        #region Public Properties

        /// <summary>
        /// Gets the smartcard controller.
        /// </summary>
        /// <value>
        /// The smartcard controller.
        /// </value>
        public SmartcardController SmartcardController { get; }

        #endregion Public Properties


        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartcardCryptoHandler"/> class.
        /// </summary>
        public SmartcardCryptoHandler() { SmartcardController = new SmartcardController(); }

        #endregion Public Constructors


        #region Public Methods

        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyBytes">The key bytes.</param>
        /// <returns></returns>
        public string Decrypt([NotNull] string data, [CanBeNull] byte[] keyBytes = null)
        {
            byte[] dataBytes = Encoding.Default.GetBytes(data);

            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY, 0x00, 0x00, keyBytes);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

            byte[] decryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT, 0x01);
            if (decryptedBytes[0] == 0x6C)
            {
                byte le = decryptedBytes[1];
                decryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT_GET_RESPONSE, 0, 0, null, le);
                decryptedBytes = TrimSwFromResponse(decryptedBytes);
            }

            return Encoding.ASCII.GetString(decryptedBytes);
        }

        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyBytes">The key bytes.</param>
        /// <returns></returns>
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
                encryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT_GET_RESPONSE, 0, 0, null, le);
                encryptedBytes = TrimSwFromResponse(encryptedBytes);
            }

            return Encoding.Default.GetString(encryptedBytes);
        }

        [NotNull]
        private static byte[] TrimSwFromResponse(byte[] responseBytes)
        {
            Array.Resize(ref responseBytes, responseBytes.Length - 2);
            return responseBytes;
        }

        /// <summary>
        /// Generates the asymmetric key pair.
        /// </summary>
        public void GenerateAsymmetricKeyPair() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR);

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPublicKey() => TrimSwFromResponse(SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PUB_KEY));

        /// <summary>
        /// Gets the private key.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPrivateKey() => TrimSwFromResponse(SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PRI_KEY));

        #endregion Public Methods
    }
}
