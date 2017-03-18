using System.Diagnostics;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using SecureIM.ChatBackend.model;

namespace SecureIM.ChatBackend.tests
{
    [TestFixture]
    public class ChatBackendTests
    {
        #region Private Properties

        private static IChatBackend Backend { get; } = ChatBackend.Instance;
        private MockRepository MockRepository { get; set; }

        #endregion Private Properties

        #region Public Methods

        [TearDown]
        public void TestCleanup() => MockRepository.VerifyAll();

        [SetUp]
        public void TestInitialize()
        {
            MockRepository = new MockRepository(MockBehavior.Strict);
        }

        [Test]
        public void TestSendMessage()
        {
//            Backend.SendMessage("hello");
        }

        #endregion Public Methods

        #region Private Methods

        private static void DisplayMessage([NotNull] MessageComposite messagecomposite)
        {
            Debug.WriteLine(messagecomposite.Message.Text);
            Debug.WriteLine(messagecomposite.Sender.Name);
            Debug.WriteLine(messagecomposite.Receiver.Name);
        }

        #endregion Private Methods

        #region Public Classes

        [SetUpFixture]
        public class ChatBackendTestsSetup
        {
            #region Public Methods

            [OneTimeTearDown]
            public void RunAfterAnyTests()
            {
                // ...
            }

            [OneTimeSetUp]
            public void RunBeforeAnyTests()
            {
                Backend.DisplayMessageDelegate = DisplayMessage;
                Backend.StartService();
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}