namespace ApplicationLayer.Dto
{
    public class ShoppingList
    {
        public int Quantity { get; set; }

        public int UnitPrice { get; set; }

        public int ProductId { get; set; }
    }
    public class OrderRequest
    {
        public int CustomerId { get; set; }

        public List<ShoppingList> Items { get; set; } = new List<ShoppingList>();
    }
}
