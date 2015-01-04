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
    public class EmployeeDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        public static List<Employee> GetEmployees(IEnumerable<Claim> claims)
        {
            List<Employee> list = new List<Employee>();
            
            string sql = "SELECT ID, EmployeeName, Address, Email, Phone FROM Employee";
            DbDataReader reader = Database.GetData(Database.GetConnection(CreateConnectionString(claims)), sql);

            while (reader.Read())
            {
                list.Add(Create(reader));
            }
            reader.Close();
            
            return list;
        }

        private static Employee Create(IDataRecord record)
        {
            return new Employee()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                EmployeeName = record["EmployeeName"].ToString(),
                Address = record["Address"].ToString(),
                Email = record["Email"].ToString(),
                Phone = record["Phone"].ToString()
            };
        }

        public static int UpdateEmployee(Employee p, IEnumerable<Claim> claims)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction(CreateConnectionString(claims));

                string sql = "UPDATE Employee SET EmployeeName = @EmployeeName, Address = @Address, Email = @Email, Phone = @Phone WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@EmployeeName", p.EmployeeName);
                DbParameter par2 = Database.AddParameter("ConnectionString", "@Address", p.Address);
                DbParameter par3 = Database.AddParameter("ConnectionString", "@ID", p.ID);
                DbParameter par4 = Database.AddParameter("ConnectionString", "@Email", p.Email);
                DbParameter par5 = Database.AddParameter("ConnectionString", "@Phone", p.Phone);
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

        public static int DeleteEmployee(int id, IEnumerable<Claim> claims)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction(CreateConnectionString(claims));

                string sql = "DELETE FROM Employee WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@ID", id);
                rowsaffected += Database.ModifyData(trans, sql, par1);

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

        public static int SaveEmployee(Employee p, IEnumerable<Claim> claims)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction(CreateConnectionString(claims));

                string sql = "INSERT INTO Employee (EmployeeName, Address, Email, Phone) VALUES(@EmployeeName, @Address, @Email, @Phone)";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@EmployeeName", p.EmployeeName);
                DbParameter par2 = Database.AddParameter("ConnectionString", "@Address", p.Address);
                DbParameter par3 = Database.AddParameter("ConnectionString", "@Email", p.Email);
                DbParameter par4 = Database.AddParameter("ConnectionString", "@Phone", p.Phone);
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4);

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

        public static Employee GetEmployeeByName(string name, Organisation org)
        {
            Employee result = null;
            ConnectionStringSettings cs = Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(org.DbName), Cryptography.Decrypt(org.DbLogin), Cryptography.Decrypt(org.DbPassword));
            string sql = "SELECT ID FROM Employee WHERE EmployeeName = @Name";
            DbParameter par1 = Database.AddParameter("AdminDB", "@Name", name);
            DbDataReader reader = Database.GetData(Database.GetConnection(cs), sql, par1);

            while (reader.Read())
            {
                result = new Employee()
                {
                    ID = Int32.Parse(reader["ID"].ToString())
                };
            }

            reader.Close();

            return result;
        }
    }
}