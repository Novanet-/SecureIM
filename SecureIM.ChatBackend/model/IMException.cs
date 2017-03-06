using System;
using System.Runtime.Serialization;

namespace SecureIM.ChatBackend.model
{
    /// <summary>
    /// IMException
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class IMException : Exception
    {
        #region Public Properties

        public static string DisplayMessageDelegateError { get; } = "No display message delegate has been set";

        #endregion Public Properties


        #region Public Constructors

        public IMException(string message) : base(message) { }

        public IMException(string message, Exception innerException) : base(message, innerException) { }

        #endregion Public Constructors


        #region Protected Constructors

        protected IMException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #endregion Protected Constructors
    }
}