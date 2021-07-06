namespace LoyaltyCard.Domain.Interfaces
{
    public interface ISerializer
    {
        byte[] SerializeToByteArray(object obj);

        object DeserializeFromByteArray(byte[] data, string typeName);

        string SerializeToString(object obj);

        object DeserializeFromString(string json, string typeName);
    }
}
