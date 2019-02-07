using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Libber_Manager.Helpers;
using System.Threading.Tasks;

namespace Libber_Manager.Controls
{
    public sealed partial class CampsAndParks : UserControl
    {
        public static CampsAndParks _CampsAndParks;

        public CampsAndParks()
        {
            this.InitializeComponent();
            _CampsAndParks = this;
        }

        int intinerarieID;

        public static void LoadCampsAndParks(int id)
        {
            SQLCampsAndParks.LoadCampingsAndParks(id);
                        
            _CampsAndParks.lview_dlgCamps.ItemsSource = SQLCampsAndParks.CampingsCaP;
            _CampsAndParks.lview_dlgParks.ItemsSource = SQLCampsAndParks.ParksCaP;

            _CampsAndParks.intinerarieID = id;
        }

        private async void Park_checkbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true)
            {
                FrameworkElement p0 = sender as FrameworkElement;
                SQLRelationships.ListItem p = p0.DataContext as SQLRelationships.ListItem;
                
                await SQLRelationships.AddRelationship(intinerarieID, p.ID, "park");

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
            else
            {
                FrameworkElement p0 = sender as FrameworkElement;
                SQLRelationships.ListItem p = p0.DataContext as SQLRelationships.ListItem;

                SQLRelationships.RemoveRelationship(intinerarieID, p.ID, "park");

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
        }

        private async void Camping_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true)
            {
                FrameworkElement p0 = sender as FrameworkElement;
                SQLRelationships.ListItem p = p0.DataContext as SQLRelationships.ListItem;

                await SQLRelationships.AddRelationship(intinerarieID, p.ID, "camping");

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
            else
            {
                FrameworkElement p0 = sender as FrameworkElement;
                SQLRelationships.ListItem p = p0.DataContext as SQLRelationships.ListItem;

                SQLRelationships.RemoveRelationship(intinerarieID, p.ID, "camping");

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
        }
    }
}
