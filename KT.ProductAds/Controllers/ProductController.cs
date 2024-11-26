using KT.Models.Common;
using KT.ProductAds.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFM.Registration.Contracts;

namespace KT.ProductAds.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductsAndAdsService _productsService;

        public ProductController(ProductsAndAdsService productsService
           )
        {
            _productsService = productsService;

        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.ProductAds.products)]
        public IActionResult ProductsList()
        {

            try
            {

                var response = _productsService.GetProductsList();
                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest(new BadRequestResponse
                {
                    Code = "400",
                    Message = e.Message
                });

            }
        }
        [AllowAnonymous]
        [HttpPost(ApiRoutes.ProductAds.ads)]
        public IActionResult AdsList()
        {

            try
            {

                var response = _productsService.GetAdsList();
                return Ok(response);

            }
            catch (Exception e)
            {

                return BadRequest(new BadRequestResponse
                {
                    Code = "400",
                    Message = e.Message
                });

            }
        }
    }
}
