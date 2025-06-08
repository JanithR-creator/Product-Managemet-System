namespace Common.Events
{
    public class ProductCommonEventDto
    {
        public Dictionary<Guid, int> ProductQuantities { get; set; } = new Dictionary<Guid, int>();
    }
}
