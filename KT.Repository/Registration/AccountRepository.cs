using KT.Exceptions.API;
using KT.Interfaces.Repositories;
using KT.Models.DB.User;

namespace KT.Repositories
{
    public class AccountRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AccountModel> _accountRepository;
        private readonly IRepository<ApplicationUserModel> _applicationUserRepository;

        public AccountRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = _unitOfWork.Repository<AccountModel>();
            _applicationUserRepository = _unitOfWork.Repository<ApplicationUserModel>();
        }

        public async Task<AccountModel> CreateAccountWOC(ApplicationUserModel applicationUserModel)
        {
            var accountModel = new AccountModel()
            {
                ApplicationUser = applicationUserModel,
                AccountName = applicationUserModel.FirstName.Trim() + " " + applicationUserModel.LastName.TrimEnd(),
            };
            accountModel = await _accountRepository.InsertAsync(accountModel);
            return accountModel;
        }

        public AccountModel GetAccount(long accountId)
        {
            var account = _accountRepository.GetAll().Where(r => r.AccountId == accountId).FirstOrDefault();
            if (account == null)
            {
                throw new NonConnectivityException("Invalid Account Id is provided");
            }
            return account;
        }

        public AccountModel GetAccountByUserId(long userId)
        {
            var account = _accountRepository.GetAll().Where(r => r.UserId == userId).FirstOrDefault();
            if (account == null)
            {
                throw new NonConnectivityException("Invalid Account Id is provided");
            }
            return account;
        }

    }
}