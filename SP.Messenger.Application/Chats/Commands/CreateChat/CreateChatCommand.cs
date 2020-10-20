using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Domains.Entities;
using System;
using System.Collections.Generic;
using SP.Messenger.Common.Implementations;
using Serilog;

namespace SP.Messenger.Application.Chats.Commands.CreateChat
{
  public class CreateChatCommand : IRequest<ProcessingResult<long>>
  {
    public CreateChatCommand(string name, string chatTypeMnemonic, bool isActive,
                            ChatData data, ICollection<AccountMessengerDTO> accounts)
    {
      Name = name;
      ChatTypeMnemonic = chatTypeMnemonic;
      IsActive = isActive;
      Data = data;
      Accounts = accounts;
    }
    public string Name { get; }
    public string ChatTypeMnemonic { get; }
    public bool IsActive { get; }
    public ChatData Data { get; }
    public int ChatTypeId { get; }
    public ICollection<AccountMessengerDTO> Accounts { get; }

    public static CreateChatCommand Create(string name, string chatTypeMnemonic, bool isActive,
                            Guid documentId, long documentTypeId, string documentStatusMnemonic,
                            ModuleName moduleName, ICollection<AccountMessengerDTO> accounts,
                            Guid? parentDocumentId = null, long? contractorId = null)
    {
      var data = new ChatData
      {
        DocumentId = documentId.ToString(),
        DocumentTypeId = documentTypeId,
        ParentDocumentId = parentDocumentId.ToString(),
        DocumentStatusMnemonic = documentStatusMnemonic,
        Module = moduleName.ToString(),
        ContractorId = contractorId
      };
      Log.Information($"{nameof(CreateChatCommand)} created");
      return new CreateChatCommand(name, chatTypeMnemonic, isActive, data, accounts);
    }
  }
}
