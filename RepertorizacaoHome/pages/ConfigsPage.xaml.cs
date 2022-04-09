using RepertorizacaoHome.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace RepertorizacaoHome.pages
{
    /// <summary>
    /// Possíveis métodos de repertorização
    /// </summary>
    public enum Methods { Add, Symptoms, Elimination };

    /// <summary>
    /// Interaction logic for ConfigsPage.xaml
    /// </summary>
    public partial class ConfigsPage : Page, INotifyPropertyChanged
    {
        #region Eventos PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        /// <summary>
        /// Atualiza campo gerando evento para GUI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">Campo a ser atualizado</param>
        /// <param name="value">Novo valor do campo</param>
        /// <param name="propertyName"></param>
        protected void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            OnPropertyChanged(propertyName);
        }

        #endregion

        public Program Program { get; set; } = ((MainWindow)Application.Current.MainWindow).Program;

        private Visibility _pageVisibility = Visibility.Collapsed;
        /// <summary>
        /// Visibilidade desta pagina
        /// </summary>
        public Visibility PageVisibility
        {
            get => _pageVisibility;
            set => SetField(ref _pageVisibility, value);
        }

        
        private string _currenteVersion;
        /// <summary>
        /// Versão atual do programa
        /// </summary>
        public string CurrenteVersion
        {
            get => _currenteVersion;
            set => SetField(ref _currenteVersion, value);
        }


        public ConfigsPage()
        {
            InitializeComponent();

            Program.ConfigsPage = this;

            DataContext = new { Program, Configs = this };

            try
            {
                CurrenteVersion = "V: " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch
            {
                CurrenteVersion = "";
            }
        }

        private void BtnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PageVisibility = Visibility.Collapsed;
        }
    }

    public class MethodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : Binding.DoNothing;
        }
    }
}
