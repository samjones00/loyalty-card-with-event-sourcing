namespace LoyaltyCard.Providers.Cosmos.Extensions
{
    public static class CosmosExtensions
    {
        public static string GetContainerName<T>() => $"{typeof(T).Name}";
    }
}
