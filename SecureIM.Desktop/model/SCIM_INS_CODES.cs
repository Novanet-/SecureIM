namespace SecureIM.Desktop.model
{
    public static class SCIM_INS_CODES
    {
        #region Public Fields

        public const byte InsDecrypt = (byte) 0xD0;
        public const byte InsEncrypt = (byte) 0xE0;
        public const byte InsIssue = (byte) 0x40;
        public const byte InsSetPrivExp = (byte) 0x22;
        public const byte InsSetPrivModulus = (byte) 0x12;
        public const byte InsSetPubExp = (byte) 0x32;
        public const byte InsSetPubModulus = (byte) 0x02;

        #endregion Public Fields
    }
}