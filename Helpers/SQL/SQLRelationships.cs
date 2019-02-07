using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    class SQLRelationships
    {
        public class Relations
        {
            public int Id { get; set; }

            public int IntinerarieID { get; set; }

            public int CategoryID { get; set; }

            public string CategoryType { get; set; }
        }

        public class ListItem
        {
            public int ID { get; set; }

            public string Title { get; set; }

            public bool IsChecked { get; set; }
        }

        public async static Task LoadTableAndRelationships(string tableName, string relationType, int intiID, List<ListItem> listToAdd, List<Relations> listRelations)
        {
            // Carrega as relações
            SqliteCommand cmd = new SqliteCommand("SELECT * FROM Relationships WHERE owner_id = @intiID AND category_type = '" + relationType + "'", SQLBasics.conn);
            cmd.Parameters.Add(new SqliteParameter("@intiID", intiID));

            using (SqliteDataReader datareader = await cmd.ExecuteReaderAsync())
            {
                while (await datareader.ReadAsync())
                {
                    Relations relItem = new Relations()
                    {
                        Id = Convert.ToUInt16(datareader[0]),
                        IntinerarieID = Convert.ToUInt16(datareader[1]),
                        CategoryID = Convert.ToUInt16(datareader[2]),
                        CategoryType = (string)datareader[3]
                    };

                    listRelations.Add(relItem);
                }
            }

            // Carrega a Lista em questão e checa suas relações
            SqliteCommand cmd2 = new SqliteCommand("SELECT * FROM " + tableName, SQLBasics.conn);

            using (SqliteDataReader datareader = await cmd2.ExecuteReaderAsync())
            {
                while (await datareader.ReadAsync())
                {
                    ListItem item = new ListItem()
                    {
                        ID = Convert.ToUInt16(datareader[0]),
                        Title = (string)datareader[2],
                        IsChecked = CheckStatus()
                    };

                    // Checa se o item em questão possui relação com o Roteiro
                    bool CheckStatus()
                    {
                        if (listRelations.Exists(e => e.IntinerarieID == intiID & e.CategoryID == Convert.ToUInt16(datareader[0]) & e.CategoryType == relationType))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    listToAdd.Add(item);
                }
            }
        }

        public async static Task AddRelationship(int intiID, int catID, string relType)
        {
            var cmdV = SQLBasics.conn.CreateCommand();
            cmdV.CommandText = String.Format("SELECT * FROM Relationships WHERE owner_id = @intiID AND category_id = @catID AND category_type = @relType");
            cmdV.Parameters.Add(new SqliteParameter("@intiID", intiID));
            cmdV.Parameters.Add(new SqliteParameter("@catID", catID));
            cmdV.Parameters.Add(new SqliteParameter("@relType", relType));

            using (SqliteDataReader datareader = await cmdV.ExecuteReaderAsync())
            {
                if (datareader.HasRows == false)
                {
                    var cmd = SQLBasics.conn.CreateCommand();

                    cmd.CommandText = String.Format("INSERT INTO Relationships (owner_id, category_id, category_type) VALUES (@intiID, @catID, @relType)");

                    cmd.Parameters.Add(new SqliteParameter("@intiID", intiID));
                    cmd.Parameters.Add(new SqliteParameter("@catID", catID));
                    cmd.Parameters.Add(new SqliteParameter("@relType", relType));

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static void RemoveRelationship(int intiID, int catID, string relType)
        {
            var cmd = SQLBasics.conn.CreateCommand();

            cmd.CommandText = String.Format("DELETE FROM Relationships WHERE owner_id = @intiID AND category_id = @catID AND category_type = @relType");

            cmd.Parameters.Add(new SqliteParameter("@intiID", intiID));
            cmd.Parameters.Add(new SqliteParameter("@catID", catID));
            cmd.Parameters.Add(new SqliteParameter("@relType", relType));

            cmd.ExecuteNonQuery();
        }
    }
}

