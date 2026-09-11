using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using LibreHardwareMonitor.Hardware;
using System.IO;
using System.Runtime.InteropServices;
using AutoUpdaterDotNET;
using System.Linq;

namespace PcProgramKurucu
{
    public partial class Form1 : Form
    {
        Computer bilgisayar;
        PerformanceCounter diskSayaci;
        private bool _yuklendi = false;

        ChartValues<double> cpuDegerleri = new ChartValues<double>();
        ChartValues<double> gpuDegerleri = new ChartValues<double>();
        ChartValues<double> hddDegerleri = new ChartValues<double>();

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            if (timerSistem != null)
                timerSistem.Tick += timerSistem_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (_yuklendi) return;
            _yuklendi = true;

            Task.Run(() =>
            {
                try { AutoUpdater.Start("https://raw.githubusercontent.com/manyaksisko35/PcProgramKurucu/master/update.xml"); }
                catch { }
            });

            // Tüm ID'ler winget-pkgs deposundaki gerçek, güncel paket ID'leriyle
            // doğrulandı (winget install --id ... -e komutlarıyla birebir eşleşiyor).
            Dictionary<string, string[]> uygulamalar = new Dictionary<string, string[]>();

            // ---- Tarayıcılar ----
            uygulamalar.Add("Google Chrome", new string[] { "Google.Chrome", "google.com" });
            uygulamalar.Add("Mozilla Firefox", new string[] { "Mozilla.Firefox", "firefox.com" });
            uygulamalar.Add("Microsoft Edge", new string[] { "Microsoft.Edge", "explore.microsoft.com/tr-tr/edge/features" });
            uygulamalar.Add("Opera", new string[] { "Opera.Opera", "opera.com" });
            uygulamalar.Add("Brave", new string[] { "BraveSoftware.BraveBrowser", "brave.com" });
            uygulamalar.Add("Tor Browser", new string[] { "TorProject.TorBrowser", "torproject.org" });

            // ---- İletişim ----
            uygulamalar.Add("Discord", new string[] { "Discord.Discord", "discord.com" });
            uygulamalar.Add("WhatsApp", new string[] { "9NKSQGP7F2NH", "whatsappbusiness.com" });
            uygulamalar.Add("Telegram", new string[] { "Telegram.TelegramDesktop", "telegram.org" });
            uygulamalar.Add("Signal", new string[] { "Signal.Signal", "signal.org" });
            uygulamalar.Add("Slack", new string[] { "SlackTechnologies.Slack", "slack.com" });
            uygulamalar.Add("Zoom", new string[] { "Zoom.Zoom", "zoom.us" });
            uygulamalar.Add("Microsoft Teams", new string[] { "Microsoft.Teams", "skype.com" });

            // ---- Müzik / Medya ----
            uygulamalar.Add("Spotify", new string[] { "Spotify.Spotify", "spotify.com" });
            uygulamalar.Add("VLC Media Player", new string[] { "VideoLAN.VLC", "videolan.org" });
            uygulamalar.Add("OBS Studio", new string[] { "OBSProject.OBSStudio", "obsproject.com" });
            uygulamalar.Add("Audacity", new string[] { "Audacity.Audacity", "audacityteam.org" });
            uygulamalar.Add("DaVinci Resolve", new string[] { "BlackmagicDesign.DaVinciResolve", "blackmagicdesign.com" });
            uygulamalar.Add("HandBrake", new string[] { "HandBrake.HandBrake", "handbrake.fr" });
            uygulamalar.Add("IrfanView", new string[] { "IrfanSkiljan.IrfanView", "irfanview.com" });
            uygulamalar.Add("GIMP", new string[] { "GIMP.GIMP", "gimp.org" });
            uygulamalar.Add("ShareX", new string[] { "ShareX.ShareX", "getsharex.com" });

