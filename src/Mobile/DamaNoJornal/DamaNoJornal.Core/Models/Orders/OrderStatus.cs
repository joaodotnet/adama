namespace DamaNoJornal.Core.Models.Orders
{
    public enum OrderStatus
    {
        PENDING,
        SUBMITTED, //Feito a transfência
        PREPARING,
        SENT,
        DELIVERED,
        CANCELED
    }
}