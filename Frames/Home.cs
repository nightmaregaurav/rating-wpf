using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Home : Form
    {
        public int uid;
        public string name;
        public string username;
        private const int EM_GETLINECOUNT = 0xba;
        [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);
        public Home(int uid)
        {
            InitializeComponent();
            this.uid = uid;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            get_posts();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Add_Post ap = new Add_Post(uid);
            ap.Show();
            ap.Closed += (s, args) => this.get_posts();

        }
        public void button1_Click(object sender, EventArgs e)
        {
            get_posts();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.Closed += (s, args) => this.Close();
            login.Show();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string sql = "SELECT topics.topic as topic FROM reviewers_speciality INNER JOIN topics ON reviewers_speciality.topicID = topics.topicID WHERE userID = " + uid;
            DBManager dbm = new DBManager();
            List<Dictionary<string,object>> r = dbm.execute(sql);
            string speciality = "";
            foreach(Dictionary<string, object> row in r)
            {
                speciality = speciality + row["topic"].ToString() + "; ";
            }
            Register register = new Register(uid, name, username, speciality);
            register.Closed += (s, args) => this.get_posts();
            register.Show();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            get_posts();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            get_posts();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            get_posts();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            get_posts();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            get_posts();
        }
        public void get_posts()
        {
            string s = "SELECT Name, username FROM users WHERE userID=" + uid;
            DBManager dbm = new DBManager();
            List<Dictionary<string, object>> r = dbm.execute(s);
            name = r[0]["Name"].ToString();
            username = r[0]["username"].ToString();
            label5.Text = name;
            label7.Text = username;

            if (dbm.table_not_exists("posts"))
                return;
            string Search = textBox1.Text.Trim();
            string View = comboBox1.Text.Trim();
            string Type = comboBox3.Text.Trim();
            string Short = comboBox2.Text.Trim();

            string revSrt = "DESC";
            Search = Search.Replace("'", "").Replace("\"", "").Replace("`", "").Replace("%", "").Replace("?", "").Replace("_", "").Replace("*", "");

            if (Type.Equals("All"))
                Type = "(posts.userID = " + this.uid + " OR a.userID = " + this.uid + ")";
            else if (Type.Equals("My Works"))
                Type = "(posts.userID = " + this.uid + ")";
            else
                Type = "(a.userID = " + this.uid + ")";

            if (View.Equals("All"))
                View = " ";
            else if (View.Equals("Reviewed"))
                View = " AND (reviews.reviewID IS NOT NULL) ";
            else
                View = " AND (reviews.reviewID IS NULL) ";

            if (Short.Equals("Oldest Posts First"))
                Short = " ORDER BY posts.postedON ASC";
            else if(Short.Equals("Oldest Reviews First"))
            {
                Short = "ORDER BY CASE WHEN reviews.postedON IS NULL THEN 1 ELSE 0 END, reviews.postedON";
                revSrt = "ASC";
            }
            else if (Short.Equals("Newest Posts First"))
                Short = " ORDER BY posts.postedON DESC";
            else
                Short = " ORDER BY reviews.postedON DESC";
            string sql = "SELECT posts.postID as postID, posts.userID as userID, posts.topicID as topicID, posts.postedON as postedON, posts.caption as caption, posts.document_name as document_name FROM posts INNER JOIN assignments_posts a ON posts.postID=a.postID LEFT JOIN reviews ON posts.postID = reviews.postID WHERE " + Type + " AND (posts.caption LIKE '%" + Search + "%' OR reviews.content LIKE '%" + Search + "%')" + View + Short;
            List<Dictionary<string, object>> posts = dbm.execute(sql);
            List<int> postID = new List<int>();
            List<Dictionary<string, object>> unique_posts = new List<Dictionary<string, object>>();
            foreach (Dictionary<string, object> row in posts)
            {
                if (!postID.Contains(Convert.ToInt32(row["postID"])))
                {
                    postID.Add(Convert.ToInt32(row["postID"]));
                    unique_posts.Add(row);
                }
            }
            paint_posts(unique_posts, revSrt);
        }
        public void paint_posts(List<Dictionary<string, object>> result, string Short)
        {
            this.panel2.Controls.Clear();

            if (result.Count > 0)
            {
                int h = 10;
                foreach (Dictionary<string, object> row in result)
                {
                    Custom_Widgets.Post post = new Custom_Widgets.Post(uid, row, Short, this);
                    post.Location = new System.Drawing.Point(5, h);
                    this.panel2.Controls.Add(post);
                    h += post.Height + 5;
                }
            }
            this.panel2.Refresh();
        }
    }
}
