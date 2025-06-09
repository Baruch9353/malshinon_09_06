using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace malshinon_09_06
{
    internal class DAL
    {
        public People People;
        public IntelReports IntelReports;

        private string connStr = "server=127.0.0.1;user=root;password=;database=malshinon";
        private MySqlConnection _conn;
        private MySqlCommand cmd = null;
        private MySqlDataReader reader = null;

        public DAL()
        {
            try
            {
                openConnection();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }
        }

        public MySqlConnection openConnection()
        {
            if (_conn == null)
            {
                _conn = new MySqlConnection(connStr);
            }

            if (_conn.State != System.Data.ConnectionState.Open)
            {
                _conn.Open();
                Console.WriteLine("Connection successful.");
            }

            return _conn;
        }

        public void closeConnection()
        {
            if (_conn != null && _conn.State == System.Data.ConnectionState.Open)
            {
                _conn.Close();
                _conn = null;
            }
        }

        public string GetSecretCode()
        {
            openConnection();
            Random res = new Random();
            string str = "abcdefghijklmnopqrstuvwxyz123456789_)(*&^%$#@!~";
            int size = 35;
            string ran = "";
            for (int i = 0; i < size; i++)
            {
                int x = res.Next(str.Length);
                ran += str[x];
            }
            return ran;
        }

        public string GetType()
        {
            openConnection();
            string type = "reporter";
            return type;
        }

        public void InsertPeople(string firstName, string lastName)
        {
            People people = new People(firstName, lastName);

            try
            {
                openConnection();
                string query = "INSERT INTO People (first_name, last_name, secret_code, type, num_reports, num_mentions) " +
                               "VALUES (@FirstName, @LastName, @SecretCode, @Type, 0, 0)";
                MySqlCommand cmd = new MySqlCommand(query, _conn);

                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@SecretCode", GetSecretCode());
                cmd.Parameters.AddWithValue("@Type", GetType());

                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine($"Duplicate entry: {firstName} {lastName} already exists.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }
            finally
            {
                closeConnection();
            }
        }


        public int GetNumReports()
        {
            return People.NumReports;
        }
        public int GetNumTargets()
        {
            return People.NumMentions;
        }

        public string GetTime()
        {
            DateTime time = DateTime.Now;
            return time.ToString();
        }
        public void InsertReports(string text)
        {

        }
    }
}
