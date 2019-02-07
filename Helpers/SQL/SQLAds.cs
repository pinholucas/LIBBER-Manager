using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    public class SQLAds
    {
        public class ads
        {
            public int id { get; set; }

            public byte[] image { get; set; }

            public string title { get; set; }

            public string content { get; set; }
        }

        public static List<ads> Ads;

        public async static void LoadAdsTable()
        {
            Ads = new List<ads>();

            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Ads", SQLBasics.conn);

            try
            {
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    Ads = SQLBasics.DataReaderMapToList<ads>(datareader);
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR A TABELA ADS. \par Erro 01#015");
            }
        }

        public static void UpdateAd(int id, byte[] img, string title, string content)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Ads SET image = @image, title = @title, content = @content WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@content", content));

            cmd.ExecuteNonQuery();
        }

        public static void CreateAd(byte[] img, string title, string content)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO Ads (image, title, content) VALUES (@image, @title, @content)");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@content", content));

            cmd.ExecuteNonQuery();
        }

        public static void RemoveAd(int id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM Ads WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));

            cmd.ExecuteNonQuery();
        }

    }
}
