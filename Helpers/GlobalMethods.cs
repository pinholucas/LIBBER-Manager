using Libber_Manager.Controls;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Libber_Manager.Helpers
{
    class GlobalMethods
    {
        // Cores Globais
        public static SolidColorBrush btnLightBlue = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        public static SolidColorBrush btnDarkRed = new SolidColorBrush(Colors.DarkRed);

        // Variáveis Globais        
        public static bool isConnectionOpen = false;
        public static bool isSomeComponentActive = false;

        public static bool doSQLCreate = false;
        public static bool doSQLOpen = false;
        
        public static bool isDBEditted = false;
        public static string dbFileName = "N/D";

        // Constantes Globais
        public const int Campings = 0;
        public const int Parks = 1;
        public const int Additionals = 2;

        // Funções Gerais
        public static void SetDBEdited()
        {
            isDBEditted = true;
            ApplicationView.GetForCurrentView().Title = "*" + dbFileName;
        }

        public static void SetDBSaved()
        {
            isDBEditted = false;
            ApplicationView.GetForCurrentView().Title = dbFileName;
        }

        public static bool IsLocalsCategory()
        {
            return Categories._placesUControl.cBox_categories.SelectedIndex <= Parks ? true : false;       
        }

        public static bool IsAdditionalsCategory()
        {
            return Categories._placesUControl.cBox_categories.SelectedIndex == Additionals ? true : false;
        }

        public async static void SetFocus(Control txtbox)
        {
            await Task.Delay(50);
            txtbox.Focus(FocusState.Programmatic);
        }

        public static bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9.,-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
