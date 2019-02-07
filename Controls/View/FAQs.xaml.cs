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
using Windows.UI;
using Windows.System;

namespace Libber_Manager.Controls
{
    public sealed partial class FAQs : UserControl
    {
        public static FAQs _faqsUControl;

        public FAQs()
        {
            this.InitializeComponent();
            _faqsUControl = this;
        }

        public static bool isNewFAQ = false;
        public static bool isEditing = false;     

        int lvSelectedIndex = -1;
        int lvNewSelectedIndex = -1;

        int selectionID;
        string originalTitle;
        string originalContent;

        public static void LoadFAQs()
        {
            SQLFaqs.LoadFAQsTable();
            _faqsUControl.lview_faqs.ItemsSource = SQLFaqs.FAQs;
        }

        private void EnableFAQComponents()
        {
            txtBox_faqTitle.IsEnabled = true;
            txtBox_faqContent.IsEnabled = true;
        }
        public static void DisableFAQComponents()
        {
            EditionFinished();
            _faqsUControl.originalTitle = "";
            _faqsUControl.originalContent = "";

            _faqsUControl.selectionID = -1;

            _faqsUControl.lview_faqs.SelectedIndex = -1;
            _faqsUControl.lview_faqs.IsEnabled = true;
            _faqsUControl.lview_faqs.SelectedItem = null;

            _faqsUControl.txtBox_faqTitle.Text = "";
            _faqsUControl.txtBox_faqTitle.IsEnabled = false;
            _faqsUControl.txtBox_faqContent.Text = "";
            _faqsUControl.txtBox_faqContent.IsEnabled = false;

            _faqsUControl.btn_faqApply.IsEnabled = false;
            _faqsUControl.btn_faqApply.Content = "APLICAR";

            _faqsUControl.btn_faqCreate.Content = "NOVA FAQ";
            _faqsUControl.btn_faqCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void ClearFAQComponents()
        {
            isNewFAQ = true;

            _faqsUControl.lvSelectedIndex = -1;
            _faqsUControl.lvNewSelectedIndex = -1;
            selectionID = -1;

            lview_faqs.SelectedItem = null;
            lview_faqs.IsEnabled = false;            

            txtBox_faqTitle.Text = "";
            txtBox_faqTitle.IsEnabled = true;
            txtBox_faqContent.Text = "";
            txtBox_faqContent.IsEnabled = true;

            btn_faqApply.IsEnabled = false;
            btn_faqApply.Content = "CRIAR";

            _faqsUControl.btn_faqCreate.Content = "CANCELAR";
            _faqsUControl.btn_faqCreate.Background = new SolidColorBrush(Colors.DarkRed);
        }

        private void Edited(bool isEdited)
        {
            if (isEdited)
            {
                isEditing = true;
                btn_faqApply.IsEnabled = true;

                btn_faqCreate.Content = "CANCELAR";
                btn_faqCreate.Background = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                isEditing = false;
                btn_faqApply.IsEnabled = false;

                if (!isNewFAQ)
                {
                    btn_faqCreate.Content = "NOVA FAQ";
                    btn_faqCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                }
            }
        }

        public static void EditionFinished()
        {
            isNewFAQ = false;
            isEditing = false;            

            _faqsUControl.btn_faqApply.IsEnabled = false;
            _faqsUControl.btn_faqCreate.IsEnabled = true;

            _faqsUControl.btn_faqCreate.Content = "NOVA FAQ";
            _faqsUControl.btn_faqCreate.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        public async static Task QuestionAlterations()
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 SUAS ALTERAÇÕES \b NÃO \b0 FORAM SALVAS, DESEJA CONTINUAR?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                DisableFAQComponents();

                if (_faqsUControl.lvNewSelectedIndex != _faqsUControl.lvSelectedIndex) 
                {
                    if (_faqsUControl.lvNewSelectedIndex <= _faqsUControl.lview_faqs.Items.Count)
                    {
                        _faqsUControl.lview_faqs.SelectedIndex = _faqsUControl.lvNewSelectedIndex;
                        _faqsUControl.lview_faqs.SelectedItem = _faqsUControl.lvNewSelectedIndex;                     
                    }
                }
                else //criação de faq
                {
                    _faqsUControl.lview_faqs.SelectedIndex = _faqsUControl.lvSelectedIndex; 
                    _faqsUControl.lview_faqs.SelectedItem = _faqsUControl.lvSelectedIndex;
                }
            }
        }

