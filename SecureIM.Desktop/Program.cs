using Eto;
using Eto.Forms;
using System;

namespace SecureIM.Desktop
{
    public class Program
    {
        #region Public Methods

        [STAThread]
        public static void Main(string[] args)
        {
            if (args[0].Equals("server"))
            {
            }
            else
            {
                new Application(Platform.Detect).Run(new MainForm());
            }
        }

        #endregion Public Methods
    }
}