            // ---- Oyun ----
            uygulamalar.Add("Steam", new string[] { "Valve.Steam", "steampowered.com" });
            uygulamalar.Add("Epic Games Launcher", new string[] { "EpicGames.EpicGamesLauncher", "epicgames.com" });
            uygulamalar.Add("GOG Galaxy", new string[] { "GOG.Galaxy", "gog.com" });
            uygulamalar.Add("EA app", new string[] { "ElectronicArts.EADesktop", "help.ea.com" });
            uygulamalar.Add("Ubisoft Connect", new string[] { "Ubisoft.Connect", "ubisoft.com" });
            uygulamalar.Add("Battle.net", new string[] { "Blizzard.BattleNet", "battle.net" });
            uygulamalar.Add("Minecraft Launcher", new string[] { "Mojang.MinecraftLauncher", "minecraft.net" });
            uygulamalar.Add("Roblox Studio", new string[] { "Roblox.RobloxStudio", "create.roblox.com" });
            uygulamalar.Add("Unity Hub", new string[] { "Unity.UnityHub", "unity.com" });

            // ---- Geliştirici Araçları ----
            uygulamalar.Add("VS Code", new string[] { "Microsoft.VisualStudioCode", "code.visualstudio.com" });
            uygulamalar.Add("Visual Studio 2022", new string[] { "Microsoft.VisualStudio.2022.Community", "visualstudio.microsoft.com" });
            uygulamalar.Add("Git", new string[] { "Git.Git", "git-scm.com" });
            uygulamalar.Add("GitHub Desktop", new string[] { "GitHub.GitHubDesktop", "desktop.github.com" });
            uygulamalar.Add("Node.js", new string[] { "OpenJS.NodeJS.LTS", "nodejs.org" });
            uygulamalar.Add("Python 3", new string[] { "Python.Python.3.12", "python.org" });
            uygulamalar.Add("Postman", new string[] { "Postman.Postman", "postman.com" });
            uygulamalar.Add("Docker Desktop", new string[] { "Docker.DockerDesktop", "docker.com" });
            uygulamalar.Add("Notepad++", new string[] { "Notepad++.Notepad++", "notepad-plus-plus.org" });
            uygulamalar.Add("DB Browser for SQLite", new string[] { "DBBrowserForSQLite.DBBrowserForSQLite", "sqlitebrowser.org" });
            uygulamalar.Add("Android Studio", new string[] { "Google.AndroidStudio", "developer.android.com" });
            uygulamalar.Add("JetBrains Toolbox", new string[] { "JetBrains.Toolbox", "jetbrains.com" });
            uygulamalar.Add("DBeaver", new string[] { "dbeaver.dbeaver", "dbeaver.io" });
            uygulamalar.Add("PostgreSQL", new string[] { "PostgreSQL.PostgreSQL", "postgresql.org" });
            uygulamalar.Add("Windows Terminal", new string[] { "Microsoft.WindowsTerminal", "microsoft.com" });
            uygulamalar.Add("PowerShell", new string[] { "Microsoft.PowerShell", "microsoft.com" });
            uygulamalar.Add("Arduino IDE", new string[] { "ArduinoSA.IDE.stable", "arduino.cc" });
            uygulamalar.Add("Figma", new string[] { "Figma.Figma", "figma.com" });
            uygulamalar.Add("Obsidian", new string[] { "Obsidian.Obsidian", "obsidian.md" });
            uygulamalar.Add("Notion", new string[] { "Notion.Notion", "notion.so" });

