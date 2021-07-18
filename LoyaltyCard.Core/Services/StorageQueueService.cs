using Azure.Storage.Queues;
using LoyaltyCard.Domain.Extensions;
using LoyaltyCard.Domain.Interfaces;
using LoyaltyCard.Domain.Models;
using System.Threading.Tasks;

namespace LoyaltyCard.Core.Services
{
    public class StorageQueueService : IQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ISerializer _serializer;

        public StorageQueueService(ISerializer serializer, QueueClient queueClient)
        {
            _serializer = serializer.ThrowIfNull(nameof(serializer));
            _queueClient = queueClient.ThrowIfNull(nameof(queueClient));
        }

        public async Task<ValueModel<Message>> Enqueue<T>(T contract) where T : class
        {
            contract.ThrowIfNull(nameof(contract));

            var data = _serializer.SerializeToJson(contract);

            var message = new Message
            {
                Type = contract.GetType().Name,
                Data = data
            };

            var messageJson = _serializer.SerializeToJson(message);

            await _queueClient.CreateIfNotExistsAsync();

            if (await _queueClient.ExistsAsync())
            {
                await _queueClient.SendMessageAsync(messageJson);
                return new ValueModel<Message>(message);
            }

            return new ValueModel<Message>();
        }
    }
}
