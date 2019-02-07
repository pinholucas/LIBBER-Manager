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
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;

namespace Libber_Manager.Controls
{
    public sealed partial class VehicleImages : UserControl
    {
        public static VehicleImages _VehicleImagesUControl;

        public VehicleImages()
        {
            this.InitializeComponent();
            _VehicleImagesUControl = this;
        }

        int vehicleID;

        int selectionID;
        int selectedImageIndex;
        bool isNewPhoto = false;
        bool imgChanged = false;
        byte[] originalImage;
        string originalDescription = "";

        SQLVehicleImages.vehicleImages vi;

        public static async Task LoadVehiclePhotos(int id)
        {
            await SQLVehicleImages.LoadVehicleImagesTable(id);
            _VehicleImagesUControl.lview_vehImages.ItemsSource = SQLVehicleImages.VehicleImages;

            _VehicleImagesUControl.vehicleID = id;

            DisableVehiclePComponents();
        }

        public static void EnableVehiclePComponents()
        {
            _VehicleImagesUControl.image_source.Tag = "enabled";
            _VehicleImagesUControl.btn_vehicleImageChange.Visibility = Visibility.Visible;
            _VehicleImagesUControl.btn_vehicleImageAdd.Margin = new Thickness(0, -45, 0, 0);
            _VehicleImagesUControl.txtBox_vehPhotoDesc.IsEnabled = true;

            _VehicleImagesUControl.lview_vehImages.IsEnabled = false;
        }

        public static void DisableVehiclePComponents()
        {
            _VehicleImagesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));

            _VehicleImagesUControl.isNewPhoto = false;

            _VehicleImagesUControl.imgChanged = false;
            _VehicleImagesUControl.image_source.Tag = "disabled";
            _VehicleImagesUControl.btn_vehicleImageChange.Visibility = Visibility.Collapsed;
            _VehicleImagesUControl.btn_vehicleImageAdd.Margin = new Thickness(0);
            _VehicleImagesUControl.btn_vehicleImageAdd.Content = "NOVA IMAGEM";
            _VehicleImagesUControl.txtBox_vehPhotoDesc.IsEnabled = false;
            _VehicleImagesUControl.txtBox_vehPhotoDesc.Text = "";
            _VehicleImagesUControl.btn_applyPhoto.Content = "APLICAR";
            _VehicleImagesUControl.btn_applyPhoto.IsEnabled = false;

            _VehicleImagesUControl.lview_vehImages.IsEnabled = true;
        }

        public static void ClearVehiclePComponents()
        {
            _VehicleImagesUControl.btn_vehicleImageChange.Visibility = Visibility.Visible;
            _VehicleImagesUControl.btn_vehicleImageAdd.Margin = new Thickness(0, -45, 0, 0);

            _VehicleImagesUControl.imgChanged = false;
            _VehicleImagesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            _VehicleImagesUControl.originalDescription = "";
            _VehicleImagesUControl.txtBox_vehPhotoDesc.IsEnabled = true;
            _VehicleImagesUControl.txtBox_vehPhotoDesc.Text = "";

            _VehicleImagesUControl.btn_applyPhoto.IsEnabled = true;
        }    

        private async void btn_applyPhoto_Click(object sender, RoutedEventArgs e)
        {
            btn_applyPhoto.IsEnabled = false;

            if (isNewPhoto)
            {
                SQLVehicleImages.CreatePhoto(ImageHandler.imgOutput, txtBox_vehPhotoDesc.Text, vehicleID);

                await LoadVehiclePhotos(vehicleID);

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
            else
            {
                SQLVehicleImages.UpdatePhoto(vi.id, ImageHandler.imgOutput, txtBox_vehPhotoDesc.Text, vehicleID);

                await LoadVehiclePhotos(vehicleID);
                lview_vehImages.SelectedIndex = selectedImageIndex;

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
        }

        private async void lview_vehImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_vehImages.SelectedIndex > -1)
            {
                ClearVehiclePComponents();
                selectedImageIndex = lview_vehImages.SelectedIndex;
                
                vi = lview_vehImages.SelectedItem as SQLVehicleImages.vehicleImages;

                selectionID = vi.id;

                imgChanged = false;
                originalImage = vi.image;
                ImageHandler.imgOutput = originalImage;
                await ImageHandler.ByteToBitmap(originalImage); //possível futuro bug
                originalDescription = vi.description;

                image_source.Source = ImageHandler.ConvertedByteToBitmap;
                txtBox_vehPhotoDesc.Text = vi.description;
            }
            else
            {
                DisableVehiclePComponents();
            }
        }

        private async void btn_vehicleImageAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewPhoto)
            {
                await ImageHandler.ChangeImage();
                if (ImageHandler.ConvertedByteToBitmap != null)
                {
                    isNewPhoto = true;
                    ClearVehiclePComponents();
                    EnableVehiclePComponents();
                    btn_vehicleImageAdd.Content = "CANCELAR";
                    btn_applyPhoto.Content = "ADICIONAR";

                    image_source.Source = ImageHandler.ConvertedByteToBitmap;
                }
            }
            else
            {
                isNewPhoto = false;
                DisableVehiclePComponents();
            }
        }

        private async void btn_vehicleImageChange_Click(object sender, RoutedEventArgs e)
        {
            await ImageHandler.ChangeImage();
            if (ImageHandler.ConvertedByteToBitmap != null)
            {
                image_source.Source = ImageHandler.ConvertedByteToBitmap;

                imgChanged = true;
                btn_applyPhoto.IsEnabled = true;
            }
        }

        private async void lview_vehImages_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                SQLVehicleImages.RemovePhoto(selectionID);
                DisableVehiclePComponents();
                await LoadVehiclePhotos(vehicleID);
            }
        }

        private void txtBox_vehPhotoDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (imgChanged == true | txtBox_vehPhotoDesc.Text != originalDescription)
            {
                btn_applyPhoto.IsEnabled = true;
            }
            else
            {
                btn_applyPhoto.IsEnabled = false;
            }
        }

        private void image_source_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((string)image_vehicle.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Visible;

                VisualGraphics.doBlurOnElement(image_source);
            }
        }

        private void stackp_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if ((string)image_vehicle.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Collapsed;

                VisualGraphics.removeBlurOnElement(image_source);
            }
        }        
    }
}
