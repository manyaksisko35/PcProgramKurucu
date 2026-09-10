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

namespace PcProgramKurucu
{

    public partial class Form1 : Form
    {
        Computer bilgisayar;
        PerformanceCounter diskSayaci;

        ChartValues<double> cpuDegerleri = new ChartValues<double>();
        ChartValues<double> gpuDegerleri = new ChartValues<double>();
        ChartValues<double> hddDegerleri = new ChartValues<double>();

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            typeof(FlowLayoutPanel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, flowLayoutPanel1, new object[] { true });

            this.Load += Form1_Load;

            if (timerSistem != null)
                timerSistem.Tick += timerSistem_Tick;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            AutoUpdater.Start("https://raw.githubusercontent.com/manyaksisko35/PcProgramKurucu/master/update.xml");

            Dictionary<string, string[]> uygulamalar = new Dictionary<string, string[]>();
            uygulamalar.Add("Google Chrome", new string[] { "Google.Chrome", "google.com" });
            uygulamalar.Add("Mozilla Firefox", new string[] { "Mozilla.Firefox", "firefox.com" });
            uygulamalar.Add("Microsoft Edge", new string[] { "Microsoft.Edge", "microsoftedgeinsider.com" });
            uygulamalar.Add("Discord", new string[] { "Discord.Discord", "discord.com" });
            uygulamalar.Add("WhatsApp", new string[] { "WhatsApp.WhatsApp", "whatsapp.com" });
            uygulamalar.Add("Spotify", new string[] { "Spotify.Spotify", "spotify.com" });
            uygulamalar.Add("Steam", new string[] { "Valve.Steam", "steampowered.com" });
            uygulamalar.Add("VS Code", new string[] { "Microsoft.VisualStudioCode", "code.visualstudio.com" });
            uygulamalar.Add("OBS Studio", new string[] { "OBSProject.OBSStudio", "obsproject.com" });
            uygulamalar.Add("Epic Games Launcher", new string[] { "EpicGames.EpicGamesLauncher", "epicgames.com" });
            uygulamalar.Add("Unity Hub", new string[] { "Unity.UnityHub", "unity.com" });
            uygulamalar.Add("Roblox Studio", new string[] { "Roblox.RobloxStudio", "roblox.com" });
            uygulamalar.Add("EA app", new string[] { "ElectronicArts.EADesktop", "ea.com" });
            uygulamalar.Add("GOG Galaxy", new string[] { "GOG.Galaxy", "gog.com" });
            uygulamalar.Add("Visual Studio 2022", new string[] { "Microsoft.VisualStudio.2022.Community", "visualstudio.microsoft.com" });
            uygulamalar.Add("Python 3", new string[] { "Python.Python.3.12", "python.org" });
            uygulamalar.Add("Arduino IDE", new string[] { "ArduinoSA.IDE.stable", "arduino.cc" });
            uygulamalar.Add("Notepad++", new string[] { "Notepad++.Notepad++", "notepad-plus-plus.org" });
            uygulamalar.Add("GitHub Desktop", new string[] { "GitHub.GitHubDesktop", "desktop.github.com" });
            uygulamalar.Add("DB Browser for SQLite", new string[] { "DBBrowserForSQLite.DBBrowserForSQLite", "sqlitebrowser.org" });
            uygulamalar.Add("VLC Media Player", new string[] { "VideoLAN.VLC", "videolan.org" });
            uygulamalar.Add("Audacity", new string[] { "Audacity.Audacity", "audacityteam.org" });
            uygulamalar.Add("DaVinci Resolve", new string[] { "BlackmagicDesign.DaVinciResolve", "blackmagicdesign.com" });
            uygulamalar.Add("Blender", new string[] { "BlenderFoundation.Blender", "blender.org" });
            uygulamalar.Add("Paint.NET", new string[] { "dotPDNLLC.paintdotnet", "getpaint.net" });
            uygulamalar.Add("WinRAR", new string[] { "RARLab.WinRAR", "rarlab.com" });
            uygulamalar.Add("7-Zip", new string[] { "7zip.7zip", "7-zip.org" });
            uygulamalar.Add("TeamViewer", new string[] { "TeamViewer.TeamViewer", "teamviewer.com" });
            uygulamalar.Add("WinSCP", new string[] { "MartinPrikryl.WinSCP", "winscp.net" });
            uygulamalar.Add("Rufus", new string[] { "Rufus.Rufus", "rufus.ie" });

            foreach (var program in uygulamalar)
            {
                UygulamaKarti yeniKart = new UygulamaKarti();
                yeniKart.KartBilgileriniAyarla(program.Key, program.Value[0], program.Value[1]);
                yeniKart.Tag = program.Key.ToLower();
                flowLayoutPanel1.Controls.Add(yeniKart);
            }

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
                        foreach (ISensor sensor in donanim.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total") cpuK = sensor.Value.GetValueOrDefault();
                            if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core Average")) cpuS = sensor.Value.GetValueOrDefault();
                        }
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
            }
            catch { }
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
                await Task.Run(() =>
                {
                    Process islem = new Process();
                    islem.StartInfo.FileName = "cmd.exe";
                    islem.StartInfo.Arguments = $"/c winget install --id {wingetKodu} -e --accept-package-agreements --accept-source-agreements";
                    islem.StartInfo.UseShellExecute = false;
                    islem.StartInfo.CreateNoWindow = true;
                    islem.Start();
                    islem.WaitForExit();
                });

                tiklananButon.Text = "Kuruldu"; tiklananButon.FillColor = Color.SeaGreen;
                if (lblSagDurum != null) { lblSagDurum.Text = "%100 - Kurulum Tamamlandı!"; lblSagDurum.ForeColor = Color.SeaGreen; }
                if (progBar != null) { progBar.Style = ProgressBarStyle.Blocks; progBar.Value = 100; }
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
            string arananMetin = txtArama.Text.ToLower();

            foreach (Control kart in flowLayoutPanel1.Controls)
            {
                if (kart.Tag != null && kart.Tag.ToString().Contains(arananMetin))
                {
                    kart.Visible = true;
                }
                else
                {
                    kart.Visible = false;
                }
            }
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