using System.Runtime.Serialization;
using System.ServiceModel;

namespace SecureIM.ChatBackend
{
    public delegate void DisplayMessageDelegate(CompositeType data);

    [ServiceContract]
    public interface IChatBackend
    {
        #region Public Methods


        /// <summary>
        /// Displays the message.
        /// </summary>
        /// <param name="composite">The composite.</param>
        [OperationContract(IsOneWay = true)]
        void DisplayMessage(CompositeType composite);

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="text">The text.</param>
        void SendMessage(string text);


        #endregion Public Methods
    }

    [DataContract]
    public class CompositeType
    {
        #region Public Properties


        [DataMember] public string Message { get; set; } = "";
        [DataMember] public string Username { get; set; } = "Anonymous";


        #endregion Public Properties




        #region Public Constructors


        public CompositeType() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeType"/> class.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="m">The m.</param>
        public CompositeType(string u, string m)
        {
            Username = u;
            Message = m;
        }


        #endregion Public Constructors
    }
}