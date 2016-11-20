using PCSC.Iso7816;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PCSC;

// ReSharper disable InconsistentNaming

namespace SecureIM.Desktop.controller
{
    internal static class APDUFactory
    {
        #region Public Methods

        public static CommandApdu SELECT(byte[] appletAID, SCardProtocol scardProtocol)
        {
            var selectAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x0,
                INS = 0xA4,
                Data = appletAID,
                P1 = 0x04,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!selectAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return selectAPDU;
        }

        public static CommandApdu ISSUE(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case2Short, scardProtocol)
            {
                CLA = 0x0,
                INS = 0x40,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
                Le = 0x01
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        

        public static CommandApdu SET_PUB_EXP(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x0,
                INS = 0x32,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
                Data = null
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu SET_PUB_MOD(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x0,
                INS = 0x02,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu SET_PRIV_EXP(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x0,
                INS = 0x22,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag sizE
            };

            if (!issueAPDU.IsValid)
            {
                throw new Exception("Invalid APDU");
            }

            return issueAPDU;
        }

        public static CommandApdu SET_PRIV_MOD(SCardProtocol scardProtocol)
        {
            var issueAPDU = new CommandApdu(IsoCase.Case3Short, scardProtocol)
            {
                CLA = 0x0,
                INS = 0x12,
                P1 = 0x0,
                P2 = 0x0, // We don't know the ID tag size
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