using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// adicionando o using newtonsoft.json
using Newtonsoft.Json;
// adicionando o using MauiAppTempoAgora1.Models;
using MauiAppTempoAgora1.Models;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora1.Services
{
    // mudando o internal para public
    public class DataService
    {
        public static async Task<ResultadoPrevisao> GetPrevisao(string cidade)
        {
            // vamos criar o objeto tempo aqui dentro
            Tempo? t = null; // colocado ? pois a varial pode ser nula ou ter o tempo
            // está como null porque ainda não sabemos se o lugar
            //que vamos pesquisar existe ou não

            // vamos criar a task lá em cima
            // vamos definir a varial string que vai ser a chave da api
            string chave = "5f6ac045d5053d543c4f6685cccba0f2";
            // criar a string da url de consulta
            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&appid={chave}&lang=pt_br&units=metric";
            // agora vamos abrir um navegador aqui dentro
            using (HttpClient client = new HttpClient())
            {
                // depois do laço using, há o despejo de memória
                // sempre coloque o HttpClient dentro do using

                try
                {
                    // agora vamos pegar a resposta da url
                    HttpResponseMessage resp = await client.GetAsync(url);

                    // verificar se a resposta foi ok
                    if (!resp.IsSuccessStatusCode)
                    {
                        // se a resposta não for OK, retorna apenas o código de erro
                        return new ResultadoPrevisao { Dados = null, StatusCode = resp.StatusCode };
                    }

                    string json = await resp.Content.ReadAsStringAsync();
                    // agora vamos converter este json

                    var rascunho = JObject.Parse(json);
                    // o JObject não tem no using, então clica com o botão
                    // direito e na luz, e adicionar o using

                    // criando o datetime por causa do nascer do sol
                    DateTime sunrise = DateTimeOffset.FromUnixTimeSeconds((long)rascunho["sys"]["sunrise"]).ToLocalTime().DateTime;
                    DateTime sunset = DateTimeOffset.FromUnixTimeSeconds((long)rascunho["sys"]["sunset"]).ToLocalTime().DateTime;

                    // preencher o objeto tempo
                    t = new()
                    {
                        lat = (double)rascunho["coord"]["lat"],
                        // tivemos que aceitar o double pois foi a sugestão do vs
                        lon = (double)rascunho["coord"]["lon"],

                        // alt + espaço vai trazer as sugestões do Tempo.cs
                        description = (string)rascunho["weather"][0]["description"],
                        main = (string)rascunho["weather"][0]["main"],
                        // ao fazer os códigos, ele começou a reclamar por não ser nulo
                        // então colocamos ? na classe Tempo.cs para todos as propriedades

                        temp_min = (double)rascunho["main"]["temp_min"],
                        temp_max = (double)rascunho["main"]["temp_max"],
                        speed = (double)rascunho["wind"]["speed"],
                        visibility = (int)rascunho["visibility"],
                        sunrise = sunrise.ToString(),
                        sunset = sunset.ToString(),
                    }; // fecha o objeto Tempo

                    // retorna os dados e o status OK
                    return new ResultadoPrevisao { Dados = t, StatusCode = resp.StatusCode };
                }
                catch
                {
                    // se houver erro de rede ou exceção, retorna código de serviço indisponível
                    return new ResultadoPrevisao { Dados = null, StatusCode = System.Net.HttpStatusCode.ServiceUnavailable };
                }
            } // fecha o using do HttpClient

            // vamos deixar o nosso objeto assincrono senão vai ficar pensando muito
        }
    }
}