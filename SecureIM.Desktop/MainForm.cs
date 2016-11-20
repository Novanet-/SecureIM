using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Eto.Forms;
using PCSC;
using PCSC.Iso7816;
using SecureIM.Desktop.controller;
using SecureIM.Desktop.model;

namespace SecureIM.Desktop
{
    public partial class MainForm
    {
        #region Private Fields

        private static byte INS_DECRYPT = (byte) 0xD0;
        private static byte INS_ENCRYPT = (byte) 0xE0;
        private static byte INS_ISSUE = (byte) 0x40;
        private static byte INS_SET_PRIV_EXP = (byte) 0x22;
        private static byte INS_SET_PRIV_MODULUS = (byte) 0x12;
        private static byte INS_SET_PUB_EXP = (byte) 0x32;
        private static byte INS_SET_PUB_MODULUS = (byte) 0x02;
        private static byte STATE_INIT = 0;
        private static byte STATE_ISSUED = 1;
        private CommandApdu _selectApdu;
        private byte[] SECUREIMCARD_AID = new byte[] {0xA0, 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x10, 0x01};

        #endregion Private Fields

        #region Public Constructors

        public MainForm()
        {
//            InitializeComponent();
//            AsynchronousClient.StartClient();

            SmartcardTest();
            Application.Instance.Quit();
        }

        #endregion Public Constructors

        #region Public Methods

        public void SmartcardTest()
        {
            Debug.WriteLine("\n");
            var controller = new SmartcardController();
            byte[] response = controller.SendCommand(SecureIMCardInstructions.IssueCard);
            if (response.Length > 0)
            {
                Debug.WriteLine("response: \n");

                foreach (byte t in response)
                    Debug.Write($"{t:X2} ");

                Debug.WriteLine("\n");
            }
            else
            {
                Debug.WriteLine("ISSUE command failed\n");
            }
        }

        #endregion Public Methods
    }
}