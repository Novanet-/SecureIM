using System.Diagnostics.CodeAnalysis;

namespace SecureIM.WPF.model.smartcard
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SCIM_INS_CODES
    {
        #region Public Fields

        public const byte INS_ECC_GEN_KEYPAIR = (byte)0x41;
        public const byte INS_ECC_GENA = (byte)0x42;
        public const byte INS_ECC_GENP = (byte)0x43;
        public const byte INS_ECC_GET_S = (byte)0x44;
        public const byte INS_ECC_GET_W = (byte)0x45;
        public const byte INS_ECC_SET_S = (byte)0x46;
        public const byte INS_ECC_SET_GUEST_W = (byte)0x47;
        public const byte INS_ECC_SIGN = (byte)0x48;
        public const byte INS_ECC_VERIFY = (byte)0x49;
        public const byte INS_ECC_GEN_SECRET = (byte)0x50;
        public const byte INS_ECC_GEN_3DES_KEY = (byte)0x51;
        public const byte INS_ECC_SET_INPUT_TEXT = (byte)0x59;
        public const byte INS_ECC_DO_DES_CIPHER = (byte)0x70;

        #endregion Public Fields
    }
}