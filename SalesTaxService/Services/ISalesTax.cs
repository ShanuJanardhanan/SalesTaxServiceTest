using SalesTaxService.Controllers;
using SalesTaxService.Model;

namespace SalesTaxService.Services
{
    public interface ISalesTax
    {
        public dynamic CalculateSalesTax(string text);
    }
}
