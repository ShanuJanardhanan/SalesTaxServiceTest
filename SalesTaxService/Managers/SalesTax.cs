using SalesTaxService.Controllers;
using SalesTaxService.Model;
using SalesTaxService.Services;
using System.Text;
using System.Xml.Linq;

namespace SalesTaxService.Managers
{
    public class SalesTax : ISalesTax
    {
        public SalesTax()
        {

        }

        public dynamic CalculateSalesTax(string text)
        {
            try
            {
                // sanitaized string
                text = getRidOfUnprintablesAndUnicode(text);

                // Adding the root element over the text so that we can have the root element while parsing
                text = "<root>" + text + "</root>";

                var expenseData = ExtractData(text);

                // Default cost_centre to 'UNKNOWN' if missing
                if (string.IsNullOrEmpty(expenseData.CostCentre))
                {
                    expenseData.CostCentre = "UNKNOWN";
                }

                // Calculate sales tax and total excluding tax
                var (salesTax, totalExcludingTax) = CalculateTaxAndTotal(expenseData.Total);

                var result = new
                {
                    vendor_name = expenseData.Vendor,
                    description = expenseData.Description,
                    date = expenseData.Date,
                    cost_centre = expenseData.CostCentre,
                    total_including_tax = expenseData.Total,
                    sales_tax = salesTax,
                    total_excluding_tax = totalExcludingTax
                };

                return result;
            }
            catch (Exception ex)
            {
                 throw  ;
            }
        }

        /// <summary>
        ///  Avoid Unicode characters
        /// </summary>
        /// <param name="inpString"></param>
        /// <returns></returns>
        private string getRidOfUnprintablesAndUnicode(string inpString)
        {
            StringBuilder outputs = new StringBuilder();
            for (int jj = 0; jj < inpString.Length; jj++)
            {
                char ch = inpString[jj];
                if (((int)(byte)ch) >= 32 & ((int)(byte)ch) <= 128)
                {
                    outputs.Append(ch);
                }
            }
            return outputs.ToString();
        }
        private ExpenseData ExtractData(string text)
        {
            try
            {
                // Console.WriteLine($"Received XML text: {text}");
                var root = XElement.Parse(text);

                // Console.WriteLine($"Parsed XML: {root}");
                var expenseElement = root.Descendants("expense").FirstOrDefault();


                 var costCentreElement = expenseElement!=null? expenseElement.Descendants("cost_centre").FirstOrDefault():null;
                 var totalElement = expenseElement != null ? expenseElement.Descendants("total").FirstOrDefault() : null;
                var vendorElement = root.Descendants("vendor").FirstOrDefault();
                var descriptionElement = root.Descendants("description").FirstOrDefault();
                var dateElement = root.Descendants("date").FirstOrDefault();


                // Missing <total>. In this case the entire message must be rejected.
                if (totalElement == null)
                {
                    throw new Exception("Missing required fields in expense block");
                }

                var expenseData = new ExpenseData
                {
                    CostCentre = costCentreElement != null ? costCentreElement.Value : null,
                    Total = Convert.ToDouble(totalElement.Value)
                };

                if (vendorElement != null && descriptionElement != null && dateElement != null)
                {
                    expenseData.Vendor = vendorElement.Value;
                    expenseData.Description = descriptionElement.Value;
                    expenseData.Date = dateElement.Value;
                }

                return expenseData;
            }
            catch (Exception ex)
            {
                // Log the exception or print the details for debugging
                // Console.WriteLine($"Error parsing XML: {ex.Message}");
                throw new Exception("Invalid XML format");
            }
        }

        /// <summary>
        /// Calcualte tax based on sales tax rate..
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        private (double, double) CalculateTaxAndTotal(double total)
        {
            const double salesTaxRate = 0.1; // Assume a sales tax rate of 10%
            var salesTax = total * salesTaxRate;
            var totalExcludingTax = total - salesTax;
            return (salesTax, totalExcludingTax);
        }
    }
}
