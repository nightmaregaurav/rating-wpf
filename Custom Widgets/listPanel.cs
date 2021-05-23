using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace rating.Custom_Widgets
{
    class listPanel : Panel
    {
        private readonly Label label1 = new Label();
        private readonly Label label2 = new Label();
        private readonly Button button1 = new Button();
        private readonly Button button2 = new Button();
        public string name;
        public int uid;
        public Frames.Admin main_window;
        public listPanel(int sn, string nme, int id, Frames.Admin main_window)
        {
            name = nme;
            uid = id;
            this.main_window = main_window;

            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Size = new System.Drawing.Size(758, 40);
            this.TabIndex = 0;

            this.button1.BackColor = System.Drawing.Color.Teal;
            this.button1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(478, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 30);
            this.button1.TabIndex = 6;
            this.button1.Text = "Edit";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);

            this.button2.BackColor = System.Drawing.Color.Crimson;
            this.button2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(580, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 30);
            this.button2.TabIndex = 6;
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);

            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(153, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = nme;

            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(30, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = sn.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "SELECT username FROM users WHERE userID=" + uid;
            DBManager dbm = new DBManager();
            List<Dictionary<string, object>> r = dbm.execute(sql);
            Dictionary<string, object> row = r[0];
            string username = row["username"].ToString();
            sql = "SELECT topics.topic as topic FROM reviewers_speciality INNER JOIN topics ON reviewers_speciality.topicID = topics.topicID WHERE userID = " + uid;
            r = dbm.execute(sql);
            string speciality = "";
            foreach (Dictionary<string, object> a in r)
            {
                speciality = speciality + a["topic"].ToString() + "; ";
            }
            Frames.Register register = new Frames.Register(uid, name, username, speciality);
            register.Closed += main_window.button2_Click;
            register.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this user??",
                                   "Confirm!!",
                                   MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                DBManager dbm = new DBManager();
                dbm.delete_user(uid);
                main_window.button2_Click(this, new EventArgs());
            }
        }
    }
}