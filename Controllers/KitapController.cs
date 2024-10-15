using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebUygulama1.Models;
using WebUygulama1.Utility;

namespace WebUygulama1.Controllers
{
    public class KitapController : Controller
    {
        // DEpendency Injection: Arkada bizim için nesneler üretir. Bu oluşturulan nesneler bizim tarafımızdan kullanılıyor ama
        // bu nesneleri oluşturan şey ASP .Net mekanizmesı. Bu nesne veri tabanıyla entity framework arasında bir köprü nesnesi.
        //private readonly UygulamaDbContext _uygulamaDbContext;

        private readonly IKitapRepository _kitapRepository;
        private readonly IKitapTuruRepository _kitapTuruRepository;
        public readonly IWebHostEnvironment _webHostEnvironment;

        public KitapController(IKitapRepository kitapRepository, IKitapTuruRepository kitapTuruRepository, IWebHostEnvironment webHostEnvironment)
        {
            _kitapRepository = kitapRepository;
            _kitapTuruRepository = kitapTuruRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Ogrenci")]
        public IActionResult Index()
        {

            //List<Kitap> objKitapList = _kitapRepository.GetAll().ToList();
            List<Kitap> objKitapList = _kitapRepository.GetAll(includeProps:"KitapTuru").ToList();

            return View(objKitapList); // Listeyi View a gönderdik
        }
        // Kİtap Eklemek için

        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult EkleGuncelle(int? id)
        {
            // Verileri Veri Tabanından Çekme
            IEnumerable<SelectListItem> KitapTuruList = _kitapTuruRepository.GetAll()
                .Select(k => new SelectListItem
                {
                    Text = k.Ad,
                    Value = k.Id.ToString()
                });

            // ViewBag Controller dan çekilen verilerin View katmanına aktarılmasını sağlar. KÖprü gibi veri aktarır fakat
            // tersi olmaz. Yani View den Contorller an veri aktarılmaz. Controller gitti KitapTuruRepository den
            // kitap türlerini aldı daha sonra ViewBag in içine koydu. ViewBag de View e gönderdi
            ViewBag.KitapTuruList = KitapTuruList;

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

                Kitap? kitapVt = _kitapRepository.Get(u => u.Id == id);

                if (kitapVt == null)
                {
                    return NotFound();
                }

                // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
                return View(kitapVt);
            }

        }

        // Butona bastıktan sonra formadaki post sayesinde nesne veri tabanına ekleniyor.
        [HttpPost]
        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult EkleGuncelle(Kitap kitap, IFormFile? file)
        {
            // ModelState kaynaklı hataları anlamak için kullanılır
            var errors = ModelState.Values.SelectMany(x => x.Errors);

            // Eğer modeldeki koşullar sağlanmışsa girebilir.
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string kitapPath = Path.Combine(wwwRootPath, @"img");

                if (file != null)
                {

                    using (var fileStream = new FileStream(Path.Combine(kitapPath, file.FileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    kitap.ResimUrl = @"\img\" + file.FileName;
                }

                if(kitap.Id == 0)
                {
                    _kitapRepository.Ekle(kitap);// Bu nesneyi eklemeye hazırlan
                    TempData["basarili"] = "Yeni Kitap başarıyla oluşturuldu!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                }
                else
                {
                    _kitapRepository.Guncelle(kitap);// Bu nesneyi eklemeye hazırlan
                    TempData["basarili"] = " Kitap başarılı!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                }

                _kitapRepository.Kaydet();// Ekleme burada yapılıyor. Eğer bunu yapmazsak veriler database e eklenmez

                return RedirectToAction("Index", "Kitap"); // Index sayfasına dönüş yapmayı sağlar. Hangi controller olduğunu yazmalıyız
            }
            return View();
        }
        /*
        public IActionResult Guncelle(int? id)
        {
            // Id primary key olduğu için null ya da 0 olamaz. Yani bu if statement ına gerek yok ama yine de öğrenmek için koydum
            if (id == null || id == 0)
            {
                return NotFound();
            }
            
            // Dependency injection (_uygulamaDbContext) sayesinde tek satırda veritabanı ile entity frameworkü bağlayabiliyoruz
            // Eğer id yi bulamazsa null döner. Null dönerse kitapVt nul olur. Nullable (Kitap?) yaparsak uyarı almayız
            //Bu sayede SQL için kendimiz elle sorgu yazmamıyoruz, Entity Framework bizim için kendi yapıyor. Kendi içinde select 
            // oluşturup çekiyor
            
            Kitap? kitapVt = _kitapRepository.Get(u=>u.Id==id);

            if(kitapVt == null)
            {
                return NotFound();
            }

            // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
            return View(kitapVt);
        }*/

        /*
        [HttpPost]
        public IActionResult Guncelle(Kitap kitap)
        {
            if (ModelState.IsValid)
            {
                _kitapRepository.Guncelle(kitap);
                _kitapRepository.Kaydet();
                TempData["basarili"] = "Yeni Kitap başarıyla güncellendi!"; // Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek.

                return RedirectToAction("Index", "Kitap");
            }
            return View();
        }*/

        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult Sil(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Kitap? kitapVt = _kitapRepository.Get(u=>u.Id==id);

            if (kitapVt == null)
            {
                return NotFound();
            }

            // Nesneyi aktarması için böyle yaptık. Parametre aktarması için butona asp-route-id i eklemeliyiz.
            return View(kitapVt);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = UserRoles.Role_Admin)]
        public IActionResult SilPOST(int? id)
        {
            Kitap? kitap = _kitapRepository.Get(u => u.Id == id);
            if (kitap == null)
            {
                return NotFound();
            }
            _kitapRepository.Sil(kitap);
            _kitapRepository.Kaydet();
            TempData["basarili"] = "Kayıt Silme işlemi başarılı!";// Bir değişkene değer atıyoruz ve sonra bunu consume etmemiz gerek. 
                                                                  // Bunu Index sayfasında yapacağız. Sadece bir kereliğine çalışır.
                                                                  // View ile controller, controller ile view arasında veri taşımak için kullanılan bir yapıdır.

            return RedirectToAction("Index", "Kitap");
        }
    }
}
