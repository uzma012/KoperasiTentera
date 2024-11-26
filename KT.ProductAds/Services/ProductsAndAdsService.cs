using KT.Interfaces.Repositories;
using KT.Models.Common.Options;
using KT.Models.DB.Ads;
using KT.Models.DB.Products;
using KT.Repositories.ProductAndAds;
using Microsoft.Extensions.Options;

namespace KT.ProductAds.Services
{
    public class ProductsAndAdsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ProductAndAdsRepository _productsAndAdsRepository;
        public ProductsAndAdsService( IUnitOfWork unitOfWork,
             IConfiguration configuration, ProductAndAdsRepository productAndAdsRepository)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _productsAndAdsRepository = productAndAdsRepository;
        }

        public List<ProductModel> GetProductsList()
        {
            var productsList = _productsAndAdsRepository.GetProducts();
            return productsList;
        }

        public List<AdsModel> GetAdsList()
        {
            var adsList = _productsAndAdsRepository.GetAds();
            return adsList;
        }

        
    }
}
