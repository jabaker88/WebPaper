using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using WebPaper.Utilities;

namespace WebPaper
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Window
    {

        public int PictureTimeout { set; get; }
        public string SearchTerm { set; get; }
        public WinConfigWrapper.Style PictureStyle { set; get; }

        public ControlPanel()
        {
            InitializeComponent();

            SearchTerm = "Wallpaper";
            searchTextBox.Text = "Wallpaper";

            timeoutComboBox.SelectedIndex = 0;
            picPostionComboBox.SelectedIndex = 0;

            /*
             * Reference
            WinConfigWrapper.Style.Centered;
            WinConfigWrapper.Style.Fill;
            WinConfigWrapper.Style.Fit;
            WinConfigWrapper.Style.Span;
            WinConfigWrapper.Style.Stretched;
            WinConfigWrapper.Style.Tiled;
            */

            //Add Pic position items
            picPostionComboBox.Items.Add("Centered");
            picPostionComboBox.Items.Add("Fill");
            picPostionComboBox.Items.Add("Fit");
            picPostionComboBox.Items.Add("Span");
            picPostionComboBox.Items.Add("Stretched");
            picPostionComboBox.Items.Add("Tiled");

            //Timeouts
            timeoutComboBox.Items.Add(30);
            timeoutComboBox.Items.Add(60);
            timeoutComboBox.Items.Add(90);
            timeoutComboBox.Items.Add(120);
            timeoutComboBox.Items.Add(150);
            timeoutComboBox.Items.Add(300);
            timeoutComboBox.Items.Add(900);
        }

        //Search Text changes
        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTerm = searchTextBox.Text;
        }

        //Close Button
        private void CloseButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        //Selection changes on timeout
        private void timeoutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PictureTimeout = (int)((ComboBox)sender).SelectedValue;
        }

        //Selection changes on picture combobox
        private void picPostionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox selection = (ComboBox)sender;

            switch(selection.SelectedValue.ToString())
            {
                case "Centered":
                    PictureStyle = WinConfigWrapper.Style.Centered;
                    break;
                case "Fill":
                    PictureStyle = WinConfigWrapper.Style.Fill;
                    break;
                case "Fit":
                    PictureStyle = WinConfigWrapper.Style.Fit;
                    break;
                case "Span":
                    PictureStyle = WinConfigWrapper.Style.Span;
                    break;
                case "Stretched":
                    PictureStyle = WinConfigWrapper.Style.Stretched;
                    break;
                case "Tiled":
                    PictureStyle = WinConfigWrapper.Style.Tiled;
                    break;
                default:
                    PictureStyle = WinConfigWrapper.Style.Centered;
                    break;
            }
        }

        //Control is deactivated
        private void ControlPanelWindow_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
