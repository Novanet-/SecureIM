using System.Linq;
using Moq;
using NUnit.Framework;
using SecureIM.Smartcard.model.abstractions;
using SecureIM.Smartcard.model.smartcard;

namespace SecureIM.Smartcard.controller.smartcard.tests
{
    [TestFixture]
    public class SmartcardCryptoHandlerTests
    {
        #region Private Fields

        private static ICryptoHandler _smartcardCryptoHandler;
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
        [Order(4)]
        public void TestSCHDecrypt()
        {
            const string plaintext = "hello";
            string ciphertext = _smartcardCryptoHandler.Encrypt(plaintext, _guestPubKey);
            string decryptedText = _smartcardCryptoHandler.Decrypt(ciphertext, _guestPubKey);
            Assert.NotNull(ciphertext);
            Assert.NotNull(decryptedText);
            Assert.AreEqual(decryptedText, plaintext);
        }

        [Test]
        [Order(3)]
        public void TestSCHEncrypt()
        {
            string ciphertext = _smartcardCryptoHandler.Encrypt("hello", _guestPubKey);
            Assert.NotNull(ciphertext);
        }

        [Test]
        [Order(5)]
        public void TestSCHEncryptAndDecryptEmptyString()
        {
            string ciphertext = _smartcardCryptoHandler.Encrypt("", _guestPubKey);
            Assert.AreEqual(0, ciphertext.Length);
            string plaintext = _smartcardCryptoHandler.Decrypt(ciphertext, _guestPubKey);
            Assert.AreEqual(0, plaintext.Length);
        }

        [Test]
        [Order(6)]
        public void TestSCHEncryptAndDecryptEmptyPubKey()
        {
            Assert.Throws<SmartcardException>(() =>
            {
                _smartcardCryptoHandler.Encrypt("hello", new byte[] {});
            });
            Assert.Throws<SmartcardException>(() =>
            {
                _smartcardCryptoHandler.Decrypt("hello", new byte[] {});
            });
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
            Assert.IsNotNull(_smartcardCryptoHandler.SmartcardController);
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
                SmartcardController smartcardController = _smartcardCryptoHandler.SmartcardController;

                smartcardController.ConnectToSCardReader(smartcardController.GetSCardReaders()[0]);

                _smartcardCryptoHandler.GenerateAsymmetricKeyPair();
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}