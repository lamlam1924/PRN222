using BusinessObjects;

namespace DataAccessObjects
{
    public class AccountDAO
    {
        private static AccountDAO? instance = null;
        private static readonly object instanceLock = new object();

        private AccountDAO() { }

        public static AccountDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AccountDAO();
                    }
                    return instance;
                }
            }
        }

        public AccountMember? GetAccountById(string memberId)
        {
            AccountMember? account = null;
            try
            {
                using var context = new MyStoreContext();
                account = context.AccountMembers.FirstOrDefault(a => a.MemberId == memberId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }

        public AccountMember? CheckLogin(string email, string password)
        {
            AccountMember? account = null;
            try
            {
                using var context = new MyStoreContext();
                account = context.AccountMembers
                    .FirstOrDefault(a => a.EmailAddress == email && a.MemberPassword == password);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return account;
        }
    }
}
