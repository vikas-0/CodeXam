using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Program_Submission_Portal
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            loginform init = new loginform();
            MainForm main;
            Application.Run(init);
            string username;
            if (init.loginconfirm() == true)
            {
                username = init.getUserName().ToLower();
                int totalscore = utilities.getScore(username);
                int timeleft =  utilities.getRemainingTime(username, utilities.getTotalQuestons()*20);
                main = new MainForm(username, timeleft, totalscore );
                Application.Run(main);
            }
          
        }
    }
}
