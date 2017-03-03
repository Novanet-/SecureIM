using System;

namespace SecureIM.ChatBackend.model
{
    [Flags]
    public enum MessageFlags
    {
        None = 0,
        Encoded = 1,
        Encrypted = 4,
        Broadcast = 8,
        Local = 16
    }
}