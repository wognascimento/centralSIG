using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace CentralSIG
{
    public partial class MainWindow : Window
    {
        private const string DefaultSigRoot = @"C:\SIG";

        public MainWindow()
        {
            InitializeComponent();

            SigRoot = Environment.GetEnvironmentVariable("SIG_ROOT") ?? DefaultSigRoot;
            Modules = CreateModules();
            DataContext = this;
        }

        public string SigRoot { get; }

        public IReadOnlyList<SigModule> Modules { get; }

        private async void OnAtualizarSistemaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            await ((App)Application.Current).CheckForUpdatesAsync(true);
        }

        private void OnSobreSistemaClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var version = ((App)Application.Current).InstalledVersion;
            MessageBox.Show($"Central S.I.G. - Sistema Integrado de Gerenciamento\n\nVersão atual: {version}", "Sobre o sistema", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private IReadOnlyList<SigModule> CreateModules()
        {
            return new[]
            {
                CreateModule("APONTAMENTO", "Apontamento S.I.G", "Apontamento.exe", "#FFEAFF00"),
                CreateModule("PRODUÇÃO", "Producao S.I.G", "producao.exe", "#FF33CC35"),
                CreateModule("COMPRAS", "Compras S.I.G", "compras.exe", "#FFBA309B"),
                CreateModule("EXPEDIÇÃO", "Expedicao S.I.G", "expedicao.exe", "#FF6251A1"),
                CreateModule("ALMOXARIFADO", "Almoxarifado S.I.G", "almoxarifado.exe", "#FF33B3CC"),
                CreateModule("OPERACIONAL", "Operacional S.I.G", "operacional.exe", "#FFBA5C30"),
                CreateModule("COMERCIAL", "Comercial S.I.G", "comercial.exe", "#FF860E0E"),
                CreateModule("FINANCEIRO", "Financeiro S.I.G", "Financeiro.exe", "#FF797979"),
                CreateModule("RECURSOS HUMANOS", "Recursos Humanos S.I.G", "GestaoRH.exe", "#FF534FF6")
            };
        }

        private SigModule CreateModule(string name, string folderName, string executableName, string color)
        {
            var executablePath = Path.Combine(SigRoot, folderName, executableName);

            return new SigModule
            {
                Name = name,
                FolderName = folderName,
                ExecutableName = executableName,
                ExecutablePath = executablePath,
                Brush = (Brush)new BrushConverter().ConvertFromString(color),
                Status = File.Exists(executablePath) ? "INSTALADO" : "NÃO INSTALADO"
            };
        }

        private void OnModuleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: SigModule module })
            {
                OpenModule(module);
            }
        }

        private void OpenModule(SigModule module)
        {
            if (!File.Exists(module.ExecutablePath))
            {
                RadWindow.Alert(new DialogParameters
                {
                    Header = "S.I.G",
                    Content = $"Módulo {module.Name} não está instalado.\n\nArquivo esperado:\n{module.ExecutablePath}"
                });
                return;
            }

            Process.Start(new ProcessStartInfo(module.ExecutablePath)
            {
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(module.ExecutablePath)
            });
        }
    }

    public class SigModule
    {
        public string Name { get; set; }

        public string FolderName { get; set; }

        public string ExecutableName { get; set; }

        public string ExecutablePath { get; set; }

        public Brush Brush { get; set; }

        public string Status { get; set; }
    }
}
