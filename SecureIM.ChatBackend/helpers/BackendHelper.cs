using JetBrains.Annotations;
using SecureIM.Smartcard.helpers;
using static System.Text.Encoding;

namespace SecureIM.ChatBackend.helpers
{
    internal static class BackendHelper
    {
        #region Internal Methods

        /// <summary>
        /// Decodes to byte array base64.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        [CanBeNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        internal static byte[] DecodeToByteArrayBase64([NotNull] string data) => Default.GetBytes(Default.DecodeBase64(data));

        /// <summary>
        /// Encodes from byte array base64.
        /// </summary>
        /// <param name="dataBytes">The data bytes.</param>
        /// <returns></returns>
        [CanBeNull]
        internal static string EncodeFromByteArrayBase64([NotNull] byte[] dataBytes) => Default.EncodeBase64(Default.GetString(dataBytes));

        #endregion Internal Methods
    }
}