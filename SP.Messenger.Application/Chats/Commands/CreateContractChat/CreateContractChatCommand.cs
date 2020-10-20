using System;
using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Chats.Commands.CreateContractChat
{
    public class CreateContractChatCommand : IRequest<ProcessingResult<long>>
    {
        public Guid Id { get; }

        public Guid? Parent { get; }

        public int ContractStatusId { get; }

        public int ContractTypeId { get; }

        public long CustomerOrganizationId { get; }

        public long ContractorOrganizationId { get; }

        public string Number { get; }

        public DateTime StartDate { get; }

        public DateTime FinishDate { get; }

        public bool? SignedByCustomer { get; }

        public bool? SignedByContractor { get; }

        public CreateContractChatCommand(Guid id, Guid? parent, int contractStatusId, int contractTypeId, long customerOrganizationId, long contractorOrganizationId, string number, DateTime startDate, DateTime finishDate, bool? signedByCustomer, bool? signedByContractor)
        {
            Id = id;
            Parent = parent;
            ContractStatusId = contractStatusId;
            ContractTypeId = contractTypeId;
            CustomerOrganizationId = customerOrganizationId;
            ContractorOrganizationId = contractorOrganizationId;
            Number = number;
            StartDate = startDate;
            FinishDate = finishDate;
            SignedByCustomer = signedByCustomer;
            SignedByContractor = signedByContractor;
        }
    }
}