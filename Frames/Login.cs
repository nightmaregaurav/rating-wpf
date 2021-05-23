using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
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
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string unameRegex = @"^[A-Za-z][A-Za-z0-9@_]*$";
            string passRegex = @".";
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
                textBox1.Focus();
                return;
            }
            else if (password.Length > 50)
            {
                MessageBox.Show("Password cannot longer than 50 characters!");
                textBox1.Focus();
                return;
            }
            if (!Regex.IsMatch(password, passRegex))
            {
                MessageBox.Show("Invalid Password!");
                textBox1.Focus();
                return;
            }
            DBManager dbm = new DBManager();
            string passhash = Modules.Crypto.Sha256(password);
            string sql = "SELECT userID,user_role FROM users WHERE username='" + username + "' AND psword='" + passhash + "'";
            List<Dictionary<string, object>> r = dbm.execute(sql, true);
            if (r.Count == 0)
            {
                MessageBox.Show("Username and Password combination incorrect!");
                return;
            }
            int uid = Convert.ToInt32(r[0]["userID"]);
            int role = Convert.ToInt32(r[0]["user_role"]);
            this.Hide();
            if (role == 0)
            {
                var admin = new Admin(uid);
                admin.Closed += (s, args) => this.Close();
                admin.Show();
            }
            else
            {
                var home = new Home(uid);
                home.Closed += (s, args) => this.Close();
                home.Show();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            var register = new Register();
            register.Closed += (s, args) => this.Close();
            register.Show();
        }
    }
}
