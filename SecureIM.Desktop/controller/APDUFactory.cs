using PCSC.Iso7816;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSC;

// ReSharper disable InconsistentNaming

namespace SecureIM.Desktop.controller
{
    internal static class APDUFactory
    {
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
    }
}