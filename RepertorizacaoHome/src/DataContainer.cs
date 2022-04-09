using RepertorizacaoHome.pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LucasLauriHelpers
{
    [Serializable]
    public class DataContainer : INotifyPropertyChanged
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
        /// Nomed o arquivo onde as configurações são salvas
        /// </summary>
        public string UserSettingsFilename { get; } = "dataContainer.xml";

        private Methods _currentMethod = Methods.Add;
        /// <summary>
        /// <see cref="Methods"/> atual selecionado pelo usuário
        /// </summary>
        public Methods CurrentMethod
        {
            get => _currentMethod;
            set => SetField(ref _currentMethod, value);
        }

        private bool _saveImg;
        /// <summary>
        /// Se a lista deve ser salva em imagem (true) ou em texto (false)
        /// </summary>
        public bool SaveImg
        {
            get => _saveImg;
            set => SetField(ref _saveImg, value);
        }


        private bool _showStatistics = true;
        /// <summary>
        /// Se as mensagens de dados dos métodos serão exibidas
        /// </summary>
        public bool ShowStatistics
        {
            get => _showStatistics;
            set => SetField(ref _showStatistics, value);
        }


        /// <summary>
        /// Caminho da pasta de configurações
        /// </summary>
        [XmlIgnore]
        public string DefaultSettingsPath { get; set; }

        public DataContainer()
        {
            DefaultSettingsPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Settings\\";
        }

        #region Leitura e escrita do DataContainer

        /// <summary>
        /// Carrega os dados do <seealso cref="DataContainer"/> na pasta <seealso cref="DefaultSettingsPath"/>
        /// </summary>
        public bool LoadData(out DataContainer dataContainer)
        {
            dataContainer = null;

            if (!Directory.Exists(DefaultSettingsPath))
                Directory.CreateDirectory(DefaultSettingsPath);

            if (!File.Exists(DefaultSettingsPath + UserSettingsFilename))
                return false;

            XmlSerializer xs = new XmlSerializer(typeof(DataContainer));
            using (StreamReader fs = new StreamReader(DefaultSettingsPath + UserSettingsFilename))
            {
                dataContainer = xs.Deserialize(fs) as DataContainer;

                return true;
            }
        }

        /// <summary>
        /// Salva os dados do <seealso cref="DataContainer"/> na pasta <seealso cref="DefaultSettingsPath"/>
        /// </summary>
        public void SaveData()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataContainer));

            using (StreamWriter fs = new StreamWriter(DefaultSettingsPath + UserSettingsFilename))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }

        #endregion
    }
}
