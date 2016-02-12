using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Please indicate your name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please specify your first delivery address")]
        [DisplayName("Primary address")]
        public string Line1 { get; set; }
        
        [DisplayName("Second address")]
        public string Line2 { get; set; }

        [DisplayName("Third address")]
        public string Line3 { get; set; }

        [Required(ErrorMessage = "Please specify a city")]
        [DisplayName("Specify a city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please specify a country")]
        [DisplayName("Specify a country")]
        public string Country { get; set; }

        public bool GiftWrap { get; set; }
    }
}
