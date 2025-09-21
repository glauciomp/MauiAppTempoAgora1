using MauiAppTempoAgora1.Models;
using MauiAppTempoAgora1.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Networking; // inserindo o using para checar a conexão
using System.Globalization; // inserindo o using para formatar datas

namespace MauiAppTempoAgora1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // verificar a conexão com a Internet antes de seguir
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                lbl_res.Text = "Você está sem conexão com a Internet ou com algum problema de rede, " +
                    "verifique sua conexão ou tente novamente mais tarde.";
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text))
                {
                    // chama o serviço para obter os dados de previsão
                    ResultadoPrevisao resultado = await DataService.GetPrevisao(txt_cidade.Text);

                    if (resultado.Dados != null)
                    {
                        Tempo t = resultado.Dados;
                        string dados_previsao = "";

                        // Inicializa as strings de pôr e nascer do sol com os valores originais da API
                        string sunsetStr = t.sunset;
                        string sunriseStr = t.sunrise;

                        // Tenta analisar as strings para DateTime
                        // Se a conversão for bem-sucedida e o ano estiver entre 1900 e 2100, formata a data
                        // Caso contrário, mantém a string original da API para evitar erro
                        if (DateTime.TryParse(sunsetStr, out DateTime sunsetTime) &&
                            sunsetTime.Year >= 1900 && sunsetTime.Year <= 2100)
                        {
                            sunsetStr = sunsetTime.ToString("dd/MM/yyyy HH:mm:ss");
                        }

                        if (DateTime.TryParse(sunriseStr, out DateTime sunriseTime) &&
                            sunriseTime.Year >= 1900 && sunriseTime.Year <= 2100)
                        {
                            sunriseStr = sunriseTime.ToString("dd/MM/yyyy HH:mm:ss");
                        }

                        // monta a string com os dados formatados
                        dados_previsao = $"Latitude: {t.lat}\n" +
                                         $"Longitude: {t.lon}\n" +
                                         $"Pôr do Sol: {sunsetStr}\n" +
                                         $"Nascer do Sol: {sunriseStr}\n" +
                                         $"Temperatura Minima: {t.temp_min}ºC\n" +
                                         $"Temperatura Máxima: {t.temp_max}ºC\n" +
                                         $"Visibilidade: {t.visibility} metros\n" +
                                         $"Velocidade do Vento: {t.speed} m/s\n" +
                                         $"Condição do Tempo: {t.description}\n";

                        // exibe os dados no label
                        lbl_res.Text = dados_previsao;
                    }
                    else
                    {
                        // mensagens específicas por tipo de erro
                        switch (resultado.StatusCode)
                        {
                            case System.Net.HttpStatusCode.NotFound:
                                lbl_res.Text = "Cidade não encontrada. Verifique o nome e tente novamente.";
                                break;
                            case System.Net.HttpStatusCode.Unauthorized:
                                lbl_res.Text = "Problema com a chave da API. Verifique se está correta.";
                                break;
                            case System.Net.HttpStatusCode.ServiceUnavailable:
                                lbl_res.Text = "Serviço indisponível. Verifique sua conexão ou tente mais tarde.";
                                break;
                            default:
                                lbl_res.Text = $"Erro ao obter dados: {resultado.StatusCode}";
                                break;
                        }
                    }
                }
                else
                {
                    // caso o campo de cidade esteja vazio
                    lbl_res.Text = "Por favor, digite o nome de uma cidade para buscar a previsão.";
                }
            }
            catch (Exception ex)
            {
                // exibe alerta em caso de erro inesperado
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}