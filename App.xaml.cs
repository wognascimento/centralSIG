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
        private const string UPDATE_URL = "http://192.168.0.49/downloads/central-sig/version.json";
        private readonly string CURRENT_VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public App()
        {
           this.InitializeComponent();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Verificação de atualização em segundo plano
            await CheckForUpdatesAsync();
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                var updateChecker = new UpdateChecker(UPDATE_URL, CURRENT_VERSION);
                var updateInfo = await updateChecker.CheckForUpdatesAsync();

                var updateInfoJson =  JsonSerializer.Serialize<UpdateInfo>(updateInfo);

                if (updateInfo != null)
                {
                    // Pergunta ao usuário se deseja atualizar
                    var result = MessageBox.Show(
                        $"Nova versão disponível!\n\n" +
                        $"Versão atual: {CURRENT_VERSION}\n" +
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

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string jsonString = JsonSerializer.Serialize(updateInfo, options);

                        //Process.Start("Update.exe", @$"{updateInfoJson}, CentralSIG.exe");

                        string jsonData = JsonSerializer.Serialize(updateInfo); // Garante que o JSON está bem formatado
                        string appName = "CentralSIG.exe";

                        string arguments = $"\"{jsonData.Replace("\"", "\\\"")}\" \"{appName}\"";
                        Process.Start("Update.exe", arguments);
                        this.Shutdown();

                    }
                }
            }
            catch(HttpRequestException ex)
            {
                // Log do erro ou tratamento de exceção
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



