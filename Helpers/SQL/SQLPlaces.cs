using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    public class SQLPlaces
    {
        public class places
        {
            public int id { get; set; }

            public int icon { get; set; }

            public string title { get; set; }

            public string description { get; set; }

            public string inmap_description { get; set; }

            public double lat { get; set; }

            public double lng { get; set; }
        }

        public class additionals
        {
            public int id { get; set; }

            public byte[] icon { get; set; }

            public string title { get; set; }

            public string price { get; set; }

            public string details { get; set; }
        }

        public static List<places> Places;
        public static List<additionals> Additionals;

        public async static void LoadPlacesTable(int categoryID)
        {
            SqliteCommand cmd = null;            

            switch (categoryID)
            {
                case GlobalMethods.Campings:
                    cmd = new SqliteCommand("SELECT * FROM Campings", SQLBasics.conn);
                    break;
                case GlobalMethods.Parks:
                    cmd = new SqliteCommand("SELECT * FROM Parks", SQLBasics.conn);
                    break;
                case GlobalMethods.Additionals:
                    cmd = new SqliteCommand("SELECT * FROM Additionals", SQLBasics.conn);
                    break;
            }

            try
            {                 
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    if (GlobalMethods.IsLocalsCategory())
                    {
                        Places = new List<places>();
                        Places = SQLBasics.DataReaderMapToList<places>(datareader);
                    }
                    else if (GlobalMethods.IsAdditionalsCategory())
                    {
                        Additionals = new List<additionals>();
                        Additionals = SQLBasics.DataReaderMapToList<additionals>(datareader);
                    }
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR A CATEGORIA. \par Erro 02#015");
            }
        }

        public static void UpdatePlacesTable(int categoryID, int id, int icon, string title, string description, string inmap_description, double lat, double lng)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            switch (categoryID)
            {
                case 0:
                    cmd.CommandText = String.Format("UPDATE Campings SET icon = @icon, title = @title, description = @description, inmap_description = @inmap_description, lat = @lat, lng = @lng WHERE id = " + id);
                    break;
                case 1:
                    cmd.CommandText = String.Format("UPDATE Parks SET icon = @icon, title = @title, description = @description, inmap_description = @inmap_description, lat = @lat, lng = @lng WHERE id = " + id);
                    break;
            }

            cmd.Parameters.Add(new SqliteParameter("@icon", icon));
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@inmap_description", inmap_description));
            cmd.Parameters.Add(new SqliteParameter("@lat", lat));
            cmd.Parameters.Add(new SqliteParameter("@lng", lng));

            cmd.ExecuteNonQuery();
        }

        public static void InsertPlace(int categoryID, int id, int icon, string title, string description, string inmap_description, double lat, double lng)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            switch (categoryID)
            {
                case GlobalMethods.Campings:
                    cmd.CommandText = String.Format("INSERT INTO Campings (icon, title, description, inmap_description, lat, lng) VALUES (@icon, @title, @description, @inmap_description, @lat, @lng)");
                    break;
                case GlobalMethods.Parks:
                    cmd.CommandText = String.Format("INSERT INTO Parks (icon, title, description, inmap_description, lat, lng) VALUES (@icon, @title, @description, @inmap_description, @lat, @lng)");
                    break;
            }

            cmd.Parameters.Add(new SqliteParameter("@icon", icon));
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@inmap_description", inmap_description));
            cmd.Parameters.Add(new SqliteParameter("@lat", lat));
            cmd.Parameters.Add(new SqliteParameter("@lng", lng));

            cmd.ExecuteNonQuery();
        }

        public static void RemovePlace(int categoryID, int id)
        {
            // Remove o Local da tabela PLACES
            var cmdPLACES = SQLBasics.conn.CreateCommand();
            // Remove o Local da tabela RELATIONSHIPS
            var cmdRELATIONSHIPS = SQLBasics.conn.CreateCommand();

            switch (categoryID)
            {
                case GlobalMethods.Campings:
                    cmdPLACES.CommandText = String.Format("DELETE FROM Campings WHERE id = @id", SQLBasics.conn);
                    cmdRELATIONSHIPS.CommandText = String.Format("DELETE FROM Relationships WHERE category_id = @cID AND category_type = 'camping'");
                    break;
                case GlobalMethods.Parks:
                    cmdPLACES.CommandText = String.Format("DELETE FROM Parks WHERE id = @id", SQLBasics.conn);
                    cmdRELATIONSHIPS.CommandText = String.Format("DELETE FROM Relationships WHERE category_id = @cID AND category_type = 'park'");
                    break;
            }

            cmdPLACES.Parameters.Add(new SqliteParameter("@id", id));
            cmdRELATIONSHIPS.Parameters.Add(new SqliteParameter("@cID", id));

            cmdPLACES.ExecuteNonQuery();
            cmdRELATIONSHIPS.ExecuteNonQuery();
        }
    }
}
