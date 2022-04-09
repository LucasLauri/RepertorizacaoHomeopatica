using RepertorizacaoHome.src;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RepertorizacaoHome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Program Program { get; set; }


        public MainWindow()
        {
            InitializeComponent();

            Program = new Program();

            DataContext = new { Main = this, Program };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Program.CloseProgram();
        }

        private void BtnAdd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.UpadateMedicines(InputRichTextBox);
        }

        private void BtnClear_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Program.ClearMedicines();
        }

        private void BtnSite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Medicine medicineBuffer = (Medicine)((Grid)sender).DataContext;

            if (!medicineBuffer.LinkAddress.Equals(string.Empty))
                System.Diagnostics.Process.Start(medicineBuffer.LinkAddress);
        }

        private void BtnFilterRecorrencia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.CurrenteFilterMode = Program.FilterMode.Recorrencia;
            Program.UpdatesMedicinesView();
        }

        private void BtnFilterPontos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.CurrenteFilterMode = Program.FilterMode.Pontos;
            Program.UpdatesMedicinesView();
        }

        private void BtnFilterNome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.CurrenteFilterMode = Program.FilterMode.Nome;
            Program.UpdatesMedicinesView();
        }

        private void InputRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //InputRichTextBox.Paste();
            /*

            if (e.Key == Key.LeftCtrl && e.Key== Key.V)
            {
                InputRichTextBox.Document.Blocks.Clear();
                InputRichTextBox.Document.Blocks.Add(new Paragraph(new Run((string)Clipboard.GetText(TextDataFormat.CommaSeparatedValue))));

                e.Handled = true;
            }*/
        }

        private void BtnImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.SaveMedicineList();
        }

        private void BtnHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.HelpPage.HelpVisibility = Visibility.Visible;
        }

        private void BtnConfigs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.ConfigsPage.PageVisibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Program.ProgramLoaded();
        }
    }

    public class LinkToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((string)value).Equals(string.Empty))
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FilterModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Program.FilterMode filterMode = (Program.FilterMode)value;

            if (((int)filterMode).ToString().Equals((string)parameter))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
