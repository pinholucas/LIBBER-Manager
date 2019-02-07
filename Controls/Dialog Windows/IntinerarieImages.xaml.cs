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
    public sealed partial class IntinerarieImages : UserControl
    {
        public static IntinerarieImages _IntinerarieImagesUControl;

        public IntinerarieImages()
        {
            this.InitializeComponent();
            _IntinerarieImagesUControl = this;            
        }        

        int intinerarieID;

        #region INTINERARIE MAP

        public async static Task LoadIntiMap(int id)
        {
            SQLIntinerarieImages.LoadIntinerarieMap(id);
            
            if (SQLIntinerarieImages.mapimg != null)
            {
                ImageHandler.imgOutput = SQLIntinerarieImages.mapimg;
                await ImageHandler.ByteToBitmap(SQLIntinerarieImages.mapimg);
                _IntinerarieImagesUControl.mapImage_source.Source = ImageHandler.ConvertedByteToBitmap;
                _IntinerarieImagesUControl.btn_intinerarieMapImage.Content = "ALTERAR IMAGEM DO MAPA";
            }
            else
            {
                _IntinerarieImagesUControl.mapImage_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
                _IntinerarieImagesUControl.btn_intinerarieMapImage.Content = "ADICIONAR IMAGEM DO MAPA";
            }

            _IntinerarieImagesUControl.intinerarieID = id;            
        }        

        private async void btn_intinerarieMapImageChange_Click(object sender, RoutedEventArgs e)
        {
            await ImageHandler.ChangeImage();
            if (ImageHandler.ConvertedByteToBitmap != null)
            {
                mapImage_source.Source = ImageHandler.ConvertedByteToBitmap;

                btn_intinerarieMapImage.Content = "ALTERAR IMAGEM DO MAPA";
                btn_applyMap.IsEnabled = true;
            }
        }

        private async void btn_applyMap_Click(object sender, RoutedEventArgs e)
        {
            SQLIntinerarieImages.UpdateMap(intinerarieID, ImageHandler.imgOutput);

            await LoadIntiMap(intinerarieID);

            SQLIntinerarieImages.mapimg = null;
            btn_applyMap.IsEnabled = false;

            GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
        }

        private void mapImage_source_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((string)mapImage_intinerarie.Tag == "enabled")
            {
                mapStackp.Visibility = Visibility.Visible;

                VisualGraphics.doBlurOnElement(mapImage_source);
            }
        }

        private void mapStackp_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if ((string)mapImage_intinerarie.Tag == "enabled")
            {
                mapStackp.Visibility = Visibility.Collapsed;

                VisualGraphics.removeBlurOnElement(mapImage_source);
            }
        }

        #endregion

        #region INTINERARIES IMAGES
        int selectionID;
        int selectedImageIndex;
        bool isNewPhoto = false;
        bool imgChanged = false;
        byte[] originalImage;
        string originalPlace = "";

        SQLIntinerarieImages.intinerarieImages ii;

        public static async Task LoadIntiPhotos(int id)
        {
            await SQLIntinerarieImages.LoadIntinerarieImagesTable(id);
            _IntinerarieImagesUControl.lview_intiImages.ItemsSource = SQLIntinerarieImages.IntinerarieImages;

            _IntinerarieImagesUControl.intinerarieID = id;
            
            DisableIntiPComponents();
        }        

        public static void EnableIntiPComponents()
        {
            _IntinerarieImagesUControl.image_source.Tag = "enabled";
            _IntinerarieImagesUControl.btn_intinerarieImageChange.Visibility = Visibility.Visible;
            _IntinerarieImagesUControl.btn_intinerarieImageAdd.Margin = new Thickness(0, -45, 0, 0);
            _IntinerarieImagesUControl.txtBox_intiPhotoPlace.IsEnabled = true;

            _IntinerarieImagesUControl.lview_intiImages.IsEnabled = false;
        }

        public static void DisableIntiPComponents()
        {
            _IntinerarieImagesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));

            _IntinerarieImagesUControl.isNewPhoto = false;

            _IntinerarieImagesUControl.imgChanged = false;
            _IntinerarieImagesUControl.image_source.Tag = "disabled";
            _IntinerarieImagesUControl.btn_intinerarieImageChange.Visibility = Visibility.Collapsed;
            _IntinerarieImagesUControl.btn_intinerarieImageAdd.Margin = new Thickness(0);
            _IntinerarieImagesUControl.btn_intinerarieImageAdd.Content = "NOVA IMAGEM";
            _IntinerarieImagesUControl.txtBox_intiPhotoPlace.IsEnabled = false;
            _IntinerarieImagesUControl.txtBox_intiPhotoPlace.Text = "";
            _IntinerarieImagesUControl.btn_applyPhoto.Content = "APLICAR";
            _IntinerarieImagesUControl.btn_applyPhoto.IsEnabled = false;

            _IntinerarieImagesUControl.lview_intiImages.IsEnabled = true;
        }
        
        public static void ClearIntiPComponents()
        {
            _IntinerarieImagesUControl.btn_intinerarieImageChange.Visibility = Visibility.Visible;
            _IntinerarieImagesUControl.btn_intinerarieImageAdd.Margin = new Thickness(0, -45, 0, 0);

            _IntinerarieImagesUControl.imgChanged = false;
            _IntinerarieImagesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            _IntinerarieImagesUControl.originalPlace = "";
            _IntinerarieImagesUControl.txtBox_intiPhotoPlace.IsEnabled = true;
            _IntinerarieImagesUControl.txtBox_intiPhotoPlace.Text = "";           

            _IntinerarieImagesUControl.btn_applyPhoto.IsEnabled = true;
        }

        private async void btn_applyPhoto_Click(object sender, RoutedEventArgs e)
        {
            btn_applyPhoto.IsEnabled = false;

            if (isNewPhoto)
            {
                SQLIntinerarieImages.CreatePhoto(ImageHandler.imgOutput, txtBox_intiPhotoPlace.Text, intinerarieID);

                await LoadIntiPhotos(intinerarieID);

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
            else
            {
                SQLIntinerarieImages.UpdatePhoto(ii.id, ImageHandler.imgOutput, txtBox_intiPhotoPlace.Text, intinerarieID);

                await LoadIntiPhotos(intinerarieID);
                lview_intiImages.SelectedIndex = selectedImageIndex;

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
        }

        private async void lview_intiImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_intiImages.SelectedIndex > -1)
            {
                ClearIntiPComponents();
                selectedImageIndex = lview_intiImages.SelectedIndex;

                ii = lview_intiImages.SelectedItem as SQLIntinerarieImages.intinerarieImages;

                selectionID = ii.id;

                imgChanged = false;
                originalImage = ii.image;
                ImageHandler.imgOutput = originalImage;
                await ImageHandler.ByteToBitmap(originalImage); //possível futuro bug
                originalPlace = ii.place;

                image_source.Source = ImageHandler.ConvertedByteToBitmap;
                txtBox_intiPhotoPlace.Text = ii.place;
            }
            else
            {
                DisableIntiPComponents();
            }
        }

        private async void btn_intinerarieImageAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewPhoto)
            {
                await ImageHandler.ChangeImage();
                if (ImageHandler.ConvertedByteToBitmap != null)
                {
                    isNewPhoto = true;
                    ClearIntiPComponents();
                    EnableIntiPComponents();
                    btn_intinerarieImageAdd.Content = "CANCELAR";
                    btn_applyPhoto.Content = "ADICIONAR";

                    image_source.Source = ImageHandler.ConvertedByteToBitmap;
                }
            }
            else
            {
                isNewPhoto = false;
                DisableIntiPComponents();
            }
        }

        private async void btn_adsimageChange_Click(object sender, RoutedEventArgs e)
        {
            await ImageHandler.ChangeImage();
            if (ImageHandler.ConvertedByteToBitmap != null)
            {
                image_source.Source = ImageHandler.ConvertedByteToBitmap;

                imgChanged = true;
                btn_applyPhoto.IsEnabled = true;
            }
        }

        private async void lview_intiImages_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                SQLIntinerarieImages.RemovePhoto(selectionID);
                DisableIntiPComponents();
                await LoadIntiPhotos(intinerarieID);
            }
        }

        private void txtBox_intiPhotoPlace_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (imgChanged == true | txtBox_intiPhotoPlace.Text != originalPlace)
            {
                btn_applyPhoto.IsEnabled = true;
            }
            else
            {
                btn_applyPhoto.IsEnabled = false;
            }
        }

        private void image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((string)image_intinerarie.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Visible;

                VisualGraphics.doBlurOnElement(image_source);
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

        #endregion
    }
}
