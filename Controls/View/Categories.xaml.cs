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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace Libber_Manager.Controls
{    
    public sealed partial class Categories : UserControl
    {
        public static Categories _placesUControl;
        public Categories()
        {
            this.InitializeComponent();
            _placesUControl = this;
        }

        public static bool isViewing = false;
        public static bool isNewCategoryItem = false;
        public static bool isEditing = false;

        int lvSelectedIndex = -1;
        int lvNewSelectedIndex = -1;

        int categorySelectedIndex;
        int categoryNewSelectedIndex;

        // Categoria Locais
        int selectionID;
        int originalIcon;
        string originalTitle = "";
        string originalDescription = "";
        string originalInMapDescription = "";
        double originalLat;
        double originalLng;

        // Categoria Adicionais
        bool imgChanged = false;
        public static byte[] intiImage_byte;
        string originalAdditionalTitle = "";
        string originalAdditionalPrice = "";
        string originalAdditionalDetails = "";

        public static void LoadCategory()
        {
            _placesUControl.categorySelectedIndex = _placesUControl.cBox_categories.SelectedIndex;
            SQLPlaces.LoadPlacesTable(_placesUControl.categorySelectedIndex);

            if (GlobalMethods.IsLocalsCategory())
            {
                _placesUControl.lview_categories.ItemsSource = SQLPlaces.Places;
            }
            else if (GlobalMethods.IsAdditionalsCategory())
            {
                _placesUControl.lview_categories.ItemsSource = SQLPlaces.Additionals;
            }
        }        

        private void EnableCategoryComponents()
        {
            if (GlobalMethods.IsLocalsCategory())
            {
                cBox_placesIcons.IsEnabled = true;

                txtBox_placeTitle.IsEnabled = true;
                txtBox_placeDescription.IsEnabled = true;
                txtBox_placeInMapDescription.IsEnabled = true;
                txtBox_placeLat.IsEnabled = true;
                txtBox_placeLng.IsEnabled = true;
            }
            else if (GlobalMethods.IsAdditionalsCategory())
            {
                imageBorder_additional.Tag = "enabled";

                txtBox_additionalTitle.IsEnabled = true;
                txtBox_additionalPrice.IsEnabled = true;
                txtBox_additionalDetails.IsEnabled = true;
            }
        }

        public static void DisableCategoryComponents()
        {
            EditionFinished();

            _placesUControl.selectionID = -1;

            _placesUControl.cBox_categories.IsEnabled = true;

            _placesUControl.lview_categories.SelectedIndex = -1;
            _placesUControl.lview_categories.SelectedItem = null;
            _placesUControl.lview_categories.IsEnabled = true;

            if (GlobalMethods.IsLocalsCategory())
            {
                _placesUControl.originalIcon = 0;
                _placesUControl.originalTitle = "";
                _placesUControl.originalDescription = "";
                _placesUControl.originalInMapDescription = "";
                _placesUControl.originalLat = 0;
                _placesUControl.originalLng = 0;     

                _placesUControl.cBox_placesIcons.SelectedIndex = -1;
                _placesUControl.cBox_placesIcons.SelectedItem = null;
                _placesUControl.cBox_placesIcons.IsEnabled = false;

                _placesUControl.txtBox_placeTitle.Text = "";
                _placesUControl.txtBox_placeTitle.IsEnabled = false;
                _placesUControl.txtBox_placeDescription.Text = "";
                _placesUControl.txtBox_placeDescription.IsEnabled = false;
                _placesUControl.txtBox_placeInMapDescription.Text = "";
                _placesUControl.txtBox_placeInMapDescription.IsEnabled = false;
                _placesUControl.txtBox_placeLat.Text = "";
                _placesUControl.txtBox_placeLat.IsEnabled = false;
                _placesUControl.txtBox_placeLng.Text = "";
                _placesUControl.txtBox_placeLng.IsEnabled = false;

                _placesUControl.btn_placeApply.Content = "APLICAR";

                _placesUControl.btn_categoryCreate.Content = "NOVO LOCAL";
            }
            else if (GlobalMethods.IsAdditionalsCategory())
            {
                _placesUControl.imgChanged = false;
                _placesUControl.originalAdditionalTitle = "";
                _placesUControl.originalAdditionalPrice = "";
                _placesUControl.originalAdditionalDetails = "";

                _placesUControl.image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
                _placesUControl.imageBorder_additional.Tag = "disabled";
                _placesUControl.txtBox_additionalTitle.Text = "";
                _placesUControl.txtBox_additionalTitle.IsEnabled = false;
                _placesUControl.txtBox_additionalPrice.Text = "";
                _placesUControl.txtBox_additionalPrice.IsEnabled = false;
                _placesUControl.txtBox_additionalDetails.Text = "";
                _placesUControl.txtBox_additionalDetails.IsEnabled = false;

                _placesUControl.btn_additionalApply.Content = "APLICAR";

                _placesUControl.btn_categoryCreate.Content = "NOVO ADICIONAL";
            }
            
            _placesUControl.btn_categoryCreate.Background = GlobalMethods.btnLightBlue;
        }

        private void ClearCategoryComponents()
        {
            isNewCategoryItem = true;
            isViewing = false;

            _placesUControl.lvSelectedIndex = -1;
            _placesUControl.lvNewSelectedIndex = -1;
            selectionID = -1;

            cBox_categories.IsEnabled = false;
            lview_categories.IsEnabled = false;
            lview_categories.SelectedItem = null;

            if (GlobalMethods.IsLocalsCategory())
            {
                cBox_placesIcons.SelectedItem = null;
                cBox_placesIcons.IsEnabled = true;

                txtBox_placeTitle.Text = "";
                txtBox_placeTitle.IsEnabled = true;
                txtBox_placeDescription.Text = "";
                txtBox_placeDescription.IsEnabled = true;
                txtBox_placeInMapDescription.Text = "";
                txtBox_placeInMapDescription.IsEnabled = true;
                txtBox_placeLat.Text = "";
                txtBox_placeLat.IsEnabled = true;
                txtBox_placeLng.Text = "";
                txtBox_placeLng.IsEnabled = true;

                btn_placeApply.IsEnabled = false;
                btn_placeApply.Content = "CRIAR";
            }
            else if (GlobalMethods.IsAdditionalsCategory())
            {
                _placesUControl.btn_additionalImageChange.Content = "ADICIONAR";

                image_source.Source = new BitmapImage(new Uri("ms-appx:///Assets/bgimg.png"));
                imageBorder_additional.Tag = "enabled";
                _placesUControl.txtBox_additionalTitle.Text = "";
                _placesUControl.txtBox_additionalTitle.IsEnabled = true;
                _placesUControl.txtBox_additionalPrice.Text = "";
                _placesUControl.txtBox_additionalPrice.IsEnabled = true;
                _placesUControl.txtBox_additionalDetails.Text = "";
                _placesUControl.txtBox_additionalDetails.IsEnabled = true;

                btn_additionalApply.Content = "CRIAR";
            }

            btn_categoryCreate.Content = "CANCELAR";
            btn_categoryCreate.Background = GlobalMethods.btnDarkRed;
        }

        private void Edited(bool isEdited)
        {
            if (isEdited)
            {
                isEditing = true;

                if (GlobalMethods.IsLocalsCategory())
                {
                    btn_placeApply.IsEnabled = true;
                }
                else if (GlobalMethods.IsAdditionalsCategory())
                {
                    btn_additionalApply.IsEnabled = true;
                }

                btn_categoryCreate.Content = "CANCELAR";
                btn_categoryCreate.Background = GlobalMethods.btnDarkRed;
            }
            else
            {
                isEditing = false;

                if (GlobalMethods.IsLocalsCategory())
                {
                    btn_placeApply.IsEnabled = false;
                }
                else if (GlobalMethods.IsAdditionalsCategory())
                {
                    btn_additionalApply.IsEnabled = false;
                }

                if (!isNewCategoryItem)
                {
                    if (GlobalMethods.IsLocalsCategory())
                    {
                        btn_categoryCreate.Content = "NOVO LOCAL";
                    }
                    else if (GlobalMethods.IsAdditionalsCategory())
                    {
                        btn_categoryCreate.Content = "NOVO ADICIONAL";
                    }

                    btn_categoryCreate.Background = GlobalMethods.btnLightBlue;
                }
            }
        }

        public static void EditionFinished()
        {
            isNewCategoryItem = false;
            isEditing = false;
            isViewing = false;

            if (GlobalMethods.IsLocalsCategory())
            {
                _placesUControl.btn_placeApply.IsEnabled = false;

                _placesUControl.btn_categoryCreate.Content = "NOVO LOCAL";
            }
            else if (GlobalMethods.IsAdditionalsCategory())
            {
                _placesUControl.btn_additionalImageChange.Content = "ALTERAR";

                _placesUControl.btn_additionalApply.IsEnabled = false;

                _placesUControl.btn_categoryCreate.Content = "NOVO ADICIONAL";                
            }

            _placesUControl.btn_categoryCreate.Background = GlobalMethods.btnLightBlue;
        }

        public async static Task QuestionAlterations()
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 SUAS ALTERAÇÕES \b NÃO \b0 FORAM SALVAS, DESEJA CONTINUAR?");            

            if (DialogMessage.Result == DLGAction.Yes)
            {
                DisableCategoryComponents();

                if (_placesUControl.lvNewSelectedIndex != _placesUControl.lvSelectedIndex)
                {
                    if (_placesUControl.lvNewSelectedIndex < _placesUControl.lview_categories.Items.Count)
                    {
                        _placesUControl.lview_categories.SelectedIndex = _placesUControl.lvNewSelectedIndex;
                        _placesUControl.lview_categories.SelectedItem = _placesUControl.lvNewSelectedIndex;
                    }

                    _placesUControl.cBox_categories.SelectedIndex = _placesUControl.categoryNewSelectedIndex;
                    _placesUControl.cBox_categories.SelectedItem = _placesUControl.categoryNewSelectedIndex;
                }
                else //criação de faq
                {
                    _placesUControl.lview_categories.SelectedIndex = _placesUControl.lvSelectedIndex;
                    _placesUControl.lview_categories.SelectedItem = _placesUControl.lvSelectedIndex;

                    _placesUControl.cBox_categories.SelectedIndex = _placesUControl.categoryNewSelectedIndex;
                    _placesUControl.cBox_categories.SelectedItem = _placesUControl.categoryNewSelectedIndex;                    
                }
            } 
            else if (DialogMessage.Result == DLGAction.No)
            {
                _placesUControl.categoryNewSelectedIndex = _placesUControl.cBox_categories.SelectedIndex;
            }
        }

        public async static Task QuestionDelete(string itemName)
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 DELETAR O ITEM \b'" + itemName + @"'\b0  PERMANENTEMENTE?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                if (GlobalMethods.IsLocalsCategory())
                {
                    SQLPlaces.RemovePlace(_placesUControl.categorySelectedIndex, _placesUControl.selectionID);
                }
                else if (GlobalMethods.IsAdditionalsCategory())
                {
                    SQLVehiclesAdditionals.RemoveAdditional(_placesUControl.selectionID);
                }

                DisableCategoryComponents();
                LoadCategory();
            }
        }

        private void CheckFields()
        {
            if (isViewing == true)
            {
                if (GlobalMethods.IsLocalsCategory())
                {
                    string lat = txtBox_placeLat.Text == "" ? "0" : txtBox_placeLat.Text;
                    string lng = txtBox_placeLng.Text == "" ? "0" : txtBox_placeLng.Text;

                    if (cBox_placesIcons.SelectedIndex != originalIcon | txtBox_placeTitle.Text != originalTitle |
                        txtBox_placeDescription.Text != originalDescription | txtBox_placeInMapDescription.Text != originalInMapDescription |
                        lat != originalLat.ToString() | lng != originalLng.ToString())
                    {
                        Edited(true);
                    }
                    else
                    {
                        Edited(false);
                    }
                }
                else if (GlobalMethods.IsAdditionalsCategory())
                {
                    if (imgChanged == true |txtBox_additionalTitle.Text != originalAdditionalTitle | 
                        txtBox_additionalPrice.Text != originalAdditionalPrice |txtBox_additionalDetails.Text != originalAdditionalDetails)
                    {
                        Edited(true);
                    }
                    else
                    {
                        Edited(false);
                    }
                }
            }
        }

        public static async Task CancelCategoryEdition()
        {
            if (isEditing == false && isNewCategoryItem == true)
            {
                DisableCategoryComponents();
            }
            else
            {
                await QuestionAlterations();
            }
        }

        private async void btn_categoryCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewCategoryItem && !isEditing)
            {
                ClearCategoryComponents();
                isViewing = true;

                if (GlobalMethods.IsLocalsCategory())
                {
                    _placesUControl.originalIcon = -1;
                    _placesUControl.originalTitle = "";
                    _placesUControl.originalDescription = "";
                    _placesUControl.originalInMapDescription = "";
                    _placesUControl.originalLat = 0;
                    _placesUControl.originalLng = 0;

                    GlobalMethods.SetFocus(_placesUControl.txtBox_placeTitle);
                }
                else if (GlobalMethods.IsAdditionalsCategory())
                {
                    _placesUControl.imgChanged = false;
                    _placesUControl.originalAdditionalTitle = "";
                    _placesUControl.originalAdditionalPrice = "";
                    _placesUControl.originalAdditionalDetails = "";

                    GlobalMethods.SetFocus(_placesUControl.txtBox_additionalTitle);
                }
            }
            else
            {
                await CancelCategoryEdition();
            }
        }

        private async void cBox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalMethods.isConnectionOpen == true)
            {
                if (isEditing)
                {
                    if (categorySelectedIndex != cBox_categories.SelectedIndex)
                    {
                        categoryNewSelectedIndex = cBox_categories.SelectedIndex;

                        _placesUControl.lvSelectedIndex = -1;
                        _placesUControl.lvNewSelectedIndex = -1;
                    }
                    
                    cBox_categories.SelectedIndex = categorySelectedIndex;
                    cBox_categories.SelectedItem = categorySelectedIndex;

                    await QuestionAlterations();
                }
                else
                {
                    if (GlobalMethods.IsLocalsCategory()) // Grupo de categorias do Locais
                    {
                        grid_additionals.Visibility = Visibility.Collapsed;
                        grid_places.Visibility = Visibility.Visible;

                        DisableCategoryComponents();                        
                    }
                    else if (GlobalMethods.IsAdditionalsCategory()) // Adicionais do veículo
                    {
                        grid_places.Visibility = Visibility.Collapsed;
                        grid_additionals.Visibility = Visibility.Visible;

                        DisableCategoryComponents();
                    }

                    categorySelectedIndex = cBox_categories.SelectedIndex;
                    categoryNewSelectedIndex = cBox_categories.SelectedIndex;

                    LoadCategory();
                }                
            }
        }        

        private async void lview_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_categories.SelectedIndex > -1 && lview_categories.SelectedItem != null)
            {
                if (isEditing)
                {
                    if (lvSelectedIndex != lview_categories.SelectedIndex)
                    {
                        lvNewSelectedIndex = lview_categories.SelectedIndex;
                    }

                    lview_categories.SelectedIndex = lvSelectedIndex;
                    lview_categories.SelectedItem = lvSelectedIndex;

                    await QuestionAlterations();
                }
                else
                {
                    lvSelectedIndex = lview_categories.SelectedIndex;
                    lvNewSelectedIndex = lview_categories.SelectedIndex;

                    isViewing = true;                    

                    if (GlobalMethods.IsLocalsCategory()) // Grupo de categorias do Locais
                    {
                        EnableCategoryComponents();

                        SQLPlaces.places place = lview_categories.SelectedItem as SQLPlaces.places;

                        selectionID = place.id;

                        originalIcon = place.icon;
                        cBox_placesIcons.SelectedIndex = originalIcon;
                        originalTitle = place.title;
                        txtBox_placeTitle.Text = originalTitle;
                        originalDescription = place.description;
                        txtBox_placeDescription.Text = originalDescription;
                        originalInMapDescription = place.inmap_description;
                        txtBox_placeInMapDescription.Text = originalInMapDescription;
                        originalLat = place.lat;
                        txtBox_placeLat.Text = originalLat.ToString();
                        originalLng = place.lng;
                        txtBox_placeLng.Text = originalLng.ToString();                                             
                    }
                    else if (GlobalMethods.IsAdditionalsCategory()) // Adicionais do veículo
                    {
                        EnableCategoryComponents();

                        SQLPlaces.additionals additional = lview_categories.SelectedItem as SQLPlaces.additionals;

                        selectionID = additional.id;

                        intiImage_byte = additional.icon;
                        //await AdHandler.ByteToBitmap(adImage_byte);
                        ImageHandler.imgOutput = intiImage_byte;
                        await ImageHandler.ByteToBitmap(intiImage_byte); //possível futuro bug
                        image_source.Source = ImageHandler.ConvertedByteToBitmap;
                        originalAdditionalTitle = additional.title;
                        txtBox_additionalTitle.Text = originalAdditionalTitle;
                        originalAdditionalPrice = additional.price;
                        txtBox_additionalPrice.Text = originalAdditionalPrice;
                        originalAdditionalDetails = additional.details;                        
                        txtBox_additionalDetails.Text = originalAdditionalDetails;
                    }                    
                }
            }
        }

        private async void lview_categories_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                string itemName = "";

                if (GlobalMethods.IsLocalsCategory())
                {
                    SQLPlaces.places place = lview_categories.SelectedItem as SQLPlaces.places;

                    itemName = place.title;
                }
                else if (GlobalMethods.IsAdditionalsCategory())
                {
                    SQLPlaces.additionals additional = lview_categories.SelectedItem as SQLPlaces.additionals;

                    itemName = additional.title;
                }

                await QuestionDelete(itemName.ToUpper());
            }
        }

        #region PLACES

        private async void btn_placeApply_Click(object sender, RoutedEventArgs e)
        {
            if (isNewCategoryItem == false) //Edição de local
            {
                if (cBox_placesIcons.SelectedItem != null &
                    txtBox_placeTitle.Text != "" &
                    txtBox_placeDescription.Text != "" &
                    txtBox_placeInMapDescription.Text != "" &
                    txtBox_placeLat.Text != "" &
                    txtBox_placeLng.Text != "")
                {
                    if (GlobalMethods.IsTextNumeric(txtBox_placeLat.Text) & GlobalMethods.IsTextNumeric(txtBox_placeLng.Text))
                    {
                        SQLPlaces.UpdatePlacesTable(categorySelectedIndex,
                                                    selectionID,
                                                    cBox_placesIcons.SelectedIndex,
                                                    txtBox_placeTitle.Text,
                                                    txtBox_placeDescription.Text,
                                                    txtBox_placeInMapDescription.Text,
                                                    Convert.ToDouble(txtBox_placeLat.Text),
                                                    Convert.ToDouble(txtBox_placeLng.Text));                        

                        originalIcon = cBox_placesIcons.SelectedIndex;
                        originalTitle = txtBox_placeTitle.Text;
                        originalDescription = txtBox_placeDescription.Text;
                        originalInMapDescription = txtBox_placeInMapDescription.Text;
                        originalLat = Convert.ToDouble(txtBox_placeLat.Text);
                        originalLng = Convert.ToDouble(txtBox_placeLng.Text);

                        EditionFinished();
                        isViewing = true;
                        LoadCategory();

                        lview_categories.SelectedIndex = lvSelectedIndex;
                        lview_categories.SelectedItem = lvSelectedIndex;

                        GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                    }
                    else
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 NÃO UTILIZE LETRAS NOS CAMPOS DE LATITUDE E LONGITUDE!");
                    }
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
            else //Criação de local
            {
                if (cBox_placesIcons.SelectedItem != null &
                    txtBox_placeTitle.Text != "" &
                    txtBox_placeDescription.Text != "" &
                    txtBox_placeInMapDescription.Text != "" &
                    txtBox_placeLat.Text != "" &
                    txtBox_placeLng.Text != ""
                    )
                {
                    if (GlobalMethods.IsTextNumeric(txtBox_placeLat.Text) & GlobalMethods.IsTextNumeric(txtBox_placeLng.Text))
                    {
                        SQLPlaces.InsertPlace(categorySelectedIndex,
                                           selectionID,
                                           cBox_placesIcons.SelectedIndex,                                           
                                           txtBox_placeTitle.Text,
                                           txtBox_placeDescription.Text,
                                           txtBox_placeInMapDescription.Text,
                                           Convert.ToSingle(txtBox_placeLat.Text),
                                           Convert.ToSingle(txtBox_placeLng.Text));
                        btn_placeApply.Content = "APLICAR";

                        DisableCategoryComponents();
                        LoadCategory();

                        GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                    }
                    else
                    {
                        await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 NÃO UTILIZE LETRAS NOS CAMPOS DE LATITUDE E LONGITUDE!");
                    }
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
        }        

        private void txtBox_placeTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_placeDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_placeInMapDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_placeLat_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_placeLng_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void cBox_placesIcons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckFields();
        }

        #endregion        

        #region ADDITIONALS

        private async void btn_additionalApply_Click(object sender, RoutedEventArgs e)
        {
            if (isNewCategoryItem == false) //Edição de adicional
            {
                if (txtBox_additionalTitle.Text != "" & txtBox_additionalPrice.Text != "" &
                    txtBox_additionalDetails.Text != "")
                {
                    SQLVehiclesAdditionals.UpdateAdditional(selectionID,
                                                            ImageHandler.imgOutput,
                                                            txtBox_additionalTitle.Text,
                                                            txtBox_additionalPrice.Text,
                                                            txtBox_additionalDetails.Text);

                    imgChanged = false;
                    originalAdditionalTitle = txtBox_additionalTitle.Text;
                    originalAdditionalPrice = txtBox_additionalPrice.Text;
                    originalAdditionalDetails = txtBox_additionalDetails.Text;

                    EditionFinished();
                    isViewing = true;
                    LoadCategory();

                    lview_categories.SelectedIndex = lvSelectedIndex;
                    lview_categories.SelectedItem = lvSelectedIndex;

                    GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }

            }
            else //Criação de adicional
            {
                if (imgChanged & txtBox_additionalTitle.Text != "" &
                    txtBox_additionalPrice.Text != "" & txtBox_additionalDetails.Text != "")
                {
                    SQLVehiclesAdditionals.CreateAdditional(ImageHandler.imgOutput,
                                                            txtBox_additionalTitle.Text,
                                                            txtBox_additionalPrice.Text,
                                                            txtBox_additionalDetails.Text);

                    btn_additionalApply.Content = "APLICAR";

                    DisableCategoryComponents();
                    LoadCategory();

                    GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
        }

        private void image_source_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((string)imageBorder_additional.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Visible;

                VisualGraphics.doBlurOnElement(image_source);
            }
        }

        private void stackp_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if ((string)imageBorder_additional.Tag == "enabled")
            {
                stackp.Visibility = Visibility.Collapsed;

                VisualGraphics.removeBlurOnElement(image_source);
            }
        }

        private async void btn_additionalImageChange_Click(object sender, RoutedEventArgs e)
        {
            await ImageHandler.ChangeImage();
            if (ImageHandler.ConvertedByteToBitmap != null)
            {
                image_source.Source = ImageHandler.ConvertedByteToBitmap;

                Edited(true);
                imgChanged = true;
            }
        }        

        private void txtBox_additionalTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_additionalPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_additionalDetails_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        #endregion
    }
}