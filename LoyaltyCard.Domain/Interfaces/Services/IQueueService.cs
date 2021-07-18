using LoyaltyCard.Domain.Models;
using System.Threading.Tasks;

namespace LoyaltyCard.Domain.Interfaces
{
    public interface IQueueService
    {
        Task<ValueModel<Message>> Enqueue<T>(T contract) where T : class;
    }
}
