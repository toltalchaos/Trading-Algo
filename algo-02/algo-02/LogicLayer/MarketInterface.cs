using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


namespace algo_02
{
    class MarketInterface
    {
        static HttpClient client = new HttpClient();


        public async Task<string> Get(Uri uriIn)
        {
            Task<HttpResponseMessage> returned = client.GetAsync(uriIn);
            return await returned.Result.Content.ReadAsStringAsync();

        }

        private string HISTORY_QUERY_URL(string symbol, string fulldata)
        {
            string QUERY_URL = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=5min&apikey=Z52ESGDDV1GBC6PH{fulldata}";
            return QUERY_URL;
        }

        public string History_QueryMarket_Symbol(string symbolIn)
        {
            //incoming - string to check if market has matching symbol
            //outgoing - null string/confirmed symbol
            Uri queryUri = new Uri(HISTORY_QUERY_URL(symbolIn, null)); //add headers after this
            return Get(queryUri).Result.ToString();


        }
        public string History_QueryMarket_Symbol_Full(string symbolIn)
        {
            //incoming - string to check if market has matching symbol
            //outgoing - null string/confirmed symbol
            Uri queryUri = new Uri(HISTORY_QUERY_URL(symbolIn, "outputsize=full")); //add headers after this
            return Get(queryUri).Result.ToString();


        }


    }
}
