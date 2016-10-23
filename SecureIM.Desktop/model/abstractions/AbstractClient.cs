using System;
using System.Collections.Generic;
using System.Linq;

namespace SecureIM.Desktop.model.abstractions
{
    internal interface IClient
    {
        #region Public Methods

        string RandomString(int length);

        bool RecieveMessage(string message);

        bool SendMessage(string message);

        #endregion Public Methods
    }

    internal abstract class AbstractClient : IClient
    {
        #region Private Fields

        private List<string> _messageQueue;
        private string _userId;

        #endregion Private Fields

        #region Protected Constructors

        protected AbstractClient()
        {
            _userId = RandomString(16);
            _messageQueue = new List<string>();
        }

        #endregion Protected Constructors

        #region Public Methods

        public string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public abstract bool RecieveMessage(string message);

        public abstract bool SendMessage(string message);

        #endregion Public Methods
    }
}