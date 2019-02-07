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
    class SQLVehicleImages
    {
        public class vehicleImages
        {
            public int id { get; set; }

            public byte[] image { get; set; }

            public string description { get; set; }

            public int owner_id { get; set; }

            public BitmapImage thumb { get; set; }
        }

        public static List<vehicleImages> VehicleImages;

        public async static Task LoadVehicleImagesTable(int ownerId)
        {
            VehicleImages = new List<vehicleImages>();

            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Vehicles_Photos WHERE owner_id = " + ownerId, SQLBasics.conn);

            using (SqliteDataReader datareader = await cmd.ExecuteReaderAsync())
            {
                while (await datareader.ReadAsync())
                {
                    vehicleImages vi = new vehicleImages();

                    vi.id = Convert.ToUInt16(datareader[0]);
                    vi.image = (byte[])datareader[1];
                    vi.description = (string)datareader[2];
                    vi.owner_id = Convert.ToUInt16(datareader[3]);

                    await ImageHandler.ByteToBitmap(vi.image);
                    vi.thumb = ImageHandler.ConvertedByteToBitmap;

                    VehicleImages.Add(vi);
                }
            }
        }

        public static void CreatePhoto(byte[] img, string description, int owner_id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO Vehicles_Photos (image, description, owner_id) VALUES (@image, @description, @owner_id)");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@owner_id", owner_id));

            cmd.ExecuteNonQuery();
        }

        public static void UpdatePhoto(int id, byte[] img, string description, int ownerID)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Vehicles_Photos SET image = @image, description = @description, owner_id = @ownerID WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@description", description));
            cmd.Parameters.Add(new SqliteParameter("@ownerID", ownerID));

            cmd.ExecuteNonQuery();
        }

        public static void RemovePhoto(int id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM Vehicles_Photos WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));

            cmd.ExecuteNonQuery();
        }
    }
}
