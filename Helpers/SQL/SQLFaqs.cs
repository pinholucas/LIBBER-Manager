using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    class SQLFaqs
    {
        public class faqs
        {
            public int id { get; set; }
            public string title { get; set; }
            public string content { get; set; }
        }

        public static List<faqs> FAQs;

        public async static void LoadFAQsTable()
        {
            FAQs = new List<faqs>();

            SqliteCommand cmd = new SqliteCommand("SELECT * FROM FAQs", SQLBasics.conn);

            try
            {
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    FAQs = SQLBasics.DataReaderMapToList<faqs>(datareader);
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR A FAQs. \par Erro 04#015");
            }
        }

        public static void UpdateFAQ(int id, string title, string content)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE FAQs SET title = @title, content = @content WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@content", content));

            cmd.ExecuteNonQuery();
        }

        public static void CreateFAQ(string title, string content)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO FAQs (title, content) VALUES (@title, @content)");

            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@content", content));

            cmd.ExecuteNonQuery();
        }

        public static void RemoveFAQ(int id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM FAQs WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));

            cmd.ExecuteNonQuery();
        }

    }
}
