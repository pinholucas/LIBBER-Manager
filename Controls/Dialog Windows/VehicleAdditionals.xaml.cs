using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace Libber_Manager.Controls
{
    public sealed partial class VehicleAdditionals : UserControl
    {
        public static VehicleAdditionals _VehicleAdditionals;

        public VehicleAdditionals()
        {            
            this.InitializeComponent();
            _VehicleAdditionals = this;
        }

        int vehicleID;

        public static void LoadAdditionals(int id)
        {
            SQLVehiclesAdditionals.LoadAdditionalsTable(id);

            _VehicleAdditionals.lview_dlgVehiclesList.ItemsSource = SQLVehiclesAdditionals.Additionals;

            _VehicleAdditionals.vehicleID = id;
        }

        private async void Additional_checkbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.IsChecked == true)
            {
                FrameworkElement p0 = sender as FrameworkElement;
                SQLRelationships.ListItem p = p0.DataContext as SQLRelationships.ListItem;

                await SQLRelationships.AddRelationship(vehicleID, p.ID, "additional");

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
            else
            {
                FrameworkElement p0 = sender as FrameworkElement;
                SQLRelationships.ListItem p = p0.DataContext as SQLRelationships.ListItem;

                SQLRelationships.RemoveRelationship(vehicleID, p.ID, "additional");

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
        }
    }
}
