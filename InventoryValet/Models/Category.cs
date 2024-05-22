namespace InventoryValet.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public List<Item> Items { get; set; }

        // Navigation property to represent the collection of items in this category
        public ICollection<Item> Items { get; set; }
    }
}
