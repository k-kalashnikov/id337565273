using Microsoft.EntityFrameworkCore;
using SP.Messenger.Domains.Entities;
using System.Linq;
using SP.Messenger.Domains.Views;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SP.Messenger.Persistence
{
  public class MessengerInitializer
  {
    private const string ProviderNameConst = "Microsoft.EntityFrameworkCore.InMemory";
    public static void InitializeAsync(MessengerDbContext context)
    {
      if (!context.Database.ProviderName.Equals(ProviderNameConst))
      {
       //await context.Database.MigrateAsync();
      }


      var initializer = new MessengerInitializer();
      SeedEverything(context);
    }

    private static void SeedEverything(MessengerDbContext context)
    {
      if (!context.Database.ProviderName.Equals(ProviderNameConst))
      {
        SeedSpecificationChatsView(context);
        SeedAccountChatView(context);
        SeedLastMessagesByStockOrder(context);
        SeedGetChatByStatusDoc(context);
        SeedGetChatsWithChilds(context);
        SeedFiles(context);
        SeedStateVotingView(context);
      }

      SeedChatBotAccount(context);
      SeedDocumentType(context);
      SeedMessageType(context);
      SeedChatTypes(context);
      SeedCreateStaticChat(context);
      SeedChatTypeRoles(context);
    }

    private static void SeedChatBotAccount(MessengerDbContext context)
    {
      var chatBot = context.Accounts.FirstOrDefault(x => x.Login.Equals("bot@lht.spb.ru"));
      if (chatBot is null)
      {
        var account = new Account
        {
          Login = "bot@lht.spb.ru",
          AccountId = 999999,
          FirstName = "Bot",
          LastName = string.Empty,
          MiddleName = string.Empty,
          IsDisabled = false
        };
        context.Accounts.Add(account);
        context.SaveChanges();
      }
    }

    private static void SeedDocumentType(MessengerDbContext context)
    {
      if (context.DocumentTypes.Any())
        return;

      var documentTypes = new[]
      {
                new DocumentType { Name = "Specification", IsDisabled = false },
                new DocumentType { Name = "Logistic", IsDisabled = false },
                new DocumentType { Name = "Bid", IsDisabled = false },
                new DocumentType { Name = "BlueReport", IsDisabled = false },
                new DocumentType { Name = "MarketVote", IsDisabled = false },
                new DocumentType { Name = "MarketJournalPurchase", IsDisabled = false },
                new DocumentType { Name = "Organization", IsDisabled = false }
            };

      context.DocumentTypes.AddRange(documentTypes);
      context.SaveChanges();
    }

    private static void SeedMessageType(MessengerDbContext context)
    {
      if (context.MessageTypes.Any())
        return;

      var messageTypes = new[]
      {
                new MessageType { Name = "User", IsDisabled = false },
                new MessageType { Name = "System", IsDisabled = false },
                new MessageType { Name = "Bot", IsDisabled = false },
                new MessageType { Name = "File", IsDisabled = false },
                new MessageType { Name = "Vote", IsDisabled = false }
            };

      context.MessageTypes.AddRange(messageTypes);
      context.SaveChanges();
    }

    private static void SeedChatTypes(MessengerDbContext context)
    {
      if (context.ChatTypes.Any())
        return;

      var chatTypes = new[]
      {
                new ChatType { Mnemonic = "module.bidCenter.chat.common", Description = "Общий" },
                new ChatType { Mnemonic = "module.bidCenter.chat.private", Description = "Приватный" },
                new ChatType { Mnemonic = "module.bidCenter.chat.contract", Description = "Договор" },
                new ChatType { Mnemonic = "module.bidCenter.chat.stage", Description = "Этап" },
                new ChatType { Mnemonic = "module.bidCenter.chat.agreement", Description = "Доп. соглашение" },
                new ChatType { Mnemonic = "module.bidCenter.chat.rdp", Description = "Удаленный доступ" },
                new ChatType { Mnemonic = "module.bidCenter.chat.history", Description = "История" },
                new ChatType { Mnemonic = "module.bidCenter.chat.support", Description = "Тех. поддержка" },
                new ChatType { Mnemonic = "module.report.chat.common", Description = "Общий" },
                new ChatType { Mnemonic = "module.logistic.chat.common", Description = "Общий" },
                new ChatType { Mnemonic = "module.market.chat.vote", Description = "Комиссия" },
                new ChatType { Mnemonic = "module.market.journal.purchase", Description = "Закупки" },
                new ChatType { Mnemonic = "module.market.pusrchase.chat.private", Description = "Приватный закупки" },
                new ChatType { Mnemonic = "module.mdm.chat.organization.chat.public", Description = "Чат организации" }
            };

      context.ChatTypes.AddRange(chatTypes);
      context.SaveChanges();
    }

    private static void SeedChatTypeRoles(MessengerDbContext context)
    {
      if (context.ChatTypeRoles.Any())
        return;

      var chatTypeRoles = new List<ChatTypeRoles>();
      var listRoles = new[]
      {
                new ChatRolesLight {ChatTypeMnemonic = "module.market.journal.purchase", RoleMnemonic = "manager.module.market" },
                new ChatRolesLight {ChatTypeMnemonic = "module.market.journal.purchase", RoleMnemonic = "superuser.module.platform" },

                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.common", RoleMnemonic = "customer.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.common", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.common", RoleMnemonic = "executor.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.common", RoleMnemonic = "admin.module.logistic" },


                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.private", RoleMnemonic = "customer.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.private", RoleMnemonic = "LOGIST" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.private", RoleMnemonic = "admin.module.logistic" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.private", RoleMnemonic = "PERFORMER" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.private", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.private", RoleMnemonic = "executor.module.bidCenter" },

                new ChatRolesLight {ChatTypeMnemonic = "module.market.pusrchase.chat.private", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.market.pusrchase.chat.private", RoleMnemonic = "manager.module.market" },
                new ChatRolesLight {ChatTypeMnemonic = "module.market.pusrchase.chat.private", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.market.pusrchase.chat.private", RoleMnemonic = "contractor.module.platform" },

                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.contract", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.contract", RoleMnemonic = "customer.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.contract", RoleMnemonic = "executor.module.bidCenter" },

                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.stage", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.stage", RoleMnemonic = "customer.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.stage", RoleMnemonic = "executor.module.bidCenter" },

                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.agreement", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.agreement", RoleMnemonic = "customer.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.bidCenter.chat.agreement", RoleMnemonic = "executor.module.bidCenter" },

                new ChatRolesLight {ChatTypeMnemonic = "module.logistic.chat.common", RoleMnemonic = "superuser.module.platform" },
                new ChatRolesLight {ChatTypeMnemonic = "module.logistic.chat.common", RoleMnemonic = "customer.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.logistic.chat.common", RoleMnemonic = "executor.module.bidCenter" },
                new ChatRolesLight {ChatTypeMnemonic = "module.logistic.chat.common", RoleMnemonic = "LOGIST" },
                new ChatRolesLight {ChatTypeMnemonic = "module.logistic.chat.common", RoleMnemonic = "admin.module.logistic" },
            };

      var chatTypes = context.ChatTypes.ToArray();
      foreach (var item in listRoles)
      {
        var chatType = chatTypes.FirstOrDefault(x => x.Mnemonic.Equals(item.ChatTypeMnemonic));
        chatTypeRoles.Add(new ChatTypeRoles(chatType.ChatTypeId, item.RoleMnemonic));
      }

      context.ChatTypeRoles.AddRange(chatTypeRoles);
      context.SaveChanges();
    }

    private static void SeedCreateStaticChat(MessengerDbContext context)
    {
      var documentChatIdPurchase = "07e1136b-74d6-427c-85b2-2791505a8334";

      var chatPurchase = context.ChatView.FirstOrDefault(x => x.DocumentId == documentChatIdPurchase);
      if (chatPurchase is null)
      {
        var data = new ChatData
        {
          ContractorId = null,
          DocumentId = documentChatIdPurchase,
          DocumentStatusMnemonic = string.Empty,
          DocumentTypeId = 6,
          Module = "Market",
          ParentDocumentId = null
        };
        var chat = new Chat
        {
          ChatTypeId = 12,
          IsDisabled = false,
          ParentId = null,
          Name = "Закупки",
          Data = JsonConvert.SerializeObject(data)
        };
        var modelDbChat = context.Chats.Add(chat);
        context.SaveChanges();
        var chatId = modelDbChat.Entity.ChatId;

        var document = new Document
        {
          DocumentId = Guid.Parse(documentChatIdPurchase),
          DocumentTypeId = 6
        };
        context.Documents.Add(document);

        chat.Documents.Add(new ChatDocument
        {
          ChatId = chatId,
          DocumentId = document.DocumentId
        });

        chat.Accounts.Add(new AccountChat
        {
          AccountId = 1,
          ChatId = chatId
        });
        chat.Accounts.Add(new AccountChat
        {
          AccountId = 12144,
          ChatId = chatId
        });
        context.SaveChanges();
      }
    }
    private static void SeedSpecificationChatsView(MessengerDbContext context)
    {
      var scriptCreate =
       $@"CREATE VIEW {SpecificationView.View} AS SELECT ch.'ChatID' as 'ChatID',
               ch.'Name' as 'Name',
               chd.'DocumentID' as 'DocumentID',
               ch.'IsDisabled' as 'IsDisabled'
              FROM 'Chats' ch
                JOIN 'ChatDocument' chd ON chd.'ChatID' = ch.'ChatID'
                JOIN 'Documents' doc ON doc.'DocumentID' = chd.'DocumentID'
             WHERE doc.'DocumentTypeID' = 1;"
           .Replace("'", "\"");

      ExecuteSqlCommands(context, SpecificationView.View, scriptCreate);
    }

    private static void SeedAccountChatView(MessengerDbContext context)
    {
      var scriptCreate =
      $@"CREATE VIEW {AccountChatView.View} AS SELECT 
                acc.'AccountID' as 'AccountID',
                acc.'Login' as 'Login',
                acc.'FirstName' as 'FirstName',
                acc.'LastName' as 'LastName',
                acc.'MiddleName' as 'MiddleName',
                chat.'ChatID' as 'ChatID',
                acc.'IsDisabled' as 'IsDisabled'
             FROM 'Accounts' acc
              INNER JOIN 'AccountChat' as ac on ac.'AccountID' = acc.'AccountID'
              INNER JOIN 'Chats' as chat ON chat.'ChatID' = ac.'ChatID';"
          .Replace("'", "\"");

      ExecuteSqlCommands(context, AccountChatView.View, scriptCreate);
    }

    private static void SeedLastMessagesByStockOrder(MessengerDbContext context)
    {
      var scriptCreate = $@"CREATE VIEW {LastMessagesView.View} AS  SELECT msg.'ChatID',
            chat.'ParentID',
            chat.'Name',
            chat.'ChatTypeID',
            msg.'AccountID',
            msg.'CreateDate',
            msg.'MessageID',
            msg.'MessageTypeID',
            msg.'Content',
            chatdoc.'DocumentID',
            ct.'Mnemonic',
            msg.'Pined'
            FROM 'Messages' msg
                JOIN 'Chats' chat ON chat.'ChatID' = msg.'ChatID'
            JOIN 'ChatDocument' chatdoc ON chatdoc.'ChatID' = chat.'ChatID'
            JOIN 'Documents' docs ON chatdoc.'DocumentID' = docs.'DocumentID'
            JOIN 'ChatTypes' ct ON chat.'ChatTypeID' = ct.'ChatTypeID'
            WHERE (msg.'MessageID' IN ( SELECT max('Messages'.'MessageID') AS max
            FROM 'Messages'
            GROUP BY 'Messages'.'ChatID'))		  
            UNION
                SELECT msg.'ChatID',
            chat.'ParentID',
            chat.'Name',
            chat.'ChatTypeID',
            msg.'AccountID',
            msg.'CreateDate',
            msg.'MessageID',
            msg.'MessageTypeID',
            msg.'Content',
            chatdoc.'DocumentID',
            ct.'Mnemonic',
            msg.'Pined'
            FROM 'Messages' msg
                JOIN 'Chats' chat ON chat.'ChatID' = msg.'ChatID'
            JOIN 'ChatDocument' chatdoc ON chatdoc.'ChatID' = chat.'ChatID'
            JOIN 'Documents' docs ON chatdoc.'DocumentID' = docs.'DocumentID'
            JOIN 'ChatTypes' ct ON chat.'ChatTypeID' = ct.'ChatTypeID'
            WHERE msg.'Pined'=true  
            order by 'CreateDate';"
          .Replace("'", "\"");

      ExecuteSqlCommands(context, LastMessagesView.View, scriptCreate);
    }

    private static void SeedGetChatByStatusDoc(MessengerDbContext context)
    {
      var scriptCreate = $@"CREATE VIEW {ChatView.View} AS SELECT Chat.'ChatID',
            Chat.'ParentID',
            Chat.'Name',
            Chat.'IsDisabled',
            CT.'Mnemonic',
            Chat.'Data'->>@DocumentId@ as 'DocumentId',
            Chat.'Data'->>@ParentDocumentId@ as 'ParentDocumentId',
            Chat.'Data'->>@DocumentStatusMnemonic@ as 'DocumentStatusMnemonic',
            CAST(Chat.'Data'->>@ContractorId@ as bigint) as 'ContractorId',
            Chat.'Data'->>@Module@ as 'Module'
            FROM 'Chats' AS Chat
            INNER JOIN 'ChatTypes' CT on chat.'ChatTypeID' = CT.'ChatTypeID';"
          .Replace("'", "\"")
          .Replace("@", "'");
      ExecuteSqlCommands(context, ChatView.View, scriptCreate);
    }

    private static void SeedGetChatsWithChilds(MessengerDbContext context)
    {
      var scriptCreate = $@"CREATE VIEW {ChatsParentsView.View} AS WITH RECURSIVE chats AS (
            SELECT 'ChatID','ParentID', 'Data' ->> @DocumentId@ as 'DocumentId'
            FROM 'Chats'
            UNION ALL
            SELECT chatChild.'ChatID', chatChild.'ParentID', chats.'DocumentId'
            FROM chats     	
            JOIN 'Chats' chatChild ON chatChild.'ParentID' = chats.'ChatID')
            SELECT 'ChatID','ParentID', 'DocumentId' as 'DocumentID'  FROM chats;"
          .Replace("'", "\"")
          .Replace("@", "'");
      ExecuteSqlCommands(context, ChatsParentsView.View, scriptCreate);
    }

    private static void SeedFiles(MessengerDbContext context)
    {
      var scriptCreate = $@"CREATE VIEW {MessageFilesView.View} AS select msg.'MessageID',
            msg.'ChatID',
            msg.'AccountID',
            msg.'CreateDate',
            msg.'Content'->@Tags@->> @DocumentId@ as DocumentId,
            msg.'Content'->@File@->> @Url@ as Url,
            msg.'Content'->@File@->> @Length@ as Length,
            msg.'Content'->@File@->> @Filename@ as Filename,
            msg.'Content'->@File@->> @Extension@ as Extension,
            msg.'Content'->@File@->> @ContentType@ as ContentType
            from 'Messages' as msg
            where msg.'Content'-> @File@->> @Filename@ is not null;"
          .Replace("'", "\"")
          .Replace("@", "'");
      ExecuteSqlCommands(context, MessageFilesView.View, scriptCreate);
    }

    private static void SeedStateVotingView(MessengerDbContext context)
    {
      var scriptCreate = $@"CREATE VIEW {StateVotingView.View} AS SELECT vb.'ResponseVariantId', vb.'AccountId', vb.'VotingId', vb.'IsLike',vb.'Comment'
                                FROM public.'ResponseVariant' as resV
                                left join public.'VotedBy' as vb on vb.'VotingId' = resV.'VotingId'"
                      .Replace("'", "\"")
                      .Replace("@", "'");
      ExecuteSqlCommands(context, StateVotingView.View, scriptCreate);
    }

    private static void ExecuteSqlCommands(MessengerDbContext context, string viewName, string scriptCreateForNewView)
    {
      var script = $"SELECT * FROM INFORMATION_SCHEMA.views WHERE table_name='{viewName}'";

      bool hasView;

      using (var command = context.Database.GetDbConnection().CreateCommand())
      {
        command.CommandText = script;
        context.Database.OpenConnection();
        using (var result = command.ExecuteReader())
          hasView = result.HasRows;
        if (!hasView)
          context.Database.ExecuteSqlRaw(scriptCreateForNewView);
      }
    }
  }

  public class ChatRolesLight
  {
    public string ChatTypeMnemonic { get; set; }
    public string RoleMnemonic { get; set; }
  }
}
