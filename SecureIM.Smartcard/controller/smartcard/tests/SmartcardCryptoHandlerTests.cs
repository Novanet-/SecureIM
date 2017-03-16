using System.Linq;
using Moq;
using NUnit.Framework;

namespace SecureIM.Smartcard.controller.smartcard.tests
{
    [TestFixture]
    public class SmartcardCryptoHandlerTests
    {
        #region Private Fields

        private MockRepository _mockRepository;
        private SmartcardCryptoHandler _smartcardCryptoHandler;
        private byte[] _pubKey;
        private byte[] _guestPubKey;
        private SmartcardController _smartcardController;

        #endregion Private Fields

        #region Public Methods

        [TearDown]
        public void TestCleanup() => _mockRepository.VerifyAll();

        [SetUp]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _smartcardCryptoHandler = new SmartcardCryptoHandler();
            _smartcardController = _smartcardCryptoHandler.SmartcardController;
            _smartcardController.ConnectToSCardReader(_smartcardController.GetSCardReaders()[0]);

            CheckSCHForNull();

            _smartcardCryptoHandler.GenerateAsymmetricKeyPair();
        }

        [Test, Order(1)]
        public void TestSCHConstructor() => CheckSCHForNull();

        [Test, Order(2)]
        public void TestSCHKeygen()
        {
            CheckSCHForNull();
            _smartcardCryptoHandler.GenerateAsymmetricKeyPair();
            _guestPubKey = _smartcardCryptoHandler.GetPublicKey();

            Assert.Greater(_guestPubKey.Length, 2);
        }

        [Test, Order(3)]
        public void TestSCHGetPubIdempotent()
        {
            CheckSCHForNull();
            for (var i = 0; i < 10; i++)
            {
                Assert.True(_smartcardCryptoHandler.GetPublicKey().SequenceEqual(_guestPubKey));
            }
        }

        [Test, Order(4)]
        public void TestSCHEncrypt()
        {
            CheckSCHForNull();
            _smartcardCryptoHandler.GenerateAsymmetricKeyPair();
            string ciphertext = _smartcardCryptoHandler.Encrypt("hello", _guestPubKey);
            Assert.NotNull(ciphertext);
        }

        private void CheckSCHForNull()
        {
            Assert.IsNotNull(_smartcardCryptoHandler);
            Assert.IsNotNull(_smartcardController);
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}

namespace SecureIM.Smartcard.controller.smartcard.tests
{
    [SetUpFixture]
    public class MySetUpClass
    {
        [SetUp]
        void RunBeforeAnyTests()
        {
            // ...
        }

        [TearDown]
        void RunAfterAnyTests()
        {
            // ...
        }
    }
}