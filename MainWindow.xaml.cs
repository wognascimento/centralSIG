using Newtonsoft.Json.Linq;
using Squirrel;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace CentralSIG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = @$"{Environment.GetEnvironmentVariable("USERPROFILE")}\AppData\Local";
        string Producao;
        string Compras;
        string Expedicao;
        string Almoxarifado;
        string Operacional;
        string Comercial;
        string Financeiro;

        public MainWindow()
        {
            InitializeComponent();

            Producao        = $@"{path}\Producao\";
            Compras         = @$"{path}\Compras\";
            Expedicao       = @$"{path}\Expedicao\";
            Almoxarifado    = @$"{path}\Almoxarifado\";
            Operacional     = @$"{path}\Operacional\";
            Comercial       = @$"{path}\Comercial\";
            Financeiro      = @$"{path}\Financeiro\";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        static bool ShowTheWelcomeWizard;
        private async Task RemovAtalho(string path)
        {
            try
            {

                using var mgr = new UpdateManager(path);
                SquirrelAwareApp.HandleEvents(
                    onInitialInstall: v => mgr.RemoveShortcutForThisExe(), //CreateShortcutForThisExe()
                    onAppUpdate: v => mgr.RemoveShortcutForThisExe(),
                    onAppUninstall: v => mgr.RemoveShortcutForThisExe(),
                    onFirstRun: () => ShowTheWelcomeWizard = true);

            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters() { Header = "Erro", Content = ex.Message });
            }
        }

        private async void OnOpenProducao(object sender, MouseButtonEventArgs e)
        {

            if (Directory.Exists(Producao))
                Process.Start(@$"{Producao}\producao.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Produção não esta instalado." });
        }

        private void OnOpenCompras(object sender, MouseButtonEventArgs e)
        {
            if (Directory.Exists(Compras))
                Process.Start(@$"{Compras}\compras.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Compras não esta instalado." });
        }

        private async void OnOpenExpedicao(object sender, MouseButtonEventArgs e)
        {
            if (Directory.Exists(@$"{Expedicao}"))
                Process.Start(@$"{Expedicao}\expedicao.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Expedição não esta instalado." });
        }

        private void OnOpenAlmoxarifado(object sender, MouseButtonEventArgs e)
        {
            RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Almoxarifado esta em desenvolvimento." });
            return;

            if (Directory.Exists(@$"{Almoxarifado}"))
                Process.Start(@$"{Almoxarifado}\almoxarifado.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Almoxarifado não esta instalado." });
        }

        private void OnOpenOperacional(object sender, MouseButtonEventArgs e)
        {
            RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Operacional esta em desenvolvimento." });
            return;

            if (Directory.Exists(@$"{Operacional}"))
                Process.Start(@$"{Operacional}\operacional.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Operacional não esta instalado." });
        }

        private void OnOpenComercial(object sender, MouseButtonEventArgs e)
        {
            RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Comercial esta em desenvolvimento." });
            return;

            if (Directory.Exists(@$"{Comercial}"))
                Process.Start(@$"{Comercial}\comercial.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Comercial não esta instalado." });
        }

        private void OnOpenFinanceiro(object sender, MouseButtonEventArgs e)
        {
            RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Financeiro esta em desenvolvimento." });
            return;

            if (Directory.Exists(@$"{Financeiro}"))
                Process.Start(@$"{Financeiro}\Financeiro.exe");
            else
                RadWindow.Alert(new DialogParameters() { Header = "S.I.G", Content = "Modulo Financeiro não esta instalado." });
        }

        private async Task SolicitarInstalacao()
        {
            // Substitua "seu_url_glpi", "seu_user_token" e "seu_app_token" pelos valores reais
            string glpiUrl = "https://helpdesk.cipodominio.com.br/apirest.php";
            string userToken = "H9QNDMLcGtLt7AbywUhVBKaEtPNoq8F1JYXV4Z9C";
            string appToken = "L9tYG6Svt7OLiaqmLUiHF4v0DLzmBmzLjYOGcG94";
            string sessionToken;
            string entityId = "0";
            int typeId = 2; // Substitua pelo valor inteiro correspondente
            int categoryId = 3; // Substitua pelo valor inteiro correspondente
            int requestTypeId = 8; // Substitua pelo valor inteiro correspondente
            string title = "Instalar modulo SIG";
            string description = "Solicito Instalação do Módulo Expedição";

            // URL para iniciar uma sessão no GLPI
            string initSessionEndpoint = $"{glpiUrl}/initSession";

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            };

            using (HttpClient client = new HttpClient(handler))
            {
                // Configurar os cabeçalhos para incluir o token do usuário e da aplicação
                client.DefaultRequestHeaders.Add("Authorization", $"user_token {userToken}");
                client.DefaultRequestHeaders.Add("App-Token", appToken);

                // Enviar a solicitação GET para iniciar a sessão
                HttpResponseMessage response = await client.GetAsync(initSessionEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    // Lê o conteúdo da resposta
                    string content = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(content);
                    sessionToken = json["session_token"].ToString();


                    client.DefaultRequestHeaders.Add("Session-Token", sessionToken);
                    string getUserEndpoint = $"{glpiUrl}/User/2/";
                    response = await client.GetAsync(getUserEndpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        // Lê o conteúdo da resposta
                        content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(content);
                    }
                    else
                    {
                        Console.WriteLine($"Erro: {response.StatusCode}");
                    }

                    string createTicketEndpoint = $"{glpiUrl}/Ticket";
                    client.DefaultRequestHeaders.Add("Session-Token", sessionToken);
                    // Dados JSON para a criação do novo ticket
                    string jsonData = $@"
                        {{
                            ""input"": {{
                                ""entities_id"": ""{entityId}"",
                                ""type"": {typeId},
                                ""itilcategories_id"": {categoryId},
                                ""requesttypes_id"": {requestTypeId},
                                ""name"": ""{title}"",
                                ""content"": ""{description}"",
                                ""date"": ""{DateTime.Now}""
                            }}
                        }}
                    ";
                    response = await client.PostAsync(createTicketEndpoint, new StringContent(jsonData, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        // Lê o conteúdo da resposta
                        content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(content);
                    }
                    else
                    {
                        Console.WriteLine($"Erro: {response.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine($"Erro: {response.StatusCode}");
                }
            }
        }

    }
}
