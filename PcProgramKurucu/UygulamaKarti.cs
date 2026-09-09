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
                    picLogo.LoadAsync($"https://www.google.com/s2/favicons?domain={domain}&sz=128");
                }
                catch { }
            }
        }
        private void btnIndir_Click(object sender, EventArgs e)
        {
            Form1 anaForm = (Form1)this.FindForm();
            anaForm.IndirmeyiBaslat(lblAd.Text, aktifWingetKodu, picLogo.Image, (Guna2Button)sender);
        }
    }
}