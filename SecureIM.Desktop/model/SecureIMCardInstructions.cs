using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace SecureIM.Desktop.model
{
    internal enum SecureIMCardInstructions
    {
        RSA_GENERATE_KEY_PAIR,
        RSA_GET_PUBLIC_KEY,
        RSA_GET_PRIVATE_KEY,
        RSA_SET_PUBLIC_KEY,
        RSA_SET_PRIVATE_KEY,
        RSA_SIGN,
        RSA_VERIFY,
        RSA_DO_CIPHER,
        CARD_RESET
    }
}