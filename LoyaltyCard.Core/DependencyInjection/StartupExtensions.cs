using Azure.Storage.Queues;
using LoyaltyCard.Core.Infrastructure;
using LoyaltyCard.Core.Services;
using LoyaltyCard.Domain.Extensions;
using LoyaltyCard.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoyaltyCard.Core.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddQueueClient(this IServiceCollection services, IConfiguration configuration)
        {
            var positionOptions = new MessageQueueOptions();
            configuration.GetSection(MessageQueueOptions.SectionName).Bind(positionOptions);

            positionOptions.ConnectionString.ThrowIfNullOrEmpty(nameof(MessageQueueOptions.ConnectionString));
            positionOptions.QueueName.ThrowIfNullOrEmpty(nameof(MessageQueueOptions.QueueName));

            services.AddTransient<IQueueService, StorageQueueService>();
            services.AddTransient<QueueClient>(x =>{

                var options = new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                };

                return new QueueClient(positionOptions.ConnectionString, positionOptions.QueueName, options);
            });

            return services;
        }
    }
}
