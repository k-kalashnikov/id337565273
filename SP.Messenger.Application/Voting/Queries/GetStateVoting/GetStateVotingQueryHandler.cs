using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SP.Messenger.Domains.Entities;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Voting.Queries.GetStateVoting
{
  public class GetStateVotingQueryHandler : IRequestHandler<GetStateVotingQuery, StateVotingDto>
  {
    private readonly MessengerDbContext _context;

    public GetStateVotingQueryHandler(MessengerDbContext context)
    {
      _context = context;
    }
    public async Task<StateVotingDto> Handle(GetStateVotingQuery request, CancellationToken cancellationToken)
    {
      var message = await GetInfoByChat(request.VotingId, cancellationToken);
      var chat = await _context.ChatView.AsNoTracking()
          .FirstOrDefaultAsync(x => x.ChatId.Equals(message.ChatId), cancellationToken);

      //Проголосовавшие
      var votedBy = await _context.VotedBy.AsNoTracking()
        .Where(x => x.VotingId == request.VotingId)
        .Include(x=>x.Account)
        .ToArrayAsync(cancellationToken);
      
      var voteds = await _context.StateVotingView
        .Where(x => x.VotingId.Equals(request.VotingId))
              .ToArrayAsync(cancellationToken);

      var stateVotingObjects = new List<StateVotingObject>();
      foreach (var item in voteds)
      {
        // if (stateVotingObjects.FirstOrDefault(x => x.VariantId == item.ResponseVariantId) != null)
        //   continue;

        var accounts = new List<VotedAccounts>();
        foreach (var voted in votedBy)
        {
          accounts.Add(new VotedAccounts
          {
            AccountId = voted.AccountId,
            Email = voted.Account?.Login,
            FirstName = voted.Account?.FirstName,
            LastName = voted.Account?.LastName,
            MiddleName = voted.Account?.MiddleName,
            IsLike = voted.IsLike
          });
        }
        stateVotingObjects.Add(new StateVotingObject(item.ResponseVariantId, accounts, item.IsLike));
      }
      return new StateVotingDto(message.ChatId, message.MessageId, Guid.Parse(chat.DocumentId), request.VotingId, stateVotingObjects);
    }

    private async Task<Message> GetInfoByChat(Guid voting, CancellationToken token)
    {
      var message = _context.Messages.FromSqlRaw($@"select * from public.'Messages'
                                        where 'Content'->@VotingContent@->> @VotingId@ = @{voting}@"
                                  .Replace("'", "\"")
                                  .Replace("@", "'"));
      return await message.FirstOrDefaultAsync(token);
    }
  }
}
