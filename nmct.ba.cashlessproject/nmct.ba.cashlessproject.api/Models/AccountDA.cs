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
    public class AccountDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        public static int UpdatePassword(int id, ChangePasswordBindingModel p, IEnumerable<Claim> claims)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction("AdminDB");

                string sql = "UPDATE Organisation SET Password = @Password WHERE ID = @ID AND Password = @OldPassword";
                DbParameter par1 = Database.AddParameter("ConnectionString", "@Password", Cryptography.Encrypt(p.NewPassword));
                DbParameter par2 = Database.AddParameter("ConnectionString", "@OldPassword", Cryptography.Encrypt(p.OldPassword));
                DbParameter par3 = Database.AddParameter("ConnectionString", "@ID", id);
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
    }
}