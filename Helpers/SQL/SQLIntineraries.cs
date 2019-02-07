using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    class SQLIntineraries
    {
        public class intineraries
        {
            public int id { get; set; }

            public byte[] image { get; set; }

            public decimal rating { get; set; }

            public string title { get; set; }

            public string description { get; set; }

            public string details_content { get; set; }
        }

        public static List<intineraries> Intineraries;

        public async static void LoadIntinerariesTable()
        {
            Intineraries = new List<intineraries>();

            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Intineraries", SQLBasics.conn);

            try
            {
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    Intineraries = SQLBasics.DataReaderMapToList<intineraries>(datareader);
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR A TABELA INTINERARIES. \par Erro 03#015");
            }
        }

        public static void UpdateIntinerarie(int id, byte[] img, decimal rating, string title, string description, string details_content)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Intineraries SET image = @image, rating = @rating, title = @title, description = @description, details_content = @details_content WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@rating", rating));
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@details_content", details_content));

            cmd.ExecuteNonQuery();
        }

        public static void CreateIntinerarie(byte[] img, decimal rating, string title, string description, string details_content)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO Intineraries (image, rating, title, description, details_content) VALUES (@image, @rating, @title, @description, @details_content)");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@rating", rating));
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@details_content", details_content));

            cmd.ExecuteNonQuery();
        }

        public static void RemoveIntinerarie(int id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM Intineraries WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));

            cmd.ExecuteNonQuery();
        }

    }
}
