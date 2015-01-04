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
    public class RegisterDA
    {
        private static ConnectionStringSettings CreateConnectionString(IEnumerable<Claim> claims)
        {
            string dblogin = claims.FirstOrDefault(c => c.Type == "dblogin").Value;
            string dbpass = claims.FirstOrDefault(c => c.Type == "dbpass").Value;
            string dbname = claims.FirstOrDefault(c => c.Type == "dbname").Value;

            return Database.CreateConnectionString("System.Data.SqlClient", @"IKORE\SQLEXPRESS", Cryptography.Decrypt(dbname), Cryptography.Decrypt(dblogin), Cryptography.Decrypt(dbpass));
        }

        public static List<Register> GetRegisters(IEnumerable<Claim> claims)
        {
            Dictionary<int, Register> list = new Dictionary<int, Register>();
            List<RegisterLog> logs = new List<RegisterLog>();
            
            string sql = "SELECT reg.ID, reg.RegisterName, reg.Device, re.EmployeeID, re.FromTime, re.UntilTime, e.EmployeeName FROM Register AS reg LEFT JOIN Register_Employee as re ON (re.RegisterID = reg.ID) LEFT JOIN Employee as e ON (e.ID = re.EmployeeID)";
            DbDataReader reader = Database.GetData(Database.GetConnection(CreateConnectionString(claims)), sql);

            while (reader.Read())
            {
                int key = Int32.Parse(reader["ID"].ToString());
                if (!list.ContainsKey(key))
                {
                    list.Add(key, Create(reader));
                }
                if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                {
                    logs.Add(CreateLog(reader));
                }
            }
            reader.Close();

            foreach (RegisterLog log in logs)
            {
                list[log.RegisterId].Logs.Add(log);
            }
            
            return list.Values.ToList();
        }

        private static Register Create(IDataRecord record)
        {
            return new Register()
            {
                ID = Int32.Parse(record["ID"].ToString()),
                RegisterName = record["RegisterName"].ToString(),
                Device = record["Device"].ToString(),
                Logs = new List<RegisterLog>()
            };
        }

        private static RegisterLog CreateLog(IDataRecord record)
        {
            return new RegisterLog()
            {
                RegisterId = Int32.Parse(record["ID"].ToString()),
                EmployeeId = Int32.Parse(record["EmployeeID"].ToString()),
                EmployeeName = record["EmployeeName"].ToString(),
                FromTime = (DateTime)record["FromTime"],
                UntilTime = (DateTime)record["UntilTime"]
            };
        }
    }
}