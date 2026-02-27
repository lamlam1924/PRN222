using BusinessObjects;

namespace Services
{
    public interface IAccountService
    {
        AccountMember? GetAccountById(string memberId);
        AccountMember? Login(string email, string password);
        bool IsAdmin(AccountMember account);
    }
}
