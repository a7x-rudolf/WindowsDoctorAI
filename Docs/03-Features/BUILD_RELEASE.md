# 🚀 Build & Release Guide

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Dokumen ini menjelaskan proses build dan release WindowsDoctorAI dari source code sampai distributable package. Ditujukan untuk **RIDOLF WIDI ALFISA LUMBA** sebagai developer dan future maintainers.

---

## 🛠️ Prerequisites

### Development Environment

| Tool | Version | Purpose |
|------|---------|---------|
| **Windows 11** (or Win 10 22H2+) | Latest | OS untuk development |
| **Visual Studio 2022** | 17.9+ | IDE utama |
| **VS 2022 Workloads** | — | Lihat detail di bawah |
| **.NET 8 SDK** | 8.0.400+ | Runtime & SDK |
| **Windows App SDK** | 1.5+ | WinUI 3 runtime |
| **Git** | Latest | Version control |

### Required VS 2022 Workloads

1. Open **Visual Studio Installer**
2. Modify VS 2022 Community
3. Install workloads:
   - ✅ **.NET desktop development**
   - ✅ **Windows application development** (dengan Windows App SDK components)

### Optional Tools

- **PowerShell 7+** untuk build automation
- **Inno Setup** atau **Wix Toolset** untuk installer (jika diperlukan di masa depan)
- **NuGet CLI** untuk package management

---

## 📥 Getting the Source

### Clone Repository

```bash
git clone <repository-url>
cd WindowsDoctorAI
```

### Verify File Structure

```
D:\Project\WindowsDoctorAI\
├── WindowsDoctorAI.sln
├── WindowsDoctorAI\
│   ├── WindowsDoctorAI.csproj
│   ├── App.xaml + App.xaml.cs
│   ├── MainWindow.xaml + MainWindow.xaml.cs
│   ├── Models\
│   ├── Services\
│   ├── ViewModels\
│   ├── Views\
│   ├── Dialogs\
│   ├── Converters\
│   ├── Helpers\
│   ├── Styles\
│   ├── Assets\
│   └── app.manifest
├── docs\
├── README.md
├── LICENSE
└── CHANGELOG.md
```

---

## 🔨 Building the Project

### Method 1: Visual Studio 2022 (Recommended)

#### Debug Build

1. Open `WindowsDoctorAI.sln` di Visual Studio 2022
2. Tunggu package restore selesai (progress di status bar)
3. Set configuration:
   - **Solution Platform:** `x64`
   - **Solution Configuration:** `Debug`
4. Set startup project: klik kanan `WindowsDoctorAI` → **Set as Startup Project**
5. Build: **Build > Build Solution** (`Ctrl+Shift+B`)
6. Run: Press **F5** (as administrator)

#### Release Build

1. Ganti configuration ke **Release**
2. Build: **Build > Build Solution** (`Ctrl+Shift+B`)
3. Output akan ada di: `WindowsDoctorAI\bin\x64\Release\net8.0-windows10.0.22621.0\`

### Method 2: Command Line (dotnet CLI)

```bash
cd D:\Project\WindowsDoctorAI\WindowsDoctorAI

# Restore packages
dotnet restore

# Debug build
dotnet build -c Debug -p:Platform=x64

# Release build
dotnet build -c Release -p:Platform=x64
```

### Method 3: MSBuild

```bash
msbuild WindowsDoctorAI.sln /p:Configuration=Release /p:Platform=x64
```

---

## ✅ Build Verification

### Success Criteria

Setelah build berhasil, verify:

- ✅ **0 Errors** di Error List
- ✅ **0 Warnings** di Error List
- ✅ Output message: `Build succeeded`
- ✅ Executable ada di output folder

### Common Build Locations

| Configuration | Path |
|---------------|------|
| Debug | `bin\x64\Debug\net8.0-windows10.0.22621.0\WindowsDoctorAI.exe` |
| Release | `bin\x64\Release\net8.0-windows10.0.22621.0\WindowsDoctorAI.exe` |
| Publish | `bin\x64\Release\net8.0-windows10.0.22621.0\win-x64\publish\` |

---

## 📦 Publishing for Distribution

### Self-Contained Deployment (Recommended)

**Menghasilkan folder mandiri** yang bisa didistribusikan tanpa perlu install .NET runtime di target machine.

#### Via Visual Studio

1. Klik kanan project `WindowsDoctorAI` → **Publish**
2. **Target:** Folder
3. **Location:** `bin\Release\Publish\` (atau custom path)
4. **Deployment mode:** Self-contained
5. **Target runtime:** `win-x64`
6. **Advanced settings:**
   - ✅ Produce single file: Optional (untuk single EXE)
   - ✅ Trim unused code: **Disabled** (WinUI 3 tidak support trimming)
   - ✅ Ready to Run: **Enabled** (performa startup)
7. Klik **Publish**

#### Via Command Line

**Standard self-contained folder:**

```bash
dotnet publish -c Release -r win-x64 --self-contained true `
    -p:PublishReadyToRun=true `
    -p:PublishSingleFile=false `
    -o "D:\Project\WindowsDoctorAI\Publish\Standard"
```

