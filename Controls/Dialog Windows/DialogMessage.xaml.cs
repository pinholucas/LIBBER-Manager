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
using Windows.UI.Text;
using Windows.UI.Xaml.Media.Animation;

namespace Libber_Manager.Controls
{
    public enum DLGAction
    {
        Yes,
        No,
        Ok,
        Cancel,
        Nothing
    }

    public enum DLGWType
    {
        Wait,
        Alert,
        Question,
        QuestionWithCancel,
        View
    }

    public sealed partial class DialogMessage : UserControl
    {
        public static DialogMessage _dialogMessageUControl;

        public static DLGAction Result { get; set; }

        public static bool isPaused = false;

        public DialogMessage()
        {
            this.InitializeComponent();
            _dialogMessageUControl = this;

            Result = DLGAction.Nothing;
        }        

        public async static Task ShowDialog(DLGWType wType, string wTitle, string wContent)
        {
            // Dialogo de Mensagem
            MainPage.showMessageDLG();
            VisualGraphics.doBlurOnWindow(MainPage._mainPage.mainGrid);
            _dialogMessageUControl.Focus(FocusState.Programmatic);

            _dialogMessageUControl.txtBlock_msgType.Text = wTitle;

            _dialogMessageUControl.txtBlock_msgContent.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.btnOk.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.btnCancel.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.btnCancel.SetValue(Grid.ColumnSpanProperty, 2);
            _dialogMessageUControl.btnNo.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.btnNo.SetValue(Grid.ColumnSpanProperty, 1);
            _dialogMessageUControl.btnYes.Visibility = Visibility.Collapsed;

            // Controls
            _dialogMessageUControl.CampsParksControl.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.IntinerarieImagesControl.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.VehicleAdditionalsControl.Visibility = Visibility.Collapsed;
            _dialogMessageUControl.VehicleImagesControl.Visibility = Visibility.Collapsed;

            switch (wType)
            {
                case DLGWType.Wait:
                    _dialogMessageUControl.txtBlock_msgContent.Document.SetText(TextSetOptions.FormatRtf, @"{\rtf1\ansi " + wContent + "}");
                    _dialogMessageUControl.txtBlock_msgContent.Visibility = Visibility.Visible;
                    break;

                case DLGWType.Alert:
                    _dialogMessageUControl.txtBlock_msgContent.Document.SetText(TextSetOptions.FormatRtf, @"{\rtf1\ansi " + wContent + "}");
                    _dialogMessageUControl.txtBlock_msgContent.Visibility = Visibility.Visible;

                    _dialogMessageUControl.btnOk.Visibility = Visibility.Visible;
                    GlobalMethods.SetFocus(_dialogMessageUControl.btnOk);
                    break;

                case DLGWType.Question:
                    _dialogMessageUControl.txtBlock_msgContent.Document.SetText(TextSetOptions.FormatRtf, @"{\rtf1\ansi " + wContent + "}");
                    _dialogMessageUControl.txtBlock_msgContent.Visibility = Visibility.Visible;

                    _dialogMessageUControl.btnNo.Visibility = Visibility.Visible;
                    _dialogMessageUControl.btnYes.Visibility = Visibility.Visible;
                    GlobalMethods.SetFocus(_dialogMessageUControl.btnYes);

                    while (isPaused == true)
                    {
                        await Task.Delay(50);  //set some timeout before check if task is stopped
                    };
                    break;

                case DLGWType.QuestionWithCancel:
                    _dialogMessageUControl.txtBlock_msgContent.Document.SetText(TextSetOptions.FormatRtf, @"{\rtf1\ansi " + wContent + "}");
                    _dialogMessageUControl.txtBlock_msgContent.Visibility = Visibility.Visible;

                    _dialogMessageUControl.btnCancel.Visibility = Visibility.Visible;
                    _dialogMessageUControl.btnCancel.SetValue(Grid.ColumnSpanProperty, 1);
                    _dialogMessageUControl.btnNo.Visibility = Visibility.Visible;
                    _dialogMessageUControl.btnNo.SetValue(Grid.ColumnSpanProperty, 2);
                    _dialogMessageUControl.btnYes.Visibility = Visibility.Visible;
                    GlobalMethods.SetFocus(_dialogMessageUControl.btnYes);

                    while (isPaused == true)
                    {
                        await Task.Delay(50);  //set some timeout before check if task is stopped
                    };
                    break;

                case DLGWType.View:   
                    _dialogMessageUControl.btnOk.Visibility = Visibility.Visible;
                    GlobalMethods.SetFocus(_dialogMessageUControl.btnOk);
                    break;
            }
        }

        public static void CloseDialog()
        {
            MainPage.hideMessageDLG();
            VisualGraphics.removeBlurOnWindow(MainPage._mainPage.mainGrid);

            // Wait, Alert e Question
            _dialogMessageUControl.txtBlock_msgContent.Document.SetText(TextSetOptions.FormatRtf, "");
            isPaused = false;

            // Zera os ListView do IntinerarieImages para que a próxima utilização não contenha resquícios da anterior
            if (_dialogMessageUControl.IntinerarieImagesControl.Visibility == Visibility.Visible)
            {
                IntinerarieImages.DisableIntiPComponents();
                IntinerarieImages._IntinerarieImagesUControl.lview_intiImages.ItemsSource = null;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = DLGAction.Ok;     
            CloseDialog();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = DLGAction.Yes;
            CloseDialog();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = DLGAction.No;
            CloseDialog();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = DLGAction.Cancel;
            CloseDialog();
        }
    }
}
