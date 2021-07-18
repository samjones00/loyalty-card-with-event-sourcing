namespace LoyaltyCard.Domain.Models
{
    public class ValueModel<T>
    {
        private readonly T _value;

        public ValueModel(T data) => _value = data;

        public ValueModel() { }

        public T Value => _value;

        public bool HasValue => _value != null;
    }
}
