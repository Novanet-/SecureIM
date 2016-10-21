using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureIM.Desktop.model.abstractions
{
    internal interface IServer
    {

        #region Public Methods

        string RandomString(int length);

        bool RecieveMessage();

        bool SendMessage();

        #endregion Public Methods

    }

    internal abstract class AbstractServer : IServer
    {

        #region Private Fields

        private List<string> _messageQueue;

        private string _serverId;

        #endregion Private Fields

        #region Protected Constructors

        protected AbstractServer()
        {
            _serverId = RandomString(16);
            _messageQueue = new List<string>();
        }

        #endregion Protected Constructors

        #region Public Methods

        public string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool RecieveMessage()
        {
            return false;
        }

        public bool SendMessage()
        {
            return false;
        }

        #endregion Public Methods

    }
}