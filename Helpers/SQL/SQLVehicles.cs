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
    class SQLVehicles
    {
        public class vehicles
        {
            public int id { get; set; }

            public byte[] image { get; set; }

            public string name { get; set; }

            public string card_description { get; set; }

            public decimal value { get; set; }

            public string description { get; set; }

            public string attributes { get; set; }
        }

        public static List<vehicles> Vehicles;

        public async static void LoadVehiclesTable()
        {
            Vehicles = new List<vehicles>();

            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Vehicles", SQLBasics.conn);

            try
            {
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    Vehicles = SQLBasics.DataReaderMapToList<vehicles>(datareader);
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR A TABELA VEHICLES. \par Erro 07#015");
            }
        }

        public static void UpdateVehicle(int id, byte[] img, string name, string cardDescription, decimal value, string description, string attributes)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Vehicles SET image = @image, name = @name, card_description = @cardDescription, value = @value, description = @description, attributes = @attributes WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@name", name));
            cmd.Parameters.Add(new SqliteParameter("@cardDescription", cardDescription));
            cmd.Parameters.Add(new SqliteParameter("@value", value));
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@attributes", attributes));

            cmd.ExecuteNonQuery();
        }

        public static void CreateVehicle(byte[] img, string name, string cardDescription, decimal value, string description, string attributes)
        {            
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO Vehicles (image, name, card_description, value, description, attributes) VALUES (@image, @name, @cardDescription, @value, @description, @attributes)");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@name", name));
            cmd.Parameters.Add(new SqliteParameter("@cardDescription", cardDescription));
            cmd.Parameters.Add(new SqliteParameter("@value", value));
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@attributes", attributes));

            cmd.ExecuteNonQuery();
        }

        public static void RemoveVehicle(int id)
        {            
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM Vehicles WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));

            cmd.ExecuteNonQuery();
        }

    }
}
