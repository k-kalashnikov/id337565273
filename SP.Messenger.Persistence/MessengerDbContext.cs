using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;
using SP.Messenger.Persistence.Extensions;

namespace SP.Messenger.Persistence
{
    public class MessengerDbContext : DbContext
    {
        public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
            : base(options)
        {
        }



        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatType> ChatTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<MessageBot> MessageBots { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Document> Documents { get; set; }

        public DbSet<Voting> Votings { get; set; }
        public DbSet<ResponseVariant> ResponseVariants { get; set; }
        public DbSet<VotedBy> VotedBy { get; set; }
        public DbSet<ChatTypeRoles> ChatTypeRoles { get; set; }

        public DbSet<SpecificationView> SpecificationView { get; set; }
        public DbSet<AccountChatView> AccountChatView { get; set; }
        public DbSet<LastMessagesView> LastMessagesView { get; set; }
        public DbSet<ChatView> ChatView { get; set; }
        public DbSet<ChatsParentsView> ChatsParentsView { get; set; }
        public DbSet<MessageFilesView> MessageFilesView { get; set; }
        public DbSet<StateVotingView> StateVotingView { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SpecificationView>().ToView(Domains.Entities.SpecificationView.View)
                .HasNoKey()
                .Property(e => e.ChatId).HasColumnName("ChatID");

            modelBuilder.Entity<SpecificationView>().ToView(Domains.Entities.SpecificationView.View)
                .HasNoKey()
                .Property(e => e.DocumentId).HasColumnName("DocumentID");

            modelBuilder.Entity<AccountChatView>().ToView(Domains.Views.AccountChatView.View)
                .HasNoKey()
               .Property(e => e.AccountId).HasColumnName("AccountID");
            modelBuilder.Entity<AccountChatView>().ToView(Domains.Views.AccountChatView.View)
                .HasNoKey()
                .Property(e => e.ChatId).HasColumnName("ChatID");

            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
               .Property(e => e.AccountId).HasColumnName("AccountID");
            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
                .Property(e => e.ChatId).HasColumnName("ChatID");
            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
                .Property(e => e.ParentId).HasColumnName("ParentID");
            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
               .Property(e => e.ChatTypeId).HasColumnName("ChatTypeID");
            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
                .Property(e => e.DocumentId).HasColumnName("DocumentID");
            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
                .Property(e => e.MessageId).HasColumnName("MessageID");
            modelBuilder.Entity<LastMessagesView>().ToView(Domains.Views.LastMessagesView.View)
                .HasNoKey()
                .Property(e => e.MessageTypeId).HasColumnName("MessageTypeID");

            modelBuilder.Entity<ChatView>().ToView(Domains.Views.ChatView.View)
                .HasNoKey()
                .Property(e => e.ChatId).HasColumnName("ChatID");
            modelBuilder.Entity<ChatView>().ToView(Domains.Views.ChatView.View)
                .HasNoKey()
                .Property(e => e.ParentId).HasColumnName("ParentID");
            
            modelBuilder.Entity<ChatsParentsView>().ToView(Domains.Views.ChatsParentsView.View)
                .HasNoKey()
                .Property(e => e.ChatId).HasColumnName("ChatID");
            modelBuilder.Entity<ChatsParentsView>().ToView(Domains.Views.ChatsParentsView.View)
                .HasNoKey()
                .Property(e => e.ParentId).HasColumnName("ParentID");
            modelBuilder.Entity<ChatsParentsView>().ToView(Domains.Views.ChatsParentsView.View)
                .HasNoKey()
                .Property(e => e.DocumentId).HasColumnName("DocumentID");
            
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.MessageId).HasColumnName("MessageID");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.ChatId).HasColumnName("ChatID");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.AccountId).HasColumnName("AccountID");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.DocumentId).HasColumnName("documentid");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.Url).HasColumnName("url");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.Length).HasColumnName("length");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.Filename).HasColumnName("filename");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.Extension).HasColumnName("extension");
            modelBuilder.Entity<MessageFilesView>().ToView(Domains.Views.MessageFilesView.View)
                .HasNoKey()
                .Property(e => e.ContentType).HasColumnName("contenttype");


            modelBuilder.Entity<StateVotingView>().ToView(Domains.Views.StateVotingView.View)
                .HasNoKey()
                .Property(e => e.ResponseVariantId).HasColumnName("ResponseVariantId");
            modelBuilder.Entity<StateVotingView>().ToView(Domains.Views.StateVotingView.View)
                .HasNoKey()
                .Property(e => e.AccountId).HasColumnName("AccountId");
            modelBuilder.Entity<StateVotingView>().ToView(Domains.Views.StateVotingView.View)
                .HasNoKey()
                .Property(e => e.VotingId).HasColumnName("VotingId");


           
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MessengerDbContext).Assembly);
            modelBuilder.DataTimeConfigure();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLoggerFactory(GetDbLoggerFactory()).EnableSensitiveDataLogging();
        //}

        //private static ILoggerFactory GetDbLoggerFactory()
        //{
        //    IServiceCollection serviceCollection = new ServiceCollection();
        //    serviceCollection.AddLogging(builder => builder.AddDebug().AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information));
        //    return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        //}
    }
}
