using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTempoAgora1.Models
{
    // classe criada para retornar os dados e o código de status da resposta HTTP
    public class ResultadoPrevisao
    {
        public Tempo? Dados { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }
}
