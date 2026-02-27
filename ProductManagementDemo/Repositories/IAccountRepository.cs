using BusinessObjects;

namespace Repositories
{
    public interface IAccountRepository
    {
        AccountMember? GetAccountById(string memberId);
        AccountMember? CheckLogin(string email, string password);
    }
}