**Single-file executable:**

```bash
dotnet publish -c Release -r win-x64 --self-contained true `
    -p:PublishReadyToRun=true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o "D:\Project\WindowsDoctorAI\Publish\SingleFile"
```

### Output Size Estimation

| Deployment Mode | Approx Size |
|-----------------|-------------|
| Self-contained folder | ~180-250 MB (many files) |
| Self-contained single file | ~150-180 MB (1 EXE) |
| Framework-dependent | ~5-10 MB (requires .NET installed) |

---

## 📋 Pre-Release Checklist

Sebelum release ke public, verify checklist berikut:

### Code Quality
- [ ] Build sukses tanpa errors dan warnings
- [ ] Semua diagnostic categories berfungsi
- [ ] Semua repair actions dapat dieksekusi (di test system)
- [ ] Toast notifications muncul dengan benar
- [ ] Dark mode & Light mode tested
- [ ] Fullscreen dan windowed mode responsive

### Testing
- [ ] Full scan complete tanpa error
- [ ] Report export ke HTML berhasil
- [ ] Semua dialog buka/tutup dengan benar
- [ ] Cancel scan berfungsi
- [ ] Repair execution flow (Confirm → Progress → Toast) berjalan
- [ ] Application startup < 3 seconds
- [ ] Memory usage < 200 MB idle

### Documentation
- [ ] README.md updated dengan version terbaru
- [ ] CHANGELOG.md updated dengan release notes
- [ ] Version number di `.csproj` dan `App.xaml.cs` konsisten
- [ ] LICENSE file present dan valid
- [ ] Screenshots (kalau ada) up-to-date

### Version Management
- [ ] Update `Version` di `WindowsDoctorAI.csproj`
- [ ] Update version display di About page
- [ ] Update CHANGELOG dengan tag version dan tanggal
- [ ] Git tag: `git tag v2.0.0 -m "Release v2.0.0"`

### Distribution Package
- [ ] Publish self-contained build sukses
- [ ] Test executable di clean machine (VM)
- [ ] Verifikasi UAC prompt muncul saat run
- [ ] Verifikasi tidak ada missing DLL errors
- [ ] Zip folder untuk distribution

---

## 🏷️ Versioning Strategy

### Semantic Versioning (SemVer)

Format: **MAJOR.MINOR.PATCH**

| Version Component | When to Increment | Example |
|-------------------|-------------------|---------|
| **MAJOR** | Breaking changes, complete rewrite | 1.x.x → 2.0.0 |
| **MINOR** | New features, backward-compatible | 2.0.x → 2.1.0 |
| **PATCH** | Bug fixes, backward-compatible | 2.0.0 → 2.0.1 |

### Update Version in Code

#### `WindowsDoctorAI.csproj`

```xml
<PropertyGroup>
    <Version>2.0.0</Version>
    <FileVersion>2.0.0.0</FileVersion>
    <AssemblyVersion>2.0.0.0</AssemblyVersion>
