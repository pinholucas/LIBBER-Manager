using Libber_Manager.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Libber_Manager.Controls
{
    public sealed partial class Intineraries : UserControl
    {
        public static Intineraries _intinerariesUControl;

        public Intineraries()
        {
            this.InitializeComponent();
            _intinerariesUControl = this;            
        }

        public static bool isNewIntinerarie = false;
        public static bool isEditing = false;

        public int lvSelectedIndex = -1;
        public int lvNewSelectedIndex = -1;

        int selectionID;
        bool imgChanged = false;
        public static byte[] intiImage_byte;
        decimal originalRating;
        string originalTitle;
        string originalDescription;
        string originalDContent;

        public static void LoadIntineraries()
        {
            SQLIntineraries.LoadIntinerariesTable();
            _intinerariesUControl.lview_intineraries.ItemsSource = SQLIntineraries.Intineraries;
        }

        private static void EnableIntinerarieComponents()
        {
            _intinerariesUControl.image_intinerarie.Tag = "enabled";
            _intinerariesUControl.txtBox_intiRating.IsEnabled = true;
            _intinerariesUControl.txtBox_intiTitle.IsEnabled = true;
            _intinerariesUControl.txtBox_intiDescription.IsEnabled = true;
            _intinerariesUControl.txtBox_intiDetailsContent.IsEnabled = true;

            _intinerariesUControl.btn_intiCampsParks.IsEnabled = true;
            _intinerariesUControl.btn_intiImages.IsEnabled = true;
        }

        public static void DisableIntinerarieComponents()
        {
            EditionFinished();
            _intinerariesUControl.imgChanged = false;
            _intinerariesUControl.originalRating = 0;
            _intinerariesUControl.originalTitle = "";
            _intinerariesUControl.originalDescription = "";
            _intinerariesUControl.originalDContent = "";

            _intinerariesUControl.stackp.Visibility = Visibility.Collapsed;

            _intinerariesUControl.selectionID = -1;

            _intinerariesUControl.lview_intineraries.SelectedIndex = -1;
            _intinerariesUControl.lview_intineraries.SelectedItem = null;
            _intinerariesUControl.lview_intineraries.IsEnabled = true;

            _intinerariesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            _intinerariesUControl.image_intinerarie.Tag = "disabled";
            _intinerariesUControl.txtBox_intiRating.Text = "";
            _intinerariesUControl.txtBox_intiRating.IsEnabled = false;
            _intinerariesUControl.txtBox_intiTitle.Text = "";
            _intinerariesUControl.txtBox_intiTitle.IsEnabled = false;
            _intinerariesUControl.txtBox_intiDescription.Text = "";
            _intinerariesUControl.txtBox_intiDescription.IsEnabled = false;
            _intinerariesUControl.txtBox_intiDetailsContent.Text = "";
            _intinerariesUControl.txtBox_intiDetailsContent.IsEnabled = false;
            _intinerariesUControl.btn_intiCampsParks.IsEnabled = false;
            _intinerariesUControl.btn_intiImages.IsEnabled = false;

            _intinerariesUControl.btn_intiApply.IsEnabled = false;
            _intinerariesUControl.btn_intiApply.Content = "APLICAR";

            _intinerariesUControl.btn_intiCreate.Content = "NOVO ROTEIRO";
            _intinerariesUControl.btn_intiCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void ClearIntinerarieComponents()
        {
            isNewIntinerarie = true;

            stackp.Visibility = Visibility.Collapsed;

            _intinerariesUControl.lvSelectedIndex = -1;
            _intinerariesUControl.lvNewSelectedIndex = -1;
            selectionID = -1;

            lview_intineraries.SelectedItem = null;
            lview_intineraries.IsEnabled = false;

            _intinerariesUControl.btn_intinerarieImageChange.Content = "ADICIONAR IMAGEM";

            image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
            image_intinerarie.Tag = "enabled";
            _intinerariesUControl.txtBox_intiRating.Text = "";
            _intinerariesUControl.txtBox_intiRating.IsEnabled = true;
            _intinerariesUControl.txtBox_intiTitle.Text = "";
            _intinerariesUControl.txtBox_intiTitle.IsEnabled = true;
            _intinerariesUControl.txtBox_intiDescription.Text = "";
            _intinerariesUControl.txtBox_intiDescription.IsEnabled = true;
            _intinerariesUControl.txtBox_intiDetailsContent.Text = "";
            _intinerariesUControl.txtBox_intiDetailsContent.IsEnabled = true;
            _intinerariesUControl.btn_intiCampsParks.IsEnabled = true;
            _intinerariesUControl.btn_intiImages.IsEnabled = true;

            btn_intiApply.IsEnabled = false;
            btn_intiApply.Content = "CRIAR";

            btn_intiCreate.Content = "CANCELAR";
            btn_intiCreate.Background = new SolidColorBrush(Colors.DarkRed);
        }

        private void Edited(bool isEdited)
        {
            if (isEdited)
            {
                isEditing = true;
                btn_intiApply.IsEnabled = true;

                btn_intiCreate.Content = "CANCELAR";
                btn_intiCreate.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                isEditing = false;
                btn_intiApply.IsEnabled = false;

                if (!isNewIntinerarie)
                {
                    btn_intiCreate.Content = "NOVO ROTEIRO";
                    btn_intiCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                }
            }
        }

        public static void EditionFinished()
        {
            isNewIntinerarie = false;
            isEditing = false;

            _intinerariesUControl.imgChanged = false;
            _intinerariesUControl.btn_intinerarieImageChange.Content = "ALTERAR IMAGEM";

            _intinerariesUControl.btn_intiApply.IsEnabled = false;
            _intinerariesUControl.btn_intiCreate.IsEnabled = true;

            _intinerariesUControl.btn_intiCreate.Content = "NOVO ROTEIRO";
            _intinerariesUControl.btn_intiCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void CheckFields()
        {
            string rating = txtBox_intiRating.Text == "" ? "0" : txtBox_intiRating.Text;

            if (imgChanged == true | rating != originalRating.ToString() |
                txtBox_intiTitle.Text != originalTitle | txtBox_intiDescription.Text != originalDescription |
                txtBox_intiDetailsContent.Text != originalDContent)
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
                DisableIntinerarieComponents();

                if (_intinerariesUControl.lvNewSelectedIndex != _intinerariesUControl.lvSelectedIndex)
                {
                    if (_intinerariesUControl.lvNewSelectedIndex <= _intinerariesUControl.lview_intineraries.Items.Count)
                    {
                        _intinerariesUControl.lview_intineraries.SelectedIndex = _intinerariesUControl.lvNewSelectedIndex;
                        _intinerariesUControl.lview_intineraries.SelectedItem = _intinerariesUControl.lvNewSelectedIndex;
                    }
                }
                else //criação de ad
                {
                    _intinerariesUControl.lview_intineraries.SelectedIndex = _intinerariesUControl.lvSelectedIndex;
                    _intinerariesUControl.lview_intineraries.SelectedItem = _intinerariesUControl.lvSelectedIndex;
                }
            }
        }

        public async static Task QuestionDelete(string itemName)
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 DELETAR O ITEM \b'" + itemName + @"'\b0  PERMANENTEMENTE?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                SQLIntineraries.RemoveIntinerarie(_intinerariesUControl.selectionID);
                DisableIntinerarieComponents();
                LoadIntineraries();
            }
        }

        public static async Task CancelIntinerarieEdition()
        {
            if (isEditing == false && isNewIntinerarie == true)
            {
                DisableIntinerarieComponents();
            }
            else
            {
                await QuestionAlterations();
            }
        }

        private async void btn_intiCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewIntinerarie && !isEditing)
            {
                ClearIntinerarieComponents();
                imgChanged = false;
                _intinerariesUControl.originalRating = 0;
                _intinerariesUControl.originalTitle = "";
                _intinerariesUControl.originalDescription = "";
                _intinerariesUControl.originalDContent = "";

                btn_intiCampsParks.IsEnabled = false;
                btn_intiImages.IsEnabled = false;

                GlobalMethods.SetFocus(txtBox_intiRating);
            }
            else
            {
                await CancelIntinerarieEdition();
            }
        }        

        private async void lview_intineraries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_intineraries.SelectedIndex > -1 && lview_intineraries.SelectedItem != null)
            {
                if (isEditing)
                {
                    if (lvSelectedIndex != lview_intineraries.SelectedIndex)
                    {
                        lvNewSelectedIndex = lview_intineraries.SelectedIndex;
                    }

                    try
                    {
                        lview_intineraries.SelectedIndex = lvSelectedIndex;
                        lview_intineraries.SelectedItem = lvSelectedIndex;
                    }
                    catch
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 ALGO DEU ERRADO, REINICIE O PROGRAMA E REPORTE O PROBLEMA AO DESENVOLVEDOR. \par Erro 03#002");
                    }

                    await QuestionAlterations();
                }
                else
                {
                    EnableIntinerarieComponents();

                    lvSelectedIndex = lview_intineraries.SelectedIndex;
                    lvNewSelectedIndex = lview_intineraries.SelectedIndex;

                    SQLIntineraries.intineraries inti = lview_intineraries.SelectedItem as SQLIntineraries.intineraries;

                    selectionID = inti.id;
                    intiImage_byte = inti.image;
                    //await AdHandler.ByteToBitmap(adImage_byte);
                    ImageHandler.imgOutput = intiImage_byte;
                    await ImageHandler.ByteToBitmap(intiImage_byte); //possível futuro bug
                    originalRating = inti.rating;
                    originalTitle = inti.title;
                    originalDescription = inti.description;
                    originalDContent = inti.details_content;

                    image_source.Source = ImageHandler.ConvertedByteToBitmap;
                    txtBox_intiRating.Text = originalRating.ToString();
                    txtBox_intiTitle.Text = originalTitle;
                    txtBox_intiDescription.Text = originalDescription;
                    txtBox_intiDetailsContent.Text = originalDContent;
                }
            }
        }

        private async void lview_intineraries_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                SQLIntineraries.intineraries intinerarie = lview_intineraries.SelectedItem as SQLIntineraries.intineraries;

                await QuestionDelete(intinerarie.title.ToUpper());
            }
        }

        private async void btn_intiApply_Click(object sender, RoutedEventArgs e)
        {
            if (isNewIntinerarie == false) //Edição de anúncio
            {
                if (txtBox_intiRating.Text != "" & txtBox_intiTitle.Text != "" &
                    txtBox_intiDescription.Text != "" & txtBox_intiDetailsContent.Text != "")
                {
                    if (GlobalMethods.IsTextNumeric(txtBox_intiRating.Text))
                    {
                        SQLIntineraries.UpdateIntinerarie(selectionID, ImageHandler.imgOutput, Convert.ToDecimal(txtBox_intiRating.Text), txtBox_intiTitle.Text, txtBox_intiDescription.Text, txtBox_intiDetailsContent.Text);

                        EditionFinished();
                        LoadIntineraries();
                        _intinerariesUControl.lview_intineraries.SelectedIndex = lvSelectedIndex;

                        GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                    }
                    else
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 NÃO UTILIZE LETRAS NO CAMPO DE RATING!");
                    }
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
            else //Criação de anúncio
            {
                if (imgChanged & txtBox_intiRating.Text != "" &
                    txtBox_intiTitle.Text != "" & txtBox_intiDescription.Text != "" & txtBox_intiDetailsContent.Text != "")
                {
                    if (GlobalMethods.IsTextNumeric(txtBox_intiRating.Text))
                    {
                        SQLIntineraries.CreateIntinerarie(ImageHandler.imgOutput, Convert.ToDecimal(txtBox_intiRating.Text), txtBox_intiTitle.Text, txtBox_intiDescription.Text, txtBox_intiDetailsContent.Text);
                        btn_intiApply.Content = "APLICAR";

                        EditionFinished();
                        LoadIntineraries();
                        DisableIntinerarieComponents();

                        GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                    }
                    else
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 NÃO UTILIZE LETRAS NO CAMPO DE RATING!");
                    }
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
        }

        private async void btn_intiCampsParks_Click(object sender, RoutedEventArgs e)
        {            
            await DialogMessage.ShowDialog(DLGWType.View, "CAMPING'S E PARQUES", "");
            DialogMessage._dialogMessageUControl.CampsParksControl.Visibility = Visibility.Visible;

            CampsAndParks.LoadCampsAndParks(selectionID);
        }

        private async void btn_intiImages_Click(object sender, RoutedEventArgs e)
        {
            await DialogMessage.ShowDialog(DLGWType.View, "MAPA E IMAGENS DO ROTEIRO", "");
            DialogMessage._dialogMessageUControl.IntinerarieImagesControl.Visibility = Visibility.Visible;            

            await IntinerarieImages.LoadIntiMap(selectionID);
            await IntinerarieImages.LoadIntiPhotos(selectionID);            
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

        private void txtBox_intiRating_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_intiTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_intiDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_intiDetailsTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_intiDetailsContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }
    }
}
