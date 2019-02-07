using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Libber_Manager.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Automation.Peers;
using Libber_Manager.Helpers;
using Windows.UI.Xaml.Hosting;
using Windows.ApplicationModel.Core;
using Windows.UI.Composition;
using System.Numerics;
using System.Diagnostics;

namespace Libber_Manager
{
    public sealed partial class MainPage : Page
    {
        public static MainPage _mainPage;

        public MainPage()
        {
            this.InitializeComponent();
            _mainPage = this;
            
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Color.FromArgb(100, 47, 50, 67);
            titleBar.BackgroundColor = Color.FromArgb(100, 47, 50, 67);
            
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            /*
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            TitleBar.Height = 32;
            Window.Current.SetTitleBar(MainTitleBar);
            */
            //Estende o NEON pra TitleBar

            //ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            //formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            //CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            //coreTitleBar.ExtendViewIntoTitleBar = true;

            //applyAcrylicAccent(MainGrid2);
        }

        //private void applyAcrylicAccent(Panel e)
        //{
        //    _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        //    _hostSprite = _compositor.CreateSpriteVisual();
        //    _hostSprite.Size = new Vector2((float)MainGrid2.ActualWidth, (float)MainGrid2.ActualHeight);

        //    ElementCompositionPreview.SetElementChildVisual(
        //            MainGrid2, _hostSprite);
        //    _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
        //}
        //Compositor _compositor;
        //SpriteVisual _hostSprite;

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (_hostSprite != null)
            //    _hostSprite.Size = e.NewSize.ToVector2();
        }

        public static void hideMainControls()
        {
            _mainPage._Ads.Visibility = Visibility.Collapsed;
            _mainPage._Places.Visibility = Visibility.Collapsed;
            _mainPage._Intineraries.Visibility = Visibility.Collapsed;
            _mainPage._Vehicles.Visibility = Visibility.Collapsed;
            _mainPage._FAQs.Visibility = Visibility.Collapsed;
            _mainPage._Emergency.Visibility = Visibility.Collapsed;
        }

        public static void showMessageDLG()
        {
            DialogMessage._dialogMessageUControl.Visibility = Visibility.Visible;
        }

        public static void hideMessageDLG()
        {
            DialogMessage._dialogMessageUControl.Visibility = Visibility.Collapsed;
        }

        public static void ClearAndDisableAllComponents()
        {
            Ads.DisableAdComponents();
            Categories.DisableCategoryComponents();
            Intineraries.DisableIntinerarieComponents();
            Vehicles.DisableVehicleComponents();
            FAQs.DisableFAQComponents();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Captura o evento KeyPress globalmente
            var appWindow = Window.Current.CoreWindow;
            appWindow.KeyDown += HandleKeyPress;
        }

        private async void HandleKeyPress(object sender, KeyEventArgs e)
        {
            ButtonAutomationPeer peer;
            //Debug.WriteLine("isEditing: " + Ads.isEditing + " | isNewAd: " + Ads.isNewAd);
            // Esc (cancelar ação)
            if (e.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                if (_DialogMessage.Visibility == Visibility.Collapsed)
                {
                    // Anúncios
                    if (Ads._adsUControl.Visibility == Visibility.Visible)
                    {
                        if ((Ads.isEditing && Ads.isNewAd) || (Ads.isEditing && !Ads.isNewAd))
                        {
                            await Ads.CancelAdEdition();
                        }
                        else
                        {
                            Ads.DisableAdComponents();
                        }
                    }

                    // Categorias
                    if (Categories._placesUControl.Visibility == Visibility.Visible)
                    {
                        if ((Categories.isEditing && Categories.isNewCategoryItem) || (Categories.isEditing && !Categories.isNewCategoryItem))
                        {
                            await Categories.CancelCategoryEdition();
                        }
                        else
                        {
                            Categories.DisableCategoryComponents();
                        }
                    }

                    // Roteiros
                    if (Intineraries._intinerariesUControl.Visibility == Visibility.Visible)
                    {
                        if ((Intineraries.isEditing && Intineraries.isNewIntinerarie) || (Intineraries.isEditing && !Intineraries.isNewIntinerarie))
                        {
                            await Intineraries.CancelIntinerarieEdition();
                        }
                        else
                        {
                            Intineraries.DisableIntinerarieComponents();
                        }
                    }

                    // Veículos
                    if (Vehicles._vehiclesUControl.Visibility == Visibility.Visible)
                    {
                        if ((Vehicles.isEditing && Vehicles.isNewVehicle) || (Vehicles.isEditing && !Vehicles.isNewVehicle))
                        {
                            await Vehicles.CancelVehicleEdition();
                        }
                        else
                        {
                            Vehicles.DisableVehicleComponents();
                        }
                    }

                    // FAQs
                    if (FAQs._faqsUControl.Visibility == Visibility.Visible)
                    {
                        if ((FAQs.isEditing && FAQs.isNewFAQ) || (FAQs.isEditing && !FAQs.isNewFAQ))
                        {
                            await FAQs.CancelFAQEdition();
                        }
                        else
                        {
                            FAQs.DisableFAQComponents();
                        }
                    }

                    // Emergência
                    if (Emergency._emergencyUControl.Visibility == Visibility.Visible)
                    {
                        if (Emergency.isEditting)
                        {
                            await Emergency.QuestionTask();
                        }
                    }
                }
                else
                {
                    if (DialogMessage._dialogMessageUControl.btnNo.Visibility == Visibility.Visible)
                    {                        
                        peer = new ButtonAutomationPeer(DialogMessage._dialogMessageUControl.btnNo);
                        IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        invokeProv.Invoke();
                    }
                    else if (DialogMessage._dialogMessageUControl.btnOk.Visibility == Visibility.Visible)
                    {
                        peer = new ButtonAutomationPeer(DialogMessage._dialogMessageUControl.btnOk);
                        IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        invokeProv.Invoke();
                    }
                }
            }

            //debug
            if(e.VirtualKey == Windows.System.VirtualKey.F7)
            {
                //Intineraries._intinerariesUControl.btn_intiCampsParks.IsEnabled = true;
                //Intineraries._intinerariesUControl.btn_intiImages.IsEnabled = true;

                //SQLPlaces _sqlPlaces = new SQLPlaces();
                //_sqlPlaces.LoadPlacesTable(0);
                //ActsAndCamps._ActsAndCamps.lview_dlgActs.ItemsSource = _sqlPlaces.Places;

                //GlobalMethods.SetDBEdited();
            }

        }
    }
}
