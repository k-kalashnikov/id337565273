using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SP.Consumers.Models
{
    public class AccountsChatResponse
    {
        public AccountsChatResponse(long accountId, string login,
             string firstName, string lastName, string middleName)
        {
            AccountId = accountId;
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        
        public static AccountsChatResponse Create(long accountId, string login,
            string firstName, string lastName, string middleName)
        {
            return new AccountsChatResponse(accountId, login, firstName, lastName, middleName);
        }
    }
}
