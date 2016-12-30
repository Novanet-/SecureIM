using System;
using PCSC;
using PCSC.Iso7816;

// ReSharper disable InconsistentNaming

namespace SecureIM.WPF.controller.smartcard
{
    internal static class APDUFactory
    {
        #region Public Methods

        public static CommandApdu SELECT(SCardProtocol scardProtocol, byte[] appletAID)
        {
            var selectAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0xA4,
                P1 = 0x04,
                P2 = 0x0,
                Data = appletAID
            };

            if (!selectAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return selectAPDU;
        }

        public static CommandApdu ECC_GEN_KEYPAIR(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case1, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x41,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
                Le = 0x00
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu GET_PRI_KEY(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x44,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu GET_PUB_KEY(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x45,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu SET_GUEST_PUB_KEY(SCardProtocol scardProtocol, byte[] pubKeyBytes)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x47,
                P1 = 0x0,
                P2 = 0x0,
                Data = pubKeyBytes
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu GEN_SECRET(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x50,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu GEN_DES_KEY(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x51,
                P1 = 0x0,
                P2 = 0x0
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu SET_INPUT_TEXT(SCardProtocol scardProtocol, byte[] inputTextBytes)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x59,
                P1 = 0x0,
                P2 = 0x0,
                Data = inputTextBytes
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu DO_CIPHER(SCardProtocol scardProtocol, bool decrypt)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x80,
                INS = 0x70,
                P1 = (byte)(decrypt ? 0x01 : 0x00),
                P2 = 0x0
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        #endregion Public Methods
    }
}