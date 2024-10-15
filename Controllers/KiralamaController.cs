using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebUygulama1.Models;
using WebUygulama1.Utility;

namespace WebUygulama1.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)]
    public class KiralamaController : Controller
    {
        // DEpendency Injection: Arkada bizim için nesneler üretir. Bu oluşturulan nesneler bizim tarafımızdan kullanılıyor ama
        // bu nesneleri oluşturan şey ASP .Net mekanizmesı. Bu nesne veri tabanıyla entity framework arasında bir köprü nesnesi.
        //private readonly UygulamaDbContext _uygulamaDbContext;

        private readonly IKiralamaRepository _kiralamaRepository;
        private readonly IKitapRepository _kitapRepository;
        public readonly IWebHostEnvironment _webHostEnvironment;

        public KiralamaController(IKiralamaRepository kiralamaRepository, IKitapRepository kitapRepository, IWebHostEnvironment webHostEnvironment)
        {
            _kiralamaRepository = kiralamaRepository;
            _kitapRepository = kitapRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            //List<Kitap> objKitapList = _kitapRepository.GetAll().ToList();
            List<Kiralama> objKiralamaList = _kiralamaRepository.GetAll(includeProps:"Kitap").ToList();

            return View(objKiralamaList); // Listeyi View a gönderdik
        }
        // Kİtap Eklemek için
        public IActionResult EkleGuncelle(int? id)
        {
            // Verileri Veri Tabanından Çekme
            IEnumerable<SelectListItem> KitapList = _kitapRepository.GetAll()
                .Select(k => new SelectListItem
                {
                    Text = k.KitapAdi,
                    Value = k.Id.ToString()
                });

            // ViewBag Controller dan çekilen verilerin View katmanına aktarılmasını sağlar. KÖprü gibi veri aktarır fakat
            // tersi olmaz. Yani View den Contorller an veri aktarılmaz. Controller gitti KitapTuruRepository den
            // kitap türlerini aldı daha sonra ViewBag in içine koydu. ViewBag de View e gönderdi
            ViewBag.KitapList = KitapList;

            if(id == null || id == 0)
            {
                // ekle
                return View();
            }
            else
            {
                //güncelle 

                // Dependency injection (_uygulamaDbContext) sayesinde tek satırda veritabanı ile entity frameworkü bağlayabiliyoruz
                // Eğer id yi bulamazsa null döner. Null dönerse kitapVt nul olur. Nullable (Kitap?) yaparsak uyarı almayız
                //Bu sayede SQL için kendimiz elle sorgu yazmamıyoruz, Entity Framework bizim için kendi yapıyor. Kendi içinde select 
                // oluşturup çekiyor

                Kiralama? kiralamaVt = _kiralamaRepository.Get(u => u.Id == id);

                if (kiralamaVt == null)
                {
                    return NotFound();
                }

                // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
                return View(kiralamaVt);
            }

        }

        // Butona bastıktan sonra formadaki post sayesinde nesne veri tabanına ekleniyor.
        [HttpPost]
        public IActionResult EkleGuncelle(Kiralama kiralama)
        {
            // ModelState kaynaklı hataları anlamak için kullanılır
            var errors = ModelState.Values.SelectMany(x => x.Errors);

            // Eğer modeldeki koşullar sağlanmışsa girebilir.
            if (ModelState.IsValid)
            {

                if(kiralama.Id == 0)
                {
                    _kiralamaRepository.Ekle(kiralama);// Bu nesneyi eklemeye hazırlan
                    TempData["basarili"] = "Yeni Kiralama Kaydı başarıyla oluşturuldu!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                }
                else
                {
                    _kiralamaRepository.Guncelle(kiralama);// Bu nesneyi eklemeye hazırlan
                    TempData["basarili"] = " Kiralama Kayıt güncelleme başarılı!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                }

                _kiralamaRepository.Kaydet();// Ekleme burada yapılıyor. Eğer bunu yapmazsak veriler database e eklenmez

                return RedirectToAction("Index", "Kiralama"); // Index sayfasına dönüş yapmayı sağlar. Hangi controller olduğunu yazmalıyız
            }
            return View();
        }

        // GET ACTION
        public IActionResult Sil(int? id)
        {
            // Verileri Veri Tabanından Çekme
            IEnumerable<SelectListItem> KitapList = _kitapRepository.GetAll()
                .Select(k => new SelectListItem
                {
                    Text = k.KitapAdi,
                    Value = k.Id.ToString()
                });

            ViewBag.KitapList = KitapList;

            if (id == null || id == 0)
            {
                return NotFound();
            }

            Kiralama? kiralamaVt = _kiralamaRepository.Get(u=>u.Id==id);

            if (kiralamaVt == null)
            {
                return NotFound();
            }

            // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
            return View(kiralamaVt);
        }

        [HttpPost, ActionName("Sil")]
        public IActionResult SilPOST(int? id)
        {
            Kiralama? kiralama = _kiralamaRepository.Get(u => u.Id == id);
            if (kiralama == null)
            {
                return NotFound();
            }
            _kiralamaRepository.Sil(kiralama);
            _kiralamaRepository.Kaydet();
            TempData["basarili"] = "Kayıt Silme işlemi başarılı!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek. 
                                                                  // Bunu Index sayfasında yapacağız. Sadece bir kereliğine çalışır.
                                                                  // View ile controller, controller ile view arasında veri taşımak için kullanılan bir yapıdır.

            return RedirectToAction("Index", "Kiralama");
        }
    }
}
