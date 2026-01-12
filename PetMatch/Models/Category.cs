using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PetMatch.Models
{
    public class Category
    {
        public int ID { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<Animal>? Animals { get; set; }
    }
}