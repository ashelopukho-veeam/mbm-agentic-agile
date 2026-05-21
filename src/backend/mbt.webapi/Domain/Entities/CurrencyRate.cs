using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class CurrencyRate : BaseItem
{
    public int Year { get; set; }
    public double Rate { get; set; }
}

//Currency	Effective Year	Price
//AED 2021    0.27224
//ARS 2021    0.01280
//AUD 2021    0.70210
