using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Libber_Manager.Helpers
{
    class SQLIntinerarieImages
    {
        public class intinerarieImages
        {
            public int id { get; set; }

            public byte[] image { get; set; }

            public string place { get; set; }

            public int owner_id { get; set; }

            public BitmapImage thumb { get; set; }
        }

        public static List<intinerarieImages> IntinerarieImages;

        #region INTINERARIE MAP
        public static byte[] mapimg;

        public async static void LoadIntinerarieMap(int id)
        {
            SqliteCommand cmd = new SqliteCommand("SELECT * From Intineraries WHERE id = @id", SQLBasics.conn);
            cmd.Parameters.Add(new SqliteParameter("@id", id));

            try
            {
                using (SqliteDataReader datareader = cmd.ExecuteReader())
                {
                    if (datareader.HasRows)
                    {
                        while (datareader.Read())
                        {
                            if (datareader[6] != DBNull.Value)
                            {
                                mapimg = (byte[])datareader[6];
                            }
                            else
                            {
                                mapimg = null;
                            }
                        }
                    }
                    else
                    {
                        mapimg = null;
                    }
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL CARREGAR O MAPA DO INTINERÁRIO. \par Erro 03#033");
            }
        }

        public static void UpdateMap(int id, byte[] map)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Intineraries SET map = @map WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@map", DbType.Binary);
            parameter.Value = map;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);

            cmd.ExecuteNonQuery();
        }
        #endregion

        #region INTINERARIE IMAGES
        public async static Task LoadIntinerarieImagesTable(int ownerId)
        {
            IntinerarieImages = new List<intinerarieImages>();

            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Intineraries_Photos WHERE owner_id = " + ownerId, SQLBasics.conn);
         
            using (SqliteDataReader datareader = await cmd.ExecuteReaderAsync())
            {
                while (await datareader.ReadAsync())
                {
                    intinerarieImages ii = new intinerarieImages();

                    ii.id = Convert.ToUInt16(datareader[0]);
                    ii.image = (byte[])datareader[1];
                    ii.place = (string)datareader[2];
                    ii.owner_id = Convert.ToUInt16(datareader[3]);

                    await ImageHandler.ByteToBitmap(ii.image);
                    ii.thumb = ImageHandler.ConvertedByteToBitmap;

                    IntinerarieImages.Add(ii);                                        
                }                                
            }
        }

        public static void CreatePhoto(byte[] img, string place, int owner_id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("INSERT INTO Intineraries_Photos (image, place, owner_id) VALUES (@image, @place, @owner_id)");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@place", place));
            cmd.Parameters.Add(new SqliteParameter("@owner_id", owner_id));

            cmd.ExecuteNonQuery();
        }

        public static void UpdatePhoto(int id, byte[] img, string place, int ownerID)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("UPDATE Intineraries_Photos SET image = @image, place = @place, owner_id = @ownerID WHERE id = @id");

            SqliteParameter parameter = new SqliteParameter("@image", DbType.Binary);
            parameter.Value = img;

            cmd.Parameters.Add(new SqliteParameter("@id", id));
            cmd.Parameters.Add(parameter);
            cmd.Parameters.Add(new SqliteParameter("@place", place));
            cmd.Parameters.Add(new SqliteParameter("@ownerID", ownerID));

            cmd.ExecuteNonQuery();
        }

        public static void RemovePhoto(int id)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM Intineraries_Photos WHERE id = @id");

            cmd.Parameters.Add(new SqliteParameter("@id", id));

            cmd.ExecuteNonQuery();
        }
        #endregion
    }
}
