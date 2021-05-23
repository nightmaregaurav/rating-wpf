using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace rating.Custom_Widgets
{
    class Post : Panel
    {
        public LinkLabel linkLabel1 = new LinkLabel();
        public Label label1 = new Label();
        public Label label2 = new Label();
        public TextBox textBox1 = new TextBox();
        public Button button1 = new Button();
        public Button button2 = new Button();

        public int uid;
        public bool isOwnPost = true;
        public string Short;
        public int postid;
        public string filename;
        public Frames.Home main_window;
        System.Drawing.Color color1;
        System.Drawing.Color color2;

        private const int EM_GETLINECOUNT = 0xba;
        [DllImport("user32", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        public Post(int uid, Dictionary<string,object> row, string Short, Frames.Home main_window)
        {
            this.uid = uid;
            this.Short = Short;
            this.postid = Convert.ToInt32(row["postID"]);
            if (uid != Convert.ToInt32(row["userID"]))
                isOwnPost = false;
            if (isOwnPost)
            {
                color1 = System.Drawing.SystemColors.GradientActiveCaption;
                color2 = System.Drawing.SystemColors.GradientInactiveCaption;
            }
            else
            {
                color2 = System.Drawing.SystemColors.GradientActiveCaption;
                color1 = System.Drawing.SystemColors.GradientInactiveCaption;
            }
            this.filename = row["document_name"].ToString();
            this.main_window = main_window;

            this.BackColor = color1;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.linkLabel1);
            this.Size = new System.Drawing.Size(584, 307);

            this.button1.BackColor = System.Drawing.Color.Teal;
            this.button1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(469, 5);
            this.button1.Size = new System.Drawing.Size(44, 30);
            this.button1.Text = "Edit";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);


            this.button2.BackColor = System.Drawing.Color.Crimson;
            this.button2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(519, 5);
            this.button2.Size = new System.Drawing.Size(56, 30);
            this.button2.Text = "Delete";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.Name = Convert.ToInt32(row["postID"]).ToString();


            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(2, 10);
            this.label2.MaximumSize = new System.Drawing.Size(580, 17);
            this.label2.MinimumSize = new System.Drawing.Size(580, 17);
            this.label2.Size = new System.Drawing.Size(580, 17);
            DateTime dt = Convert.ToDateTime(row["postedON"]);
            this.label2.Text = dt.ToString("ddd dd/MM/yyyy, hh:mm tt");
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(9, 30);
            this.label1.Size = new System.Drawing.Size(34, 17);

            this.textBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(19, 50);
            this.textBox1.MaxLength = 100000;
            this.textBox1.Multiline = true;
            this.textBox1.Size = new System.Drawing.Size(554, 31);
            this.textBox1.ReadOnly = true;
            this.textBox1.GotFocus += this.textBoxFocused;
            this.textBox1.BackColor = color1;
            this.textBox1.BorderStyle = BorderStyle.None;


            string caption = row["caption"].ToString().Trim();
            if (caption.Length == 0)
                caption = "No Caption...";
            this.textBox1.Text = caption;
            var numberOfLines = SendMessage(textBox1.Handle.ToInt32(), EM_GETLINECOUNT, 0, 0);
            int height = 0;
            if (numberOfLines != 1)
            {
                if (numberOfLines <= 10)
                {
                    height = (textBox1.Font.Height) * numberOfLines;
                    this.textBox1.Height = height;
                }
                else
                {
                    height = (textBox1.Font.Height) * 10;
                    this.textBox1.Height = height;
                    this.textBox1.WordWrap = true;
                    this.textBox1.ScrollBars = ScrollBars.Vertical;
                }
            }
            else
            {
                height = textBox1.Height;
            }

            int tempHeight = 50 + height + 5;

            this.linkLabel1.AutoSize = false;
            this.linkLabel1.Location = new System.Drawing.Point(2, tempHeight);
            this.linkLabel1.MaximumSize = new System.Drawing.Size(580, 15);
            this.linkLabel1.MinimumSize = new System.Drawing.Size(580, 15);
            this.linkLabel1.Size = new System.Drawing.Size(580, 15);
            this.linkLabel1.TabStop = true;
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel1.Text = row["document_name"].ToString().Trim();
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI Bold", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);

            tempHeight = tempHeight + 15 + 5;
            int comments_height = load_reviews(Convert.ToInt32(row["postID"]), this.uid, tempHeight);

            this.Height = tempHeight + comments_height + 5;

            if (isOwnPost)
            {
                this.label1.Text = "You:";
            }
            else
            {
                this.label1.Text = "Anonymous Author:";
                this.button1.Hide();
                this.button2.Hide();
            }
        }
        public int load_reviews(int postID, int userID, int startHeight)
        {
            Panel panel = new Panel();
            Button buttonA = new Button();

            panel.BackColor = color1;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Location = new System.Drawing.Point(38, startHeight);
            panel.Size = new System.Drawing.Size(504, 156);

            DBManager dbm = new DBManager();
            string sql = "SELECT * FROM reviews WHERE postID=" + postID;
            if (!isOwnPost)
                sql += " AND userID=" + userID;
            sql += " ORDER BY postedON " + Short;
            List<Dictionary<string, object>> results = dbm.execute(sql);
            int PanelHeight = 10;
            if (results.Count != 0)
            {
                foreach (Dictionary<string, object> row in results)
                {
                    Panel panelA = new Panel();
                    Label labelA = new Label();
                    Label labelB = new Label();
                    Label labelC = new Label();
                    Label labelD = new Label();
                    LinkLabel linkLabelA = new LinkLabel();
                    LinkLabel linkLabelB = new LinkLabel();
                    TextBox textBoxA = new TextBox();
                    ComboBox comboBoxA = new ComboBox();

                    panelA.BackColor = color2;
                    panelA.Controls.Add(labelA);
                    panelA.Controls.Add(linkLabelA);
                    panelA.Controls.Add(linkLabelB);
                    panelA.Controls.Add(textBoxA);
                    panelA.Controls.Add(labelB);
                    panelA.Controls.Add(labelC);
                    panelA.Controls.Add(labelD);
                    panelA.Controls.Add(comboBoxA);
                    panelA.Location = new System.Drawing.Point(17, PanelHeight);
                    panelA.Size = new System.Drawing.Size(469, 94);

                    labelA.AutoSize = true;
                    labelA.Location = new System.Drawing.Point(9, 8);
                    labelA.Size = new System.Drawing.Size(125, 15);
                    labelA.Font = new System.Drawing.Font("Segoe UI Bold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);

                    linkLabelA.AutoSize = true;
                    linkLabelA.Location = new System.Drawing.Point(423, 8);
                    linkLabelA.Size = new System.Drawing.Size(27, 15);
                    linkLabelA.TabStop = true;
                    linkLabelA.Text = "Edit";
                    linkLabelA.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelA_LinkClicked);
                    linkLabelA.Name = Convert.ToInt32(row["reviewID"]).ToString();
                    linkLabelA.Font = new System.Drawing.Font("Segoe UI Bold", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);

                    linkLabelB.AutoSize = true;
                    linkLabelB.Location = new System.Drawing.Point(380, 8);
                    linkLabelB.Size = new System.Drawing.Size(25, 15);
                    linkLabelB.TabStop = true;
                    linkLabelB.Text = "Delete";
                    linkLabelB.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelB_LinkClicked);
                    linkLabelB.Name = Convert.ToInt32(row["reviewID"]).ToString();
                    linkLabelB.LinkColor = System.Drawing.Color.Crimson;
                    linkLabelB.Font = new System.Drawing.Font("Segoe UI Bold", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);


                    textBoxA.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                    textBoxA.Location = new System.Drawing.Point(19, 25);
                    textBoxA.MaxLength = 100000;
                    textBoxA.Multiline = true;
                    textBoxA.Size = new System.Drawing.Size(431, 31);
                    textBoxA.ReadOnly = true;
                    textBoxA.GotFocus += this.textBoxFocused;
                    string review = row["content"].ToString().Trim();
                    textBoxA.Text = review;
                    textBoxA.BackColor = color2;
                    textBoxA.BorderStyle = BorderStyle.None;


                    var numberOfLines = SendMessage(textBoxA.Handle.ToInt32(), EM_GETLINECOUNT, 0, 0);
                    int height = 0;
                    if (numberOfLines != 1)
                    {
                        if (numberOfLines <= 10)
                        {
                            height = (textBoxA.Font.Height) * numberOfLines;
                            textBoxA.Height = height;
                        }
                        else
                        {
                            height = (textBoxA.Font.Height) * 10;
                            textBoxA.Height = height;
                            textBoxA.WordWrap = true;
                            textBoxA.ScrollBars = ScrollBars.Vertical;
                        }
                    }
                    else
                    {
                        height = textBoxA.Height;
                    }

                    int tempHeight = 25 + height + 5;

                    labelB.AutoSize = true;
                    labelB.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                    labelB.Location = new System.Drawing.Point(9, tempHeight + 5);
                    labelB.Size = new System.Drawing.Size(89, 13);
                    labelB.Text = "Commented on:";

                    labelC.AutoSize = true;
                    labelC.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                    labelC.Location = new System.Drawing.Point(100, tempHeight + 5);
                    labelC.Size = new System.Drawing.Size(133, 13);

                    DateTime dt = Convert.ToDateTime(row["postedON"]);
                    labelC.Text = dt.ToString("ddd dd/MM/yyyy, hh:mm tt");

                    labelD.AutoSize = true;
                    labelD.Location = new System.Drawing.Point(319, tempHeight);
                    labelD.Size = new System.Drawing.Size(66, 15);
                    labelD.Text = "Usefulness:";

                    comboBoxA.FormattingEnabled = true;
                    comboBoxA.Location = new System.Drawing.Point(391, tempHeight - 4);
                    comboBoxA.Size = new System.Drawing.Size(59, 23);
                    comboBoxA.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBoxA.Name = Convert.ToInt32(row["reviewID"]).ToString();
                    comboBoxA.BackColor = color2;

                    tempHeight = tempHeight + 19 + 8;

                    panelA.Height = tempHeight;
                    int rate;
                    try
                    {
                        rate = Convert.ToInt32(row["rate"]);
                        for (int i = 0; i <= 10; i++)
                        {
                            comboBoxA.Items.Add(i.ToString());
                        }
                        comboBoxA.SelectedIndex = comboBoxA.FindStringExact(rate.ToString());
                    }
                    catch (Exception)
                    {
                        for (int i = 0; i <= 10; i++)
                        {
                            comboBoxA.Items.Add(i.ToString());
                        }
                    }
                    if (isOwnPost)
                    {
                        labelA.Text = "Anonymous Reviewer:";
                        linkLabelA.Hide();
                        linkLabelB.Hide();
                    }
                    else
                    {
                        labelA.Text = "You:";
                        comboBoxA.Enabled = false;
                    }

                    comboBoxA.SelectedIndexChanged += new System.EventHandler(this.comboBoxA_SelectedIndexChanged);

                    PanelHeight += tempHeight + 5;
                    panel.Controls.Add(panelA);
                }
            }
            else
            {
                if (isOwnPost)
                {
                    panel.Size = new System.Drawing.Size(0, 0);
                }
            }
            if (!isOwnPost)
            {
                buttonA.BackColor = System.Drawing.Color.Teal;
                buttonA.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                buttonA.Location = new System.Drawing.Point(385, PanelHeight);
                buttonA.Size = new System.Drawing.Size(101, 30);
                buttonA.Text = "+ Add Review";
                buttonA.UseVisualStyleBackColor = false;
                buttonA.Click += new System.EventHandler(this.buttonA_Click);

                PanelHeight = PanelHeight + 30 + 10;
                panel.Height = PanelHeight;
                panel.Controls.Add(buttonA);
            }
            else
            {
                PanelHeight += 5;
                panel.Height = PanelHeight;
            }

            this.Controls.Add(panel);

            return PanelHeight;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Frames.Edit edit = new Frames.Edit(postid, true);
            edit.Show();
            edit.Closed += main_window.button1_Click;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this post??",
                                     "Confirm!!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                DBManager dbm = new DBManager();
                dbm.delete_post(Convert.ToInt32(((Button)sender).Name));
                main_window.button1_Click(this, new EventArgs());
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = @"C:\";
            SaveFileDialog1.RestoreDirectory = true;
            SaveFileDialog1.Title = "Save Document";
            SaveFileDialog1.DefaultExt = "pdf";
            SaveFileDialog1.Filter = "PDF Documents | *.pdf";
            SaveFileDialog1.FileName = filename;
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] file;
                DBManager dbm = new DBManager();
                string sql = "SELECT document FROM posts WHERE postID=" + postid;
                List<Dictionary<string, object>> docRes = dbm.execute(sql);
                file = (byte[])docRes[0]["document"];
                File.WriteAllBytes(SaveFileDialog1.FileName, file);
                MessageBox.Show("Document Saved.");
            }
        }
        private void linkLabelA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Frames.Edit edit = new Frames.Edit(Convert.ToInt32(((LinkLabel)sender).Name), false);
            edit.Show();
            edit.Closed += main_window.button1_Click;
        }
        private void linkLabelB_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this review??",
                                     "Confirm!!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                DBManager dbm = new DBManager();
                dbm.delete_review(Convert.ToInt32(((LinkLabel)sender).Name));
                main_window.button1_Click(this, new EventArgs());
            }
        }
        private void comboBoxA_SelectedIndexChanged(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to Rate usefulness of this review with " + ((ComboBox)sender).Text.Trim() + " Points??",
                                     "Confirm!!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                DBManager dbm = new DBManager();
                string sql = "UPDATE reviews SET rate=" + ((ComboBox)sender).Text.Trim() + " WHERE reviewID=" + ((ComboBox)sender).Name.Trim();
                dbm.execute(sql);
                main_window.button1_Click(this, new EventArgs());
            }
        }
        private void buttonA_Click(object sender, EventArgs e)
        {
            Frames.Edit edit = new Frames.Edit(uid, postid);
            edit.Show();
            edit.Closed += main_window.button1_Click;
        }
        private void textBoxFocused(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}