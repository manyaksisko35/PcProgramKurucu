using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PcProgramKurucu
{
    // Her uygulama için ayrı UserControl OLUŞTURMAZ.
    // Tüm kartları TEK bir Paint içinde kendisi çizer -> gerçek smooth scroll.
    // Native Windows scrollbar KULLANMAZ (AutoScroll kapalı) - buggy/senkron kaybı
    // sorunlarına sebep oluyordu. Bunun yerine sağda kendi çizdiğimiz, kendi
    // kontrol ettiğimiz ince bir scrollbar var.
    public class UygulamaListesi : Panel
    {
        public class UygulamaOgesi
        {
            public string Ad;
            public string WingetKodu;
            public string Domain;
            public Image Logo;
            public Rectangle SinirKutusu;
            public Rectangle ButonKutusu;
            public bool Gorunur = true;
        }

        private List<UygulamaOgesi> _ogeler = new List<UygulamaOgesi>();
        private const int KART_GENISLIK = 168;
        private const int KART_YUKSEKLIK = 190;
        private const int BOSLUK = 10;
        private const int SCROLLBAR_GENISLIK = 8;

        private int _scrollY = 0;          // mevcut kaydırma konumu (px)
        private int _icerikYuksekligi = 0; // tüm kartların kapladığı toplam yükseklik

        private bool _thumbSurukleniyor = false;
        private int _surukleBaslangicY;
        private int _surukleBaslangicScrollY;

        public event Action<UygulamaOgesi> IndirTiklandi;

        public UygulamaListesi()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.Selectable, true);
            this.UpdateStyles();
            this.AutoScroll = false; // native scrollbar tamamen kapalı
            this.BackColor = Color.Transparent;

            this.MouseClick += UygulamaListesi_MouseClick;
            this.MouseDown += UygulamaListesi_MouseDown;
            this.MouseMove += UygulamaListesi_MouseMove;
            this.MouseUp += UygulamaListesi_MouseUp;
            this.MouseWheel += UygulamaListesi_MouseWheel;
        }

        public void ListeyiAyarla(List<UygulamaOgesi> ogeler)
        {
            _ogeler = ogeler;
            DuzenHesapla();
            Invalidate();
        }

        public void LogoGuncelle(UygulamaOgesi oge, Image logo)
        {
            oge.Logo = logo;
            Invalidate();
        }

        public void AramaFiltreUygula(string arananMetin)
        {
            arananMetin = (arananMetin ?? "").ToLower();
            foreach (var oge in _ogeler)
                oge.Gorunur = string.IsNullOrEmpty(arananMetin) || oge.Ad.ToLower().Contains(arananMetin);

            DuzenHesapla();
            _scrollY = 0; // filtre değişince en başa dön
            Invalidate();
        }

        private void DuzenHesapla()
        {
            int genislik = Math.Max(1, this.ClientSize.Width - SCROLLBAR_GENISLIK - 4);
            int sutunSayisi = Math.Max(1, genislik / (KART_GENISLIK + BOSLUK));

            int x = BOSLUK, y = BOSLUK, sutun = 0;
            foreach (var oge in _ogeler)
            {
                if (!oge.Gorunur)
                {
                    oge.SinirKutusu = Rectangle.Empty;
                    continue;
                }

                oge.SinirKutusu = new Rectangle(x, y, KART_GENISLIK, KART_YUKSEKLIK);
                oge.ButonKutusu = new Rectangle(x + 10, y + KART_YUKSEKLIK - 40, KART_GENISLIK - 20, 30);

                sutun++;
                if (sutun >= sutunSayisi)
                {
                    sutun = 0;
                    x = BOSLUK;
                    y += KART_YUKSEKLIK + BOSLUK;
                }
                else
                {
                    x += KART_GENISLIK + BOSLUK;
                }
            }

            _icerikYuksekligi = y + KART_YUKSEKLIK + BOSLUK * 2;
            ScrollYSinirla();
        }

        private int MaxScrollY()
        {
            return Math.Max(0, _icerikYuksekligi - this.ClientSize.Height);
        }

        private void ScrollYSinirla()
        {
            int max = MaxScrollY();
            if (_scrollY < 0) _scrollY = 0;
            if (_scrollY > max) _scrollY = max;
        }

        // ----- Scrollbar geometrisi -----
        private bool ScrollbarGerekli() => _icerikYuksekligi > this.ClientSize.Height;

        private Rectangle ScrollbarTrackKutusu()
        {
            return new Rectangle(this.ClientSize.Width - SCROLLBAR_GENISLIK - 2, 2, SCROLLBAR_GENISLIK, this.ClientSize.Height - 4);
        }

        private Rectangle ScrollbarThumbKutusu()
        {
            var track = ScrollbarTrackKutusu();
            int max = MaxScrollY();
            if (max <= 0) return track;

            float oran = (float)this.ClientSize.Height / _icerikYuksekligi;
            int thumbYukseklik = Math.Max(30, (int)(track.Height * oran));
            int kalanAlan = track.Height - thumbYukseklik;
            int thumbY = track.Y + (int)((float)_scrollY / max * kalanAlan);

            return new Rectangle(track.X, thumbY, track.Width, thumbYukseklik);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DuzenHesapla();
            Invalidate();
        }

        private void UygulamaListesi_MouseWheel(object sender, MouseEventArgs e)
        {
            int adim = 60;
            _scrollY -= Math.Sign(e.Delta) * adim;
            ScrollYSinirla();
            Invalidate();
        }

        private void UygulamaListesi_MouseDown(object sender, MouseEventArgs e)
        {
            if (!ScrollbarGerekli()) return;

            var thumb = ScrollbarThumbKutusu();
            if (thumb.Contains(e.Location))
            {
                _thumbSurukleniyor = true;
                _surukleBaslangicY = e.Y;
                _surukleBaslangicScrollY = _scrollY;
                return;
            }

            var track = ScrollbarTrackKutusu();
            if (track.Contains(e.Location))
            {
                // track'e tıklayınca o noktaya sıçra (sayfa atlama)
                float oran = (float)(e.Y - track.Y) / track.Height;
                _scrollY = (int)(oran * MaxScrollY());
                ScrollYSinirla();
                Invalidate();
            }
        }

        private void UygulamaListesi_MouseMove(object sender, MouseEventArgs e)
        {
            if (_thumbSurukleniyor)
            {
                var track = ScrollbarTrackKutusu();
                var thumb = ScrollbarThumbKutusu();
                int kalanAlan = Math.Max(1, track.Height - thumb.Height);

                int deltaY = e.Y - _surukleBaslangicY;
                int maxY = MaxScrollY();

                _scrollY = _surukleBaslangicScrollY + (int)((float)deltaY / kalanAlan * maxY);
                ScrollYSinirla();
                Invalidate();
            }
        }

        private void UygulamaListesi_MouseUp(object sender, MouseEventArgs e)
        {
            _thumbSurukleniyor = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int kaydirmaY = -_scrollY;

            using (var kartFirca = new SolidBrush(Color.FromArgb(45, 45, 45)))
            using (var kenarKalem = new Pen(Color.FromArgb(255, 128, 0), 2))
            using (var adFontu = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var butonFirca = new SolidBrush(Color.FromArgb(255, 128, 0)))
            using (var butonFontu = new Font("Segoe UI", 9, FontStyle.Bold))
            {
                foreach (var oge in _ogeler)
                {
                    if (!oge.Gorunur) continue;

                    var kutu = new Rectangle(
                        oge.SinirKutusu.X,
                        oge.SinirKutusu.Y + kaydirmaY,
                        oge.SinirKutusu.Width,
                        oge.SinirKutusu.Height);

                    if (kutu.Bottom < 0 || kutu.Top > this.ClientSize.Height) continue;

                    using (var yol = YuvarlatilmisDikdortgen(kutu, 12))
                    {
                        g.FillPath(kartFirca, yol);
                        g.DrawPath(kenarKalem, yol);
                    }

                    if (oge.Logo != null)
                    {
                        var logoKutu = new Rectangle(kutu.X + kutu.Width / 2 - 32, kutu.Y + 15, 64, 64);
                        g.DrawImage(oge.Logo, logoKutu);
                    }

                    var adKutu = new Rectangle(kutu.X, kutu.Y + 85, kutu.Width, 25);
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(oge.Ad, adFontu, Brushes.White, adKutu, sf);

                    var butonKutu = new Rectangle(
                        oge.ButonKutusu.X,
                        oge.ButonKutusu.Y + kaydirmaY,
                        oge.ButonKutusu.Width,
                        oge.ButonKutusu.Height);

                    using (var butonYol = YuvarlatilmisDikdortgen(butonKutu, 8))
                    {
                        g.FillPath(butonFirca, butonYol);
                    }
                    g.DrawString("İNDİR", butonFontu, Brushes.Black, butonKutu, sf);
                }
            }

            // ----- Kendi scrollbar'ımızı çiz -----
            if (ScrollbarGerekli())
            {
                var track = ScrollbarTrackKutusu();
                var thumb = ScrollbarThumbKutusu();

                using (var trackFirca = new SolidBrush(Color.FromArgb(30, 30, 30)))
                using (var thumbFirca = new SolidBrush(_thumbSurukleniyor ? Color.FromArgb(255, 160, 60) : Color.FromArgb(255, 128, 0)))
                {
                    g.FillRectangle(trackFirca, track);

                    using (var thumbYol = YuvarlatilmisDikdortgen(thumb, 4))
                    {
                        g.FillPath(thumbFirca, thumbYol);
                    }
                }
            }
        }

        private GraphicsPath YuvarlatilmisDikdortgen(Rectangle r, int yaricap)
        {
            var yol = new GraphicsPath();
            int d = yaricap * 2;
            yol.AddArc(r.X, r.Y, d, d, 180, 90);
            yol.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            yol.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            yol.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            yol.CloseFigure();
            return yol;
        }

        private void UygulamaListesi_MouseClick(object sender, MouseEventArgs e)
        {
            if (_thumbSurukleniyor) return; // sürükleme bittiğinde yanlışlıkla buton tetiklenmesin

            int kaydirmaY = -_scrollY;

            foreach (var oge in _ogeler)
            {
                if (!oge.Gorunur) continue;
                var butonKutu = new Rectangle(
                    oge.ButonKutusu.X,
                    oge.ButonKutusu.Y + kaydirmaY,
                    oge.ButonKutusu.Width,
                    oge.ButonKutusu.Height);

                if (butonKutu.Contains(e.Location))
                {
                    IndirTiklandi?.Invoke(oge);
                    break;
                }
            }
        }
    }
}