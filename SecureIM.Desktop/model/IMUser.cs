﻿using SecureIM.Desktop.model.abstractions;
using System;

namespace SecureIM.Desktop.model
{
    // ReSharper disable once InconsistentNaming
    internal class IMUser : AsynchronousClient
    {
        public bool RecieveMessage(string message)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}