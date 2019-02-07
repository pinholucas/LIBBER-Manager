using Libber_Manager.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Libber_Manager.Controls
{
    public sealed partial class Vehicles : UserControl
    {
        public static Vehicles _vehiclesUControl;

        public Vehicles()
        {
            this.InitializeComponent();
            _vehiclesUControl = this;
        }        

        public static bool isNewVehicle = false;
        public static bool isEditing = false;

        public int lvSelectedIndex = -1;
        public int lvNewSelectedIndex = -1;

        int selectionID;
        bool imgChanged = false;
        public static byte[] intiImage_byte;        
        string originalName;
        string originalCardDescription;
        decimal originalValue;
        string originalDescription;
        string originalAttributes;

        public static void LoadVehicles()
        {
            SQLVehicles.LoadVehiclesTable();
            _vehiclesUControl.lview_vehicles.ItemsSource = SQLVehicles.Vehicles;
        }

        private void EnableVehicleComponents()
        {
            image_intinerarie.Tag = "enabled";
            txtBox_vehName.IsEnabled = true;
            txtBox_vehCardDescription.IsEnabled = true;
            txtBox_vehValue.IsEnabled = true;
            txtBox_vehDescription.IsEnabled = true;
            txtBox_vehAttributes.IsEnabled = true;

            btn_vehAdditionals.IsEnabled = true;
            btn_vehImages.IsEnabled = true;
        }

        public static void DisableVehicleComponents()
        {
            EditionFinished();
            _vehiclesUControl.imgChanged = false;
            _vehiclesUControl.originalName = "";
            _vehiclesUControl.originalCardDescription = "";
            _vehiclesUControl.originalValue = 0;
            _vehiclesUControl.originalDescription = "";
            _vehiclesUControl.originalAttributes = "";

            _vehiclesUControl.stackp.Visibility = Visibility.Collapsed;

            _vehiclesUControl.selectionID = -1;

            _vehiclesUControl.lview_vehicles.SelectedIndex = -1;
            _vehiclesUControl.lview_vehicles.SelectedItem = null;
            _vehiclesUControl.lview_vehicles.IsEnabled = true;

            _vehiclesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            _vehiclesUControl.image_intinerarie.Tag = "disabled";
            _vehiclesUControl.txtBox_vehName.Text = "";
            _vehiclesUControl.txtBox_vehName.IsEnabled = false;
            _vehiclesUControl.txtBox_vehCardDescription.Text = "";
            _vehiclesUControl.txtBox_vehCardDescription.IsEnabled = false;
            _vehiclesUControl.txtBox_vehValue.Text = "";
            _vehiclesUControl.txtBox_vehValue.IsEnabled = false;
            _vehiclesUControl.txtBox_vehDescription.Text = "";
            _vehiclesUControl.txtBox_vehDescription.IsEnabled = false;
            _vehiclesUControl.txtBox_vehAttributes.Text = "";
            _vehiclesUControl.txtBox_vehAttributes.IsEnabled = false;
            _vehiclesUControl.btn_vehAdditionals.IsEnabled = false;
            _vehiclesUControl.btn_vehImages.IsEnabled = false;

            _vehiclesUControl.btn_vehicleApply.IsEnabled = false;
            _vehiclesUControl.btn_vehicleApply.Content = "APLICAR";

            _vehiclesUControl.btn_vehicleCreate.Content = "NOVO VEÍCULO";
            _vehiclesUControl.btn_vehicleCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void ClearVehicleComponents()
        {
            isNewVehicle = true;

            stackp.Visibility = Visibility.Collapsed;

            lvSelectedIndex = -1;
            lvNewSelectedIndex = -1;
            selectionID = -1;

            lview_vehicles.SelectedItem = null;
            lview_vehicles.IsEnabled = false;

            btn_vehicleImageChange.Content = "ADICIONAR IMAGEM";

            image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            image_intinerarie.Tag = "enabled";
            txtBox_vehName.Text = "";
            txtBox_vehName.IsEnabled = true;
            txtBox_vehCardDescription.Text = "";
            txtBox_vehCardDescription.IsEnabled = true;
            txtBox_vehValue.Text = "";
            txtBox_vehValue.IsEnabled = true;
            txtBox_vehDescription.Text = "";
            txtBox_vehDescription.IsEnabled = true;
            txtBox_vehAttributes.Text = "";
            txtBox_vehAttributes.IsEnabled = true;

            btn_vehicleApply.IsEnabled = false;
            btn_vehicleApply.Content = "CRIAR";

            btn_vehicleCreate.Content = "CANCELAR";
            btn_vehicleCreate.Background = new SolidColorBrush(Colors.DarkRed);
        }

        private void Edited(bool isEdited)
        {
            if (isEdited)
            {
                isEditing = true;
                btn_vehicleApply.IsEnabled = true;

                btn_vehicleCreate.Content = "CANCELAR";
                btn_vehicleCreate.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                isEditing = false;
                btn_vehicleApply.IsEnabled = false;

                if (!isNewVehicle)
                {
                    btn_vehicleCreate.Content = "NOVO VEÍCULO";
                    btn_vehicleCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                }
            }
        }

        private static void EditionFinished()
        {
            isNewVehicle = false;
            isEditing = false;

            _vehiclesUControl.imgChanged = false;
            _vehiclesUControl.btn_vehicleImageChange.Content = "ALTERAR IMAGEM";

            _vehiclesUControl.btn_vehicleApply.IsEnabled = false;
            _vehiclesUControl.btn_vehicleCreate.IsEnabled = true;

            _vehiclesUControl.btn_vehicleCreate.Content = "NOVO VEÍCULO";
            _vehiclesUControl.btn_vehicleCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void CheckFields()
        {
            string value = txtBox_vehValue.Text == "" ? "0" : txtBox_vehValue.Text;

            if (imgChanged == true | value != originalValue.ToString() |
                txtBox_vehName.Text != originalName | txtBox_vehCardDescription.Text != originalCardDescription |
                txtBox_vehDescription.Text != originalDescription | txtBox_vehAttributes.Text != originalAttributes)
            {
                Edited(true);
            }
            else
            {
                Edited(false);
            }
        }

        public async static Task QuestionAlterations()
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 SUAS ALTERAÇÕES \b NÃO \b0 FORAM SALVAS, DESEJA CONTINUAR?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                DisableVehicleComponents();

                if (_vehiclesUControl.lvNewSelectedIndex != _vehiclesUControl.lvSelectedIndex)
                {
                    if (_vehiclesUControl.lvNewSelectedIndex <= _vehiclesUControl.lview_vehicles.Items.Count)
                    {
                        _vehiclesUControl.lview_vehicles.SelectedIndex = _vehiclesUControl.lvNewSelectedIndex;
                        _vehiclesUControl.lview_vehicles.SelectedItem = _vehiclesUControl.lvNewSelectedIndex;
                    }
                }
                else //criação de ad
                {
                    _vehiclesUControl.lview_vehicles.SelectedIndex = _vehiclesUControl.lvSelectedIndex;
                    _vehiclesUControl.lview_vehicles.SelectedItem = _vehiclesUControl.lvSelectedIndex;
                }
            }
        }

        public async static Task QuestionDelete(string itemName)
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 DELETAR O ITEM \b'" + itemName + @"'\b0  PERMANENTEMENTE?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                SQLVehicles.RemoveVehicle(_vehiclesUControl.selectionID);
                DisableVehicleComponents();
                LoadVehicles();
            }
        }

        public static async Task CancelVehicleEdition()
        {
            if (isEditing == false && isNewVehicle == true)
            {
                DisableVehicleComponents();
            }
            else
            {
                await QuestionAlterations();
            }
        }

        private async void btn_vehicleCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewVehicle && !isEditing)
            {
                ClearVehicleComponents();
                imgChanged = false;
                originalName = "";
                originalCardDescription = "";
                originalValue = 0;                
                originalDescription = "";
                originalAttributes = "";

                btn_vehAdditionals.IsEnabled = false;
                btn_vehImages.IsEnabled = false;

                GlobalMethods.SetFocus(txtBox_vehName);
            }
            else
            {
                await CancelVehicleEdition();
            }
        }

        private async void btn_vehicleApply_Click(object sender, RoutedEventArgs e)
        {
            if (isNewVehicle == false) //Edição de anúncio
            {
                if (txtBox_vehName.Text != "" & txtBox_vehCardDescription.Text != "" &
                    txtBox_vehValue.Text != "" & txtBox_vehDescription.Text != "" &
                    txtBox_vehAttributes.Text != "")
                {
                    if (GlobalMethods.IsTextNumeric(txtBox_vehValue.Text))
                    {
                        SQLVehicles.UpdateVehicle(selectionID, ImageHandler.imgOutput, txtBox_vehName.Text, txtBox_vehCardDescription.Text, Convert.ToDecimal(txtBox_vehValue.Text), txtBox_vehDescription.Text, txtBox_vehAttributes.Text);

                        EditionFinished();
                        LoadVehicles();
                        lview_vehicles.SelectedIndex = lvSelectedIndex;

                        GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                    }
                    else
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 NÃO UTILIZE LETRAS NO CAMPO DA DIÁRIA!");
                    }
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
            else //Criação de anúncio
            {
                if (imgChanged & txtBox_vehName.Text != "" & txtBox_vehCardDescription.Text != "" &
                    txtBox_vehValue.Text != "" & txtBox_vehDescription.Text != "" &
                    txtBox_vehAttributes.Text != "")
                {
                    if (GlobalMethods.IsTextNumeric(txtBox_vehValue.Text))
                    {
                        SQLVehicles.CreateVehicle(ImageHandler.imgOutput, txtBox_vehName.Text, txtBox_vehCardDescription.Text, Convert.ToDecimal(txtBox_vehValue.Text), txtBox_vehDescription.Text, txtBox_vehAttributes.Text);
                        btn_vehicleApply.Content = "APLICAR";

                        EditionFinished();
                        LoadVehicles();
                        DisableVehicleComponents();

                        GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                    }
                    else
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 NÃO UTILIZE LETRAS NO CAMPO DA DIÁRIA!");
                    }
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
        }

        private async void lview_vehicles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_vehicles.SelectedIndex > -1 && lview_vehicles.SelectedItem != null)
            {
                if (isEditing)
                {
                    if (lvSelectedIndex != lview_vehicles.SelectedIndex)
                    {
                        lvNewSelectedIndex = lview_vehicles.SelectedIndex;
                    }

                    try
                    {
                        lview_vehicles.SelectedIndex = lvSelectedIndex;
                        lview_vehicles.SelectedItem = lvSelectedIndex;
                    }
                    catch
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 ALGO DEU ERRADO, REINICIE O PROGRAMA E REPORTE O PROBLEMA AO DESENVOLVEDOR. \par Erro 63#002");
                    }

                    await QuestionAlterations();
                }
                else
                {
                    EnableVehicleComponents();

                    lvSelectedIndex = lview_vehicles.SelectedIndex;
                    lvNewSelectedIndex = lview_vehicles.SelectedIndex;

                    SQLVehicles.vehicles veh = lview_vehicles.SelectedItem as SQLVehicles.vehicles;

                    selectionID = veh.id;
                    intiImage_byte = veh.image;
                    //await AdHandler.ByteToBitmap(adImage_byte);
                    ImageHandler.imgOutput = intiImage_byte;
                    await ImageHandler.ByteToBitmap(intiImage_byte); //possível futuro bug
                    originalName = veh.name;
                    originalCardDescription = veh.card_description;
                    originalValue = veh.value;
                    originalDescription = veh.description;
                    originalAttributes = veh.attributes;

                    image_source.Source = ImageHandler.ConvertedByteToBitmap;
                    txtBox_vehName.Text = originalName;
                    txtBox_vehCardDescription.Text = originalCardDescription;
                    txtBox_vehValue.Text = originalValue.ToString();
                    txtBox_vehDescription.Text = originalDescription;
                    txtBox_vehAttributes.Text = originalAttributes;
                }
            }
        }

        private async void lview_vehicles_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                SQLVehicles.vehicles vehicle = lview_vehicles.SelectedItem as SQLVehicles.vehicles;

                await QuestionDelete(vehicle.name.ToUpper());
            }
        }

        private async void btn_vehAdditionals_Click(object sender, RoutedEventArgs e)
        {            
            await DialogMessage.ShowDialog(DLGWType.View, "ADICIONAIS", "");
            DialogMessage._dialogMessageUControl.VehicleAdditionalsControl.Visibility = Visibility.Visible;

            VehicleAdditionals.LoadAdditionals(selectionID);
        }

        private async void btn_vehImages_Click(object sender, RoutedEventArgs e)
        {
            await DialogMessage.ShowDialog(DLGWType.View, "IMAGENS DO VEÍCULO", "");
            DialogMessage._dialogMessageUControl.VehicleImagesControl.Visibility = Visibility.Visible;

            await VehicleImages.LoadVehiclePhotos(selectionID);
        }

        private async void btn_vehicleImageChange_Click(object sender, RoutedEventArgs e)
        {
            await ImageHandler.ChangeImage();
            if (ImageHandler.ConvertedByteToBitmap != null)
            {
                image_source.Source = ImageHandler.ConvertedByteToBitmap;

                Edited(true);
                imgChanged = true;
            }
        }

        private void stackp_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if ((string)image_intinerarie.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Collapsed;

                VisualGraphics.removeBlurOnElement(image_source);
            }
        }

        private void image_source_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((string)image_intinerarie.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Visible;

                VisualGraphics.doBlurOnElement(image_source);
            }
        }

        private void txtBox_vehName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_vehCardDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_vehValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_vehDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_vehAttributes_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }        
    }
}
