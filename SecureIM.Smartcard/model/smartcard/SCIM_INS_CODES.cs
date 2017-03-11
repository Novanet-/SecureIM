using System.Diagnostics.CodeAnalysis;

namespace SecureIM.Smartcard.model.smartcard
{
    /// <summary>
    /// SCIM_INS_CODES
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class SCIM_INS_CODES
    {
        #region Public Fields

        internal const byte INS_ECC_GEN_KEYPAIR = 0x41;
        internal const byte INS_ECC_GET_S = 0x44;
        internal const byte INS_ECC_GET_W = 0x45;
        internal const byte INS_ECC_SET_GUEST_W = 0x47;
        internal const byte INS_ECC_GEN_SECRET = 0x50;
        internal const byte INS_ECC_GEN_3DES_KEY = 0x51;
        internal const byte INS_ECC_SET_INPUT_TEXT = 0x59;
        internal const byte INS_ECC_DO_DES_CIPHER = 0x70;

        #endregion Public Fields
    }
}