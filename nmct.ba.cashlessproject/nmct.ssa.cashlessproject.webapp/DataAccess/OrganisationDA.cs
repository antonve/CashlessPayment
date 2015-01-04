using nmct.ssa.cashlessproject.webapp.helper;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace nmct.ssa.cashlessproject.webapp.DataAccess
{
    public class OrganisationDA
    {
        const string cs = "AdminDB";

        public static List<Organisation> GetOrganisations()
        {
            List<Organisation> results = new List<Organisation>();
            string sql = "SELECT ID, Login, DbName, DbLogin, OrganisationName, Address, Email, Phone FROM Organisation";
            DbDataReader reader = Database.GetData(Database.GetConnection(cs), sql);

            while (reader.Read())
            {
                Organisation org = CreateOrganisation(reader);
                results.Add(org);
            }
            reader.Close();
            
            return results;
        }

        public static Organisation GetOrganisation(int id)
        {
            string sql = "SELECT ID, Login, DbName, DbLogin, OrganisationName, Address, Email, Phone FROM Organisation WHERE ID = @ID";

            DbParameter par1 = Database.AddParameter(cs, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(cs), sql, par1);

            while (reader.Read())
            {
                return CreateOrganisation(reader);
            }

            reader.Close();

            return null;
        }

        private static Organisation CreateOrganisation(IDataReader reader)
        {
            return new Organisation()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                Login = Cryptography.Decrypt(reader["Login"].ToString()),
                DbName = Cryptography.Decrypt(reader["DbName"].ToString()),
                DbLogin = Cryptography.Decrypt(reader["DbLogin"].ToString()),
                OrganisationName = reader["OrganisationName"].ToString(),
                Address = reader["Address"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"].ToString()
            };
        }

        public static int Save(Organisation org)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction(cs);

                if (org.ID == 0)
                {
                    string sql = 
@"INSERT INTO Organisation 
(OrganisationName, Address, Email, Phone, Login, Password, DbName, DbLogin, DbPassword)
VALUES (@Org, @Address, @Email, @Phone, @Login, @Password, @DbName, @DbLogin, @DbPassword)";

                    DbParameter par1 = Database.AddParameter(cs, "@Org", org.OrganisationName);
                    DbParameter par2 = Database.AddParameter(cs, "@Address", org.Address);
                    DbParameter par3 = Database.AddParameter(cs, "@Email", org.Email);
                    DbParameter par4 = Database.AddParameter(cs, "@Phone", org.Phone);
                    DbParameter par5 = Database.AddParameter(cs, "@Login", Cryptography.Encrypt(org.Login));
                    DbParameter par6 = Database.AddParameter(cs, "@Password", Cryptography.Encrypt(org.Password));
                    DbParameter par7 = Database.AddParameter(cs, "@DbName", Cryptography.Encrypt(org.DbName));
                    DbParameter par8 = Database.AddParameter(cs, "@DbLogin", Cryptography.Encrypt(org.DbLogin));
                    DbParameter par9 = Database.AddParameter(cs, "@DbPassword", Cryptography.Encrypt(org.DbPassword));
                    rowsaffected += Database.InsertData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);
                }
                else
                {
                    DbParameter parPass = null;
                    string sql = "UPDATE Organisation SET Login = @Login, OrganisationName = @Org, Address = @Address, Email = @Email, Phone = @Phone";
                    
                    if (org.Password != null)
                    {
                        sql += ", Password = @Password";
                        parPass = Database.AddParameter(cs, "@Password", Cryptography.Encrypt(org.Password));
                    }

                    sql += " WHERE ID = @ID";

                    DbParameter par1 = Database.AddParameter(cs, "@ID", org.ID);
                    DbParameter par2 = Database.AddParameter(cs, "@Login", Cryptography.Encrypt(org.Login));
                    DbParameter par3 = Database.AddParameter(cs, "@Org", org.OrganisationName);
                    DbParameter par4 = Database.AddParameter(cs, "@Address", org.Address);
                    DbParameter par5 = Database.AddParameter(cs, "@Email", org.Email);
                    DbParameter par6 = Database.AddParameter(cs, "@Phone", org.Phone);
                    rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, parPass);

                }
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
    }
}