using MediatR;
using System;

namespace SP.Messenger.Application.Chats.Commands.AddNewAccountsToExistsChats
{
    public class AddNewAccountsToExistsChatsCommand : IRequest<bool>
    {
        private AddNewAccountsToExistsChatsCommand(long accountId, 
            string login, 
            string firstName, 
            string lastName, 
            string middleName,
            long? organization, 
            string[] roles,
            Guid organizationGuid)
        {
            AccountId = accountId;
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
            Organization = organization;
            Roles = roles;
            OrganizationGuid = organizationGuid;
        }

        public long AccountId { get; }
        public string Login { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }
        public long? Organization { get; }
        public string[] Roles { get; }
        public Guid OrganizationGuid { get; }

        public static AddNewAccountsToExistsChatsCommand Create(long accountId, 
            string login, 
            string firstName, 
            string lastName, 
            string middleName, 
            long? organization, 
            string[] roles,
            Guid organizationGuid)
        => new AddNewAccountsToExistsChatsCommand(accountId, login, firstName, lastName, 
        middleName, organization, roles, organizationGuid);
    }
}