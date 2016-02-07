namespace GameStore.Domain.Entities
{
    public class CartLine
    {
        public Game Game { get; set; }

        public int Quantity { get; set; }
    }
}