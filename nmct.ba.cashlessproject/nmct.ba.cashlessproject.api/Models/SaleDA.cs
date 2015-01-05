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
    public class SaleDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        public static List<Sale> GetSales(IEnumerable<Claim> claims)
        {
            List<Sale> list = new List<Sale>();

            string sql = 
@"SELECT s.ID, [Timestamp], CustomerID, RegisterID, ProductID, Amount, TotalPrice, ProductName, RegisterName, CustomerName, Price FROM Sale as s
LEFT JOIN Customer as c ON (c.id = s.CustomerID)
LEFT JOIN Register as r ON (r.id = s.RegisterID)
LEFT JOIN Product as p On (p.id = s.ProductID)";
            DbDataReader reader = Database.GetData(Database.GetConnection(CreateConnectionString(claims)), sql);

            while (reader.Read())
            {
                list.Add(Create(reader));
            }
            reader.Close();
            
            return list;
        }

        private static Sale Create(IDataRecord record)
        {
            return new Sale()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                Timestamp = (DateTime)record["Timestamp"],
                ProductName = record["ProductName"].ToString(),
                CustomerName = record["CustomerName"].ToString(),
                RegisterName = record["RegisterName"].ToString(),
                ProductID = Int32.Parse(record["ProductID"].ToString()),
                CustomerID = Int32.Parse(record["CustomerID"].ToString()),
                RegisterID = Int32.Parse(record["RegisterID"].ToString()),
                Amount = Int32.Parse(record["Amount"].ToString()),
                TotalPrice = Double.Parse(record["TotalPrice"].ToString()),
                SinglePrice = Double.Parse(record["Price"].ToString())
            };
        }

        public static int SaveSale(Sale sale, IEnumerable<Claim> claims)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                double TotalPrice = sale.SinglePrice * sale.Amount;
                trans = Database.BeginTransaction(CreateConnectionString(claims));

                string sql = "INSERT INTO Sale ([Timestamp], CustomerID, RegisterID, ProductID, Amount, TotalPrice) VALUES(GetDate(), @CID, @RID, @PID, @Amount, @TP)";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@CID", sale.CustomerID);
                DbParameter par2 = Database.AddParameter("ConnectionString", "@RID", sale.RegisterID);
                DbParameter par3 = Database.AddParameter("ConnectionString", "@PID", sale.ProductID);
                DbParameter par4 = Database.AddParameter("ConnectionString", "@Amount", sale.Amount);
                DbParameter par5 = Database.AddParameter("ConnectionString", "@TP", TotalPrice);
                rowsaffected += Database.InsertData(trans, sql, par1, par2, par3, par4, par5);

                sql = "UPDATE Customer SET Balance = Balance - @TP WHERE ID = @CID";
                par1 = Database.AddParameter("ConnectionString", "@TP", TotalPrice);
                par2 = Database.AddParameter("ConnectionString", "@CID", sale.CustomerID);
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