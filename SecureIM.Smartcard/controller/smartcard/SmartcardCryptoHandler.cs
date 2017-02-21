using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard.enums;
using static SecureIM.Smartcard.helpers.ByteArrayHelper;

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
        public SmartcardCryptoHandler()
        {
            SmartcardController = new SmartcardController();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Decrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyBytes">The key bytes.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException">Condition.</exception>
        public string Decrypt([NotNull] string data, [CanBeNull] byte[] keyBytes = null)
        {
            byte[] dataBytes = Encoding.Default.GetBytes(data);
            byte[] decryptedBytes = {};
            try
            {
                byte[] setGuestResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY, 0x00, 0x00, keyBytes);
                byte[] setGenSecretResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
                byte[] setGen3DESResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
                byte[] setInputResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

                var successSw = new byte[] {0x90, 0x00};

                ErrorCheck(setGuestResponse, successSw, setGenSecretResponse, setGen3DESResponse, setInputResponse);

                decryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT, 0x01);

                if (decryptedBytes.Length <= 2)
                    if (decryptedBytes[0].Equals(0x6C))
                    {
                        byte le = decryptedBytes[1];
                        decryptedBytes = SmartcardController.SendCommand(
                            SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT_GET_RESPONSE,
                            0,
                            0,
                            null,
                            le);
                        decryptedBytes = TrimSwFromResponse(decryptedBytes);
                    }
                    else
                    {
                        throw new SmartcardException($"Decrypt failed with response: {ToHexString(decryptedBytes)}");
                    }
            }
            catch (OverflowException e)
            {
                e.ToString();
            }

            return Encoding.ASCII.GetString(decryptedBytes);
        }

        private static void ErrorCheck(byte[] setGuestResponse, byte[] successSw, byte[] setGenSecretResponse, byte[] setGen3DESResponse,
            byte[] setInputResponse)
        {
            if (!setGuestResponse.SequenceEqual(successSw))
                throw new SmartcardException($"Set Guest Pub Key failed with response: {ToHexString(setGuestResponse)}");
            if (setGenSecretResponse.Length <= 2 && !setGenSecretResponse[0].Equals(0x6C))
                throw new SmartcardException($"Gen secret failed with response: {ToHexString(setGenSecretResponse)}");
            if (setGen3DESResponse.Length <= 2 && !setGen3DESResponse[0].Equals(0x6C))
                throw new SmartcardException($"Gen 3DES failed with response: {ToHexString(setGen3DESResponse)}");
            if (!setInputResponse.SequenceEqual(successSw))
                throw new SmartcardException($"Set input text failed with response: {ToHexString(setInputResponse)}");
        }

        /// <summary>
        /// Encrypts the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyBytes">The key bytes.</param>
        /// <returns></returns>
        /// <exception cref="SmartcardException">Condition.</exception>
        public string Encrypt([NotNull] string data, [NotNull] byte[] keyBytes)
        {
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            byte[] setGuestResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY, 0x00, 0x00, keyBytes);
            byte[] setGenSecretResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            byte[] setGen3DESResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            byte[] setInputResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);

            var successSw = new byte[] {0x90, 0x00};

            ErrorCheck(setGuestResponse, successSw, setGenSecretResponse, setGen3DESResponse, setInputResponse);

//            if (!setGuestResponse.SequenceEqual(successSw))
//                throw new SmartcardException($"Set Guest Pub Key failed with response: {ToHexString(setGuestResponse)}");
//            if (setGenSecretResponse.Length <= 2 && !setGenSecretResponse[0].Equals(0x6C))
//                throw new SmartcardException($"Gen secret failed with response: {ToHexString(setGenSecretResponse)}");
//            if (setGen3DESResponse.Length <= 2 && !setGen3DESResponse[0].Equals(0x6C))
//                throw new SmartcardException($"Gen 3DES failed with response: {ToHexString(setGen3DESResponse)}");
//            if (!setInputResponse.SequenceEqual(successSw))
//                throw new SmartcardException($"Set input text failed with response: {ToHexString(setInputResponse)}");

            byte[] encryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT);

            if (encryptedBytes.Length <= 2)
                if (encryptedBytes[0].Equals(0x6C))
                {
                    byte le = encryptedBytes[1];
                    encryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT_GET_RESPONSE, 0, 0, null,
                        le);
                    encryptedBytes = TrimSwFromResponse(encryptedBytes);
                }
                else
                {
                    throw new SmartcardException($"Decrypt failed with response: {ToHexString(encryptedBytes)}");
                }

            return Encoding.Default.GetString(encryptedBytes);
        }

        /// <summary>
        /// Generates the asymmetric key pair.
        /// </summary>
        public void GenerateAsymmetricKeyPair() => SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_KEYPAIR);

        /// <summary>
        /// Gets the private key.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPrivateKey() => TrimSwFromResponse(SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PRI_KEY));

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <returns></returns>
        public byte[] GetPublicKey() => TrimSwFromResponse(SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GET_PUB_KEY));

        #endregion Public Methods

        #region Private Methods

        [NotNull]
        private static byte[] TrimSwFromResponse(byte[] responseBytes)
        {
            if (responseBytes.Length > 2)
                Array.Resize(ref responseBytes, responseBytes.Length - 2);

            return responseBytes;
        }

        #endregion Private Methods
    }
}