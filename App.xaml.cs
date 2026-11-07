using BibliotecasSIG;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Controls;

namespace CentralSIG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DefaultUpdateUrl = "https://atualizasig.cipolatti.com.br/downloads/central-sig/version.json";
        private static readonly string CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
        private static readonly string UpdateUrl = ResolveUpdateUrl();

        private static string ResolveUpdateUrl()
        {
            var configured = Environment.GetEnvironmentVariable("CENTRALSIG_UPDATE_URL");
            if (string.IsNullOrWhiteSpace(configured))
                return DefaultUpdateUrl;

            if (Uri.TryCreate(configured.Trim(), UriKind.Absolute, out var uri)
                && uri.Host == "192.168.0.49")
                return DefaultUpdateUrl;

            return configured.Trim();
        }

        public App()
        {
            StyleManager.ApplicationTheme = new Office2016Theme();
            this.InitializeComponent();
        }

        private bool checkingForUpdates;

        public string InstalledVersion => CurrentVersion;

        public async Task CheckForUpdatesAsync(Window owner, bool showUpToDate = false)
        {
            if (checkingForUpdates) return;
            checkingForUpdates = true;
            try
            {
                var updateChecker = new UpdateChecker(UpdateUrl, CurrentVersion);
                var updateInfo = await updateChecker.CheckForUpdatesAsync();
                if (!owner.IsVisible) return;

                if (updateInfo == null)
                {
                    if (showUpToDate)
                        MessageBox.Show(owner, $"O sistema já está atualizado.\n\nVersão atual: {CurrentVersion}", "Atualização do sistema", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (updateInfo != null)
                {
                    var result = MessageBox.Show(
                        owner,
                        $"Nova versão disponível!\n\n" +
                        $"Versão atual: {CurrentVersion}\n" +
                        $"Nova versão: {updateInfo.updateVersion}\n\n" +
                        "Changelog:\n" +
                        string.Join("\n", updateInfo.changelog) +
                        "\n\nDeseja baixar a atualização?",
                        "Atualização Disponível",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        string jsonData = JsonSerializer.Serialize(updateInfo);
                        string appName = "CentralSIG.exe";
                        string arguments = $"\"{jsonData.Replace("\"", "\\\"")}\" \"{appName}\"";

                        Process.Start(new ProcessStartInfo("Update.exe", arguments)
                        {
                            UseShellExecute = true
                        });

                        this.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!owner.IsVisible) return;
                MessageBox.Show(
                    owner,
                    $"Erro ao verificar atualizações: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                checkingForUpdates = false;
            }
        }
    }
}



