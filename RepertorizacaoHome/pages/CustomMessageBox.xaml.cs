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
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Page, INotifyPropertyChanged
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

        private string _message;
        /// <summary>
        /// Mensagem a ser exibida ao usuário
        /// </summary>
        public string Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }


        private Visibility _messageVisibility = Visibility.Collapsed;
        /// <summary>
        /// Visibilidade da mensagem
        /// </summary>
        public Visibility MessageVisibility
        {
            get => _messageVisibility;
            set => SetField(ref _messageVisibility, value);
        }




        public CustomMessageBox()
        {
            InitializeComponent();

            Program.CustomMessageBox = this;

            MessageVisibility = Visibility.Collapsed;
            DataContext = new { MessageBox = this};
        }

        /// <summary>
        /// Exibide mensagem ao usuário
        /// </summary>
        /// <param name="message"></param>
        public void Show(string message)
        {
            Message = message;
            MessageVisibility = Visibility.Visible;
        }

        private void BtnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageVisibility = Visibility.Collapsed;
        }
    }
}
