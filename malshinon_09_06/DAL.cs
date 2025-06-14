﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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
        public void InsertPeople(string firstName, string lastName, string type)
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
                    cmd.Parameters.AddWithValue("@SecretCode", CreateSecretCode());
                    cmd.Parameters.AddWithValue("@Type",type);

                    Console.WriteLine($"{firstName} {lastName} adding as a {type}"); 

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

                    Console.WriteLine($"{malshin} reported on {target}, tath: {text}");

                    cmd.ExecuteNonQuery();
                    UpdateNumReports(malshin);
                    UpdateNumTargets(target);
                    UpdateType(malshin,target);
 
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
                string query = $"SELECT COUNT(`reporter_id`) FROM intelreports WHERE `reporter_id` = {GetPersonId(first_Name)}";
                using (var cmd = new MySqlCommand(query, _conn))
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
                string query = $"SELECT COUNT(`target_id`) FROM intelreports WHERE `target_id`= {GetPersonId(first_Name)}";
                using (MySqlCommand cmd = new MySqlCommand(query, _conn)) 
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
        public decimal GetAverage(string first_Name)
        {
            decimal average = 0;
            try
            {
                openConnection();
                string query = "SELECT AVG(CHAR_LENGTH(`text`)) FROM `intelreports` WHERE `reporter_id` = @first_Name";
                using (var cmd = new MySqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@first_Name", GetPersonId(first_Name));

                    var result = cmd.ExecuteScalar();
                    average = Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading average: " + ex.Message);
            }
            return average;
        }
        public string GetType(string first_Name)
        {
            try
            {
                openConnection();
                string quary = $"SELECT `type` FROM `people` WHERE `first_name`=@first_name ";
                using (var cmd = new MySqlCommand(quary, _conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", first_Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("No found");
                        }
                        string typey = reader.GetString("Type");
                        return typey;
                    }
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
            return "";
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
        public string GetPersonByName(string first_Name)
        {
            try
            {
                openConnection();
                string quary = $"SELECT * FROM `people` WHERE `first_name`=@first_name ";
                using (var cmd = new MySqlCommand(quary, _conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", first_Name);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("Not found");
                        }

                        int id = reader.GetInt32("id");
                        string first_name = reader.GetString("first_name");
                        string last_name = reader.GetString("last_name");
                        string secret_code = reader.GetString("secret_code");
                        string type = reader.GetString("type");
                        int num_reports = reader.GetInt32("num_reports");
                        int num_mentions = reader.GetInt32("num_mentions");

                        return ($"id: {id}. first_name: {first_name}. last_name: {last_name}. secret_code: {secret_code}. type: {type}. num_reports: {num_reports}. num_mentions: {num_mentions}." );
                    }
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
            return "";
        }
        public string GetPersonBySecretCode(string secret_code)
        {
            try
            {
                openConnection();
                string quary = $"SELECT * FROM `people` WHERE `secret_code`=@secret_code ";
                using (var cmd = new MySqlCommand(quary, _conn))
                {
                    cmd.Parameters.AddWithValue("@secret_code", secret_code);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("Not found");
                        }

                        int id = reader.GetInt32("id");
                        string first_name = reader.GetString("first_name");
                        string last_name = reader.GetString("last_name");
                        string secretCode = reader.GetString("secret_code");
                        string type = reader.GetString("type");
                        int num_reports = reader.GetInt32("num_reports");
                        int num_mentions = reader.GetInt32("num_mentions");

                        return ($"id: {id}. first_name: {first_name}. last_name: {last_name}. secret_code: {secretCode}. type: {type}. num_reports: {num_reports}. num_mentions: {num_mentions}.");
                    }
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
            return "";
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
        public string CreateSecretCode()
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
        public void UpdateToPotentialAgent(string first_Name)
        {
            try
            {
                openConnection();
                string query = $"UPDATE people SET `type` = 'potential_agent' WHERE `first_name`=@first_name";
                using (var cmd = new MySqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", first_Name);
                    Console.WriteLine(first_Name+ " type-  Updated to potential agent");
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
        public void UpdateToBoth(string first_Name)
        {
            try
            {
                openConnection();
                string query = $"UPDATE people SET `type` = 'both' WHERE `first_name`=@first_name ";
                using (var cmd = new MySqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@first_name", first_Name);
                    Console.WriteLine(first_Name + " type-  Updated to both");
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
        public void UpdateType(string malshin, string target)
        {
            if(GetAverage(malshin)>1 && GetNumReports(malshin) > 1)
            {
                UpdateToPotentialAgent(malshin);
            }
            else if(GetType(target) == "reporter" && GetType(malshin) == "target")
            {
                UpdateToBoth(target);
                UpdateToBoth(malshin);
            }
            else if (GetType(target)== "reporter")
            {
                UpdateToBoth(target);
            }
            else if (GetType(malshin) == "target")
            {
                UpdateToBoth(malshin);
            }
        }
    }
}
