using System.Runtime.Serialization;
using System.ServiceModel;

namespace SecureIM.ChatBackend
{
    public delegate void DisplayMessageDelegate(CompositeType data);

    [ServiceContract]
    public interface IChatBackend
    {
        #region Public Methods

        [OperationContract(IsOneWay = true)]
        void DisplayMessage(CompositeType composite);

        void SendMessage(string text);

        #endregion Public Methods
    }

    [DataContract]
    public class CompositeType
    {
        #region Public Constructors

        public CompositeType()
        {
        }

        public CompositeType(string u, string m)
        {
            Username = u;
            Message = m;
        }

        #endregion Public Constructors

        #region Public Properties

        [DataMember]
        public string Message { get; set; } = "";

        [DataMember]
        public string Username { get; set; } = "Anonymous";

        #endregion Public Properties
    }
}