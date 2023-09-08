using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Warcraft_II_Port_Test
{
    /// <summary>
    /// Application main calss.
    /// </summary>
    public static class Program
    {
        #region static method Main

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new wfrm_Main());
        }

        #endregion
    }
}