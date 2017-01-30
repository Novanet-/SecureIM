namespace SecureIM.Smartcard.model.smartcard.enums
{
    public enum SecureIMCardInstructions
    {
        INS_ECC_GEN_KEYPAIR,
        INS_ECC_GET_PRI_KEY,
        INS_ECC_GET_PUB_KEY,
        INS_ECC_SET_GUEST_PUB_KEY,
        INS_ECC_GEN_SECRET,
        INS_ECC_GEN_3DES_KEY,
        INS_ECC_SET_INPUT_TEXT,
        INS_ECC_DO_DES_CIPHER_ENCRYPT,
        INS_ECC_DO_DES_CIPHER_DECRYPT,
        INS_SELECT_SCIM,
        INS_ECC_DO_DES_CIPHER_ENCRYPT_GET_RESPONSE,
        INS_ECC_DO_DES_CIPHER_DECRYPT_GET_RESPONSE
    }
}