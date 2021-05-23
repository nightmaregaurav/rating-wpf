using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Edit : Form
    {
        public bool isPost;
        public int id;
        public int postID;
        public bool isComment = false;

        public Edit(int id, int postID)
        {
            this.id = id;
            this.postID = postID;
            this.isComment = true;
            InitializeComponent();
            this.Text = "Add Review";
            this.label1.Text = "Add Review";
            this.label1.Location = new System.Drawing.Point(155, 18);
            this.label2.Text = "Review Text (Upto 100000 chars):";
            this.button1.Text = "+ Add";
        }
        public Edit(int id, bool isPost)
        {
            this.isPost = isPost;
            this.id = id;
            InitializeComponent();

            DBManager dbm = new DBManager();
            string sql;
            if(isPost)
                sql = "SELECT caption as content FROM posts WHERE postID=" + id;
            else
                sql = "SELECT content FROM reviews WHERE reviewID=" + id;
            List<Dictionary<string, object>> r = dbm.execute(sql);
            string content = r[0]["content"].ToString();
            textBox1.Text = content;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DBManager dbm = new DBManager();
            string content = textBox1.Text.Trim();
            if(content.Length == 0)
            {
                MessageBox.Show("Text Field is empty!");
                this.Close();
                return;
            }
            SqlCommand sc = new SqlCommand();
            if (isComment)
            {
                dbm.add_review(id, postID, content);
                this.Close();
                return;
            }

            if (isPost)
                sc.CommandText = "UPDATE posts SET caption = @content WHERE postID="+id;
            else
                sc.CommandText = "UPDATE reviews SET content = @content WHERE reviewID=" + id;
            sc.Parameters.Add("@content", System.Data.SqlDbType.VarChar).Value = content;
            dbm.conn.Open();
            sc.Connection = dbm.conn;
            sc.ExecuteNonQuery();
            dbm.conn.Close();
            this.Close();
        }
    }
}
