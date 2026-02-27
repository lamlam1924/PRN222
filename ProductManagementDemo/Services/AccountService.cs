using BusinessObjects;
using Repositories;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService()
        {
            _accountRepository = new AccountRepository();
        }

        public AccountMember? GetAccountById(string memberId)
        {
            if (string.IsNullOrWhiteSpace(memberId))
            {
                throw new ArgumentException("Member ID cannot be empty");
            }
            
            return _accountRepository.GetAccountById(memberId);
        }

        public AccountMember? Login(string email, string password)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty");
            }
            
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }

            var account = _accountRepository.CheckLogin(email, password);
            
            if (account == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            return account;
        }

        public bool IsAdmin(AccountMember account)
        {
            // MemberRole = 1 is Admin (example business logic)
            return account.MemberRole == 1;
        }
    }
}
