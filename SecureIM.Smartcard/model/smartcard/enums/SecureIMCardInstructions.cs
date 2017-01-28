// ReSharper disable InconsistentNaming

namespace SecureIM.Smartcard.model.smartcard.enums
{
    public enum SecureIMCardInstructions
    {
        INS_ECC_GEN_KEYPAIR,
        INS_ECC_GET_S,
        INS_ECC_GET_W,
        INS_ECC_SET_S,
        INS_ECC_SET_GUEST_W,
        INS_ECC_GEN_SECRET,
        INS_ECC_GEN_3DES_KEY,
        INS_ECC_SET_INPUT_TEXT,
        INS_ECC_DO_DES_CIPHER,
        INS_SELECT_SCIM
    }
}