using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace rating.Frames
{
    public partial class Add_Topic : Form
    {
        public Add_Topic()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            string namereg = @"^[a-zA-Z][a-zA-Z0-9 ]*$";
            if (!Regex.IsMatch(name, namereg))
            {
                MessageBox.Show("Invalid Topic Name!");
                textBox1.Focus();
                return;
            }
            DBManager dbm = new DBManager();
            if (!dbm.table_not_exists("topics"))
            {
                string sql = "SELECT * FROM topics where topic='" + name + "'";
                List<Dictionary<string, object>> r = dbm.execute(sql, true);
                if (r.Count != 0)
                {
                    MessageBox.Show("Topic Name already available!");
                    textBox1.Focus();
                    return;
                }
            }
            dbm.add_topic(name);
            MessageBox.Show("Added!");
            Close();
        }
    }
}