            // ---- Sistem / Yardımcı Araçlar ----
            uygulamalar.Add("7-Zip", new string[] { "7zip.7zip", "7-zip.org" });
            uygulamalar.Add("WinRAR", new string[] { "RARLab.WinRAR", "rarlab.com" });
            uygulamalar.Add("Everything", new string[] { "voidtools.Everything", "voidtools.com" });
            uygulamalar.Add("CPU-Z", new string[] { "CPUID.CPU-Z", "cpuid.com" });
            uygulamalar.Add("GPU-Z", new string[] { "TechPowerUp.GPU-Z", "techpowerup.com" });
            uygulamalar.Add("HWiNFO", new string[] { "REALiX.HWiNFO", "hwinfo.com" });
            uygulamalar.Add("MSI Afterburner", new string[] { "Guru3D.Afterburner", "msi.com" });
            uygulamalar.Add("Malwarebytes", new string[] { "Malwarebytes.Malwarebytes", "malwarebytes.com" });
            uygulamalar.Add("CCleaner", new string[] { "Piriform.CCleaner", "ccleaner.com" });
            uygulamalar.Add("Rufus", new string[] { "Rufus.Rufus", "rufus.ie" });
            uygulamalar.Add("WinSCP", new string[] { "WinSCP.WinSCP", "winscp.net" });
            uygulamalar.Add("TeamViewer", new string[] { "TeamViewer.TeamViewer", "teamviewer.com" });
            uygulamalar.Add("PowerToys", new string[] { "Microsoft.PowerToys", "microsoft.com" });
            uygulamalar.Add("WinDirStat", new string[] { "WinDirStat.WinDirStat", "windirstat.net" });
            uygulamalar.Add("Wireshark", new string[] { "WiresharkFoundation.Wireshark", "wireshark.org" });
            uygulamalar.Add("qBittorrent", new string[] { "qBittorrent.qBittorrent", "qbittorrent.org" });
            uygulamalar.Add("Bitwarden", new string[] { "Bitwarden.Bitwarden", "bitwarden.com" });
            uygulamalar.Add("ProtonVPN", new string[] { "ProtonTechnologies.ProtonVPN", "protonvpn.com" });
            uygulamalar.Add("LibreOffice", new string[] { "TheDocumentFoundation.LibreOffice", "libreoffice.org" });
            uygulamalar.Add("Adobe Acrobat Reader", new string[] { "Adobe.Acrobat.Reader.64-bit", "adobe.com" });
            uygulamalar.Add("Paint.NET", new string[] { "dotPDNLLC.paintdotnet", "getpaint.net" });

            var ogeler = new List<UygulamaListesi.UygulamaOgesi>();
            foreach (var program in uygulamalar)
            {
                var oge = new UygulamaListesi.UygulamaOgesi
                {
                    Ad = program.Key,
                    WingetKodu = program.Value[0],
                    Domain = program.Value[1]
                };
                ogeler.Add(oge);

                string url = $"https://www.google.com/s2/favicons?domain={program.Value[1]}&sz=128";
                System.Net.WebClient istemci = new System.Net.WebClient();
                istemci.DownloadDataCompleted += (s, indirmeSonucu) =>
                {
                    if (indirmeSonucu.Error != null || indirmeSonucu.Cancelled) return;
                    try
                    {
                        using (var ms = new MemoryStream(indirmeSonucu.Result))
                        {
                            var img = Image.FromStream(ms);
                            if (uygulamaListesi.InvokeRequired)
                                uygulamaListesi.Invoke(new Action(() => uygulamaListesi.LogoGuncelle(oge, img)));
                            else
                                uygulamaListesi.LogoGuncelle(oge, img);
                        }
                    }
                    catch { }
                };
                try { istemci.DownloadDataAsync(new Uri(url)); } catch { }
            }

            uygulamaListesi.ListeyiAyarla(ogeler);
            uygulamaListesi.IndirTiklandi += (oge) =>
            {
                IndirmeyiBaslatBasit(oge.Ad, oge.WingetKodu);
            };

            GrafikTasarla(chartCPU, cpuDegerleri, "CPU", System.Windows.Media.Color.FromRgb(17, 125, 187));
            GrafikTasarla(chartGPU, gpuDegerleri, "GPU", System.Windows.Media.Color.FromRgb(118, 185, 0));
            GrafikTasarla(chartHDD, hddDegerleri, "HDD", System.Windows.Media.Color.FromRgb(255, 128, 0));

            bilgisayar = new Computer { IsCpuEnabled = true, IsGpuEnabled = true };
            bilgisayar.Open();

            try
            {
                diskSayaci = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            }
            catch { }
        }

