using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Application.Accounts.Commands.CreateSuperUser
{
	public class CreateSuperUserCommand : IRequest<long>
	{
		public long AccountId { get; set; }
		public string Login { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string MiddleName { get; set; }
	}
}
