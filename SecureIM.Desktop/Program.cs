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
//            if ((args.Length > 0) && args[0].Equals("server"))
//            {
//                new Application(Platform.Detect).Run(new MainForm());
//            }
//            else
//            {
                new Application(Platform.Detect).Run(new MainForm());
//            }
        }

        #endregion Public Methods
    }
}