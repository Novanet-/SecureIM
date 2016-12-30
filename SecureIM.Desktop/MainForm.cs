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
//            Debug.WriteLine("\n");
//            var controller = new SmartcardController();
////            byte[] response = controller.SendCommand(SecureIMCardInstructions.IssueCard);
//            if (response.Length > 0)
//            {
//                Debug.WriteLine("response: \n");
//
//                foreach (byte t in response)
//                    Debug.Write($"{t:X2} ");
//
//                Debug.WriteLine("\n");
//            }
//            else
//            {
//                Debug.WriteLine("ISSUE command failed\n");
//            }
        }

        #endregion Public Methods
    }
}