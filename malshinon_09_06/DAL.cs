using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using ZstdSharp.Unsafe;

namespace malshinon_09_06
{
    internal class DAL
    {
        private string connStr = "server=127.0.0.1;user=root;password=;database=malshinon";
        private MySqlConnection _conn;
        private MySqlCommand cmd = null;
        

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
        public void InsertPeople(string firstName, string lastName)
        {

            try
            {
                openConnection();
                string query = "INSERT INTO People (first_name, last_name, secret_code, type) " +
                               "VALUES (@FirstName, @LastName, @SecretCode, @Type)";
                using (var cmd = new MySqlCommand(query, _conn))
                { 
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@SecretCode", GetSecretCode());
                    cmd.Parameters.AddWithValue("@Type", "reporter");

                    cmd.ExecuteNonQuery();
                }
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
        public void insertReports(string malshin, string target, string text)
        {
            try
            {
                openConnection();
                int reporterId = GetPersonId(malshin);
                int targetId = GetPersonId(target);

                string quary = $"INSERT INTO`intelreports` (`reporter_id`,`target_id`,`text`)" +
                       "VALUES (@reporter_id, @target_id, @text)";

                using (var cmd = new MySqlCommand(quary, _conn))
                {
                    cmd.Parameters.AddWithValue("@reporter_id", reporterId);
                    cmd.Parameters.AddWithValue("@target_id", targetId);
                    cmd.Parameters.AddWithValue("@text", text);
                    

                    cmd.ExecuteNonQuery();
                    UpdateNumReports(malshin);
                    UpdateNumTargets(target);
                    UpdateType(malshin);
                }
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
        public int GetNumReports(string first_Name)
        {
            int count = 0;
            try
            {
                openConnection();
                string query = $"SELECT COUNT(*) FROM intelreports WHERE target_id = {GetPersonId(first_Name)}";
                using (var cmd = new MySqlCommand(query, _conn));
                {
                    var result = cmd.ExecuteScalar();
                    count = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading report count: " + ex.GetType().Name +"-" +ex.Message);
            }
            return count;
        }
        public int GetNumTargets(string first_Name)
        {
            int count = 0;
            try
            {
                openConnection();
                string query = $"SELECT COUNT(*) FROM intelreports WHERE reporter_id = {GetPersonId(first_Name)}";
                using (MySqlCommand cmd = new MySqlCommand(query, _conn)) ;
                {
                    var result = cmd.ExecuteScalar();
                    count = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading report count: " + ex.Message);
            }
            return count;
        }
        public List<string> GetAllNames()
        {
            List<string> names = new List<string>();

            string query = "SELECT `first_name` FROM `people`";

            using (var cmd = new MySqlCommand(query, _conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString("first_name");
                    names.Add(name);
                }
            }
            return names;
        }
        public int GetPersonId(string firstName)
        {
            int idPerson;
            string query = "SELECT id FROM people WHERE first_name = @FirstName";
            using (var cmd = new MySqlCommand(query, _conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", firstName);

                var result = cmd.ExecuteScalar();
                idPerson = Convert.ToInt32(result);
            }
            return idPerson;
        }
        public string GetSecretCode()
        {
            Random res = new Random();
            string str = "abcdefghijklmnopqrstuvwxyz123456789_)(*&^%$#@!~";
            string ran = "";
            for (int i = 0; i < 12; i++)
            {
                int x = res.Next(str.Length - 1);
                ran += str[x];
            }
            return ran;
        }
        public void UpdateNumReports(string firstName)
        {
            try
            {
                openConnection();
                string query = $"UPDATE people SET num_reports = num_reports + 1 WHERE first_name = @FirstName";
                using (var cmd = new MySqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.ExecuteNonQuery();
                }
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
        public void UpdateNumTargets(string firstName)
        {
            try
            {
                openConnection();
                string query = $"UPDATE people SET num_mentions = num_mentions + 1 WHERE first_name = @FirstName";
                using (var cmd = new MySqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.ExecuteNonQuery();
                }
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
        public void UpdateType(string first_Name)
        {
            try
            {
                openConnection();
                string query = $"UPDATE people SET `type` = 'potential_agent' WHERE `num_reports` >10";
                using (var cmd = new MySqlCommand(query, _conn))
                {
                    cmd.ExecuteNonQuery();
                }
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
    }
}
