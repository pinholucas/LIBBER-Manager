using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Libber_Manager.Helpers
{
    class SQLVehiclesAdditionals
    {
        public class Relationships
        {
            public int Id { get; set; }

            public int IntinerarieID { get; set; }

            public int CategoryID { get; set; }

            public string Type { get; set; }
        }

        public class vehiclesAdditionals
        {
            public int id { get; set; }

            public BitmapImage icon { get; set; }

            public string title { get; set; }

            public string price { get; set; }

            public int details { get; set; }

            public bool IsChecked { get; set; }
        }

        public static List<SQLRelationships.Relations> AdditionalsRelations;

        public static List<SQLRelationships.ListItem> Additionals;

        #region VEHICLE ADDITIONALS

        public async static void LoadAdditionalsTable(int intiID)
        {
            // Carrega a lista de Adicionais e suas relações com o intiID
            AdditionalsRelations = new List<SQLRelationships.Relations>();
            Additionals = new List<SQLRelationships.ListItem>();

            await SQLRelationships.LoadTableAndRelationships("Additionals", "additional", intiID, Additionals, AdditionalsRelations);
        }

        #endregion

        #region VEHICLE ADDITIONALS MANAGER

        public static void UpdateAdditional(int id, byte[] img, string title, string price, string details)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Additionals SET icon = @icon, title = @title, price = @price, details = @details WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@icon", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@price", price));
            cmd.Parameters.Add(new SqliteParameter("@details", details));

            cmd.ExecuteNonQuery();
        }

        public static void CreateAdditional(byte[] img, string title, string price, string details)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO Additionals (icon, title, price, details) VALUES (@icon, @title, @price, @details)");

            SqliteParameter parameter = new SqliteParameter("@icon", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@title", title));
            cmd.Parameters.Add(new SqliteParameter("@price", price));
            cmd.Parameters.Add(new SqliteParameter("@details", details));

            cmd.ExecuteNonQuery();
        }

        public static void RemoveAdditional(int id)
        {
            // Remove o Local da tabela PLACES
            var cmdPLACES = SQLBasics.conn.CreateCommand();
            // Remove o Local da tabela RELATIONSHIPS
            var cmdRELATIONSHIPS = SQLBasics.conn.CreateCommand();

            cmdPLACES.CommandText = String.Format("DELETE FROM Additionals WHERE id = @id", SQLBasics.conn);
            cmdRELATIONSHIPS.CommandText = String.Format("DELETE FROM Relationships WHERE category_id = @cID AND category_type = 'additional'");

            cmdPLACES.Parameters.Add(new SqliteParameter("@id", id));
            cmdRELATIONSHIPS.Parameters.Add(new SqliteParameter("@cID", id));

            cmdPLACES.ExecuteNonQuery();
            cmdRELATIONSHIPS.ExecuteNonQuery();
        }

        #endregion
    }
}