        private void GrafikTasarla(LiveCharts.WinForms.CartesianChart grafik, ChartValues<double> degerler, string isim, System.Windows.Media.Color renk)
        {
            if (grafik == null) return;

            grafik.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = isim + " Kullanımı",
                    Values = degerler,
                    PointGeometry = null,
                    LineSmoothness = 0.6,
                    StrokeThickness = 3,
                    Stroke = new System.Windows.Media.SolidColorBrush(renk),
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(60, renk.R, renk.G, renk.B))
                }
            };

            grafik.AxisX.Add(new Axis { IsMerged = true, ShowLabels = false, Separator = new Separator { StrokeThickness = 0 } });
            grafik.AxisY.Add(new Axis { MinValue = 0, MaxValue = 100, IsMerged = true, ShowLabels = false, Separator = new Separator { StrokeThickness = 0 } });
            grafik.BackColor = Color.Transparent;
        }

        private void timerSistem_Tick(object sender, EventArgs e)
        {
            try
            {
                bilgisayar.Accept(new UpdateVisitor());

                float cpuK = 0, cpuS = 0;
                float gpuK = 0, gpuS = 0;
                float hddK = 0;

                foreach (IHardware donanim in bilgisayar.Hardware)
                {
                    if (donanim.HardwareType == HardwareType.Cpu)
                    {
                        // Sıcaklık sensörünün adı işlemci markasına/modeline göre değişiyor:
                        // Intel'de genelde "Core Average" var, AMD Ryzen'de yok - onun yerine
                        // "CPU Package" ya da "Core (Tctl/Tdie)" gibi isimler kullanılıyor.
                        // Tek bir isme bakmak yerine, bilinen tüm olası isimleri sırayla deniyoruz,
                        // hiçbiri tutmazsa bulunan İLK Temperature sensörünü kullanıyoruz (fallback).
                        ISensor sicaklikSensoru = null;

                        foreach (ISensor sensor in donanim.Sensors)
                        {
                            if (sensor.SensorType != SensorType.Load) continue;
                            if (sensor.Name == "CPU Total") cpuK = sensor.Value.GetValueOrDefault();
                        }

                        string[] bilinenSicaklikIsimleri = { "Core Average", "CPU Package", "Core (Tctl/Tdie)", "Core Max" };
                        foreach (var isim in bilinenSicaklikIsimleri)
                        {
                            sicaklikSensoru = donanim.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature && s.Name == isim);
                            if (sicaklikSensoru != null) break;
                        }

                        // Hiçbiri bulunamadıysa, bulunan ilk sıcaklık sensörünü kullan
                        if (sicaklikSensoru == null)
                            sicaklikSensoru = donanim.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);

                        if (sicaklikSensoru != null)
                            cpuS = sicaklikSensoru.Value.GetValueOrDefault();
                    }

                    if (donanim.HardwareType == HardwareType.GpuNvidia || donanim.HardwareType == HardwareType.GpuAmd)
                    {
                        foreach (ISensor sensor in donanim.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core") gpuK = sensor.Value.GetValueOrDefault();
                            if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core") gpuS = sensor.Value.GetValueOrDefault();
                        }
                    }
                }

                if (diskSayaci != null)
                {
                    try { hddK = diskSayaci.NextValue(); } catch { }
                    if (hddK > 100) hddK = 100;
                }

                cpuDegerleri.Add(cpuK);
                gpuDegerleri.Add(gpuK);
                hddDegerleri.Add(hddK);

                if (cpuDegerleri.Count > 40)
                {
                    cpuDegerleri.RemoveAt(0);
                    gpuDegerleri.RemoveAt(0);
                    hddDegerleri.RemoveAt(0);
                }

                if (lblCpuSicaklik != null) lblCpuSicaklik.Text = $"Sıcaklık: {Math.Round(cpuS)}°C";
                if (lblGpuSicaklik != null) lblGpuSicaklik.Text = $"Sıcaklık: {Math.Round(gpuS)}°C";

                // SSD/HDD kullanım yüzdesi hiç gösterilmiyordu - label3 ("x" yazan yer)
                // hiçbir yerde güncellenmiyordu, şimdi disk kullanım yüzdesini yazıyoruz.
                if (label3 != null) label3.Text = $"Kullanım: %{Math.Round(hddK)}";
            }
            catch { }
        }

        public async void IndirmeyiBaslatBasit(string uygulamaAdi, string wingetKodu)
        {
            if (lblSagAd != null) lblSagAd.Text = uygulamaAdi;
            if (lblSagDurum != null) { lblSagDurum.Text = "İndiriliyor ve Kuruluyor..."; lblSagDurum.ForeColor = Color.Goldenrod; }
            if (progBar != null) { progBar.Value = 0; progBar.Style = ProgressBarStyle.Marquee; }

            try
            {
                int cikisKodu = -1;
                string cikti = "";
                string hataCiktisi = "";

                await Task.Run(() =>
                {
                    Process islem = new Process();
                    islem.StartInfo.FileName = "cmd.exe";
                    islem.StartInfo.Arguments = $"/c winget install --id {wingetKodu} -e --accept-package-agreements --accept-source-agreements";
                    islem.StartInfo.UseShellExecute = false;
                    islem.StartInfo.CreateNoWindow = true;
                    islem.StartInfo.RedirectStandardOutput = true;
                    islem.StartInfo.RedirectStandardError = true;
                    islem.Start();

                    cikti = islem.StandardOutput.ReadToEnd();
                    hataCiktisi = islem.StandardError.ReadToEnd();
                    islem.WaitForExit();
                    cikisKodu = islem.ExitCode;
                });

                if (cikisKodu == 0)
                {
                    if (lblSagDurum != null) { lblSagDurum.Text = "%100 - Kurulum Tamamlandı!"; lblSagDurum.ForeColor = Color.SeaGreen; }
                    if (progBar != null) { progBar.Style = ProgressBarStyle.Blocks; progBar.Value = 100; }
                }
                else
                {
                    throw new Exception($"winget exit code: {cikisKodu}\n{hataCiktisi}\n{cikti}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (lblSagDurum != null) { lblSagDurum.Text = "Hata Oluştu!"; lblSagDurum.ForeColor = Color.Red; }
                if (progBar != null) { progBar.Style = ProgressBarStyle.Blocks; progBar.Value = 0; }
            }
        }

        public async void IndirmeyiBaslat(string uygulamaAdi, string wingetKodu, Image logo, Guna2Button tiklananButon)
        {
            if (lblSagAd != null) lblSagAd.Text = uygulamaAdi;
            if (picSagLogo != null) picSagLogo.Image = logo;
            if (lblSagDurum != null) { lblSagDurum.Text = "İndiriliyor ve Kuruluyor..."; lblSagDurum.ForeColor = Color.Goldenrod; }
            if (progBar != null) { progBar.Value = 0; progBar.Style = ProgressBarStyle.Marquee; }

            tiklananButon.Enabled = false; tiklananButon.Text = "Kuruluyor..."; tiklananButon.FillColor = Color.Goldenrod;

            try
            {
                int cikisKodu = -1;
                string cikti = "";
                string hataCiktisi = "";

                await Task.Run(() =>
                {
                    Process islem = new Process();
                    islem.StartInfo.FileName = "cmd.exe";
                    islem.StartInfo.Arguments = $"/c winget install --id {wingetKodu} -e --accept-package-agreements --accept-source-agreements";
                    islem.StartInfo.UseShellExecute = false;
                    islem.StartInfo.CreateNoWindow = true;
                    islem.StartInfo.RedirectStandardOutput = true;
                    islem.StartInfo.RedirectStandardError = true;
                    islem.Start();

                    cikti = islem.StandardOutput.ReadToEnd();
                    hataCiktisi = islem.StandardError.ReadToEnd();
                    islem.WaitForExit();
                    cikisKodu = islem.ExitCode;
                });

                if (cikisKodu == 0)
                {
                    tiklananButon.Text = "Kuruldu"; tiklananButon.FillColor = Color.SeaGreen;
                    if (lblSagDurum != null) { lblSagDurum.Text = "%100 - Kurulum Tamamlandı!"; lblSagDurum.ForeColor = Color.SeaGreen; }
                    if (progBar != null) { progBar.Style = ProgressBarStyle.Blocks; progBar.Value = 100; }
                }
                else
                {
                    throw new Exception($"winget exit code: {cikisKodu}\n{hataCiktisi}\n{cikti}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tiklananButon.Text = "İndir"; tiklananButon.Enabled = true;
                if (lblSagDurum != null) { lblSagDurum.Text = "Hata Oluştu!"; lblSagDurum.ForeColor = Color.Red; }
                if (progBar != null) { progBar.Style = ProgressBarStyle.Blocks; progBar.Value = 0; }
            }
        }

        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer) { computer.Traverse(this); }
            public void VisitHardware(IHardware hardware) { hardware.Update(); foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this); }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        private void btnDestekOl_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://buymeacoffee.com/hasanalpgungor");
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/manyaksisko35");
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            uygulamaListesi.AramaFiltreUygula(txtArama.Text);
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 0x112, 0xf012, 0);
            }
        }
    }
}