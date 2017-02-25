using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SecureIM.ChatBackend.model
{
    [Serializable]
    internal class IMException : Exception
    {
        public static string DisplayMessageDelegateError { get; } = "No display message delegate has been set";

        public IMException(string message) : base(message)
        {
        }

        public IMException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IMException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}