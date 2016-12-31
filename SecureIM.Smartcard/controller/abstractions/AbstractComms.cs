namespace SecureIM.Smartcard.controller.abstractions
{
    internal interface IComms
    {
        #region Public Methods


        /// <summary>
        ///     Recieves the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool RecieveMessage(string message);

        /// <summary>
        ///     Resolves the address.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        bool ResolveAddress(string ip);

        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        bool SendMessage(string message);


        #endregion Public Methods
    }

    internal abstract class AbstractComms : IComms
    {
        #region Public Methods


        /// <summary>
        ///     Recieves the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public abstract bool RecieveMessage(string message);

        /// <summary>
        ///     Resolves the address.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns></returns>
        public abstract bool ResolveAddress(string ip);

        /// <summary>
        ///     Sends the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public abstract bool SendMessage(string message);


        #endregion Public Methods
    }
}