using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TrenRezervasyon.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string RezervasyonKontrol([FromBody] ClsGelen clsGelen)
        {

            ClsSonuc clsSonuc = new ClsSonuc { RezervasyonYapilabilir = false, YerlesimAyrinti = new List<ClsYerlesimAyrinti>() };

            if (clsGelen.KisilerFarkliVagonlaraYerlestirilebilir)
            {
                foreach (ClsVagon item in clsGelen.Tren.Vagonlar)
                {
                    ClsYerlesimAyrinti clsYerlesimAyrinti = new ClsYerlesimAyrinti
                    {
                        VagonAdi = item.Ad,
                        KisiSayisi = 0
                    };

                    while (0 < clsGelen.RezervasyonYapilacakKisiSayisi && item.DoluKoltukAdet < Math.Floor(item.Kapasite * 0.7))
                    {
                        clsYerlesimAyrinti.KisiSayisi++;
                        clsGelen.RezervasyonYapilacakKisiSayisi--;
                        item.DoluKoltukAdet++;
                    }
                    if (item.DoluKoltukAdet < Math.Floor(item.Kapasite * 0.7)) clsSonuc.RezervasyonYapilabilir = true; 
                    else clsSonuc.RezervasyonYapilabilir = false;
                    if (clsYerlesimAyrinti.KisiSayisi != 0) clsSonuc.YerlesimAyrinti.Add(clsYerlesimAyrinti);
                }
            }
            else
            {
                foreach (ClsVagon item in clsGelen.Tren.Vagonlar)
                {
                    if (item.DoluKoltukAdet + clsGelen.RezervasyonYapilacakKisiSayisi <= Math.Floor(item.Kapasite * 0.7))
                    {
                        clsSonuc.YerlesimAyrinti = new List<ClsYerlesimAyrinti> { new ClsYerlesimAyrinti { VagonAdi = item.Ad, KisiSayisi = clsGelen.RezervasyonYapilacakKisiSayisi } };
                        item.DoluKoltukAdet += clsGelen.RezervasyonYapilacakKisiSayisi;
                    }
                    if (item.DoluKoltukAdet < Math.Floor(item.Kapasite * 0.7) && clsSonuc.RezervasyonYapilabilir == false) clsSonuc.RezervasyonYapilabilir = true;
                }
            }

            return JsonConvert.SerializeObject(clsSonuc);
        }
    }
    public class ClsGelen
    {
        public ClsTren Tren { get; set; }
        public int RezervasyonYapilacakKisiSayisi { get; set; }
        public bool KisilerFarkliVagonlaraYerlestirilebilir { get; set; }
    }
    public class ClsTren
    {
        public string Ad { get; set; }
        public List<ClsVagon> Vagonlar { get; set; }
    }
    public class ClsVagon
    {
        public string Ad { get; set; }
        public int Kapasite { get; set; }
        public int DoluKoltukAdet { get; set; }
        public bool VagonDolu { get; set; } = false;
    }
    public class ClsSonuc
    {
        public bool RezervasyonYapilabilir { get; set; }
        public List<ClsYerlesimAyrinti> YerlesimAyrinti { get; set; }
    }
    public class ClsYerlesimAyrinti
    {
        public string VagonAdi { get; set; }
        public int KisiSayisi { get; set; }
    }
}

