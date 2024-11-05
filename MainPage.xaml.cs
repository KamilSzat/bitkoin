using System.Net.Http;
using System.Text.Json;

namespace bitcoin
{
    public class BitcoinRate
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public double? Rate_float { get; set; }
    }

    public class BitcoinRate2
    {
        public BitcoinRate? USD { get; set; }
        public BitcoinRate? GBP { get; set; }
        public BitcoinRate? EUR { get; set; }
    }

    public class Bitcoin
    {
        public string? ChartName { get; set; }
        public BitcoinRate2 Bpi { get; set; }
    }

    public class USD
    {
        public string? Code { get; set; }
        public IList<Rate> Rates { get; set; }
    }

    public class Rate
    {
        public double? Ask { get; set; }
        public double? Bid { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadData().ConfigureAwait(false);
        }

        public async Task LoadData()
        {
            double usd = 0;  // Zmienna przechowująca kurs USD z API NBP 
            string dzis = DateTime.Now.ToString("yyyy-MM-dd");

            string urlUSD = $"https://api.nbp.pl/api/exchangerates/rates/c/usd/{dzis}/?format=json";
            string json;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    json = await httpClient.GetStringAsync(urlUSD);
                    USD dolar = JsonSerializer.Deserialize<USD>(json);
                    if (dolar?.Rates != null && dolar.Rates.Count > 0)
                    {
                        usd = dolar.Rates[0].Ask ?? 0;  // Pobranie kursu sprzedaży
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd pobierania kursu USD: {ex.Message}");
                    return;
                }
            }

            // API CoinDesk
            string urlBitcoin = "https://api.coindesk.com/v1/bpi/currentprice.json";
            using (var httpClient = new HttpClient())
            {
                    json = await httpClient.GetStringAsync(urlBitcoin);
                    Bitcoin bitcoin = JsonSerializer.Deserialize<Bitcoin>(json);

                    if (bitcoin?.Bpi?.USD?.Rate_float != null)
                    {
                        string s = $"{bitcoin.Bpi.USD.Code}: {bitcoin.Bpi.USD.Rate_float.Value:# ###.####}";
                        lblUSD.Text = s;

                        s = $"{bitcoin.Bpi.GBP.Code}: {bitcoin.Bpi.GBP.Rate_float.Value:# ###.####}";
                        lblGBP.Text = s;

                        s = $"{bitcoin.Bpi.EUR.Code}: {bitcoin.Bpi.EUR.Rate_float.Value:# ###.####}";
                        lblEUR.Text = s;

                        s = $"PLN: {(usd * bitcoin.Bpi.USD.Rate_float.Value):# ###.####}";
                        lblPLN.Text = s;
                    }
                
            }
        }
    }
}
