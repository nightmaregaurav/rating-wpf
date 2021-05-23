All database structure will be created automatically except the creation of database itself.
Execute the first line of DBDesign.txt file in sqlServer script manager and adjust connection string from Modules/DBManager.cs to connect to your server.
Admin user will not be automatically created. Either insert manually a user with username, password(SHA256 hash), and role 0 or uncomment the admin user creation line in program.cs once(Keep in mind, only once, run the project and comment it shortly without performing any other actions. And run yor project normally.)
Delete (.git) folder before you upload or submit this project elsewhere.(It contains real author's information.)
Move (Move me away) folder somewhere else once you get the project running.(It contains documentations scratch.)

YOU ARE GOOD TO GO...
