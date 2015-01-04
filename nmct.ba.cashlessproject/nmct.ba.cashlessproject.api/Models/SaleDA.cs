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
    }
}