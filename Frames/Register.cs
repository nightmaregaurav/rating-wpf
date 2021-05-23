using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Register : Form
    {
        public int uid;
        public string name;
        public string username;
        public string speciality;
        public bool isUpdateWindow = false;
        public int a = 2;
        public Register()
        {
            InitializeComponent();
            refresh_topic_list();
            button3.Enabled = false;
            button3.Hide();
        }
        public Register(int a)
        {
            this.a = a;
            InitializeComponent();
            refresh_topic_list();
            this.linkLabel1.Hide();
            button3.Enabled = false;
            button3.Hide();
        }

        public Register(int uid, string name, string username, string speciality)
        {
            this.uid = uid;
            this.name = name;
            this.username = username;
            this.speciality = speciality;
            this.isUpdateWindow = true;

            InitializeComponent();
            refresh_topic_list();

            linkLabel1.Hide();
            textBox3.Text = name;
            textBox1.Text = username;
            label1.Text = "Update Account";
            label1.Location = new System.Drawing.Point(88, 9);
            this.Text = "Update Account";
            button1.Text = "Update";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Add_Topic at = new Add_Topic();
            at.Closed += (s, args) => this.refresh_topic_list();
            at.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox2.UseSystemPasswordChar = false;
            else
                textBox2.UseSystemPasswordChar = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox3.Text.Trim();
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string nameRegex = @"^[A-Za-z][A-Za-z ,.'-]*$";
            string unameRegex = @"^[A-Za-z][A-Za-z0-9@_]*$";
            string passRegex = @".";

            if (name.Length == 0)
            {
                MessageBox.Show("Name cannot be empty!");
                textBox3.Focus();
                return;
            }
            else if (name.Length > 255)
            {
                MessageBox.Show("Name cannot longer than 255 characters!");
                textBox3.Focus();
                return;
            }
            if (!Regex.IsMatch(name, nameRegex))
            {
                MessageBox.Show("Invalid Name!");
                textBox3.Focus();
                return;
            }
            if (username.Length == 0)
            {
                MessageBox.Show("Username cannot be empty!");
                textBox1.Focus();
                return;
            }
            else if (username.Length > 50)
            {
                MessageBox.Show("Username cannot longer than 50 characters!");
                textBox1.Focus();
                return;
            }
            if (!Regex.IsMatch(username, unameRegex))
            {
                MessageBox.Show("Invalid Username!");
                textBox1.Focus();
                return;
            }
            if (password.Length == 0)
            {
                MessageBox.Show("Password cannot be empty!");
                textBox2.Focus();
                return;
            }
            else if (password.Length > 50)
            {
                MessageBox.Show("Password cannot longer than 50 characters!");
                textBox2.Focus();
                return;
            }
            if (!Regex.IsMatch(password, passRegex))
            {
                MessageBox.Show("Invalid Password!");
                textBox2.Focus();
                return;
            }
            string speciality = "";
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                {
                    speciality = speciality + checkedListBox1.Items[i].ToString() + "; ";
                }
            }
            speciality = this.speciality + speciality;
            if (speciality == "")
            {
                MessageBox.Show("Select atleast one speciality!");
                return;
            }
            if (isUpdateWindow)
            {
                DBManager dbm = new DBManager();
                string sql = "SELECT * FROM users WHERE username='" + username + "' AND userID!=" + uid;
                List<Dictionary<string, object>> r = dbm.execute(sql, true);
                if (r.Count != 0)
                {
                    MessageBox.Show("Username already taken!");
                    textBox1.Focus();
                    return;
                }

                string passhash = Modules.Crypto.Sha256(password);
                dbm.update_user(uid, name, username, passhash, 2, speciality);
                MessageBox.Show("Account Successfully Updated!");
                this.Close();
            }
            else
            {
                DBManager dbm = new DBManager();
                if (!dbm.table_not_exists("users"))
                {
                    string sql = "SELECT * FROM users WHERE username='" + username + "'";
                    List<Dictionary<string, object>> r = dbm.execute(sql, true);
                    if (r.Count != 0)
                    {
                        MessageBox.Show("Username already taken!");
                        textBox1.Focus();
                        return;
                    }
                }
                
                string passhash = Modules.Crypto.Sha256(password);
                dbm.add_user(name, username, passhash, 1, speciality);
                MessageBox.Show("Account Successfully Created!");
                if (a != 2)
                {
                    this.Close();
                }
                else
                {
                    this.Hide();
                    var login = new Login();
                    login.Closed += (s, args) => this.Close();
                    login.Show();
                }
                
            }
        }
        public void refresh_topic_list()
        {
            checkedListBox1.Items.Clear();
            DBManager dbm = new DBManager();
            if (dbm.table_not_exists("topics"))
                return;
            string sql = "SELECT * FROM topics";
            List<Dictionary<string, object>> r = dbm.execute(sql);
            int i = 0;
            foreach (Dictionary<string, object> row in r)
            {
                checkedListBox1.Items.Insert(i, row["topic"].ToString());
                i++;
            }
            if (isUpdateWindow)
            {
                string[] specialities = speciality.Split("; ");
                List<int> lst = new List<int>();
                for (i = checkedListBox1.Items.Count-1; i>=0 ; i--)
                {
                    int pos = Array.IndexOf(specialities, checkedListBox1.Items[i].ToString());
                    if (pos > -1)
                        lst.Add(i);
                }
                foreach (int a in lst)
                    checkedListBox1.Items.RemoveAt(a);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            var login = new Login();
            login.Closed += (s, args) => this.Close();
            login.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DBManager dbm = new DBManager();
            string sql = "SELECT t.topic as topic, r.AVGrate as AVGrate FROM reviewers_speciality r inner join topics t ON r.topicID=t.topicID WHERE userID = " + uid;
            List<Dictionary<string, object>> tbl = new List<Dictionary<string, object>>();
            tbl = dbm.execute(sql);
            string message = "Your Skill points(average rating) for each speciality are as follow: \n\n";
            foreach(Dictionary<string, object> row in tbl)
            {
                message += row["topic"].ToString() + ": " + Convert.ToInt32(row["AVGrate"]) + "\n";
            }
            MessageBox.Show(message, "Skill Points");
        }
    }
}
