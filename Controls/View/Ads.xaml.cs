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
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.UI;
using Windows.System;

namespace Libber_Manager.Controls
{
    public sealed partial class Ads : UserControl
    {
        public static Ads _adsUControl;        

        public Ads()
        {
            this.InitializeComponent();
            _adsUControl = this;
        }

        public static bool isNewAd = false;
        public static bool isEditing = false;

        int lvSelectedIndex = -1;
        int lvNewSelectedIndex = -1;

        int selectionID;
        bool imgChanged = false;
        public static byte[] adImage_byte;
        string originalTitle;
        string originalContent;

        public static void LoadAds()
        {
            SQLAds.LoadAdsTable();
            _adsUControl.lview_ads.ItemsSource = SQLAds.Ads;
        }

        private void EnableAdComponents()
        {
            image_ad.Tag = "enabled";
            txtBox_adTitle.IsEnabled = true;
            txtBox_adContent.IsEnabled = true;
        }

        public static void DisableAdComponents()
        {
            EditionFinished();
            _adsUControl.originalTitle = "";
            _adsUControl.originalContent = "";

            _adsUControl.stackp.Visibility = Visibility.Collapsed;
           
            _adsUControl.selectionID = -1;

            _adsUControl.lview_ads.SelectedIndex = -1;
            _adsUControl.lview_ads.SelectedItem = null;
            _adsUControl.lview_ads.IsEnabled = true;

            _adsUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            _adsUControl.image_ad.Tag = "disabled";
            _adsUControl.txtBox_adTitle.Text = "";
            _adsUControl.txtBox_adTitle.IsEnabled = false;
            _adsUControl.txtBox_adContent.Text = "";
            _adsUControl.txtBox_adContent.IsEnabled = false;

            _adsUControl.btn_adApply.IsEnabled = false;
            _adsUControl.btn_adApply.Content = "APLICAR";

            _adsUControl.btn_adCreate.Content = "NOVO ANÚNCIO";
            _adsUControl.btn_adCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void ClearAdComponents()
        {            
            isNewAd = true;

            stackp.Visibility = Visibility.Collapsed;

            _adsUControl.lvSelectedIndex = -1;
            _adsUControl.lvNewSelectedIndex = -1;
            selectionID = -1;

            lview_ads.SelectedItem = null;
            lview_ads.IsEnabled = false;

            _adsUControl.btn_adsimageChange.Content = "ADICIONAR IMAGEM";

            image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            image_ad.Tag = "enabled";
            txtBox_adTitle.Text = "";
            txtBox_adTitle.IsEnabled = true;
            txtBox_adContent.Text = "";
            txtBox_adContent.IsEnabled = true;

            btn_adApply.IsEnabled = false;
            btn_adApply.Content = "CRIAR";

            btn_adCreate.Content = "CANCELAR";
            btn_adCreate.Background = new SolidColorBrush(Colors.DarkRed);
        }

        private void Edited(bool isEdited)
        {
            if (isEdited)
            {
                isEditing = true;
                btn_adApply.IsEnabled = true;

                btn_adCreate.Content = "CANCELAR";
                btn_adCreate.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                isEditing = false;
                btn_adApply.IsEnabled = false;

                if (!isNewAd)
                {
                    btn_adCreate.Content = "NOVO ANÚNCIO";
                    btn_adCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                }
            }
        }

        public static void EditionFinished()
        {
            isNewAd = false;
            isEditing = false;

            _adsUControl.imgChanged = false;
            _adsUControl.btn_adsimageChange.Content = "ALTERAR IMAGEM";

            _adsUControl.btn_adApply.IsEnabled = false;
            _adsUControl.btn_adCreate.IsEnabled = true;

            _adsUControl.btn_adCreate.Content = "NOVO ANÚNCIO";
            _adsUControl.btn_adCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        public async static Task QuestionAlterations()
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 SUAS ALTERAÇÕES \b NÃO \b0 FORAM SALVAS, DESEJA CONTINUAR?");            

            if (DialogMessage.Result == DLGAction.Yes)
            {
                DisableAdComponents();  

                if (_adsUControl.lvNewSelectedIndex != _adsUControl.lvSelectedIndex)
                {
                    if (_adsUControl.lvNewSelectedIndex <= _adsUControl.lview_ads.Items.Count)
                    {
                        _adsUControl.lview_ads.SelectedIndex = _adsUControl.lvNewSelectedIndex;
                        _adsUControl.lview_ads.SelectedItem = _adsUControl.lvNewSelectedIndex;
                    }
                }
                else //criação de ad
                {
                    _adsUControl.lview_ads.SelectedIndex = _adsUControl.lvSelectedIndex;
                    _adsUControl.lview_ads.SelectedItem = _adsUControl.lvSelectedIndex;
                }
            }
        }

        public async static Task QuestionDelete(string itemName)
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 DELETAR O ITEM \b'" + itemName + @"'\b0  PERMANENTEMENTE?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                SQLAds.RemoveAd(_adsUControl.selectionID);
                DisableAdComponents();
                LoadAds();
            }
        }

