using KT.Interfaces.Repositories;
using KT.Models.DB.Ads;
using KT.Models.DB.Products;

namespace KT.Repositories.ProductAndAds
{
    public class ProductAndAdsRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductModel> _productRepository;
        private readonly IRepository<AdsModel> _adsRepository;
        public  ProductAndAdsRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _productRepository= _unitOfWork.Repository<ProductModel>();
            _adsRepository= _unitOfWork.Repository<AdsModel>();

        }

        public List<ProductModel> GetProducts()
        {
            return _productRepository.GetAll().ToList();
        }
        public List<AdsModel> GetAds()
        {
            return _adsRepository.GetAll().ToList();
        }
    }
}
