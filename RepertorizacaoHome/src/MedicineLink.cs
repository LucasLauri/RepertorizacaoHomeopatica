using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RepertorizacaoHome.src
{
    [Serializable]
    public class MedicineLink : INotifyPropertyChanged
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

        private string _link;
        public string Link
        {
            get => _link;
            set => SetField(ref _link, value);
        }


        private string _medicineName;
        public string MedicineName
        {
            get => _medicineName;
            set => SetField(ref _medicineName, value);
        }


        public MedicineLink(string medicineName, string link)
        {
            MedicineName = medicineName;
            Link = link;
        }

        public MedicineLink()
        {

        }
    }
}
