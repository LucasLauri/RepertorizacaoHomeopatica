using RepertorizacaoHome.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for HelpPage.xaml
    /// </summary>
    public partial class HelpPage : Page, INotifyPropertyChanged
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


        private Visibility _helpVisibility = Visibility.Collapsed;
        /// <summary>
        /// 
        /// </summary>
        public Visibility HelpVisibility
        {
            get => _helpVisibility;
            set => SetField(ref _helpVisibility, value);
        }


        public HelpPage()
        {

            InitializeComponent();


            Program.HelpPage = this;
        }

        private void BtnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HelpVisibility = Visibility.Collapsed;
        }
    }
}
