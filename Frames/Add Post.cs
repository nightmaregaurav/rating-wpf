using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Add_Post : Form
    {
        public int uid;
        public Add_Post(int uid)
        {
            InitializeComponent();
            this.uid = uid;
            DBManager dbm = new DBManager();
            string sql = "SELECT topic from topics";
            List<Dictionary<string, object>> r = dbm.execute(sql);
            int i = 0;
            foreach(Dictionary<string, object> row in r)
            {
                comboBox1.Items.Insert(i, row["topic"].ToString());
                i++;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            DBManager dbm = new DBManager();
            string sql = "SELECT topicID from topics WHERE topic='" + comboBox1.Text.Trim() + "'";
            List<Dictionary<string, object>> r = dbm.execute(sql);
            int topicID = Convert.ToInt32(r[0]["topicID"]);
            sql = "SELECT MAX(AVGrate) as maxrate FROM reviewers_speciality WHERE topicID=" + topicID + " AND userID!="+uid;
            r = dbm.execute(sql);
            if (r.Count == 0 )
            {
                MessageBox.Show("Sorry, No reviewer available for this topic yet.");
                return;
            }
            try
            {
                Convert.ToInt32((r[0]["maxrate"]));
            }catch(Exception)
            {
                MessageBox.Show("Sorry, No reviewer available for this topic yet.");
                return;
            }
            int max = Convert.ToInt32(r[0]["maxrate"]);
            for (int i = 0; i <= max; i++)
                comboBox2.Items.Insert(i, i.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDF Documents | *.pdf";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                String path = dialog.FileName;
                textBox2.Text = path;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string caption = textBox1.Text.Trim();
            string topic = comboBox1.Text.Trim();
            string mincredit = comboBox2.Text.Trim();
            string FilePath = textBox2.Text.Trim();
            if (topic == "")
            {
                MessageBox.Show("Select one topic.");
                comboBox1.Focus();
                return;
            }
            if (mincredit == "")
            {
                MessageBox.Show("Select minimun credit requirement.");
                comboBox2.Focus();
                return;
            }
            int mincred = Convert.ToInt32(mincredit);
            if (FilePath == "")
            {
                MessageBox.Show("File is required.");
                comboBox2.Focus();
                return;
            }
            string filename = Path.GetFileName(FilePath);
            if (filename.Length > 140)
            {
                MessageBox.Show("Filename cannot be longer than 140 characters. Please rename and try again.");
                comboBox2.Focus();
                return;
            }
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            long length = new System.IO.FileInfo(FilePath).Length;
            if (length > 52428800)
            {
                MessageBox.Show("Filename size cannot be more than 50MB.");
                comboBox2.Focus();
                return;
            }
            decimal len = (decimal)((decimal)length / (decimal)1048576);
            len = Math.Ceiling(len * 100) / 100;

            BinaryReader br = new BinaryReader(fs);
            byte[] document = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();
            DBManager dbm = new DBManager();
            filename = filename.Remove(filename.Length - 4);
            dbm.add_post(uid, topic, caption, filename.ToLower().Trim() + " [" + len + "MB].pdf", document, mincred);
            MessageBox.Show("Posted");
            this.Close();
        }
    }
}
