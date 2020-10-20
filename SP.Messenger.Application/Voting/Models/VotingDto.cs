using SP.Market.Core.Extension;
using SP.Messenger.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Application.Voting.Models
{
  public class VotingDto
  {
    public VotingDto(Guid id, long createBy, string name, IReadOnlyCollection<ResponseVariantDto> responseVariantDto, IReadOnlyCollection<VotedByDto> votedCollection, bool isClosed)
    {
      Id = id;
      CreateBy = createBy;
      Name = name;
      ResponseVariants = responseVariantDto;
      VotedCollection = votedCollection;
      IsClosed = isClosed;
    }

    public Guid Id { get; }
    public long CreateBy { get; }
    public string Name { get; }
    public bool IsClosed { get; }
    public IReadOnlyCollection<ResponseVariantDto> ResponseVariants { get; }
    public IReadOnlyCollection<VotedByDto> VotedCollection { get; }

    public static VotingDto Create(Domains.Entities.Voting model)
    {
      if (model is null)
        return null;

      return new VotingDto(model.Id,
          model.CreateBy,
          model.Name,
          ResponseVariantDto.Create(model.ResponseVariants).ToList(),
          VotedByDto.Create(model.VotedCollection).ToList(),
          model.IsClosed);
    }
  }

  public class ResponseVariantDto
  {
    public ResponseVariantDto(Guid id, Guid votingId, string name, IEnumerable<VotingContractorDto> contractors)
    {
      Id = id;
      VotingId = votingId;
      Name = name;
      VotingContractor = contractors;
    }

    public Guid Id { get; }
    public Guid VotingId { get; }
    public string Name { get; }
    public IEnumerable<VotingContractorDto> VotingContractor { get; }


    public static ResponseVariantDto Create(ResponseVariant model)
    {
      var organizationContent = model.OrganizationsContent.FromJson<OrganizationContent>();
      var votingContractorDtos = organizationContent.Organizations.Select(x => new VotingContractorDto
      {
        ContractorId = x.ContractorId,
        ContractorName = x.ContractorName,
        PriceOffer = x.PriceOffer,
        DeviationBestPrice = x.DeviationBestPrice,
        Term = x.Term,
        TermDeviation = x.TermDeviation,
        DefermentPayment = x.DefermentPayment,
        DefermentDeviation = x.DefermentDeviation,
        PercentDifferentByPurchase = x.PercentDifferentByPurchase,
        PercentDifferentByBestContractorOffer = x.PercentDifferentByBestContractorOffer,
        TermLimit = x.TermLimit
      }).ToArray();
      return new ResponseVariantDto(model.Id, model.VotingId, model.Name, votingContractorDtos);
    }

    public static IEnumerable<ResponseVariantDto> Create(IEnumerable<ResponseVariant> models)
        => models.Select(Create);
  }

  public class VotedByDto
  {
    public VotedByDto(Guid id, Guid votingId, Guid? responseVariantId, long voted, string email, string firstName, string lastName, string middleName, bool? isLike)
    {
      Id = id;
      VotingId = votingId;
      ResponseVariantId = responseVariantId;
      Voted = voted;
      Email = email;
      FirstName = firstName;
      LastName = lastName;
      MiddleName = middleName;
      IsLike = isLike;
    }

    public Guid Id { get; }
    public Guid VotingId { get; }
    public Guid? ResponseVariantId { get; }
    public long Voted { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string MiddleName { get; }
    public string Email { get; }
    public bool? IsLike { get; }

    public static VotedByDto Create(Guid id, Guid votingId, Guid? responseVariantId, long voted, string email, string firstName, string lastName, string middleName, bool? isLike)
        => new VotedByDto(id, votingId, responseVariantId, voted, email, firstName, lastName, middleName, isLike);

    public static VotedByDto Create(VotedBy model)
        => new VotedByDto
        (
            model.Id,
            model.VotingId,
            model.ResponseVariantId,
            model.AccountId,
            model.Account.Login,
            model.Account.FirstName,
            model.Account.LastName,
            model.Account.MiddleName,
            model.IsLike
         );
    public static IEnumerable<VotedByDto> Create(IEnumerable<VotedBy> models)
        => models.Select(Create);
  }

  public class VotingContractorDto
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
