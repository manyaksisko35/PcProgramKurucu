using System;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace PcProgramKurucu
{
    public partial class UygulamaKarti : UserControl
    {
        private string aktifWingetKodu;

        public UygulamaKarti()
        {
            InitializeComponent();

            // UserControl kendi içinde double buffer edilmezse scroll sırasında
            // her kart tek tek yeniden çizilir -> flicker/glitch oluşur
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

            btnIndir.Click += btnIndir_Click;
        }

        public void KartBilgileriniAyarla(string uygulamaAdi, string wingetKodu, string domain)
        {
            lblAd.Text = uygulamaAdi;
            aktifWingetKodu = wingetKodu;

            if (!string.IsNullOrEmpty(domain))
            {
                try
                {
                    picLogo.ErrorImage = null;
                    picLogo.InitialImage = null;

                    // LoadAsync her kart için ayrı HTTP isteği açıyor ve
                    // sonuç geldikçe PictureBox'ı invalidate ediyor.
                    // Scroll sırasında bu istekler üst üste tamamlanınca
                    // glitch/lag oluşuyordu. LoadCompleted ile tek seferde
                    // ve kontrollü şekilde işaretliyoruz.
                    picLogo.LoadCompleted -= PicLogo_LoadCompleted;
                    picLogo.LoadCompleted += PicLogo_LoadCompleted;
                    picLogo.LoadAsync($"https://www.google.com/s2/favicons?domain={domain}&sz=128");
                }
                catch { }
            }
        }

        private void PicLogo_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                picLogo.Invalidate();
            }
        }

        private void btnIndir_Click(object sender, EventArgs e)
        {
            Form1 anaForm = (Form1)this.FindForm();
            anaForm.IndirmeyiBaslat(lblAd.Text, aktifWingetKodu, picLogo.Image, (Guna2Button)sender);
        }
    }
}