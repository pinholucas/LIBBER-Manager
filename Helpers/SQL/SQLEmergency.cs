using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libber_Manager.Controls;

namespace Libber_Manager.Helpers
{
    class SQLEmergency
    {
        public static string phone = null;
        public static string sms = null;
        public static string email = null;

        public static bool hasRows = false;

        public async static void LoadEmergencyTable()
        {
            SqliteCommand cmd = new SqliteCommand("SELECT * From Emergency", SQLBasics.conn);

            try
            {
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    if (datareader.HasRows)
                    {
                        while (datareader.Read())
                        {
                            phone = datareader[1].ToString();
                            sms = datareader[2].ToString();
                            email = datareader[3].ToString();
                        }

                        hasRows = true;
                    }
                    else
                    {
                        phone = "";
                        sms = "";
                        email = "";
                    }
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR A TABELA EMERGENCY. \par Erro 05#015");
            }
        }

        public static void UpdateEmergencyTable(string phone, string sms, string email)
        {
            SqliteCommand cmd = SQLBasics.conn.CreateCommand();

            if (hasRows)
            {
                cmd.CommandText = String.Format("UPDATE Emergency SET libber_phoneNumber = @phone, libber_smsNumber = @sms, libber_email = @email WHERE id = 1");
            }
            else
            {
                cmd.CommandText = String.Format("INSERT INTO Emergency (libber_phoneNumber, libber_smsNumber, libber_email) VALUES (@phone, @sms, @email)");
            }

            cmd.Parameters.Add(new SqliteParameter("@phone", phone));
            cmd.Parameters.Add(new SqliteParameter("@sms", sms));
            cmd.Parameters.Add(new SqliteParameter("@email", email));

            cmd.ExecuteNonQuery();
        }
    }
}
