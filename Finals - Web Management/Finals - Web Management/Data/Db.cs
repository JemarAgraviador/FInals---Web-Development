using MySql.Data.MySqlClient;

namespace LibrarySystem.Data
{
    public class Db
    {
        private string connStr = "server=localhost;database=librarydb;user=root;password=Finals;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connStr);
        }
    }
}
