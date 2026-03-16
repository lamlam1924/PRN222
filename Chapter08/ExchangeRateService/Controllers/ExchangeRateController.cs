using Microsoft.AspNetCore.Mvc;

namespace ExchangeRateService.Controllers;

[ApiController]

[Route("[controller]")]

public class ExchangeRateController: ControllerBase{

    [HttpGet]

    [Route("GetLatestRates")]

    public CurrencyExchange GetLatestRates()

    {

        var rates = new Dictionary<string, decimal>();

        rates.Add("CAD",1.260046m);

        rates.Add("CHF", 0.933058m);

        rates.Add("EUR", 0.806942m);

        rates.Add("GBP", 0.719154m);

        CurrencyExchange currencyExchange = new CurrencyExchange {

            Base = "USD",

            Date = DateTime.Now,

            Rates =rates

        };

        return currencyExchange;

    }

}