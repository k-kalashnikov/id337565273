using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Domains.Entities
{
  public class ResponseVariant
  {
    private ResponseVariant()
    {

    }
    public ResponseVariant(Guid votingId, string name, Guid? decisionId, IEnumerable<Contractor> organizations)
    {
      Id = Guid.NewGuid();
      VotingId = votingId;
      Name = name;
      DecisionId = decisionId;
      OrganizationsContent = JsonConvert.SerializeObject(new OrganizationContent(organizations));
    }
    public Guid Id { get; private set; }
    public Guid? DecisionId { get; private set; } // предложение
    public Guid VotingId { get; private set; }
    public string Name { get; private set; }
    public string OrganizationsContent { get; private set; }
  }

  public class OrganizationContent
  {
    protected OrganizationContent()
    {

    }
    public OrganizationContent(IEnumerable<Contractor> organizations)
    {
      Organizations = organizations.ToArray();
    }
    public Contractor[] Organizations { get; private set; }
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
