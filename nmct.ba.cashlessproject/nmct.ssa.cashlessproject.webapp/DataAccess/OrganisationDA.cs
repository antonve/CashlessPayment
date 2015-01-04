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
    }
}