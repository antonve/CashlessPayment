using nmct.ba.cashlessproject.api.helper;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace nmct.ba.cashlessproject.api.Models
{
    public class OrganisationDA
    {
        public static Organisation CheckCredentials(string username, string password)
        {
            string sql = "SELECT * FROM Organisation WHERE Login=@Login AND Password=@Password";

            DbParameter par1 = Database.AddParameter("AdminDB", "@Login", Cryptography.Encrypt(username));
            DbParameter par2 = Database.AddParameter("AdminDB", "@Password", Cryptography.Encrypt(password));
            try
            {
                DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql, par1, par2);
                reader.Read();
                return new Organisation()
                {
                    ID = Int32.Parse(reader["ID"].ToString()),
                    Login = reader["Login"].ToString(),
                    Password = reader["Password"].ToString(),
                    DbName = reader["DbName"].ToString(),
                    DbLogin = reader["DbLogin"].ToString(),
                    DbPassword = reader["DbPassword"].ToString(),
                    OrganisationName = reader["OrganisationName"].ToString(),
                    Address = reader["Address"].ToString(),
                    Email = reader["Email"].ToString(),
                    Phone = reader["Phone"].ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public static List<Organisation> GetOrganisations()
        {
            List<Organisation> results = new List<Organisation>();
            string sql = "SELECT ID, OrganisationName FROM Organisation";
            DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql);

            while (reader.Read())
            {
                Organisation org = new Organisation()
                {
                    ID = Convert.ToInt32(reader["ID"].ToString()),
                    OrganisationName = reader["OrganisationName"].ToString()
                };

                results.Add(org);
            }

            reader.Close();

            return results;
        }

        public static Organisation GetOrganisation(int id)
        {
            Organisation result = null;
            string sql = "SELECT ID, DbName, DbLogin, DbPassword, OrganisationName FROM Organisation WHERE ID = @ID";
            DbParameter par1 = Database.AddParameter("AdminDB", "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql, par1);

            while (reader.Read())
            {
                result = new Organisation()
                {
                    ID = Int32.Parse(reader["ID"].ToString()),
                    DbName = reader["DbName"].ToString(),
                    DbLogin = reader["DbLogin"].ToString(),
                    DbPassword = reader["DbPassword"].ToString(),
                    OrganisationName = reader["OrganisationName"].ToString()
                };
            }

            reader.Close();

            return result;
        }

        public static ConnectionStringSettings GetCSById(int id)
        {
            Organisation org = null;
            string sql = "SELECT ID, DbName, DbLogin, DbPassword, OrganisationName FROM Organisation WHERE ID = @ID";
            DbParameter par1 = Database.AddParameter("AdminDB", "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection("AdminDB"), sql, par1);

            while (reader.Read())
            {
                org = new Organisation()
                {
                    ID = Int32.Parse(reader["ID"].ToString()),
                    DbName = reader["DbName"].ToString(),
                    DbLogin = reader["DbLogin"].ToString(),
                    DbPassword = reader["DbPassword"].ToString(),
                    OrganisationName = reader["OrganisationName"].ToString()
                };
            }

            reader.Close();

            if (org == null)
            {
                return null;
            }

            ConnectionStringSettings cs = Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(org.DbName), Cryptography.Decrypt(org.DbLogin), Cryptography.Decrypt(org.DbPassword));
            
            return cs;
        }

    }
}