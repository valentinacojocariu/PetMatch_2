namespace PetMatchMobile.Models
{
    public class Animal
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; } = "default_paw.jpg";
        public string Description { get; set; }
        public double Age { get; set; }
        public string CategoryName { get; set; }
        public string ShelterName { get; set; }
        public bool IsAdopted { get; set; } 
    }
}