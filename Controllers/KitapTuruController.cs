using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUygulama1.Models;
using WebUygulama1.Utility;

namespace WebUygulama1.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)]
    public class KitapTuruController : Controller
    {
        // DEpendency Injection: Arkada bizim için nesneler üretir. Bu oluşturulan nesneler bizim tarafımızdan kullanılıyor ama
        // bu nesneleri oluşturan şey ASP .Net mekanizmesı. Bu nesne veri tabanıyla entity framework arasında bir köprü nesnesi.
        //private readonly UygulamaDbContext _uygulamaDbContext;

        private readonly IKitapTuruRepository _kitapTuruRepository;
        public KitapTuruController(IKitapTuruRepository context)
        {
            _kitapTuruRepository = context;
        }
        public IActionResult Index()
        {
            // Verileri Veri Tabanından Çekme
            List<KitapTuru> objKitapTuruList = _kitapTuruRepository.GetAll().ToList();
            return View(objKitapTuruList); // Listeyi View a gönderdik
        }
        // Kİtap Eklemek için
        public IActionResult Ekle()
        {
            return View();
        }

        // Butona bastıktan sonra formadaki post sayesinde nesne veri tabanına ekleniyor.
        [HttpPost]
        public IActionResult Ekle(KitapTuru kitapTuru)
        {
            // Eğer modeldeki koşullar sağlanmışsa girebilir.
            if (ModelState.IsValid)
            {
                _kitapTuruRepository.Ekle(kitapTuru);// Bu nesneyi eklemeye hazırlan
                _kitapTuruRepository.Kaydet();// Ekleme burada yapılıyor. Eğer bunu yapmazsak veriler database e eklenmez
                TempData["basarili"] = "Yeni Kitap Türü başarıyla oluşturuldu!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                return RedirectToAction("Index", "KitapTuru"); // Index sayfasına dönüş yapmayı sağlar. Hangi controller olduğunu yazmalıyız
            }
            return View();
        }
        public IActionResult Guncelle(int? id)
        {
            // Id primary key olduğu için null ya da 0 olamaz. Yani bu if statement ına gerek yok ama yine de öğrenmek için koydum
            if (id == null || id == 0)
            {
                return NotFound();
            }
            
            // Dependency injection (_uygulamaDbContext) sayesinde tek satırda veritabanı ile entity frameworkü bağlayabiliyoruz
            // Eğer id yi bulamazsa null döner. Null dönerse kitapTuruVt nul olur. Nullable (KitapTuru?) yaparsak uyarı almayız
            //Bu sayede SQL için kendimiz elle sorgu yazmamıyoruz, Entity Framework bizim için kendi yapıyor. Kendi içinde select 
            // oluşturup çekiyor
            
            KitapTuru? kitapTuruVt = _kitapTuruRepository.Get(u=>u.Id==id);

            if(kitapTuruVt == null)
            {
                return NotFound();
            }

            // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
            return View(kitapTuruVt);
        }

        [HttpPost]
        public IActionResult Guncelle(KitapTuru kitapTuru)
        {
            if (ModelState.IsValid)
            {
                _kitapTuruRepository.Guncelle(kitapTuru);
                _kitapTuruRepository.Kaydet();
                TempData["basarili"] = "Yeni Kitap Türü başarıyla güncellendi!"; // Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                return RedirectToAction("Index", "KitapTuru");
            }
            return View();
        }

        public IActionResult Sil(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            KitapTuru? kitapTuruVt = _kitapTuruRepository.Get(u=>u.Id==id);

            if (kitapTuruVt == null)
            {
                return NotFound();
            }

            // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
            return View(kitapTuruVt);
        }

        [HttpPost, ActionName("Sil")]
        public IActionResult SilPOST(int? id)
        {
            KitapTuru? kitapTuru = _kitapTuruRepository.Get(u => u.Id == id);
            if (kitapTuru == null)
            {
                return NotFound();
            }
            _kitapTuruRepository.Sil(kitapTuru);
            _kitapTuruRepository.Kaydet();
            TempData["basarili"] = "Kayıt Silme işlemi başarılı!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek. 
                                                                  // Bunu Index sayfasında yapacağız. Sadece bir kereliğine çalışır.
                                                                  // View ile controller, controller ile view arasında veri taşımak için kullanılan bir yapıdır.

            return RedirectToAction("Index", "KitapTuru");
        }
    }
}
