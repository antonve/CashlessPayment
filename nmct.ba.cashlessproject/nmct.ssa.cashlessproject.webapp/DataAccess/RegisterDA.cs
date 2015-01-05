using nmct.ssa.cashlessproject.webapp.helper;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace nmct.ssa.cashlessproject.webapp.DataAccess
{
    public class RegisterDA
    {
        const string cs = "AdminDB";

        public static List<OrganisationRegister> GetRegisters()
        {
            Dictionary<int, Organisation> orgs = OrganisationDA.GetOrganisationsDict();
            List<OrganisationRegister> results = new List<OrganisationRegister>();
            string sql = "SELECT ID, RegisterName, Device, OrganisationID, ExternalID FROM Register";
            DbDataReader reader = Database.GetData(Database.GetConnection(cs), sql);

            while (reader.Read())
            {
                OrganisationRegister reg = CreateRegister(reader);
                reg.Organisation = orgs[reg.OrganisationID];
                results.Add(reg);
            }
            reader.Close();
            
            return results;
        }

        private static OrganisationRegister CreateRegister(IDataReader reader)
        {
            return new OrganisationRegister()
            {
                ID = Convert.ToInt32(reader["ID"].ToString()),
                RegisterName = reader["RegisterName"].ToString(),
                Device = reader["Device"].ToString(),
                OrganisationID = Convert.ToInt32(reader["OrganisationID"].ToString()),
                ExternalID = Convert.ToInt32(reader["ExternalID"].ToString()),
            };
        }

        /*
        public static Register GetRegister(int id)
        {
            string sql = "SELECT ID, Login, DbName, DbLogin, RegisterName, Address, Email, Phone FROM Register WHERE ID = @ID";

            DbParameter par1 = Database.AddParameter(cs, "@ID", id);
            DbDataReader reader = Database.GetData(Database.GetConnection(cs), sql, par1);

            while (reader.Read())
            {
                return CreateRegister(reader);
            }

            reader.Close();

            return null;
        }

        public static int Save(Register org)
        {
            int rowsaffected = 0;
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction(cs);

                if (org.ID == 0)
                {
                    string sql = 
@"INSERT INTO Register 
(RegisterName, Address, Email, Phone, Login, Password, DbName, DbLogin, DbPassword)
VALUES (@Org, @Address, @Email, @Phone, @Login, @Password, @DbName, @DbLogin, @DbPassword)";

                    DbParameter par1 = Database.AddParameter(cs, "@Org", org.RegisterName);
                    DbParameter par2 = Database.AddParameter(cs, "@Address", org.Address);
                    DbParameter par3 = Database.AddParameter(cs, "@Email", org.Email);
                    DbParameter par4 = Database.AddParameter(cs, "@Phone", org.Phone);
                    DbParameter par5 = Database.AddParameter(cs, "@Login", Cryptography.Encrypt(org.Login));
                    DbParameter par6 = Database.AddParameter(cs, "@Password", Cryptography.Encrypt(org.Password));
                    DbParameter par7 = Database.AddParameter(cs, "@DbName", Cryptography.Encrypt(org.DbName));
                    DbParameter par8 = Database.AddParameter(cs, "@DbLogin", Cryptography.Encrypt(org.DbLogin));
                    DbParameter par9 = Database.AddParameter(cs, "@DbPassword", Cryptography.Encrypt(org.DbPassword));
                    rowsaffected += Database.InsertData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

                    org.ID = rowsaffected;
                    CreateDatabase(org);
                }
                else
                {
                    DbParameter parPass = null;
                    string sql = "UPDATE Register SET Login = @Login, RegisterName = @Org, Address = @Address, Email = @Email, Phone = @Phone";
                    
                    if (org.Password != null)
                    {
                        sql += ", Password = @Password";
                        parPass = Database.AddParameter(cs, "@Password", Cryptography.Encrypt(org.Password));
                    }

                    sql += " WHERE ID = @ID";

                    DbParameter par1 = Database.AddParameter(cs, "@ID", org.ID);
                    DbParameter par2 = Database.AddParameter(cs, "@Login", Cryptography.Encrypt(org.Login));
                    DbParameter par3 = Database.AddParameter(cs, "@Org", org.RegisterName);
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


        private static void CreateDatabase(Register o)
        {
            // create the actual database
            string create = File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/create.txt"));
            string sql = create.Replace("@@DbName", o.DbName).Replace("@@DbLogin", o.DbLogin).Replace("@@DbPassword", o.DbPassword);
            foreach (string commandText in RemoveGo(sql))
            {
                try
                {
                    int res = Database.ModifyData(Database.GetConnection("AdminDB"), commandText);
                    Console.WriteLine(res.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // create login, user and tables
            DbTransaction trans = null;
            try
            {
                trans = Database.BeginTransaction("AdminDB");

                string fill = File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/fill.txt"));
                string sql2 = fill.Replace("@@DbName", o.DbName).Replace("@@DbLogin", o.DbLogin).Replace("@@DbPassword", o.DbPassword);

                foreach (string commandText in RemoveGo(sql2))
                {
                    Database.ModifyData(trans, commandText);
                }

                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                Console.WriteLine(ex.Message);
            }
        }

        private static string[] RemoveGo(string input)
        {
            //split the script on "GO" commands
            string[] splitter = new string[] { "\r\nGO\r\n" };
            string[] commandTexts = input.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            return commandTexts;
        }*/
    }
}