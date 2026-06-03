using BibliotecasSIG;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace CentralSIG
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string DefaultUpdateUrl = "http://192.168.0.49/downloads/central-sig/version.json";
        private static readonly string CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
        private static readonly string UpdateUrl = Environment.GetEnvironmentVariable("CENTRALSIG_UPDATE_URL") ?? DefaultUpdateUrl;

        public App()
        {
           this.InitializeComponent();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await CheckForUpdatesAsync();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                var updateChecker = new UpdateChecker(UpdateUrl, CurrentVersion);
                var updateInfo = await updateChecker.CheckForUpdatesAsync();

                if (updateInfo != null)
                {
                    var result = MessageBox.Show(
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
            catch(HttpRequestException ex)
            {
                MessageBox.Show(
                    $"Erro ao verificar atualizações: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao verificar atualizações: {ex.Message}",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}