        private void CheckFields()
        {
            if (imgChanged == true | txtBox_adTitle.Text != originalTitle | txtBox_adContent.Text != originalContent)
            {
                Edited(true);
            }
            else
            {
                Edited(false);
            }
        }             

        private void image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((string)image_ad.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Visible;

                VisualGraphics.doBlurOnElement(image_source);
            }
        }

        private void stackp_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            stackp.Visibility = Visibility.Collapsed;

            VisualGraphics.removeBlurOnElement(image_source);
        }

        private async void btn_adCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewAd && !isEditing)
            {
                ClearAdComponents();
                imgChanged = false;
                _adsUControl.originalTitle = "";
                _adsUControl.originalContent = "";

                GlobalMethods.SetFocus(txtBox_adTitle);
            }
            else if (isNewAd | isEditing)
            {
                await CancelAdEdition();
            }
        }

        public static async Task CancelAdEdition()
        {
            if (isEditing == false && isNewAd == true)
            {
                DisableAdComponents();
            }
            else 
            {
                await QuestionAlterations();
            }
        }

        private async void btn_adsimageChange_Click(object sender, RoutedEventArgs e)
        {
            await ImageHandler.ChangeImage();
            if (ImageHandler.ConvertedByteToBitmap != null)
            {
                image_source.Source = ImageHandler.ConvertedByteToBitmap;

                Edited(true);
                imgChanged = true;
            }
        }

        private async void btn_adApply_Click(object sender, RoutedEventArgs e)
        {
            if (isNewAd == false) //Edição de anúncio
            {
                if (txtBox_adTitle.Text != "" & txtBox_adContent.Text != "")
                {
                    SQLAds.UpdateAd(selectionID, ImageHandler.imgOutput, txtBox_adTitle.Text, txtBox_adContent.Text);

                    EditionFinished();
                    LoadAds();
                    _adsUControl.lview_ads.SelectedIndex = lvSelectedIndex;

                    GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
            else //Criação de anúncio
            {
                if (imgChanged == true & txtBox_adTitle.Text != "" & txtBox_adContent.Text != "")
                {
                    SQLAds.CreateAd(ImageHandler.imgOutput, txtBox_adTitle.Text, txtBox_adContent.Text);
                    btn_adApply.Content = "APLICAR";

                    EditionFinished();
                    LoadAds();
                    DisableAdComponents();

                    GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }            
        }        

        private void txtBox_adTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_adContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private async void lview_ads_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                SQLAds.ads ad = lview_ads.SelectedItem as SQLAds.ads;

                await QuestionDelete(ad.title.ToUpper());
            }            
        }

        private async void lview_ads_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_ads.SelectedIndex > -1 && lview_ads.SelectedItem != null)
            {
                if (isEditing)
                {
                    if (lvSelectedIndex != lview_ads.SelectedIndex)
                    {
                        lvNewSelectedIndex = lview_ads.SelectedIndex;
                    }

                    lview_ads.SelectedIndex = lvSelectedIndex;
                    lview_ads.SelectedItem = lvSelectedIndex;

                    await QuestionAlterations();
                }
                else
                {                    
                    EnableAdComponents();

                    lvSelectedIndex = lview_ads.SelectedIndex;
                    lvNewSelectedIndex = lview_ads.SelectedIndex;

                    SQLAds.ads ad = lview_ads.SelectedItem as SQLAds.ads;

                    selectionID = ad.id;
                    adImage_byte = ad.image;
                    //await AdHandler.ByteToBitmap(adImage_byte);
                    ImageHandler.imgOutput = adImage_byte;
                    await ImageHandler.ByteToBitmap(adImage_byte); //possível futuro bug
                    originalTitle = ad.title;
                    originalContent = ad.content;

                    image_source.Source = ImageHandler.ConvertedByteToBitmap;
                    txtBox_adTitle.Text = ad.title;
                    txtBox_adContent.Text = ad.content;                    
                }
            }
        }
    }
}
