using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesTaxService.Services;
using System.Text;

using System.Xml.Linq;

namespace SalesTaxService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesTaxController : ControllerBase
    {
        private ISalesTax _SalesTaxService;
        public SalesTaxController(ISalesTax SalesTaxService)
        {
            _SalesTaxService = SalesTaxService;
        }
        [HttpPost]
        public IActionResult ProcessText([FromBody] string text)
        {
            try
            {
                return Ok(_SalesTaxService.CalculateSalesTax(text));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }

}
