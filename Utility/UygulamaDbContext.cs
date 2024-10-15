using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebUygulama1.Models;

// Veri tabanında EF Tablo oluşması için ilgili model sınıflarını buraya eklemeliyiz.
namespace WebUygulama1.Utility
{
    public class UygulamaDbContext : IdentityDbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options) {

            // Aldıgımız options nesnesini base(options) aracılıgıyla üst sınıfa yani base'e (DbContext) e otomatik olarak gönderiyoruz 

        }

        // Kitab türü sınıfına tekabül eden veri tabanındaki tabloyu oluşturabilmek için buraya bir property yani özellik
        // eklememiz lazım
        public DbSet<KitapTuru> KitapTurleri { get; set; } // Bundan sonra bir migration yapmam gerekli
        // Migration: Kod tarafında (Entity Framework) yaptıgım bir işlemin Veri Tabanındaki karşılığını oluşturmam için
        // Bir migration işlemi yapmam gerekir. Paket yöneticisine add-migration KitapTurleriTablosuEkle. Sonra update-database 

        public DbSet<Kitap> Kitaplar { get; set; }
        public DbSet<Kiralama> Kiralamalar { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
