using JetBrains.Annotations;
using PCSC;
using PCSC.Iso7816;
using SecureIM.Smartcard.model.smartcard;

// ReSharper disable InconsistentNaming

namespace SecureIM.Smartcard.controller.smartcard
{
    /// <summary>
    /// APDUFactory
    /// </summary>
    internal static class APDUFactory
    {
        #region Private Properties

        private static SCardProtocol SCardProtocol { get; } = SCardProtocol.T0;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Does the cipher.
        /// </summary>
        /// <param name="decrypt">if set to <c>true</c> [decrypt].</param>
        /// <param name="expectedLength">The expected length.</param>
        /// <exception cref="SmartcardException"></exception>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu DO_CIPHER(bool decrypt, byte expectedLength = 0)
        {
            var apdu = new CommandApdu(IsoCase.Case2Short, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_DO_DES_CIPHER,
                P1 = (byte) (decrypt ? 0x01 : 0x00),
                P2 = 0x0
            };

            if (expectedLength > 0)
                apdu.Le = expectedLength;

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

        /// <summary>
        /// Gens the ECC keypair.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu ECC_GEN_KEYPAIR()
        {
            var apdu = new CommandApdu(IsoCase.Case1, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_GEN_KEYPAIR,
                P1 = 0x0,
                P2 = 0x0 // We don't know the ID tag size
            };

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

        /// <summary>
        /// Gens the DES key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GEN_DES_KEY()
        {
            var apdu = new CommandApdu(IsoCase.Case2Short, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_GEN_3DES_KEY,
                P1 = 0x0,
                P2 = 0x0,
                Le = 0x18
            };

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

        /// <summary>
        /// Gens the secret.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GEN_SECRET()
        {
            var apdu = new CommandApdu(IsoCase.Case2Short, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_GEN_SECRET,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
                Le = 0x18
            };

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

//        /// <summary>
//        /// Gets the pri key.
//        /// </summary>
//        /// <returns></returns>
//        /// <exception cref="System.Exception">Invalid APDU</exception>
//        [NotNull]
//        public static CommandApdu GET_PRI_KEY()
//        {
//            var apdu = new CommandApdu(IsoCase.Case2Short, SCardProtocol)
//            {
//                CLA = 0x80,
//                INS = SCIM_INS_CODES.INS_ECC_GET_S,
//                P1 = 0x0,
//                P2 = 0x0, // We don't know the ID tag size
//                Le = 0x18
//            };
//
//            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");
//
//            return apdu;
//        }

        /// <summary>
        /// Gets the pub key.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GET_PUB_KEY()
        {
            var apdu = new CommandApdu(IsoCase.Case2Short, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_GET_W,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
                Le = 0x31
            };

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

        /// <summary>
        /// Selects the specified applet.
        /// </summary>
        /// <param name="appletAID">The applet aid.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu SELECT([NotNull] byte[] appletAID)
        {
            var selectAPDU = new CommandApdu(IsoCase.Case3Short, SCardProtocol)
            {
                CLA = 0x00,
                INS = 0xA4,
                P1 = 0x04,
                P2 = 0x00,
                Data = appletAID
            };

            if (!selectAPDU.IsValid) throw new SmartcardException("Invalid APDU");

            return selectAPDU;
        }

        /// <summary>
        /// Sets the guest pub key.
        /// </summary>
        /// <param name="pubKeyBytes">The pub key bytes.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu SET_GUEST_PUB_KEY([NotNull] byte[] pubKeyBytes)
        {
            var apdu = new CommandApdu(IsoCase.Case3Short, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_SET_GUEST_W,
                P1 = 0x0,
                P2 = 0x0,
                Data = pubKeyBytes
            };

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

        /// <summary>
        /// Sets the input text.
        /// </summary>
        /// <param name="inputTextBytes">The input text bytes.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu SET_INPUT_TEXT([NotNull] byte[] inputTextBytes)
        {
            var apdu = new CommandApdu(IsoCase.Case3Short, SCardProtocol)
            {
                CLA = 0x80,
                INS = SCIM_INS_CODES.INS_ECC_SET_INPUT_TEXT,
                P1 = 0x0,
                P2 = 0x0,
                Data = inputTextBytes
            };

            if (!apdu.IsValid) throw new SmartcardException("Invalid APDU");

            return apdu;
        }

        #endregion Public Methods
    }
}