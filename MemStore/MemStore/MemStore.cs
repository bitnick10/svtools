using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemStore
{
    class MemStore
    {
        public string RAMDBName;
        public string HDDBName;

        string ServerIP = "192.168.0.170";

        public List<string> AllTableNames()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = ServerIP;
            sb.SslMode = MySqlSslMode.None;
            sb.UserID = "root";
            sb.Password = GetPassword.Get();
            //sb.Database = dbName;

            List<string> ret = new List<string>();

            string query = string.Format("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_SCHEMA = '{0}'", RAMDBName);

            using (MySqlConnection connection = new MySqlConnection(sb.ToString()))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader.GetString(0);
                            ret.Add(id);
                        }
                    }
                }
                connection.Close();
            }
            return ret;
        }
        void ExecuteNonQuery(MySqlConnection connection, string query)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        public DateTime LastDateTime(MySqlConnection connection, string tableName)
        {
            DateTime ret = DateTime.Parse("2010-01-01 12:00:00");
            string query = string.Format("select max(datetime) from {0}.{1}", HDDBName, tableName);
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                            break;
                        ret = reader.GetDateTime(0);
                    }
                }
            }
            return ret;
        }
        void Persistence(MySqlConnection connection, string tableName)
        {
            string query1 = string.Format("CREATE TABLE IF NOT EXISTS {0}.{1} like {2}.{3}", HDDBName, tableName, RAMDBName, tableName);
            //Console.WriteLine(query1);
            ExecuteNonQuery(connection, query1);
            DateTime lastTime = LastDateTime(connection, tableName);
            string query2 = string.Format("replace into {0}.{1} SELECT * from {2}.{3} WHERE datetime >= '{4}'", HDDBName, tableName, RAMDBName, tableName, lastTime);
            //Console.WriteLine(query2);
            ExecuteNonQuery(connection, query2);
        }
        public void Persistence()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = ServerIP;
            sb.SslMode = MySqlSslMode.None;
            sb.UserID = "root";
            sb.Password = GetPassword.Get();
            //sb.Database = dbName;

            var names = AllTableNames();

            using (MySqlConnection connection = new MySqlConnection(sb.ToString()))
            {
                connection.Open();
                foreach (string name in names) {
                    Persistence(connection, name);
                    Console.WriteLine(name+ " Persistence");
                }
                connection.Close();
            }
        }
        void Restore(MySqlConnection connection, string tableName)
        {
            string query = string.Format("replace into {0}.{1} SELECT * from {2}.{3}", RAMDBName, tableName, HDDBName, tableName);
            //Console.WriteLine(query);
            ExecuteNonQuery(connection, query);
        }
        public void Restore()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = ServerIP;
            sb.SslMode = MySqlSslMode.None;
            sb.UserID = "root";
            sb.Password = GetPassword.Get();
            //sb.Database = dbName;

            var names = AllTableNames();

            using (MySqlConnection connection = new MySqlConnection(sb.ToString()))
            {
                connection.Open();
                foreach (string name in names)
                {
                    Restore(connection, name);
                    Console.WriteLine(name+ " Restore");
                }
                connection.Close();
            }
        }
    }
}
