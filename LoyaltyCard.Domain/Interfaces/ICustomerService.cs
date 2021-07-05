using System.Threading.Tasks;

namespace LoyaltyCard.Domain.Interfaces
{
    public interface ICustomerService
    {
        Task Handle<T>(T command) where T : class;
    }
}