        public async static Task QuestionDelete(string itemName)
        {
            DialogMessage.isPaused = true;

            await DialogMessage.ShowDialog(DLGWType.Question, "ATENÇÃO", @"\b0 DELETAR O ITEM \b'" + itemName + @"'\b0  PERMANENTEMENTE?");

            if (DialogMessage.Result == DLGAction.Yes)
            {
                SQLFaqs.RemoveFAQ(_faqsUControl.selectionID);
                DisableFAQComponents();
                LoadFAQs();
            }
        }

        private void CheckFields()
        {
            if (txtBox_faqTitle.Text != originalTitle | txtBox_faqContent.Text != originalContent)
            {
                Edited(true);
            }
            else
            {
                Edited(false);
            }
        }

        public static async Task CancelFAQEdition()
        {
            if (isEditing == false && isNewFAQ == true)
            {
                DisableFAQComponents();
            }
            else
            {
                await QuestionAlterations();
            }
        }

        private async void lview_faqs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lview_faqs.SelectedIndex > -1 && lview_faqs.SelectedItem != null)
            {
                if (isEditing)
                {
                    if (lvSelectedIndex != lview_faqs.SelectedIndex)
                    {
                        lvNewSelectedIndex = lview_faqs.SelectedIndex;
                    }

                    lview_faqs.SelectedIndex = lvSelectedIndex;
                    lview_faqs.SelectedItem = lvSelectedIndex;

                    await QuestionAlterations();
                }
                else
                {
                    EnableFAQComponents();

                    lvSelectedIndex = lview_faqs.SelectedIndex;
                    lvNewSelectedIndex = lview_faqs.SelectedIndex;

                    SQLFaqs.faqs faq = lview_faqs.SelectedItem as SQLFaqs.faqs;

                    selectionID = faq.id;
                    originalTitle = faq.title;
                    originalContent = faq.content;

                    txtBox_faqTitle.Text = faq.title;
                    txtBox_faqContent.Text = faq.content;
                }
            }
        }

        private async void lview_faqs_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Delete)
            {
                SQLFaqs.faqs faq = lview_faqs.SelectedItem as SQLFaqs.faqs;

                await QuestionDelete(faq.title.ToUpper());
            }
        }

        private async void btn_faqCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!isNewFAQ && !isEditing)
            {
                ClearFAQComponents();
                _faqsUControl.originalTitle = "";
                _faqsUControl.originalContent = "";

                GlobalMethods.SetFocus(txtBox_faqTitle);
            }
            else
            {
                await CancelFAQEdition();
            }
        }

        private async void btn_faqApply_Click(object sender, RoutedEventArgs e)
        {
            if (isNewFAQ == false) //Edição de FAQ
            {
                if (txtBox_faqTitle.Text != "" & txtBox_faqContent.Text != "")
                {
                    SQLFaqs.UpdateFAQ(selectionID,txtBox_faqTitle.Text, txtBox_faqContent.Text);

                    EditionFinished();
                    LoadFAQs();
                    _faqsUControl.lview_faqs.SelectedIndex = lvSelectedIndex;

                    GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
            else //Criação de FAQ
            {
                if (txtBox_faqTitle.Text != "" & txtBox_faqContent.Text != "")
                {
                    SQLFaqs.CreateFAQ(txtBox_faqTitle.Text, txtBox_faqContent.Text);
                    btn_faqApply.Content = "APLICAR";

                    EditionFinished();
                    LoadFAQs();
                    DisableFAQComponents();

                    GlobalMethods.SetDBEdited(); // Altera o status da DB para 'alterações não salvas'
                }
                else
                {
                    await DialogMessage.ShowDialog(DLGWType.Alert, "ATENÇÃO", @"\b0 TODOS OS CAMPOS DEVEM SER PREENCHIDOS!");
                }
            }
        }       

        private void txtBox_faqTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }

        private void txtBox_faqContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckFields();
        }
    }
}
