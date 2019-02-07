using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Data.Sqlite;
using Windows.Storage.AccessCache;
using Libber_Manager.Helpers;

namespace Libber_Manager.Controls
{
    public sealed partial class NavigationMenu : UserControl
    {
        public static NavigationMenu _navigationMenuUControl;

        SolidColorBrush bgnormalColor = new SolidColorBrush(Color.FromArgb(255, 47, 50, 67));
        SolidColorBrush bgactiveColor = new SolidColorBrush(Color.FromArgb(255, 35, 40, 50));

        SolidColorBrush fgnormalColor = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
        SolidColorBrush fgactiveColor = new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));        

        public NavigationMenu()
        {
            this.InitializeComponent();
            _navigationMenuUControl = this;

            ////'this' is MainPage, but can be any UIElement
            //var visual = ElementCompositionPreview.GetElementVisual(navGrid);
            //var brush = visual.Compositor.CreateHostBackdropBrush();
            //var sprite = visual.Compositor.CreateSpriteVisual();
            //sprite.Brush = brush;
            ////Set to the size of the area, update on SizeChanged
            //sprite.Size = new System.Numerics.Vector2(100, 100);
            //ElementCompositionPreview.SetElementChildVisual(navGrid, sprite);



            //applyAcrylicAccent(MainGrid2);
        }

        //Compositor _compositor;
        //SpriteVisual _hostSprite;
        //private void applyAcrylicAccent(Panel e)
        //{
        //    _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        //    _hostSprite = _compositor.CreateSpriteVisual();
        //    _hostSprite.Size = new Vector2((float)MainGrid2.ActualWidth, (float)MainGrid2.ActualHeight);

        //    ElementCompositionPreview.SetElementChildVisual(
        //            MainGrid2, _hostSprite);
        //    _hostSprite.Brush = _compositor.CreateHostBackdropBrush();
        //}

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (_hostSprite != null)
            //    _hostSprite.Size = e.NewSize.ToVector2();
        }

        public void changeCategory(int button)
        {
            switch (button)
            {
                case 1:
                    resetButtonsState();
                    btn_Ads.Background = bgactiveColor;
                    btn_Ads.Foreground = fgactiveColor;

                    MainPage.hideMainControls();
                    Ads._adsUControl.Visibility = Visibility.Visible;               
                    break;
                case 2:
                    resetButtonsState();
                    btn_Categories.Background = bgactiveColor;
                    btn_Categories.Foreground = fgactiveColor;

                    MainPage.hideMainControls();
                    Categories._placesUControl.Visibility = Visibility.Visible;
                    break;
                case 3:
                    resetButtonsState();
                    btn_Intineraries.Background = bgactiveColor;
                    btn_Intineraries.Foreground = fgactiveColor;

                    MainPage.hideMainControls();
                    Intineraries._intinerariesUControl.Visibility = Visibility.Visible;
                    break;
                case 4:
                    resetButtonsState();
                    btn_Vehicles.Background = bgactiveColor;
                    btn_Vehicles.Foreground = fgactiveColor;

                    MainPage.hideMainControls();
                    Vehicles._vehiclesUControl.Visibility = Visibility.Visible;
                    break;
                case 5:
                    resetButtonsState();
                    btn_FAQs.Background = bgactiveColor;
                    btn_FAQs.Foreground = fgactiveColor;

                    MainPage.hideMainControls();
                    FAQs._faqsUControl.Visibility = Visibility.Visible;
                    break;
                case 6:
                    resetButtonsState();
                    btn_Emergency.Background = bgactiveColor;
                    btn_Emergency.Foreground = fgactiveColor;

                    MainPage.hideMainControls();
                    Emergency._emergencyUControl.Visibility = Visibility.Visible;
                    break;
            }
        }

        public void resetButtonsState()
        {
            btn_SaveDB.Visibility = Visibility.Visible;
            btn_SaveAsDB.Visibility = Visibility.Visible;

            btn_Ads.Background = bgnormalColor;
            btn_Categories.Background = bgnormalColor;
            btn_Intineraries.Background = bgnormalColor;
            btn_Vehicles.Background = bgnormalColor;
            btn_FAQs.Background = bgnormalColor;
            btn_Emergency.Background = bgnormalColor;

            btn_Ads.Foreground = fgnormalColor;
            btn_Categories.Foreground = fgnormalColor;
            btn_Intineraries.Foreground = fgnormalColor;
            btn_Vehicles.Foreground = fgnormalColor;
            btn_FAQs.Foreground = fgnormalColor;
            btn_Emergency.Foreground = fgnormalColor;
        }

        private void btn_Menu_Click(object sender, RoutedEventArgs e)
        {
            if (MainPage._mainPage.c1.Width == new GridLength(250))
            {
                MainPage._mainPage.c1.Width = new GridLength(51, GridUnitType.Pixel);
                textBlock.Visibility = Visibility.Collapsed;
                textBlock2.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainPage._mainPage.c1.Width = new GridLength(250, GridUnitType.Pixel);
                textBlock.Visibility = Visibility.Visible;
                textBlock2.Visibility = Visibility.Visible;
            }
        }

        private async void btn_NewDB_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalMethods.isDBEditted)
            {
                DialogMessage.isPaused = true;
                await DialogMessage.ShowDialog(DLGWType.QuestionWithCancel, "ATENÇÃO", @"\b0 DESEJA SALVAR AS ALTERAÇÕES EM \b " + GlobalMethods.dbFileName.ToUpper() + @"\b0 ?");

                if (DialogMessage.Result == DLGAction.Yes)
                {
                    GlobalMethods.doSQLCreate = true;
                    await DBFileManagement.SaveDBFile();
                }
                else if (DialogMessage.Result == DLGAction.No)
                {
                    await SQLBasics.SQLCreate();
                }
            }
            else
            {
                await SQLBasics.SQLCreate();
                //SQLBasics.isOpen = true;
            }
        }

        private async void btn_OpenDB_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalMethods.isDBEditted)
            {
                DialogMessage.isPaused = true;
                await DialogMessage.ShowDialog(DLGWType.QuestionWithCancel, "ATENÇÃO", @"\b0 DESEJA SALVAR AS ALTERAÇÕES EM \b " + GlobalMethods.dbFileName.ToUpper() + @"\b0 ?");

                if (DialogMessage.Result == DLGAction.Yes)
                {
                    GlobalMethods.doSQLOpen = true;
                    await DBFileManagement.SaveDBFile();                    
                }
                else if (DialogMessage.Result == DLGAction.No)
                {
                    await SQLBasics.SQLOpen();
                }
            }
            else
            {
                await SQLBasics.SQLOpen();
            }
        }

    private async void btn_SaveDB_Click(object sender, RoutedEventArgs e)
        {
            await DBFileManagement.SaveDBFile();
        }

        private async void btn_SaveAsDB_Click(object sender, RoutedEventArgs e)
        {
            
            await DBFileManagement.SaveDBFileAs();
        }

        private void btn_Ads_Click(object sender, RoutedEventArgs e)
        {
            changeCategory(1);

            Ads.LoadAds();
        }

        private void btn_Categories_Click(object sender, RoutedEventArgs e)
        {
            changeCategory(2);

            Categories.LoadCategory();
        }

        private void btn_Intineraries_Click(object sender, RoutedEventArgs e)
        {
            changeCategory(3);

            Intineraries.LoadIntineraries();
        }

        private void btn_Vehicles_Click(object sender, RoutedEventArgs e)
        {
            changeCategory(4);

            Vehicles.LoadVehicles();
        }

        private void btn_FAQs_Click(object sender, RoutedEventArgs e)
        {
            changeCategory(5);

            FAQs.LoadFAQs();
        }

        private void btn_Emergency_Click(object sender, RoutedEventArgs e)
        {
            changeCategory(6);

            Emergency.LoadEmergency();
        }

        private async void btn_About_Click(object sender, RoutedEventArgs e)
        {
            await DialogMessage.ShowDialog(DLGWType.Alert, "SOBRE", @"\b Libber Manager\b0 \par Versão RC 10 \par Desenvolvido por Lucas Pinho B. Santos \par www.lucaspinho.com \par\par Copyright © 2016-2017");
        }        
    }
}
