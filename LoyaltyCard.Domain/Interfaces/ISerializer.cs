namespace LoyaltyCard.Domain.Interfaces
{
    public interface ISerializer
    {
        byte[] SerializeToByteArray(object obj);

        object DeserializeFromByteArray(byte[] data, string typeName);

        string SerializeToJson(object obj);

        object DeserializeFromJson(string json, string typeName);
    }
}
