using System.Linq;
using Moq;
using NUnit.Framework;

namespace SecureIM.Smartcard.controller.smartcard.tests
{
    [TestFixture]
    public class SmartcardCryptoHandlerTests
    {
        #region Private Fields

        private static SmartcardController _smartcardController;
        private static SmartcardCryptoHandler _smartcardCryptoHandler;
        private byte[] _guestPubKey;
        private MockRepository _mockRepository;

        #endregion Private Fields

        #region Public Methods

        [TearDown]
        public void TestCleanup() => _mockRepository.VerifyAll();

        [SetUp]
        public void TestInitialize()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            CheckSCHForNull();
        }

        [Test]
        [Order(3)]
        public void TestSCHEncrypt()
        {
            string ciphertext = _smartcardCryptoHandler.Encrypt("hello", _guestPubKey);
            Assert.NotNull(ciphertext);
        }

        [Test]
        [Order(2)]
        public void TestSCHGetPubIdempotent()
        {
            for (var i = 0; i < 100; i++)
                Assert.True(_smartcardCryptoHandler.GetPublicKey().SequenceEqual(_guestPubKey));
        }

        [Test]
        [Order(1)]
        public void TestSCHKeygen()
        {
            _smartcardCryptoHandler.GenerateAsymmetricKeyPair();
            _guestPubKey = _smartcardCryptoHandler.GetPublicKey();

            Assert.Greater(_guestPubKey.Length, 2);
        }

        #endregion Public Methods

        #region Private Methods

        private static void CheckSCHForNull()
        {
            Assert.IsNotNull(_smartcardCryptoHandler);
            Assert.IsNotNull(_smartcardController);
        }

        #endregion Private Methods

        #region Public Classes

        [SetUpFixture]
        public class SCHTestsSetup
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
                _smartcardCryptoHandler = new SmartcardCryptoHandler();
                _smartcardController = _smartcardCryptoHandler.SmartcardController;
                _smartcardController.ConnectToSCardReader(_smartcardController.GetSCardReaders()[0]);

                _smartcardCryptoHandler.GenerateAsymmetricKeyPair();
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}