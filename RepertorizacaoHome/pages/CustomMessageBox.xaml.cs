using RepertorizacaoHome.src;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Tipos de mensagens que podem ser exibidas
        /// </summary>
        public enum MessageType { Ok, YesNo };

        /// <summary>
        /// Tipos de respostas do usuário
        /// </summary>
        public enum MessageRetuns { None, Ok, Yes, No }

        public Program Program { get; set; } = ((MainWindow)Application.Current.MainWindow).Program;

        /// <summary>
        /// Visibilidade dos botões da mensagem
        /// </summary>
        public ObservableCollection<Visibility> TypesVisilibitys { get; set; } = new ObservableCollection<Visibility>();

        /// <summary>
        /// Retorno desta mensagem
        /// </summary>
        public MessageRetuns CurrentReturn { get; set; } = MessageRetuns.None;

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

            for (int i = 0; i < Enum.GetNames(typeof(MessageType)).Length; i++)
            {
                TypesVisilibitys.Add(Visibility.Collapsed);
            }

            TypesVisilibitys[0] = Visibility.Visible;


            Program.CustomMessageBox = this;

            MessageVisibility = Visibility.Collapsed;
            DataContext = new { MessageBox = this };
        }

        /// <summary>
        /// Exibide mensagem ao usuário
        /// </summary>
        /// <param name="message"></param>
        public async Task<MessageRetuns> Show(string message, MessageType type = MessageType.Ok)
        {
            CurrentReturn = MessageRetuns.None;

            Message = message;

            switch (type)
            {
                case MessageType.Ok:
                    TypesVisilibitys[0] = Visibility.Visible;
                    TypesVisilibitys[1] = Visibility.Collapsed;
                    break;
                case MessageType.YesNo:
                    TypesVisilibitys[0] = Visibility.Collapsed;
                    TypesVisilibitys[1] = Visibility.Visible;
                    break;
            }

            MessageVisibility = Visibility.Visible;

            while (CurrentReturn == MessageRetuns.None)
            {
                await Task.Delay(100);
            }


            MessageVisibility = Visibility.Collapsed;

            return CurrentReturn;
        }

        private void BtnOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentReturn = MessageRetuns.Ok;
        }

        private void BtnYes_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentReturn = MessageRetuns.Yes;
        }

        private void BtnNo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentReturn = MessageRetuns.No;
        }
    }
}
