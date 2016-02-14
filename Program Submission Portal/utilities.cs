using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Collections;
using System.Security.Cryptography;
using System.Data.OleDb;
using System.Data;

namespace Program_Submission_Portal
{
    static class utilities
    {
        static string test = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        
        static string questiondatabase = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "CodeXam"),"QuestionDatabase.mdb");
        static string DatabasePassword = "iamtheone";
        static string connectionstring(string source="def")
        {
            if (source == "def")
            {
                string temp= System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "CodeXam"), "UserDatabase.mdb");
                temp = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + temp + ";Jet OLEDB:Database Password=" + DatabasePassword + "";
                return temp;
            }
            
            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+source+";Jet OLEDB:Database Password="+DatabasePassword+"";
        return strAccessConn;
        }
        static public int getRemainingTime(string user, int total_time)
        {

            DateTime currenttime=DateTime.Now;
            DateTime lasttime=new DateTime();
            bool flag = true;
            string select = "SELECT inittime from users WHERE username='"+user+"'";
            OleDbConnection con = new OleDbConnection(connectionstring());
            OleDbDataAdapter adpt = new OleDbDataAdapter(select, con);
            DataSet ds = new DataSet();
            adpt.Fill(ds, "users");
            foreach(DataRow datarow in ds.Tables[0].Rows)
            {
                string strlasttime = (string)datarow[0];
                lasttime = DateTime.FromBinary(long.Parse(strlasttime));
                    flag = false;
                    break;
            }
            if(!flag)
            {
                TimeSpan timeelapsed = currenttime - lasttime;
                if (timeelapsed.TotalMinutes >= 0 && timeelapsed.TotalMinutes < total_time)
                {
                    return total_time - (int)timeelapsed.TotalMinutes;
                }
                return 0;

            }        
            return total_time;

        }

        static public int compile(string filepath, string filename, int key)
        {

            //file type identification
            Dictionary<string, string> dict = getAnswer(key);
            string filetype = "";
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;
            pProcess.StartInfo.RedirectStandardInput = true;
            pProcess.StartInfo.WorkingDirectory = filepath.TrimEnd(filename.ToCharArray());
            pProcess.StartInfo.CreateNoWindow = true;
      
            if (filename.EndsWith(".java"))
            {
                filetype = "java";
            }
            else if (filename.EndsWith(".c"))
            {
                filetype = "c";
            }
            else if (filename.EndsWith(".cpp"))
            {
                filetype = "cpp";
            }

            if (filetype == "java")
            {
                pProcess.StartInfo.FileName = "javac";
                pProcess.StartInfo.Arguments = filename;
                pProcess.Start();
                string strOutput = pProcess.StandardOutput.ReadToEnd();
                pProcess.WaitForExit();

                //Succesfully Compiled


                if (strOutput == null || strOutput == "")
                {
                    pProcess.StartInfo.FileName = "java";
                    pProcess.StartInfo.Arguments = filename.TrimEnd(".java".ToCharArray());
                    pProcess.StartInfo.RedirectStandardInput = true;
                   // pProcess.StartInfo.CreateNoWindow = false;
                    
                    foreach(string keys in dict.Keys)
                    {
                        pProcess.Start();
                        string value="";
                        dict.TryGetValue(keys, out value);
                        pProcess.StandardInput.WriteLine(keys);
                        strOutput = pProcess.StandardOutput.ReadToEnd();
                        if (strOutput.Contains(value))
                        {
                            string blank = " ";
                            
                            blank = strOutput.Replace(value, blank);
                            
                            if (String.IsNullOrWhiteSpace(blank))
                            {
                                if (pProcess.WaitForExit(10 * 1000))
                                    continue;
                                pProcess.Dispose();
                                return 2; 
                            }
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    
                    return 10;
                }
            }

            return 0;
        }

        static public string Encrypt(string input, byte[] key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        static public string Decrypt(string input, byte[] key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        static public bool verifyAuth(string username, string password)
        {
            string select = "SELECT password from users WHERE username='"+username+"'";
            OleDbConnection con = new OleDbConnection(connectionstring());
            OleDbDataAdapter adptr = new OleDbDataAdapter(select,con);
            DataSet ds = new DataSet();
            adptr.Fill(ds, "users");
            foreach (DataRow datarow in ds.Tables[0].Rows)
            {
                if (password == datarow[0].ToString())
                {
                    select = "SELECT init from users WHERE username='" + username + "'";
                    DataSet ds1 = new DataSet();
                    adptr = new OleDbDataAdapter(select, con);
                    adptr.Fill(ds1, "users");
                    foreach(DataRow datarow1 in ds1.Tables[0].Rows)
                    {
                        bool flag = (bool)datarow1[0];
                        if (!flag)
                        {
                            inituser(username);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        static public void inituser(string username)
        {
            OleDbConnection con = new OleDbConnection(connectionstring());
            con.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = con;
            cmd.CommandText = "UPDATE users SET users.inittime='"+System.DateTime.Now.ToBinary().ToString()+"', users.init=yes WHERE users.username='"+username+"'";
            cmd.ExecuteNonQuery();
            con.Close();
        }
        static public int getScore(string username)
        {
            string select = "SELECT score from users WHERE username = '"+username+"'";
            OleDbConnection con = new OleDbConnection(connectionstring());
            OleDbDataAdapter aptr = new OleDbDataAdapter(select, con);
            DataSet ds = new DataSet();
            aptr.Fill(ds, "user");
            try
            {

                foreach (DataRow drow in ds.Tables[0].Rows)
                {
                    return (int)drow[0];
                }
            }
            catch (Exception)
            {

                return 0;
            }
            return 0;
        }
        static public void scoreincrement(string username, int score, int questionNumber)
        {
            OleDbConnection con = new OleDbConnection(connectionstring());
            con.Open();
            OleDbCommand cmd = new OleDbCommand("UPDATE users SET users.score="+(getScore(username)+score)+" WHERE users.username='"+username+"'", con);
            cmd.ExecuteNonQuery();
            con.Close();
            string formatted = "";
            int[] qs = attemptedquestions(username);
            foreach(int i in qs)
            {
                formatted += i + "#";
            }
            formatted += questionNumber + "#";
            con.Open();
            cmd = new OleDbCommand("UPDATE users SET users.attempted='" + formatted + "' WHERE users.username='" + username + "'", con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        static public int[] attemptedquestions(string username)
        {
            int[] arr = new int[0];
            string question="";
            OleDbConnection con = new OleDbConnection(connectionstring());
            OleDbDataAdapter adptr = new OleDbDataAdapter("SELECT attempted from users WHERE username='"+username+"'", con);
            DataSet ds = new DataSet();
            adptr.Fill(ds, "user");
            foreach(DataRow datarow in ds.Tables[0].Rows)
            {
                question = datarow[0].ToString();
            }
            if (question.Length > 1) {
                string[] array = question.Split('#');
                arr = new int[array.Length-1];
                int i = 0;
                foreach (string s in array)
                {

                    if (s.Length>0)
                    {
                        if (int.TryParse(s, out arr[i]))
                        {
                            i++;
                        } 
                    }
                }
            }
            
            return arr;
        }
        static public int getTotalQuestons()
        {
            OleDbConnection con = new OleDbConnection(connectionstring(questiondatabase));
            OleDbDataAdapter adptr = new OleDbDataAdapter("SELECT ID from questions", con);
            DataSet ds = new DataSet();
            adptr.Fill(ds, "questions");
            return ds.Tables[0].Rows.Count;
        }
        static public string getQuestion(int key)
        {
            OleDbConnection con = new OleDbConnection(connectionstring(questiondatabase));
            OleDbDataAdapter adptr = new OleDbDataAdapter("SELECT Question from questions WHERE ID=" + (key + 1) + "", con);
            DataSet ds = new DataSet();
            adptr.Fill(ds, "Questions");
            DataRow dr = ds.Tables[0].Rows[0];
            return (string)dr[0];
        }
        static Dictionary<string,string> getAnswer(int key)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            OleDbConnection con = new OleDbConnection(connectionstring(questiondatabase));
            OleDbDataAdapter adptr = new OleDbDataAdapter("SELECT Dictionary from questions WHERE ID=" + (key+1) + "", con);
            DataSet ds = new DataSet();
            adptr.Fill(ds, "questions");

            string[] arr = ds.Tables[0].Rows[0].ItemArray[0].ToString().Split('~');
            foreach (string i in arr)
            {
                string[] temp = i.Split("^".ToCharArray(), 2);
                dict.Add(temp[0], temp[1]);
            }
            return dict;
        }
    }
}
