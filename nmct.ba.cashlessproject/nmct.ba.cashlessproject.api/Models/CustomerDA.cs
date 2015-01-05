using nmct.ba.cashlessproject.api.helper;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Configuration;

namespace nmct.ba.cashlessproject.api.Models
{
    public class CustomerDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        public static List<Customer> GetCustomers(IEnumerable<Claim> claims)
        {
            List<Customer> list = new List<Customer>();
            
            string sql = "SELECT ID, CustomerName, Balance, Address, Picture FROM Customer";
            DbDataReader reader = Database.GetData(Database.GetConnection(CreateConnectionString(claims)), sql);

            while (reader.Read())
            {
                list.Add(Create(reader));
            }
            reader.Close();
            
            return list;
        }

        private static Customer Create(IDataRecord record)
        {
            return new Customer()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                CustomerName = record["CustomerName"].ToString(),
                Balance = Double.Parse(record["Balance"].ToString()),
                Address = record["Address"].ToString(),
                Picture = DBNull.Value.Equals(record["Picture"]) ? new byte[0] : (byte[])record["Picture"]
            };
        }

        public static int UpdateCustomer(Customer p, IEnumerable<Claim> claims)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction(CreateConnectionString(claims));

                string sql = "UPDATE Customer SET CustomerName = @CustomerName, Balance = @Balance, Address = @Address, Picture = @Picture WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@CustomerName", p.CustomerName);
                DbParameter par2 = Database.AddParameter("ConnectionString", "@Balance", p.Balance);
                DbParameter par3 = Database.AddParameter("ConnectionString", "@ID", p.ID);
                DbParameter par4 = Database.AddParameter("ConnectionString", "@Address", p.Address);
                DbParameter par5 = Database.AddParameter("ConnectionString", "@Picture", p.Picture);
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5);

                trans.Commit();
            }
            catch (Exception)
            {
                if (trans != null)
                    trans.Rollback();
            }
            finally
            {
                if (trans != null)
                    Database.ReleaseConnection(trans.Connection);
            }

            return rowsaffected;
        }

        public static Customer GetCustomerByName(ConnectionStringSettings cs, string name)
        {
            Customer result = null;
            string sql = "SELECT ID, CustomerName, Balance FROM Customer WHERE CustomerName = @ID";
            DbParameter par1 = Database.AddParameter("AdminDB", "@ID", name);
            DbDataReader reader = Database.GetData(Database.GetConnection(cs), sql, par1);

            while (reader.Read())
            {
                result = new Customer()
                {
                    ID = Int32.Parse(reader["ID"].ToString()),
                    CustomerName = reader["CustomerName"].ToString(),
                    Balance = Double.Parse(reader["Balance"].ToString())
                };
            }

            reader.Close();

            return result;
        }
    }
}