using System.Diagnostics.CodeAnalysis;

namespace SecureIM.Smartcard.model.smartcard
{
    /// <summary>
    /// SCIM_INS_CODES
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SCIM_INS_CODES
    {
        #region Public Fields

        public const byte INS_ECC_GEN_KEYPAIR = 0x41;
        public const byte INS_ECC_GENA = 0x42;
        public const byte INS_ECC_GENP = 0x43;
        public const byte INS_ECC_GET_S = 0x44;
        public const byte INS_ECC_GET_W = 0x45;
        public const byte INS_ECC_SET_S = 0x46;
        public const byte INS_ECC_SET_GUEST_W = 0x47;
        public const byte INS_ECC_SIGN = 0x48;
        public const byte INS_ECC_VERIFY = 0x49;
        public const byte INS_ECC_GEN_SECRET = 0x50;
        public const byte INS_ECC_GEN_3DES_KEY = 0x51;
        public const byte INS_ECC_SET_INPUT_TEXT = 0x59;
        public const byte INS_ECC_DO_DES_CIPHER = 0x70;

        #endregion Public Fields
    }
}