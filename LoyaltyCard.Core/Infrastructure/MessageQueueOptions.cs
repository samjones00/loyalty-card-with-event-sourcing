namespace LoyaltyCard.Core.Infrastructure
{
    public class MessageQueueOptions
    {
        public const string SectionName = "MessageQueue";
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
