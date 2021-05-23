using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Windows.Forms;

namespace rating
{
    class DBManager
    {
        public int max_reviewers_to_allocate_per_post = 5;
        public SqlConnection conn;
        public DBManager()
        {
            string connectionString = "Server=DESKTOP-5B5II3T\\SQLEXPRESS;Initial Catalog=rating;Integrated Security=SSPI;";
            conn = new SqlConnection(connectionString);
            if (table_not_exists("topics"))
            {
                string s = "CREATE TABLE topics (" +
                    "topicID int IDENTITY(1,1) PRIMARY KEY," +
                    "topic varchar(50) NOT NULL UNIQUE" +
                    ");";
                execute(s);
            }
            if (table_not_exists("users"))
            {
                string s = "CREATE TABLE users (" +
                    "userID int IDENTITY(1,1) PRIMARY KEY," +
                    "Name varchar(255) NOT NULL," +
                    "username varchar(50) NOT NULL UNIQUE," +
                    "psword varchar(255) NOT NULL," +
                    "user_role int NOT NULL," +
                    "works int NOT NULL DEFAULT 0" +
                    ");";
                execute(s);
            }
            if (table_not_exists("reviewers_speciality"))
            {
                string s = "CREATE TABLE reviewers_speciality (" +
                    "userID int NOT NULL," +
                    "topicID int NOT NULL," +
                    "AVGrate int NOT NULL DEFAULT 0," +
                    "FOREIGN KEY(userID) REFERENCES users(userID) ON DELETE CASCADE," +
                    "FOREIGN KEY(topicID) REFERENCES topics(topicID) ON DELETE CASCADE" +
                    ");";
                execute(s);
            }
            if (table_not_exists("posts"))
            {
                string s = "CREATE TABLE posts (" +
                    "postID int IDENTITY(1,1) PRIMARY KEY," +
                    "userID int NOT NULL," +
                    "topicID int NOT NULL," +
                    "postedON datetime NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                    "caption VARCHAR(MAX) NULL," +
                    "document_name VARCHAR(255) NOT NULL," +
                    "document VARBINARY(MAX) NOT NULL," +
                    "creditLimit int NOT NULL DEFAULT 0," +
                    "FOREIGN KEY(userID) REFERENCES users(userID) ON DELETE CASCADE," +
                    "FOREIGN KEY(topicID) REFERENCES topics(topicID) ON DELETE CASCADE" +
                    ");";
                execute(s);
            }
            if (table_not_exists("assignments_posts"))
            {
                string s = "CREATE TABLE assignments_posts (" +
                    "postID int NOT NULL," +
                    "userID int NOT NULL," +
                    "FOREIGN KEY(postID) REFERENCES posts(postID) ON DELETE CASCADE," +
                    "FOREIGN KEY(userID) REFERENCES users(userID)" +
                    ");";
                execute(s);
            }
            if (table_not_exists("reviews"))
            {
                string s = "CREATE TABLE reviews (" +
                    "reviewID int IDENTITY(1,1) PRIMARY KEY," +
                    "postID int NOT NULL," +
                    "userID int NOT NULL," +
                    "postedON datetime NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                    "content VARCHAR(MAX) NULL," +
                    "rate int NULL," +
                    "FOREIGN KEY(postID) REFERENCES posts(postID) ON DELETE CASCADE," +
                    "FOREIGN KEY(userID) REFERENCES users(userID)" +
                    ");";
                execute(s);
            }
            string sql = "IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAssignments_postsInsert]')) " +
                "EXEC dbo.sp_executesql @statement = N' " +
                "CREATE TRIGGER trgAssignments_postsInsert " +
                "ON assignments_posts " +
                "AFTER INSERT " +
                "AS " +
                "BEGIN " +
                "UPDATE users SET works = works + 1 WHERE userID IN (SELECT userID FROM inserted) " +
                "END'";
            execute(sql);
            sql = "IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgPostsInsert]')) " +
                "EXEC dbo.sp_executesql @statement = N' " +
                "CREATE TRIGGER trgPostsInsert " +
                "ON posts " +
                "AFTER INSERT " +
                "AS " +
                "BEGIN " +
                "INSERT INTO assignments_posts(postID, userID) " +
                "SELECT TOP " + max_reviewers_to_allocate_per_post + " tbl1.postID, tbl1.userID " +
                "FROM( " +
                "SELECT p.postID as postID, reviewers_speciality.userID as userID " +
                "FROM reviewers_speciality " +
                "INNER JOIN " +
                "inserted p " +
                "ON " +
                "reviewers_speciality.AVGrate >= p.creditLimit AND reviewers_speciality.topicID = p.topicID " +
                "WHERE " +
                "reviewers_speciality.userID != p.userID) as tbl1 " +
                "INNER JOIN " +
                "users " +
                "ON " +
                "tbl1.userID = users.userID " +
                "ORDER BY users.works " +
                "END'";
            execute(sql);
            sql = "IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgReviewsInsertUpdate]')) " +
                "EXEC dbo.sp_executesql @statement = N' " +
                "CREATE TRIGGER trgReviewsInsertUpdate " +
                "ON reviews " +
                "AFTER UPDATE, INSERT " +
                "AS " +
                "DECLARE @topicid int " +
                "DECLARE @userid int " +
                "DECLARE @avgrate int " +
                "SELECT @topicid = topicID FROM posts, inserted i WHERE posts.postID = i.postID " +
                "SELECT @userid = userid from inserted i " +
                "SELECT @avgrate = AVG(reviews.rate) FROM posts INNER JOIN reviews ON posts.postID = reviews.postID WHERE(reviews.userID = @userid) AND(posts.topicID = @topicid) " +
                "BEGIN " +
                "IF(@avgrate IS NOT NULL) " +
                "UPDATE reviewers_speciality set AVGrate = @avgrate WHERE(topicID = @topicid) AND(userID = @userid); " +
                "END'";
            execute(sql);
            sql = "IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgReviewrs_SpecialInsertInstead]')) " +
                "EXEC dbo.sp_executesql @statement = N' " +
                "CREATE TRIGGER trgReviewrs_SpecialInsertInstead " +
                "ON reviewers_speciality " +
                "INSTEAD OF INSERT " +
                "AS " +
                "DECLARE @uid int " +
                "DECLARE @tid int " +
                "SELECT @uid = i.userID, @tid = i.topicID FROM inserted i " +
                "BEGIN " +
                "IF(NOT EXISTS(SELECT r.userID FROM reviewers_speciality r WHERE r.userID = @uid AND r.topicID = @tid)) " +
                "INSERT INTO reviewers_speciality(userID, topicID) VALUES(@uid, @tid); " +
                "END'";
            execute(sql);
            sql = "IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgUsersDelete]')) " +
                "EXEC dbo.sp_executesql @statement = N' " +
                "CREATE TRIGGER trgUsersDelete " +
                "ON users " +
                "INSTEAD OF DELETE " +
                "AS " +
                "DECLARE @uid int " +
                "SELECT @uid = d.userID FROM deleted d " +
                "BEGIN " +
                "DELETE FROM assignments_posts WHERE userID = @uid; " +
                "DELETE FROM reviews WHERE userID = @uid; " +
                "DELETE FROM users WHERE userID = @uid; " +
                "END'";
            execute(sql);

        }
        public List<Dictionary<string, object>> execute(string sql, bool is_select = false)
        {
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();
            if (sql.Substring(0,6).ToUpper() == "SELECT")
            {
                is_select = true;
            }

            conn.Open();
            if (!is_select)
            {
                try
                {  
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                try
                {
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    List<string> names = new List<string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        names.Add(reader.GetName(i).ToString());
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row.Add(names[i], reader[names[i]]);
                            table.Add(row);
                        }
                    }
                }catch(Exception E)
                {
                    MessageBox.Show(E.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            return table;
        }
        public bool table_not_exists(string tablename)
        {
            string sql = "IF (NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '" + tablename + "')) BEGIN SELECT 'no' as result; END";
            List<Dictionary<string, object>> r = execute(sql, true);
            if (r.Count > 0)
                if (r[0]["result"].ToString() == "no")
                    return true;
                else
                    return false;
            else
                return false;
        }
        public void add_topic(string name)
        {
            string sql = "INSERT INTO topics(topic) VALUES('" + name + "')";
            execute(sql);
        }
        public void add_user(string name, string username, string password, int role, string speciality="")
        {
            string sql = "INSERT INTO users(Name, username, psword, user_role) VALUES('" + name + "', '" + username + "', '" + password + "', " + role + ")";
            execute(sql);
            if (role != 0)
            {
                string[] specialities = speciality.Split("; ");
                sql = "SELECT userID FROM users WHERE username='" + username + "'";
                List<Dictionary<string, object>> r = execute(sql, true);
                int uid = Convert.ToInt32(r[0]["userID"]);
                foreach (string special in specialities)
                {
                    if (special != "")
                    {
                        sql = "SELECT topicID FROM topics WHERE topic = '" + special + "'";
                        r = execute(sql, true);
                        int tid = Convert.ToInt32(r[0]["topicID"]);
                        sql = "INSERT INTO reviewers_speciality(userID, topicID) VALUES(" + uid + "," + tid + ")";
                        execute(sql);
                    }
                }
            }
        }
        public void update_user(int uid, string name, string username, string password, int role, string speciality = "")
        {
            string sql = "UPDATE users SET Name='" + name + "', username='" + username + "', psword='" + password + "' WHERE userID=" + uid;
            execute(sql);

            if (role != 0)
            {
                string[] specialities = speciality.Split("; ");
                foreach (string special in specialities)
                {
                    if (special != "")
                    {
                        sql = "SELECT topicID FROM topics WHERE topic = '" + special + "'";
                        List<Dictionary<string, object>> r = execute(sql, true);
                        int tid = Convert.ToInt32(r[0]["topicID"]);
                        sql = "INSERT INTO reviewers_speciality(userID, topicID) VALUES(" + uid + "," + tid + ")";
                        execute(sql);
                    }
                }
            }
        }
        public void add_post(int userId, string topic, string caption, string fileName, byte[] document, int crlimit)
        {
            string sql = "SELECT topicID FROM topics WHERE topic='" + topic + "'";
            List<Dictionary<string, object>> r = execute(sql, true);
            int tid = Convert.ToInt32(r[0]["topicID"]);
            SqlCommand sc = new SqlCommand();
            sc.CommandText = "INSERT INTO posts(userID, topicID, caption, document_name, document, creditLimit) VALUES(@uid,@tid,@cpt,@dnm,@doc,@crl)";
            sc.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = userId;
            sc.Parameters.Add("@tid", System.Data.SqlDbType.Int).Value = tid;
            sc.Parameters.Add("@cpt", System.Data.SqlDbType.VarChar).Value = caption;
            sc.Parameters.Add("@dnm", System.Data.SqlDbType.VarChar).Value = fileName;
            sc.Parameters.Add("@doc", System.Data.SqlDbType.VarBinary).Value = document;
            sc.Parameters.Add("@crl", System.Data.SqlDbType.Int).Value = crlimit;
            conn.Open();
            sc.Connection = conn;
            sc.ExecuteNonQuery();
            conn.Close();
        }
        public void add_review(int uid, int postID, string content)
        {
            SqlCommand sc = new SqlCommand();
            sc.CommandText = "INSERT INTO reviews(postID, userID, content) VALUES(@pid,@uid,@content)";
            sc.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = postID;
            sc.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = uid;
            sc.Parameters.Add("@content", System.Data.SqlDbType.VarChar).Value = content;
            conn.Open();
            sc.Connection = conn;
            sc.ExecuteNonQuery();
            conn.Close();
        }
        public void delete_post(int postID)
        {
            string sql = "DELETE FROM posts WHERE postID=" + postID;
            execute(sql);
        }
        public void delete_review(int reviewID)
        {
            string sql = "DELETE FROM reviews WHERE reviewID=" + reviewID;
            execute(sql);
        }
        public void delete_user(int userID)
        {
            string sql = "DELETE FROM users WHERE userID=" + userID;
            execute(sql);
        }
    }
}
