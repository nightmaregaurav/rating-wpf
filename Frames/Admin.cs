using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Admin : Form
    {
        public int uid;
        public Admin(int uid)
        {
            InitializeComponent();

            panel2.HorizontalScroll.Maximum = 0;
            panel2.AutoScroll = false;
            panel2.VerticalScroll.Visible = false;
            panel2.AutoScroll = true;

            this.uid = uid;
            string sql = "SELECT * FROM users WHERE user_role=" + 1;
            DBManager dbm = new DBManager();
            List<Dictionary<string, object>> r = dbm.execute(sql);
            paint(r);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Register reg = new Register(0);
            reg.Closed += this.button2_Click;
            reg.Show();
        }

        public void button2_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM users WHERE user_role=" + 1;
            DBManager dbm = new DBManager();
            List<Dictionary<string, object>> r = dbm.execute(sql);
            paint(r);
        }
        public void paint(List<Dictionary<string, object>> r)
        {
            panel2.Controls.Clear();
            panel2.Controls.Add(panel3);
            int i = 1;
            foreach (Dictionary<string, object> row in r)
            {
                int userID = Convert.ToInt32(row["userID"]);
                string name = row["Name"].ToString();
                Custom_Widgets.listPanel lp = new Custom_Widgets.listPanel(i, name, userID, this);
                lp.Location = new System.Drawing.Point(8, 55 + (i - 1) * 45);
                panel2.Controls.Add(lp);
                lp.Show();
                i++;
            }
            panel2.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            search();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            search();
        }
        public void search()
        {
            string searchString = textBox1.Text.Trim();
            searchString = searchString.Replace("'", "").Replace("\"", "").Replace("`", "").Replace("%", "").Replace("?", "").Replace("_", "").Replace("*", "");

            string sql = "SELECT * FROM users WHERE user_role !=0 AND Name LIKE '%" + searchString + "%'";
            DBManager dbm = new DBManager();
            List<Dictionary<string, object>> r = dbm.execute(sql);
            paint(r);
        }
    }
}
