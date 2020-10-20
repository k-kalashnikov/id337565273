using System;
using System.Linq;
using Newtonsoft.Json;
using SP.Messenger.Common.Settings;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;

namespace SP.Messenger.Application.Files.Models
{
    public class FileShortDto
    {
        [JsonProperty("messageId")]
        public long MessageId { get; set; }
        [JsonProperty("chatId")]
        public long ChatId { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty("author")]
        public Author Author { get; set; }
        [JsonProperty("documentId")]
        public Guid DocumentId { get; set; }
        [JsonProperty("length")]
        public long Length { get; set; }
        [JsonProperty("extension")]
        public string Extension { get; set; }

        public static FileShortDto Create(MessageFilesView view, Settings settings)
        {
            if (view is null) return null;
            return new FileShortDto
            {
                MessageId = view.MessageId,
                ChatId = view.ChatId,
                DocumentId = Guid.Parse(view.DocumentId),
                CreatedDate = view.CreateDate,
                Filename = view.Filename,
                Extension = view.Extension,
                Url = $"{settings.FileServer.Storage}{view.Url}",
                Length = long.Parse(view.Length),
                ContentType = view.ContentType,
                Author = Author.Create(view.Account)
            };
        }

        public static FileShortDto[] Create(MessageFilesView[] views, Settings settings)
        {
            if (views is null) return new FileShortDto[] { };
            return views.Select(x=>Create(x, settings)).ToArray();
        }
    }

    public class Author
    {
        [JsonProperty("accountId")]
        public long AccountId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        public static Author Create(Account model)
        {
            if (model is null) return null;
            return new Author
            {
                AccountId = model.AccountId,
                Email = model.Login,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName
            };
        }
    }
}