namespace InventoryValet.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public string Image { get; set; }
        //public int CategoryId { get; set; }
        //public string Size { get; set; }
    }
}
