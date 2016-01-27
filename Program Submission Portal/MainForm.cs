using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Program_Submission_Portal
{
    public partial class MainForm : Form
    {
        String username = "UserName";
        int timeleft = 0;
        int totalscore = 0;
        TabPage[] questions;
        Button[] submit;
        Label[] status;
        ProgressBar[] animation;
        Label[] detailedquestion;
        Button button;
        public MainForm(String username, int timeleft, int totalscore)
        {

            InitializeComponent();
            this.username = username;
            this.timeleft = timeleft;
            this.totalscore = totalscore;
            this.questions = new TabPage[utilities.getTotalQuestons()];
            this.submit = new Button[questions.Length];
            this.status = new Label[questions.Length];
            this.animation = new ProgressBar[questions.Length];
            this.detailedquestion = new Label[questions.Length];
        }

        private void countdown_Tick(object sender, EventArgs e)
        {
            if (RemainingTime.Value <RemainingTime.Maximum)
            {
                RemainingTime.Value += 1;
                TimeLeft.Text = timeleftindicator();
            }
            else
            {
                countdown.Dispose();
                QuestionPanel.Enabled = false;
                MessageBox.Show("Times Up!\n Thankyou " + username + " for participting");
                
            }
        }

        private string timeleftindicator() {
            int seconds = RemainingTime.Maximum-RemainingTime.Value;
            int hours=0;
            int minutes=0;
            if (seconds >=60){
                minutes = seconds / 60;
                seconds = seconds % 60;
                if (minutes >= 60)
                {
                    hours = minutes / 60;
                    minutes = minutes % 60;
                }
            }
            string hour, minute, second;


            hour = stringizer(hours);
            minute = stringizer(minutes);
            second = stringizer(seconds);
                return hour+":"+minute+":"+second;
        }

        private string stringizer(int i)
        {
            string temp = "";
            if (i == 0)
            {
                temp = "00";
            }
            else if (i > 0 && i < 10)
            {
                temp = "0" + i;
            }
            else
            {
                temp = "" + i;
            }
            return temp;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.welcometext.Text = "Welcome " + username;
            this.QuestionPanel.Controls.Remove(tabPage1);
            tabPage1.Dispose();
            if (timeleft>0)
            {
                for (int i = 0; i < questions.Length; i++)
                {

                    questions[i] = new TabPage("Question:-" + (i + 1));
                    questions[i].UseVisualStyleBackColor = true;
                    animation[i] = new ProgressBar();
                    detailedquestion[i] = new Label();
                    status[i] = new Label();
                    submit[i] = new Button();
                    //submit button
                    this.submit[i].Dock = System.Windows.Forms.DockStyle.Bottom;
                    this.submit[i].Location = new System.Drawing.Point(3, 148);
                    this.submit[i].Name = "submit" + i;
                    this.submit[i].Size = new System.Drawing.Size(313, 23);
                    this.submit[i].TabIndex = 3;
                    this.submit[i].Text = "Choose File";
                    this.submit[i].UseVisualStyleBackColor = true;
                    this.submit[i].Click += new System.EventHandler(this.button1_Click);
                    //status
                    this.status[i].AutoSize = true;
                    this.status[i].Dock = System.Windows.Forms.DockStyle.Bottom;
                    this.status[i].Location = new System.Drawing.Point(3, 171);
                    this.status[i].Name = "status" + i;
                    this.status[i].Size = new System.Drawing.Size(0, 13);
                    //question detail
                    this.detailedquestion[i].AutoSize = true;
                    this.detailedquestion[i].Dock = System.Windows.Forms.DockStyle.Top;
                    this.detailedquestion[i].Location = new System.Drawing.Point(3, 3);
                    this.detailedquestion[i].Name = "detailedquestion" + i;
                    this.detailedquestion[i].Size = new System.Drawing.Size(35, 13);
                    this.detailedquestion[i].TabIndex = 0;
                    this.detailedquestion[i].Text = utilities.getQuestion(i);
                    this.detailedquestion[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    //animation
                    this.animation[i].Dock = System.Windows.Forms.DockStyle.Bottom;
                    this.animation[i].Location = new System.Drawing.Point(3, 184);
                    this.animation[i].MarqueeAnimationSpeed = 1;
                    this.animation[i].Name = "animation" + i;
                    this.animation[i].Size = new System.Drawing.Size(313, 12);
                    this.animation[i].Step = 100;
                    this.animation[i].Style = System.Windows.Forms.ProgressBarStyle.Marquee;
                    this.animation[i].TabIndex = 1;
                    this.animation[i].Value = 3;
                    this.animation[i].Visible = false;
                    //Question Pages
                    this.questions[i].Controls.Add(this.animation[i]);
                    this.questions[i].Controls.Add(this.submit[i]);
                    this.questions[i].Controls.Add(this.detailedquestion[i]);
                    this.questions[i].Controls.Add(this.status[i]);
                    //

                    this.QuestionPanel.Controls.Add(questions[i]);
                }
                foreach (int i in utilities.attemptedquestions(username))
                {
                    try
                    {
                        submit[i].Enabled = false;
                        status[i].Visible = true;
                        animation[i].Visible = true;
                        AcceptAnswer(i);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        MessageBox.Show("It seems that you have selected wrong question database, this program will now exit");
                        Application.Exit();
                    }

                } 
            }

            RemainingTime.Maximum = timeleft * 60;
            TimeLeft.Text = timeleftindicator() ;
            score.Text = "Score: "+stringizer(totalscore);
            countdown.Start();
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string filepath;
            string filename;
            button = (Button)sender;
            openFileDialog1.ShowDialog();
            filepath =openFileDialog1.FileName;
            filename = openFileDialog1.SafeFileName;
            button.Text = "Selected file: " + filename;
            int key=int.Parse(button.Name.TrimStart("submit".ToCharArray()));
            StartChecking(key, filepath, filename);
        }

        private void StartChecking(int key, string filepath, string filename)
        {
            submit[key].Enabled = false;
            status[key].Visible = true;
            status[key].Text = "Verifying Code";
            animation[key].Visible = true;
            int code=utilities.compile(filepath, filename, key);
            if(code==10)//successfully verified
            {
                AcceptAnswer(key);
                utilities.scoreincrement(username, 20, key);
                score.Text = "Score: " + stringizer(utilities.getScore(username));
            }
            else if (code == 1)
            {
                animation[key].Visible = false;
                status[key].Text = "Unexpeted output, please check the logic again or format your input/output as shown in example output";
                submit[key].Enabled = true;
            }
            else if (code == 0)
            {
                animation[key].Visible = false;
                status[key].Text = "Compilation failed.";
                submit[key].Enabled = true;
            }
            else if(code == 2)
            {
                animation[key].Visible = false;
                status[key].Text = "Code is not responding. try removing getch() or any unnecessary pause or input statements";
                submit[key].Enabled = true;
            }
        }
        private void AcceptAnswer(int key)
        {
            submit[key].Text += " : Accepted";
            status[key].Text = "Accepted";
            animation[key].Style = ProgressBarStyle.Continuous;
            animation[key].Value = animation[key].Maximum;
        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

    }
}