using nmct.ba.cashlessproject.api.helper;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace nmct.ba.cashlessproject.api.Models
{
    public class ProductDA
    {
        public static List<Product> GetProducts()
        {
            List<Product> list = new List<Product>();

            string sql = "SELECT ID, ProductName, Price FROM Products";
            DbDataReader reader = Database.GetData("ConnectionString", sql);

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