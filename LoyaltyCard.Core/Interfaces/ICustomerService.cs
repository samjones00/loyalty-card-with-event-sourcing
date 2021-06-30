using System.Threading.Tasks;

namespace LoyaltyCard.Core.Interfaces
{
    public interface ICustomerService
    {
        Task Handle<T>(T command) where T : class;
    }
}
