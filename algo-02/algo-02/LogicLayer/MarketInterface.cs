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

        //MarketInterface API = new MarketInterface();
        //string testing_ = API.TESTGET().Result;
        //Console.WriteLine(testing_);

        //    Console.WriteLine("press a key to exit");
        //    Console.ReadLine();

        const string QUERY_URL = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=IBM&interval=5min&apikey=Z52ESGDDV1GBC6PH";
        Uri queryUri = new Uri(QUERY_URL);

        static HttpClient client = new HttpClient();

        public async Task<string> TESTGET()
        {
            Task<HttpResponseMessage> returned = client.GetAsync(QUERY_URL);
            return await returned.Result.Content.ReadAsStringAsync();

        }



    }
}
