using MediatR;
using System;
using System.Collections.Generic;

namespace SP.Messenger.Application.Chats.Commands.CreateVoteAutoChat
{
  public class CreateVoteAutoChatCommand : IRequest<bool>
  {
    public CreateVoteAutoChatCommand(string name, Guid documentId, Guid? parentDocumentId, IEnumerable<ResponseVariantChatCommand> responseVariants, IEnumerable<long> accounts)
    {
      Name = name;
      DocumentId = documentId;
      ParentDocumentId = parentDocumentId;
      ResponseVariants = responseVariants;
      Accounts = accounts;
    }
    public CreateVoteAutoChatCommand(string name, Guid documentId, Guid? parentDocumentId, IEnumerable<ResponseVariantChatCommand> responseVariants, IEnumerable<long> accounts, string protocolNumber, DateTime? protocolStartDate)
    {
      Name = name;
      DocumentId = documentId;
      ParentDocumentId = parentDocumentId;
      ResponseVariants = responseVariants;
      Accounts = accounts;
      ProtocolNumber = protocolNumber;
      ProtocolStartDate = protocolStartDate;
    }

    public string Name { get; }
    public Guid DocumentId { get; }
    public Guid? ParentDocumentId { get; }
    public IEnumerable<ResponseVariantChatCommand> ResponseVariants { get; }
    public IEnumerable<long> Accounts { get; }
    public string ProtocolNumber { get; }
    public DateTime? ProtocolStartDate { get; set; }

    public static CreateVoteAutoChatCommand Create(string name, Guid documentId, Guid? parentDocumentId, IEnumerable<ResponseVariantChatCommand> responseVariants, IEnumerable<long> accounts)
        => new CreateVoteAutoChatCommand(name, documentId, parentDocumentId, responseVariants, accounts);
    public static CreateVoteAutoChatCommand Create(string name, Guid documentId, Guid? parentDocumentId, IEnumerable<ResponseVariantChatCommand> responseVariants, IEnumerable<long> accounts, string protocolNumber, DateTime? protocolStartDate)
      => new CreateVoteAutoChatCommand(name, documentId, parentDocumentId, responseVariants, accounts);
  }

  public class ResponseVariantChatCommand
  {
    public ResponseVariantChatCommand(string variantName, Guid? decisionId, IEnumerable<Contractor> contractors)
    {
      VariantName = variantName;
      DecisionId = decisionId;
      Contractors = contractors;
    }

    public string VariantName { get; }
    public Guid? DecisionId { get; }

    public IEnumerable<Contractor> Contractors { get; }
  }

  public class Contractor
  {
    public string ContractorName { get; set; }
    public long ContractorId { get; set; }

    public decimal? PriceOffer { get; set; }
    public decimal? DeviationBestPrice { get; set; }
    public DateTime? Term { get; set; }
    public int? TermDeviation { get; set; }

    public int? DefermentPayment { get; set; }
    public int? DefermentDeviation { get; set; }
    public decimal? PercentDifferentByPurchase { get; set; }
    public decimal? PercentDifferentByBestContractorOffer { get; set; }

    public long? TermLimit { get; set; }
  }
}
