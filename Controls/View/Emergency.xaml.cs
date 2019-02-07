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

namespace Libber_Manager.Controls
{
    public sealed partial class Emergency : UserControl
    {
        public static Emergency _emergencyUControl;

        public Emergency()
        {
            this.InitializeComponent();
            _emergencyUControl = this;
        }

        static string originalPhone = null;
        static string originalSms = null;
        static string originalEmail = null;

        public static bool isEditting = false;

        public static void LoadEmergency()
        {
            SQLEmergency.LoadEmergencyTable();

            if (SQLEmergency.phone != null && SQLEmergency.sms != null && SQLEmergency.email != null)
            {
                _emergencyUControl.txtBox_phoneNumber.Text = SQLEmergency.phone;
                _emergencyUControl.txtBox_smsNumber.Text = SQLEmergency.sms;
                _emergencyUControl.txtBox_email.Text = SQLEmergency.email;

                originalPhone = SQLEmergency.phone;
                originalSms = SQLEmergency.sms;
                originalEmail = SQLEmergency.email;
            }
        }

        private void CheckFields()
        {
            if (txtBox_phoneNumber.Text != originalPhone |
                txtBox_smsNumber.Text != originalSms |
                txtBox_email.Text != originalEmail)
            {
                isEditting = true;
                btn_emergencyApply.IsEnabled = true;
            }
            else
            {
                isEditting = false;
                btn_emergencyApply.IsEnabled = false;
            }
        }

        public async static Task QuestionTask()
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 SUAS ALTERAÇÕES \b NÃO \b0 FORAM SALVAS, DESEJA CONTINUAR?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                _emergencyUControl.txtBox_phoneNumber.Text = originalPhone;
                _emergencyUControl.txtBox_smsNumber.Text = originalSms;
                _emergencyUControl.txtBox_email.Text = originalEmail;

                _emergencyUControl.btn_emergencyApply.IsEnabled = false;
            }
        }

        private async void btn_emergencyApply_Click(object sender, RoutedEventArgs e)
        {
            if (txtBox_phoneNumber.Text != "" && txtBox_smsNumber.Text != "" && txtBox_email.Text != "")
            {
                SQLEmergency.UpdateEmergencyTable(txtBox_phoneNumber.Text, txtBox_smsNumber.Text, txtBox_email.Text);

                originalPhone = txtBox_phoneNumber.Text;
                originalSms = txtBox_smsNumber.Text;
                originalEmail = txtBox_email.Text;

                isEditting = false;

                btn_emergencyApply.IsEnabled = false;

                GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
            }
            else
            {
                await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
            }
        }

        private void txtBox_phoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_smsNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_email_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }        
    }
}
