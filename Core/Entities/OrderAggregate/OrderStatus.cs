using System.Runtime.Serialization;
namespace Core.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        [EnumMember(Value="Pending")]
        Pending,
        [EnumMember(Value = "Accepted")]
        Accepted,
        [EnumMember(Value = "Rejected")]
        Rejected
    }
}

