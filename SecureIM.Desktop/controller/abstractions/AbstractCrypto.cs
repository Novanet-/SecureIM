﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureIM.Desktop.model.abstractions
{
    internal interface ICrypto
    {

        #region Public Methods

        string DecryptAes();
        string DecryptRsa();
        string DecryptSha();

        string EncryptAes();
        string EncryptRsa();
        string EncryptSha();

        #endregion Public Methods

    }

    internal abstract class AbstractCrypto : ICrypto
    {
        #region Public Methods

        public abstract string DecryptAes();
        public abstract string DecryptRsa();
        public abstract string DecryptSha();

        public abstract string EncryptAes();
        public abstract string EncryptRsa();
        public abstract string EncryptSha();

        #endregion Public Methods
    }
}