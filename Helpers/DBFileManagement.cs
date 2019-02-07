using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Libber_Manager.Helpers
{
    public class DBFileManagement
    {
        public static DBFileManagement dbFileManagement;

        static StorageFolder localStateFolder = ApplicationData.Current.LocalFolder;

        public static StorageFile dbFile = null;

        static StorageFile localStateFile;
        static StorageFile destinationFile;

        public static bool IsFilePicked = false;

        public static string tempDB = null;

        public DBFileManagement()
        {
            dbFileManagement = this;
        }

        public static async Task OpenAndManageDBFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".db");

            StorageFile pickedFile = await picker.PickSingleFileAsync();

            if (pickedFile != null)
            {
                await DialogMessage.ShowDialog(DLGWType.Wait, "AGUARDE", @"\b0 ABRINDO A DATABASE...");

                CloseDB();

                IsFilePicked = true;
                destinationFile = pickedFile;
                pickedFile = null;

                StorageApplicationPermissions.FutureAccessList.AddOrReplace("db", destinationFile);

                tempDB = localStateFolder.Path + "\\" + destinationFile.Name + ".temp";

                await Task.Run(() =>
                {
                    Task.Yield();
                    File.Copy(destinationFile.Path, tempDB, true);
                });

                localStateFile = await localStateFolder.GetFileAsync(destinationFile.Name + ".temp");
                dbFile = localStateFile;

                NavigationMenu._navigationMenuUControl.resetButtonsState();
                MainPage.hideMainControls();

                GlobalMethods.isConnectionOpen = true; // Conexão aberta!                
                GlobalMethods.dbFileName = dbFile.Name.Remove(dbFile.Name.Length - 5);

                GlobalMethods.SetDBSaved(); // Altera o status da DB para sem alterações não salvas
                NavigationMenu._navigationMenuUControl.otherMenuTools.Visibility = Visibility.Visible;
                MainPage.ClearAndDisableAllComponents();

                DialogMessage.CloseDialog();
            }
        }

        public static async Task SaveDBFile()
        {            
            if (SQLBasics.isLocalFile == true)
            {
                if (localStateFile != null && destinationFile != null)
                {
                    await DialogMessage.ShowDialog(DLGWType.Wait, "AGUARDE", @"\b0 SALVANDO A DATABASE...");

                    await localStateFile.CopyAndReplaceAsync(destinationFile);

                    GlobalMethods.SetDBSaved(); // Altera o status da DB para sem alterações não salvas
                    DialogMessage.CloseDialog();
                    
                    if (GlobalMethods.doSQLCreate)
                    {
                        GlobalMethods.doSQLCreate = false;
                        await SQLBasics.SQLCreate();
                    }
                    else if (GlobalMethods.doSQLOpen)
                    {
                        GlobalMethods.doSQLOpen = false;
                        await SQLBasics.SQLOpen();
                    }
                }
            }
            else
            {
                await SaveDBFileAs();
            }            
        }

        public static async Task SaveDBFileAs()
        {
            string _dbName = null;

            try
            {
                if (dbFile.Name.Contains(".temp"))
                {
                    _dbName = dbFile.Name.Remove(dbFile.Name.Length - 5);
                }

                FileSavePicker savePicker = new FileSavePicker();
                savePicker.FileTypeChoices.Add("Padrão de database", new List<string>() { ".db" });
                savePicker.SuggestedFileName = _dbName;

                localStateFile = await localStateFolder.GetFileAsync(dbFile.Name);
                destinationFile = await savePicker.PickSaveFileAsync();

                if (localStateFile != null && destinationFile != null)
                {
                    await DialogMessage.ShowDialog(DLGWType.Wait, "AGUARDE", @"\b0 SALVANDO A DATABASE...");

                    await localStateFile.CopyAndReplaceAsync(destinationFile); // copia o .temp pra pasta de destino
                    //ApplicationView.GetForCurrentView().Title = destinationFile.Name; // atualiza o nome do arquivo na janela                    

                    SQLBasics.conn.Close(); // fecha a conexão com o .temp com nome antigo
                    SQLBasics.conn = null; // anula a conexão

                    if (File.Exists(localStateFolder.Path + "\\" + destinationFile.Name + ".temp"))
                    {
                        File.Delete(localStateFolder.Path + "\\" + destinationFile.Name + ".temp"); // deleta o .temp antigo

                        await destinationFile.CopyAsync(localStateFolder); // copia o .db salvo para a Local State

                        File.Move(localStateFolder.Path + "\\" + destinationFile.Name, localStateFolder.Path + "\\" + destinationFile.Name + ".temp"); // renomeia de .db para .db.temp
                    }
                    else
                    {
                        await localStateFile.RenameAsync(destinationFile.Name + ".temp"); // renomeia o .temp antigo para o novo nome + .temp
                    }

                    dbFile = localStateFile;                    
                    SQLBasics.conn = new SqliteConnection("DataSource = " + localStateFolder.Path + "\\" + destinationFile.Name + ".temp");
                    await SQLBasics.conn.OpenAsync();

                    SQLBasics.SQLRefresh();
                    SQLBasics.isLocalFile = true; // define que o arquivo definitivo já existe

                    GlobalMethods.dbFileName = dbFile.Name.Remove(dbFile.Name.Length - 5);
                    GlobalMethods.SetDBSaved(); // Altera o status da DB para sem alterações não salvas
                    DialogMessage.CloseDialog();
                    
                    if (GlobalMethods.doSQLCreate)
                    {
                        GlobalMethods.doSQLCreate = false;
                        await SQLBasics.SQLCreate();
                    }
                    else if (GlobalMethods.doSQLOpen)
                    {
                        GlobalMethods.doSQLOpen = false;
                        await SQLBasics.SQLOpen();
                    }
                }
                else
                {
                    GlobalMethods.doSQLCreate = false;
                    GlobalMethods.doSQLOpen = false;
                }
            }
            catch
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 NÃO FOI POSSÍVEL SALVAR, CONTATE O DESENVOLVEDOR IMEDIATAMENTE E NÃO FECHE O PROGRAMA! \par Erro 00#035");
            }
        }

        public async static void CloseDB()
        {
            DirectoryInfo localFolder = new DirectoryInfo(localStateFolder.Path);

            if (SQLBasics.conn != null) // se houver alguma conexão anterior, ela é desfeita e anulada
            {
                SQLBasics.conn.Dispose(); // fecha a conexão com o .temp com nome antigo
                SQLBasics.conn = null; // anula a conexão                      
            }

            foreach (FileInfo file in localFolder.GetFiles()) // remove todos os arquivos temporarios da LocalState
            {
                try
                {
                    GC.Collect(); // Começa a coleta de lixos (antiga DB fechada logo acima)
                    GC.WaitForPendingFinalizers(); // Aguarda pelo fim da coleta de lixos

                    file.Delete();
                }
                catch (IOException io)
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ERRO", @"\b0 " + io.Message);
                }
            }
        }

    }
}
