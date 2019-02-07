using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Libber_Manager.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.Storage;
using System.Threading.Tasks;

namespace Libber_Manager.Helpers
{
    public class SQLBasics
    {
        public static SQLBasics sqlBasics;

        public SQLBasics()
        {
            sqlBasics = this;
        }

        public static SqliteConnection conn = null;

        static string dbName = "NewLibberDatabase.db";
        static string dbFilePath = ApplicationData.Current.LocalFolder.Path + "\\" + dbName + ".temp";

        //public static bool isOpen = false; // define se um arquivo foi aberto
        public static bool isLocalFile = false; // define se um arquivo definitivo existe

        public static async Task SQLCreate()
        {
            DBFileManagement.CloseDB();

            ApplicationView.GetForCurrentView().Title = "Aguarde...";
            await DialogMessage.ShowDialog(DLGWType.Wait, "AGUARDE", @"\b0 CRIANDO A DATABASE...");

            conn = new SqliteConnection("Filename=" + dbName + ".temp");
            await conn.OpenAsync();

            DBFileManagement.dbFile = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName + ".temp");

            await Task.Run(() =>
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = String.Format(
                    @"
                          CREATE TABLE 'Ads' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'image' BLOB, 'title' TEXT, 'content' TEXT);
                          CREATE TABLE 'Campings' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'icon' INTEGER, 'title' TEXT, 'description' TEXT, 'inmap_description' TEXT, 'lat' INTEGER, 'lng' INTEGER);
                          CREATE TABLE 'Parks' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'icon' INTEGER, 'title' TEXT, 'description' TEXT, 'inmap_description' TEXT, 'lat' INTEGER, 'lng' INTEGER);
                          CREATE TABLE 'Relationships' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'owner_id' INTEGER NOT NULL, 'category_id' INTEGER NOT NULL, 'category_type' TEXT NOT NULL);
                          CREATE TABLE 'Intineraries' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'image', 'rating' INTEGER, 'title' TEXT, 'description' TEXT, 'details_content' TEXT, 'map' BLOB);
                          CREATE TABLE 'Intineraries_Photos' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'image' BLOB, 'place' TEXT, 'owner_id' INTEGER);
                          CREATE TABLE 'Vehicles' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'image' BLOB, 'name' TEXT, 'card_description' TEXT, 'value' INTEGER, 'description' TEXT, 'attributes' TEXT);
                          CREATE TABLE 'Vehicles_Photos' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'image' BLOB, 'description' TEXT, 'owner_id' INTEGER);
                          CREATE TABLE 'Additionals' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'icon' BLOB, 'title' TEXT, 'price' TEXT, 'details' TEXT);
                          CREATE TABLE 'FAQs' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'title' TEXT, 'content' TEXT);
                          CREATE TABLE 'Emergency' ('id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'libber_phoneNumber' TEXT, 'libber_smsNumber' TEXT, 'libber_email' TEXT);
                        ");

                cmd.ExecuteNonQueryAsync();
            });

            DBFileManagement.IsFilePicked = false;

            isLocalFile = false; // Define que o arquivo definitivo NÃO existe
            GlobalMethods.isConnectionOpen = true; // Conexão aberta!
            GlobalMethods.dbFileName = dbName; // Adquire o nome do arquivo da database

            GlobalMethods.SetDBSaved(); // Altera o status da DB para sem alterações não salvas
            NavigationMenu._navigationMenuUControl.otherMenuTools.Visibility = Visibility.Visible;
            NavigationMenu._navigationMenuUControl.resetButtonsState();
            MainPage.hideMainControls();
            MainPage.ClearAndDisableAllComponents();

            SQLRefresh();
            DialogMessage.CloseDialog();
        }

        public static async Task SQLOpen()
        {
            await DBFileManagement.OpenAndManageDBFile();

            if (DBFileManagement.IsFilePicked)
            {
                conn = new SqliteConnection("DataSource = " + DBFileManagement.tempDB);

                await conn.OpenAsync();

                SQLRefresh();

                //isOpen = true; // define que um arquivo foi aberto
                isLocalFile = true; // define que o arquivo definitivo já existe

                DBFileManagement.IsFilePicked = false;
            }
        }

        public static void SQLRefresh()
        {
            Ads.LoadAds();
            Categories.LoadCategory();
            Intineraries.LoadIntineraries();
            Vehicles.LoadVehicles();
            FAQs.LoadFAQs();
            Emergency.LoadEmergency();
        }

        public static List<T> DataReaderMapToList<T>(SqliteDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, Convert.ChangeType(dr[prop.Name], prop.PropertyType), null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

    }
}
