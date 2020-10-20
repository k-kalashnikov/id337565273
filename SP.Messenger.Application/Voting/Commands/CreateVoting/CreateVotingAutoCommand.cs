using MediatR;
using SP.Messenger.Common.Implementations;
using System;
using System.Collections.Generic;

namespace SP.Messenger.Application.Voting.Commands.CreateVoting
{
  public class CreateVotingAutoCommand : IRequest<ProcessingResult<Guid>>
  {
    public CreateVotingAutoCommand(string name, IEnumerable<ResponseVariantCommand> responseVariants, IEnumerable<long> accounts)
    {
      Name = name;
      ResponseVariants = responseVariants;
      Accounts = accounts;
    }

    public string Name { get; }
    public IEnumerable<ResponseVariantCommand> ResponseVariants { get; }
    public IEnumerable<long> Accounts { get; }

    public static CreateVotingAutoCommand Create(string name, IEnumerable<ResponseVariantCommand> responseVariants, IEnumerable<long> accounts)
        => new CreateVotingAutoCommand(name, responseVariants, accounts);
  }

  public class ResponseVariantCommand
  {
    public ResponseVariantCommand(string variantName, Guid? decisionId, IEnumerable<Contractor> contractors)
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

    public string ProtocolNumber { get; set; }
  }
}
