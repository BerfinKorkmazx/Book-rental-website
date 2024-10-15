using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebUygulama1.Models
{
    public class KitapTuru
    {
        [Key]   // primary key ---- Basına Key yazmadan da Id kendini primary key yapıyor. Id nin başındaki I nın büyük olmasındsan dolayı
        public int Id { get; set; }

        [Required(ErrorMessage ="Kitap Türü Adı boş bırakılamaz!")]  // not null
        [MaxLength(25)]
        [DisplayName("Kitap Türü Adı")]
        public string Ad {  get; set; }
    }
}
