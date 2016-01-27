using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace Program_Submission_Portal
{
    public partial class loginform : Form
    {
        bool authflag=false; //variable to flag authorization
        public loginform()
        {
            InitializeComponent();
        }
        public bool loginconfirm()
        {
            return authflag;
            
        }

        public String getUserName()
        {
            return usernametextbox.Text;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            
            
            if (usernametextbox.Text == "")
            {
                MessageBox.Show("Please Enter the Username");
                usernametextbox.Focus();
            }
            else if (passwordtextbox.Text == "")
            {
                MessageBox.Show("Please Enter the password");
                passwordtextbox.Focus();
            }
            else
            {
                if (utilities.verifyAuth(usernametextbox.Text,passwordtextbox.Text))
                {
                    authflag = true;
                    this.DestroyHandle();
                }

                else
                {
                    MessageBox.Show("Username or Password is incorrect\nPlease Contact Admin or try Again");
                    usernametextbox.Text = "";
                    passwordtextbox.Text = "";
                    usernametextbox.Focus();
                }

            }
            
        }

        private void loginform_Load(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
