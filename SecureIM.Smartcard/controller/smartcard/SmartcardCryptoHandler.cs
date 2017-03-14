using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard;
using SecureIM.Smartcard.model.smartcard.enums;
using static SecureIM.Smartcard.helpers.ByteArrayHelper;

namespace SecureIM.Smartcard.controller.smartcard
{
    /// <summary>
    /// SmartcardCryptoHandler
    /// </summary>
    /// <seealso cref="SecureIM.Smartcard.model.abstractions.ICryptoHandler" />
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
        /// Initializes a new instance of the <see cref="SmartcardCryptoHandler" /> class.
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
        public string Decrypt([NotNull] string data, [NotNull] byte[] keyBytes)
        {
            byte[] dataBytes = Encoding.Default.GetBytes(data);
            byte[] decryptedBytes = {};
            byte le = 8;
            try
            {

                InitializeCipherOperation(keyBytes, dataBytes, out byte[] setGuestResponse, out byte[] setGenSecretResponse, out byte[] setGen3DESResponse,
                    out byte[] setInputResponse);

                var successSw = new byte[] {0x90, 0x00};

                InitializationErrorCheck(setGuestResponse, successSw, setGenSecretResponse, setGen3DESResponse, setInputResponse);

                decryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT, 0x01);

                if (decryptedBytes.Length <= 2)
                {
                    decryptedBytes = GetResponseData(decryptedBytes, SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_DECRYPT_GET_RESPONSE, le);
                }
            }
            catch (OverflowException e)
            {
                e.ToString();
            }

//            return TrimSwFromResponse(decryptedBytes);
            return Encoding.Default.GetString(TrimSwFromResponse(decryptedBytes));
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
            byte[] dataBytes = Encoding.Default.GetBytes(data);

            byte le = 8;

            InitializeCipherOperation(keyBytes, dataBytes, out byte[] setGuestResponse, out byte[] setGenSecretResponse, out byte[] setGen3DESResponse,
                out byte[] setInputResponse);

            var successSw = new byte[] {0x90, 0x00};

            InitializationErrorCheck(setGuestResponse, successSw, setGenSecretResponse, setGen3DESResponse, setInputResponse);

            byte[] encryptedBytes = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT);

            if (encryptedBytes.Length <= 2)
            {
                le = encryptedBytes[1];
                encryptedBytes = GetResponseData(encryptedBytes, SecureIMCardInstructions.INS_ECC_DO_DES_CIPHER_ENCRYPT_GET_RESPONSE,
                    le);
            }

            return TrimToValidString(encryptedBytes, le);
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

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Errors the check.
        /// </summary>
        /// <param name="setGuestResponse">The set guest response.</param>
        /// <param name="successSw">The success sw.</param>
        /// <param name="setGenSecretResponse">The set gen secret response.</param>
        /// <param name="setGen3DESResponse">The set gen3 DES response.</param>
        /// <param name="setInputResponse">The set input response.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SmartcardException">
        /// </exception>
        private static void InitializationErrorCheck([NotNull] byte[] setGuestResponse, [NotNull] byte[] successSw,
            [NotNull] byte[] setGenSecretResponse, [NotNull] byte[] setGen3DESResponse,
            [NotNull] byte[] setInputResponse)
        {
            if (successSw == null) throw new ArgumentNullException(nameof(successSw));

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
        /// Trims the sw from response.
        /// </summary>
        /// <param name="responseBytes">The response bytes.</param>
        /// <returns></returns>
        [NotNull]
        private static byte[] TrimSwFromResponse(byte[] responseBytes)
        {
            if (responseBytes[responseBytes.Length - 1] == 0x00) responseBytes = TrimArray(responseBytes, 2);

            return responseBytes;
        }

        [NotNull]
        private static string TrimToValidString(byte[] byteArray, int validSizeFactor)
        {
            int overflow = byteArray.Length % validSizeFactor;
            if (overflow == 0) return Encoding.Default.GetString(byteArray);

            byteArray = TrimArray(byteArray, overflow);
            return Encoding.Default.GetString(byteArray);
        }

        [NotNull]
        private byte[] GetResponseData([NotNull] byte[] hasResponseBytes, SecureIMCardInstructions getResponseInstruction, byte le)
        {
            byte[] responseBytes;
            if (hasResponseBytes[0].Equals(0x6C))
            {
                responseBytes = SmartcardController.SendCommand(getResponseInstruction, 0, 0, null, le);
                responseBytes = TrimSwFromResponse(responseBytes);
            }
            else
            {
                throw new SmartcardException($"Decrypt failed with response: {ToHexString(hasResponseBytes)}");
            }

            return responseBytes;
        }

        private void InitializeCipherOperation([NotNull] byte[] keyBytes, [NotNull] byte[] dataBytes, [NotNull] out byte[] setGuestResponse,
            [NotNull] out byte[] setGenSecretResponse,
            [NotNull] out byte[] setGen3DESResponse, [NotNull] out byte[] setInputResponse)
        {
            setGuestResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_GUEST_PUB_KEY, 0x00, 0x00, keyBytes);
            setGenSecretResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_SECRET);
            setGen3DESResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_GEN_3DES_KEY);
            setInputResponse = SmartcardController.SendCommand(SecureIMCardInstructions.INS_ECC_SET_INPUT_TEXT, 0x00, 0x00, dataBytes);
        }

        #endregion Private Methods
    }
}