using System;
using BackendWebApi.Entities;
using System.Linq;

namespace BackendWebApi.Services
{
    public interface IMessagesService
    {
        string GetDecryptedMessage(string message, string key);
        string GetMessageById(int id);
    }

    public class MessagesService : IMessagesService
    {
        private readonly MessagesDbContext _dbContext;

        public MessagesService(MessagesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetMessageById(int id)
        {
            var message = _dbContext
                .EncryptedMessages
                .FirstOrDefault(r => r.Id == id);

            if (message != null && string.IsNullOrEmpty(message.Payload))
                throw new Exception($"No message found with ID:{id}");

            var result = message.Payload;
            return result;
        }

        public string GetDecryptedMessage(string message, string key)
        {
            return CipherUtility.Decrypt(key, message);
        }
    }
}