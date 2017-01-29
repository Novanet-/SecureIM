using System;
using JetBrains.Annotations;
using PCSC;
using PCSC.Iso7816;

// ReSharper disable InconsistentNaming

namespace SecureIM.Smartcard.controller.smartcard
{
    internal static class APDUFactory
    {
        public static SCardProtocol ScardProtocol { get; private set; } = SCardProtocol.T0;


        #region Public Methods


        /// <summary>
        ///     Selects the specified applet.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <param name="appletAID">The applet aid.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu SELECT(SCardProtocol scardProtocol, [NotNull] byte[] appletAID)
        {
            var selectAPDU = new CommandApdu(IsoCase.Case3Short, ScardProtocol)
            {
                CLA = 0x00,
                INS = 0xA4,
                P1 = 0x04,
                P2 = 0x00,
                Data = appletAID
            };

            if (!selectAPDU.IsValid) throw new Exception("Invalid APDU");

            return selectAPDU;
        }

        /// <summary>
        ///     Gens the ECC keypair.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu ECC_GEN_KEYPAIR(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case1, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x41,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Gets the pri key.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GET_PRI_KEY(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x44,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Gets the pub key.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GET_PUB_KEY(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x45,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Sets the guest pub key.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <param name="pubKeyBytes">The pub key bytes.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu SET_GUEST_PUB_KEY(SCardProtocol scardProtocol, [NotNull] byte[] pubKeyBytes)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x47,
                P1 = 0x0,
                P2 = 0x0,
                Data = pubKeyBytes
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Gens the secret.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GEN_SECRET(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x50,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Gens the DES key.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu GEN_DES_KEY(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x51,
                P1 = 0x0,
                P2 = 0x0
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Sets the input text.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <param name="inputTextBytes">The input text bytes.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu SET_INPUT_TEXT(SCardProtocol scardProtocol, [NotNull] byte[] inputTextBytes)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x59,
                P1 = 0x0,
                P2 = 0x0,
                Data = inputTextBytes
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }

        /// <summary>
        ///     Does the cipher.
        /// </summary>
        /// <param name="scardProtocol">The scard protocol.</param>
        /// <param name="decrypt">if set to <c>true</c> [decrypt].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Invalid APDU</exception>
        [NotNull]
        public static CommandApdu DO_CIPHER(SCardProtocol scardProtocol, bool decrypt)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x70,
                P1 = (byte) (decrypt ? 0x01 : 0x00),
                P2 = 0x0
            };

            if (!issueAPDU.IsValid) throw new Exception("Invalid APDU");

            return issueAPDU;
        }


        #endregion Public Methods
    }
}