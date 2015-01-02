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
    public class ProductDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        public static List<Product> GetProducts(IEnumerable<Claim> claims)
        {
            List<Product> list = new List<Product>();
            
            string sql = "SELECT ID, ProductName, Price FROM Products";
            DbDataReader reader = Database.GetData(Database.GetConnection(CreateConnectionString(claims)), sql);

            while (reader.Read())
            {
                list.Add(Create(reader));
            }
            reader.Close();
            
            return list;
        }

        private static Product Create(IDataRecord record)
        {
            return new Product()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                ProductName = record["ProductName"].ToString(),
                Price = Double.Parse(record["Price"].ToString())
            };
        }

        public static int UpdateProduct(Product p)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction("ConnectionString");

                string sql = "UPDATE Products SET ProductName = @ProductName, Price = @Price WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@ProductName", p.ProductName);
                DbParameter par2 = Database.AddParameter("ConnectionString", "@Price", p.Price);
                DbParameter par3 = Database.AddParameter("ConnectionString", "@ID", p.ID);
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3);

                trans.Commit();
            }
            catch (Exception ex)
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

        public static int DeleteProduct(int id)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction("ConnectionString");

                string sql = "DELETE FROM Products WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@ID", id);
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
            }
            catch (Exception ex)
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

        public static int SaveProduct(Product p)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction("ConnectionString");

                string sql = "INSERT INTO Products (ProductName, Price) VALUES(@ProductName, @Price)";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@ProductName", p.ProductName);
                DbParameter par2 = Database.AddParameter("ConnectionString", "@Price", p.Price);
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
            }
            catch (Exception ex)
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
    }
}