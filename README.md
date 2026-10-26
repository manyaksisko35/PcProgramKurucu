# PcProgramKurucu

PcProgramKurucu ("PC program installer" in Turkish) is a Windows Forms app for setting up a new Windows PC. It shows a searchable list of 73 common programs and installs the one you pick with a single click through `winget`. Next to the list it shows live CPU, GPU and disk usage charts with CPU and GPU temperatures. The interface is in Turkish.

## Features

- **Program catalog**: 73 programs grouped as browsers, communication, media, gaming, developer tools and system utilities. Each entry is mapped to its winget package ID (for example `Google.Chrome`, `Valve.Steam`, `Microsoft.VisualStudioCode`, `7zip.7zip`).
- **One-click install**: the "İNDİR" button runs `winget install --id <id> -e --accept-package-agreements --accept-source-agreements` in a hidden console. A side panel shows the program name, logo, status and a progress bar. If winget fails, the exit code and winget's output are shown in an error dialog.
- **Logos**: each program's icon is downloaded from Google's favicon service using the program's website domain.
- **Search**: the search box filters the list by program name.
- **Custom list control**: all cards are drawn in a single `Paint` call with a custom scrollbar (drag, click on the track, mouse wheel) instead of one control per program and the native scrollbar.
- **System monitor**, updated every second:
  - CPU load and temperature. Since sensor names differ between Intel and AMD, it tries `Core Average`, `CPU Package`, `Core (Tctl/Tdie)` and `Core Max`, then falls back to the first temperature sensor it finds.
  - GPU load and temperature for NVIDIA and AMD cards.
  - Disk activity from the `PhysicalDisk / % Disk Time` performance counter.
  - Line charts that keep the last 40 readings.
- **Auto update**: on startup it checks `update.xml` in this repository with AutoUpdater.NET and offers the newer release if there is one.
- **Runs as administrator**: the app manifest sets `requireAdministrator`, so Windows asks for elevation on launch.
- Borderless window with a draggable title bar, and buttons that open my Buy Me a Coffee page and GitHub profile.

## Built with

- C# and Windows Forms on .NET Framework 4.7.2
- [winget](https://learn.microsoft.com/windows/package-manager/winget/) (Windows Package Manager) for installing programs
- [Guna.UI2.WinForms](https://www.nuget.org/packages/Guna.UI2.WinForms) 2.0.4.8 for the UI controls
- [LiveCharts](https://www.nuget.org/packages/LiveCharts.WinForms) 0.9.7 for the usage charts
- [LibreHardwareMonitorLib](https://www.nuget.org/packages/LibreHardwareMonitorLib) 0.9.2 for CPU and GPU sensors
- [AutoUpdater.NET](https://www.nuget.org/packages/Autoupdater.NET.Official) 1.9.3 for update checks
- [Costura.Fody](https://www.nuget.org/packages/Costura.Fody) 6.2.0 to embed the referenced DLLs into the executable

## Getting started

### Requirements

- Windows 10 or 11 with winget available (it comes with the "App Installer" package from the Microsoft Store)
- Visual Studio with the ".NET desktop development" workload and the .NET Framework 4.7.2 targeting pack. The solution file uses the newer `.slnx` format, so use Visual Studio 2022 (17.13 or later) or Visual Studio 2026.
- An internet connection for the program logos, winget downloads and the update check

### Build and run

1. Open `PcProgramKurucu.slnx` in Visual Studio.
2. Restore NuGet packages. The `packages` folder is not in the repository; NuGet restores it next to the solution.
3. Build and run. Because the manifest requests administrator rights, Visual Studio will ask to restart with elevated permissions before debugging.

The Debug configuration targets x64; the Release configuration targets Any CPU.

### Releasing an update

`update.xml` holds the latest version number and the download URL of the release executable. To publish a new version, raise `AssemblyVersion` in `PcProgramKurucu/Properties/AssemblyInfo.cs`, upload the new `PcProgramKurucu.exe` to a GitHub release and update `update.xml` to match.

## Project structure

```
PcProgramKurucu.slnx                Solution (builds the PcProgramKurucu/ project)
update.xml                          Version info read by AutoUpdater.NET
PcProgramKurucu/
  Program.cs                        Entry point
  Form1.cs                          Main window: program catalog, winget install, system monitor
  UygulamaListesi.cs                Custom-drawn, scrollable list of program cards
  UygulamaKarti.cs                  Earlier per-program card control (not used by the main window)
  app.manifest                      Requests administrator rights
  FodyWeavers.xml                   Costura configuration
```

The repository root also contains an older copy of the source files and a compiled build (`PcProgramKurucu.exe` with its DLLs and localization folders). The solution does not build those root files; the current source is in `PcProgramKurucu/`.

## Türkçe

PcProgramKurucu, yeni bir Windows bilgisayarı kurarken sık kullanılan programları tek tıkla yüklemek için yazdığım bir Windows Forms uygulamasıdır. 73 programlık aranabilir bir listeden seçilen program `winget` ile arka planda kurulur ve kurulum durumu yan panelde gösterilir. Uygulama ayrıca LibreHardwareMonitor ile CPU ve GPU kullanımını ve sıcaklığını, performans sayacı ile de disk kullanımını saniyede bir grafik olarak gösterir; açılışta AutoUpdater.NET ile güncelleme kontrolü yapar.

Made by [Hasan Alp Güngör](https://hasanalpgungor.com/en)
