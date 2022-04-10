using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepertorizacaoHome.src
{

    [DebuggerDisplay("Name: {Name}")]
    public class Medicine : INotifyPropertyChanged, IEquatable<Medicine>
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

        public enum eIntensitys { Black = 1, Blue, Red}


        private eIntensitys _intensity;
        public eIntensitys Intensity
        {
            get => _intensity;
            set => SetField(ref _intensity, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private uint _score;
        public uint Score
        {
            get => _score;
            set => SetField(ref _score, value);
        }

        private uint _showTimes = 1;
        public uint ShowTimes
        {
            get => _showTimes;
            set => SetField(ref _showTimes, value);
        }

        private string _linkAddress = string.Empty;
        public string LinkAddress
        {
            get => _linkAddress;
            set => SetField(ref _linkAddress, value);
        }

        public Medicine(string name)
        {
            Name = name;
        }

        public bool Equals(Medicine other)
        {
            return Name.ToLower().Equals(other.Name.ToLower());
        }

    }
}
