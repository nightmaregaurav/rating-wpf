using System;
using System.Windows.Forms;

namespace rating
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //new DBManager().add_user("ADMIN", "admin", Modules.Crypto.Sha256("admin"), 0);//uncomment this and run project to add default admin user. close application without doing anything else, comment this line and run again.
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Frames.Login());
        }
    }
}