</PropertyGroup>
```

#### `Views/AboutPage.xaml`

```xml
<TextBlock Text="Version 2.0.0 - Premium Edition" />
```

---

## 📤 Distribution Options

### Option 1: GitHub Releases (Recommended untuk Open Source)

1. Commit dan push semua changes
2. Create git tag: `git tag v2.0.0`
3. Push tag: `git push origin v2.0.0`
4. Buka GitHub repository → Releases → **Draft a new release**
5. Pilih tag `v2.0.0`
6. Fill in release notes (copy dari CHANGELOG.md)
7. Upload build artifacts:
   - `WindowsDoctorAI-v2.0.0-x64.zip` (self-contained)
   - `WindowsDoctorAI-v2.0.0-portable.exe` (single file)
8. Publish release

### Option 2: Direct Download (Website/Hosting)

Upload ZIP ke:
- Google Drive
- Dropbox
- Personal website
- GitHub Pages

Share direct download link ke users.

### Option 3: Microsoft Store (Future - v3.0.0)

**Requires:**
- MSIX packaging (change `<WindowsPackageType>` di csproj)
- Microsoft Partner Center account ($19 one-time)
- App submission review process (~1-2 weeks)
- Compliance dengan Microsoft Store policies

### Option 4: Winget (Windows Package Manager)

**Requires:**
- Publish ke GitHub Releases dulu
- Submit manifest ke [winget-pkgs repository](https://github.com/microsoft/winget-pkgs)
- Users install via: `winget install WindowsDoctorAI`

---

## 🔐 Code Signing (Recommended untuk Public Release)

### Why Code Sign?

Tanpa code signing, Windows SmartScreen akan menampilkan warning saat user pertama kali menjalankan aplikasi. Users mungkin ragu untuk continue.

### Cost

| Provider | Price (Approx) |
|----------|----------------|
| **Sectigo** | $150-200/year |
| **DigiCert** | $400-600/year |
| **Certum (Open Source)** | ~$25/year (untuk open source projects) |

### Signing Process (Setelah punya certificate)

```bash
signtool sign /f "cert.pfx" /p "password" /tr http://timestamp.sectigo.com /td sha256 /fd sha256 WindowsDoctorAI.exe
```

**Note:** Code signing planned untuk **v2.1.0** release.

---

## 🧪 Testing Distribution Build

### Test on Clean Machine

**Recommended Setup:**
1. Buat Windows 10/11 VM di Hyper-V atau VMware
2. Tidak install Visual Studio atau .NET SDK
3. Copy published folder ke VM
4. Run `WindowsDoctorAI.exe` as administrator
5. Verify:
   - ✅ Application starts tanpa missing DLL errors
   - ✅ UAC prompt muncul
   - ✅ Full scan berjalan
   - ✅ Repair actions bisa dieksekusi
   - ✅ Report export berhasil

### Common Distribution Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| "The application was unable to start correctly" | Missing .NET runtime | Use self-contained deployment |
| "MSVCP140.dll missing" | VC++ Redistributable tidak ada | Bundle atau require VC++ Redist install |
| SmartScreen warning | No code signing | Get code signing certificate |
| Antivirus false positive | Unsigned + system operations | Get code signing + submit for whitelist |

---

## 🔄 Continuous Integration (Future)

### GitHub Actions Setup (Planned)

Contoh `.github/workflows/build.yml`:

```yaml
name: Build & Release

on:
  push:
    tags: ['v*']

jobs:
  build:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore
        run: dotnet restore WindowsDoctorAI.sln
      
      - name: Build
        run: dotnet build WindowsDoctorAI.sln -c Release -p:Platform=x64
      
      - name: Publish
        run: dotnet publish WindowsDoctorAI\WindowsDoctorAI.csproj -c Release -r win-x64 --self-contained
      
      - name: Zip
        run: Compress-Archive -Path "bin\x64\Release\net8.0-windows10.0.22621.0\win-x64\publish\*" -DestinationPath "WindowsDoctorAI-${{ github.ref_name }}-x64.zip"
      
      - name: Release
        uses: softprops/action-gh-release@v1
        with:
          files: WindowsDoctorAI-${{ github.ref_name }}-x64.zip
```

**Status:** Planned untuk v2.2.0+.

---

## 🎯 Post-Release Actions

Setelah release:

1. **Announce** di social media / relevant communities
2. **Update GitHub Wiki** dengan release notes summary
3. **Monitor Issues** untuk bug reports
4. **Update Download counters** (kalau tracking)
5. **Start planning** next version (v2.0.1 patch atau v2.1.0 minor)

---

## 📞 Support & Troubleshooting Build Issues

### Common Build Errors

**Error: "The type or namespace name 'X' could not be found"**
- **Solution:** Restore NuGet packages: `dotnet restore`

**Error: "SDK 'Microsoft.NET.Sdk' not found"**
- **Solution:** Install .NET 8 SDK dari [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)

**Error: "WinUI 3 project cannot find Microsoft.WindowsAppSDK"**
- **Solution:** Install Windows App SDK workload di Visual Studio Installer

**Error: "MSB4062: The 'Microsoft.WindowsAppSDK...' task could not be loaded"**
- **Solution:** Update Windows App SDK ke versi terbaru

### Getting Help

- **GitHub Issues:** Report build issues dengan detail
- **Documentation:** Cek dokumen lain di `docs/`

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial build & release guide | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Build & Release Guide for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>