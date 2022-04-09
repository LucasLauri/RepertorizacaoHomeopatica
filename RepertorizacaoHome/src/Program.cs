using LucasLauriHelpers;
using MarkupConverter;
using RepertorizacaoHome.pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace RepertorizacaoHome.src
{
    public class Program : INotifyPropertyChanged
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

        public enum FilterMode { Nome, Pontos, Recorrencia }

        //public static readonly string MainPath = @"C:\RepertorizacaoHomeopatica";
        //public static readonly string LinksFiles = @"C:\RepertorizacaoHomeopatica\medLinks.xml";

        public MainWindow MainWindow { get; set; } = (MainWindow)Application.Current.MainWindow;

        /// <summary>
        /// Lista de medicamentos atualmente em análise
        /// </summary>
        public List<Medicine> Medicines { get; set; } = new List<Medicine>();

        public HelpPage HelpPage { get; set; }
        public ConfigsPage ConfigsPage { get; set; }
        public CustomMessageBox CustomMessageBox { get; set; }

        public DataContainer DataContainer { get; set; }

        private ICollectionView _medicinesView;
        /// <summary>
        /// View da lista <see cref="Medicines"/>
        /// </summary>
        public ICollectionView MedicinesView
        {
            get => _medicinesView;
            set => SetField(ref _medicinesView, value);
        }

        private FilterMode _currenteFilterMode = FilterMode.Pontos;
        /// <summary>
        /// Filtro atual da lista (ordem de organização)
        /// </summary>
        public FilterMode CurrenteFilterMode
        {
            get => _currenteFilterMode;
            set => SetField(ref _currenteFilterMode, value);
        }

        private Visibility _busyOverlay = Visibility.Collapsed;
        /// <summary>
        /// Visibilidade da mensagem de "trabalhando" na interpretação do texto copiado pelo usuário
        /// </summary>
        public Visibility BusyOverlay
        {
            get => _busyOverlay;
            set => SetField(ref _busyOverlay, value);
        }


        IMarkupConverter markupConverter = new MarkupConverter.MarkupConverter();

        public Program()
        {

            //System.Diagnostics.Debugger.Launch();

            //Directory.CreateDirectory(MainPath);

            DataContainer = new DataContainer();

            if (DataContainer.LoadData(out DataContainer newData))
            {
                DataContainer = newData;
            }

            //LoadLinks();
        }

        /// <summary>
        /// Método que é executado ao abrir o programa
        /// </summary>
        public void ProgramLoaded()
        {
            Task.Factory.StartNew(async () => 
            { 
                await Task.Delay(2000);
                CheckAndInstallUpdates();
            });
        }

        /// <summary>
        /// Método que é executado ao fechar o programa
        /// </summary>
        public void CloseProgram()
        {
            DataContainer.SaveData();
            //SaveLinks();
        }

        public void UpadateMedicines(RichTextBox richTextBox)
        {
            BusyOverlay = Visibility.Visible;

            Task.Factory.StartNew(async () =>
            {

                BusyOverlay = Visibility.Visible;

                await Task.Delay(500);

                MainWindow.Dispatcher.Invoke(() =>
                {
                    string clipBoardText = Clipboard.GetText(TextDataFormat.Html);

                    if (clipBoardText.Length > 0)
                    {
                        using (var stream = GenerateStreamFromString(ConvertHtmlToRtf(clipBoardText)))
                        {
                            richTextBox.SelectAll();
                            richTextBox.Selection.Load(stream, DataFormats.Rtf);
                        }
                    }
                    else
                    {
                        clipBoardText = Clipboard.GetText(TextDataFormat.Rtf);
                        if (clipBoardText.Length > 0)
                        {
                            using (var stream = GenerateStreamFromString(clipBoardText))
                            {
                                richTextBox.SelectAll();
                                richTextBox.Selection.Load(stream, DataFormats.Rtf);
                            }
                        }
                    }

                    if(clipBoardText.Length == 0)
                    {
                        BusyOverlay = Visibility.Collapsed;
                        CustomMessageBox.Show($"Infelizmente texto copiado (Ctrl+C) não esta em uma codificação reconhecida por este programa.{Environment.NewLine}Favor tentar novamente em versões futuras.");
                        return;
                    }

                    string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
                    text = string.Join(" ", Regex.Split(text, @"(?:\r\n|\n|\r)"));

                    if (text.Trim().Equals(""))
                    {
                        BusyOverlay = Visibility.Collapsed;
                        CustomMessageBox.Show("Não foi possível encontrar nenhum medicamento no texto copiado (Ctrl+C)");
                        return;
                    }

                    List<Medicine> newMedicines = new List<Medicine>();

                    string[] medicineSplit = text.Split(',');

                    for (int i = 0; i < medicineSplit.Length; i++)
                        newMedicines.Add(new Medicine(medicineSplit[i].Trim()));

                    TextSelection textSelection = richTextBox.Selection;
                    TextPointer start = richTextBox.Document.ContentStart;

                    TextPointer startPoint = start;

                    TextPointer textPointer = startPoint.GetNextContextPosition(LogicalDirection.Forward);

                    while (textPointer != null)
                    {
                        textSelection.Select(startPoint, textPointer);

                        if (textSelection.Text.Trim() != "")
                        {
                            Brush fontColor = (Brush)textSelection.GetPropertyValue(Control.ForegroundProperty);
                            FontStyle fontStyle = (FontStyle)textSelection.GetPropertyValue(Control.FontStyleProperty);
                            FontWeight fontWeight = (FontWeight)textSelection.GetPropertyValue(Control.FontWeightProperty);
                            
                            string[] textSplitB = textSelection.Text.Replace(",", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty).Trim().Split('.');

                            for (int i = 0; i < textSplitB.Length; i++)
                            {
                                string textTarget = textSplitB[i];

                                foreach (Medicine medicine in newMedicines)
                                {
                                    if ((textTarget.ToLower() + ".").Equals(medicine.Name.ToLower()))
                                    {
                                        if(fontColor.ToString().Equals("#FF000000") && fontWeight != FontWeights.Bold && fontStyle != FontStyles.Italic)
                                        {
                                            if (medicine.Intensity < Medicine.eIntensitys.Black)
                                                medicine.Intensity = Medicine.eIntensitys.Black;

                                            medicine.Score += (uint)Medicine.eIntensitys.Black;
                                        }
                                        else if (fontColor.ToString().Equals("#FFFF0000") || fontWeight == FontWeights.Bold)
                                        {
                                            if (medicine.Intensity < Medicine.eIntensitys.Red)
                                                medicine.Intensity = Medicine.eIntensitys.Red;

                                            medicine.Score += (uint)Medicine.eIntensitys.Red;
                                        }
                                        else if (fontColor.ToString().Equals("#FF0000FF") || fontStyle == FontStyles.Italic)
                                        {
                                            if (medicine.Intensity < Medicine.eIntensitys.Blue)
                                                medicine.Intensity = Medicine.eIntensitys.Blue;

                                            medicine.Score += (uint)Medicine.eIntensitys.Blue;
                                        }
                                    }
                                }
                            }
                        }

                        startPoint = textPointer;

                        textPointer = startPoint.GetNextContextPosition(LogicalDirection.Forward);
                    }

                    RunMedicinesMethod(newMedicines);

                    UpdatesMedicinesView();

                    ShowMethodStatistics();

                    richTextBox.Document.Blocks.Clear();
                });

                BusyOverlay = Visibility.Collapsed;
            });
        }

        #region Methods - HTHML to RTF related

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private string ConvertHtmlToRtf(string rtfText)
        {
            var thread = new Thread((object rtf) => 
            {
                var data = rtf as ConvertRtfThreadData;
                data.HtmlText = markupConverter.ConvertHtmlToRtf(data.RtfText);
            });
            var threadData = new ConvertRtfThreadData { RtfText = rtfText };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(threadData);
            thread.Join();
            return threadData.HtmlText;
        }


        private class ConvertRtfThreadData
        {
            public string RtfText { get; set; }
            public string HtmlText { get; set; }
        }

        #endregion

        /// <summary>
        /// Executa o método selecionado em <see cref="DataContainer.CurrentMethod"/> nos medicamentos informados
        /// </summary>
        /// <param name="newMedicines"></param>
        private void RunMedicinesMethod(List<Medicine> newMedicines)
        {
            bool rangeAdded = false;

            foreach(Medicine newMedicine in newMedicines)
            {
                switch (DataContainer.CurrentMethod)
                {
                    case Methods.Add:
                        RunAddOrEliminationMethod(newMedicine);
                        break;
                    case Methods.Symptoms:

                        if (Medicines.Count == 0)
                        {
                            Medicines.AddRange(newMedicines);
                            rangeAdded = true;
                        }
                        else
                        {
                            if (Medicines.Contains(newMedicine))
                            {
                                Medicine medicine = GetMedicineByName(newMedicine.Name);
                                medicine.ShowTimes += newMedicine.ShowTimes;
                                medicine.Score += newMedicine.Score;
                            }
                        }
                        break;
                    case Methods.Elimination:
                        RunAddOrEliminationMethod(newMedicine);
                        break;
                }

                if (rangeAdded)
                    break;
            }
        }

        /// <summary>
        /// Executa o método de adição ou elimintação
        /// </summary>
        private void RunAddOrEliminationMethod(Medicine newMedicine)
        {
            if (Medicines.Contains(newMedicine))
            {
                Medicine medicine = GetMedicineByName(newMedicine.Name);
                medicine.ShowTimes += newMedicine.ShowTimes;
                medicine.Score += newMedicine.Score;
            }
            else
            {
                Medicines.Add(newMedicine);
            }
        }

        /// <summary>
        /// Apresenta ao usuário as informaçõe sobre o método executado
        /// </summary>
        private void ShowMethodStatistics()
        {
            if (!DataContainer.ShowStatistics)
                return;

            List<Medicine> MedicinesList = MedicinesView.Cast<Medicine>().ToList();

            switch (DataContainer.CurrentMethod)
            {
                case Methods.Add:
                    CustomMessageBox.Show($"{MedicinesList.Count} medicamentos encontrados e analisados pelo método da adição");
                    break;
                case Methods.Symptoms:
                    bool secondInput = false;

                    foreach(Medicine medicine in Medicines)
                        if (medicine.ShowTimes > 1)
                        {
                            secondInput = true;
                            break;
                        }

                    if(!secondInput)
                        CustomMessageBox.Show($"{MedicinesList.Count} medicamentos encontrados para o sintoma diretor");
                    else
                        CustomMessageBox.Show($"{MedicinesList.Count} medicamentos encontrados e analisados pelo método do sintoma diretor");

                    break;
                case Methods.Elimination:
                    CustomMessageBox.Show($"{MedicinesList.Count} medicamentos encontrados e analisados pelo método da eliminação");
                    break;
            }
        }

        /// <summary>
        /// Obtem um medicamento da lista <see cref="Medicines"/> de acordo com o nome informado
        /// </summary>
        private Medicine GetMedicineByName(string name)
        {
            foreach(Medicine medicine in Medicines)
                if(medicine.Name == name)
                    return medicine;

            return null;
        }

        /// <summary>
        /// Salva a lista de medicamentos como imagem ou texto
        /// </summary>
        public void SaveMedicineList()
        {
            MainWindow.RecBtnSave.Fill = Brushes.LightBlue;

            if (!DataContainer.SaveImg)
            {
                string clipText = "Remédio | Pontos | Recorrência" + Environment.NewLine;

                List<Medicine> MedicinesList = MedicinesView.Cast<Medicine>().ToList();

                foreach (Medicine medicine in MedicinesList)
                {
                    clipText += medicine.Name + " | " + medicine.Score + " | " + medicine.ShowTimes + Environment.NewLine;
                }

                Clipboard.SetText(clipText);
            }
            else
            {
                Grid control = MainWindow.GridResults;
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)(control.ActualWidth - 17), (int)control.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                Rect bounds = VisualTreeHelper.GetDescendantBounds(control);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext ctx = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(control);
                    ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }
                rtb.Render(dv);

                Clipboard.SetImage(BitmapFrame.Create(rtb));
            }

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                MainWindow.Dispatcher.Invoke(() =>
                {
                    MainWindow.RecBtnSave.Fill = Brushes.LightGray;
                });
            });
        }

        /// <summary>
        /// Atualiza a visualização dos medicamentos
        /// </summary>
        public void UpdatesMedicinesView()
        {
            CollectionViewSource viewSource = new CollectionViewSource();
            viewSource.Source = Medicines;

            MedicinesView = viewSource.View;

            switch (CurrenteFilterMode)
            {
                case FilterMode.Pontos:
                    MedicinesView.SortDescriptions.Add(new SortDescription("Score", ListSortDirection.Descending));
                    MedicinesView.SortDescriptions.Add(new SortDescription("ShowTimes", ListSortDirection.Descending));
                    break;
                case FilterMode.Recorrencia:
                    MedicinesView.SortDescriptions.Add(new SortDescription("ShowTimes", ListSortDirection.Descending));
                    MedicinesView.SortDescriptions.Add(new SortDescription("Score", ListSortDirection.Descending));
                    break;
                case FilterMode.Nome:
                    MedicinesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;
            }

            if(DataContainer.CurrentMethod == Methods.Elimination || DataContainer.CurrentMethod == Methods.Symptoms)
            {
                bool twoTimesMedicine = false;

                foreach(Medicine medicine in Medicines)
                {
                    if(medicine.ShowTimes > 1)
                    {
                        twoTimesMedicine=true;
                        break;
                    }
                }

                if(twoTimesMedicine)
                    MedicinesView.Filter = new Predicate<object>((i) => ((Medicine)i).ShowTimes > 1);
            }
        }

        /// <summary>
        /// Limpa a lista de medicamentos
        /// </summary>
        public void ClearMedicines()
        {
            Medicines.Clear();

            UpdatesMedicinesView();
        }

        private void CheckAndInstallUpdates()
        {
            Task.Factory.StartNew(async () => {

                Debug.WriteLine("Iniciando método de update...");

                UpdateCheckInfo info = null;

                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                    try
                    {
                        // Setup the trust level
                        var deployment = ApplicationDeployment.CurrentDeployment;
                        var appId = new ApplicationIdentity(deployment.UpdatedApplicationFullName);
                        var unrestrictedPerms = new PermissionSet(PermissionState.Unrestricted);
                        var appTrust = new ApplicationTrust(appId)
                        {
                            DefaultGrantSet = new PolicyStatement(unrestrictedPerms),
                            IsApplicationTrustedToRun = true,
                            Persist = true
                        };
                        ApplicationSecurityManager.UserApplicationTrusts.Add(appTrust);

                        info = ad.CheckForDetailedUpdate();

                    }
                    catch (DeploymentDownloadException dde)
                    {
                        Debug.WriteLine("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                        return;
                    }
                    catch (InvalidDeploymentException ide)
                    {
                        Debug.WriteLine("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                        return;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Debug.WriteLine("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                        return;
                    }

                    if (info.UpdateAvailable)
                    {
                        Debug.WriteLine("Update encontrado");

                        if(await CustomMessageBox.Show("Existe uma atualização pendente para este software, deseja realizar a isntalação agora?") == CustomMessageBox.MessageRetuns.Yes)
                        {
                            Debug.WriteLine("Update aceito pelo usuário");

                            try
                            {
                                ad.Update();
                                await CustomMessageBox.Show("O programa foi atualizado e será reiniciado.");
                                Application.Current.Shutdown();
                            }
                            catch (DeploymentDownloadException dde)
                            {
                                CustomMessageBox.Show("Ocorreu um problema durante a atualização do programa!");

                                Debug.WriteLine("Erro ao instalar o programa: " + dde);
                                return;
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Update negado pelo usuário");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Nenhum update encontrado");
                    }
                }
                else
                {
                    Debug.WriteLine("Sem conexão");
                }
            });
        }
    }
}
