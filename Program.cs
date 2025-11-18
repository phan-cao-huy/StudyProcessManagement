using System;
using System.Windows.Forms;
<<<<<<< HEAD
using StudyProcessManagement.Views;
using StudyProcessManagement.Views.Admin;
using StudyProcessManagement.Views.Admin.Dashboard;
using StudyProcessManagement.Views.Admin.User;
=======
using StudyProcessManagement.Views.Login;
using StudyProcessManagement.Views.Teacher;
>>>>>>> 069f66b1a88a1f5a0dc1c778d59464fe79de3f62
namespace StudyProcessManagement
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
<<<<<<< HEAD
            Application.Run(new MainForm());
=======
            // Temporarily run Form1 for testing the new UI instead of Login
            Application.Run(new Form1());
>>>>>>> 069f66b1a88a1f5a0dc1c778d59464fe79de3f62
        }
    }
}
