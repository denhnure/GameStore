using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GameStore.Domain.Entities
{
    public class Game
    {
        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [Required(ErrorMessage = "Please, enter game details")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Please, enter game description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please, enter game category")]
        public string Category { get; set; }

        [Display(Name = "Price (RUB)")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please, enter a positive value for price")]
        public decimal Price { get; set; }

        public byte[] ImageData { get; set; }

        public string ImageMimeType { get; set; }
    }
}
