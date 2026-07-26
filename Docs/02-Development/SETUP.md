# 🛠️ Development Environment Setup

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Panduan lengkap setup environment development untuk berkontribusi atau mengembangkan WindowsDoctorAI. Dokumen ini mencakup instalasi tools, konfigurasi IDE, dan verifikasi setup.

**Target Audience:**
- Developer baru yang akan bekerja dengan codebase
- Contributors (future)
- RIDOLF WIDI ALFISA LUMBA (sebagai reference)

---

## 🎯 Development Stack Overview

| Component | Version | Purpose |
|-----------|---------|---------|
| **Windows OS** | 10 (22H2+) / 11 | Development platform |
| **Visual Studio 2022** | 17.9+ Community | Primary IDE |
| **.NET SDK** | 8.0.400+ | Runtime & compiler |
| **Windows App SDK** | 1.5+ | WinUI 3 framework |
| **Git** | Latest | Version control |
| **Visual Studio Code** (optional) | Latest | Documentation editing |

---

## 📥 Step 1: Install Windows Prerequisites

### 1.1 Verify Windows Version

Buka PowerShell dan run:
```powershell
[System.Environment]::OSVersion.Version
```

**Required:** Windows 10 Build 17763+ atau Windows 11 (any build).

### 1.2 Enable Developer Mode

1. Open **Settings**
2. Navigate to **Privacy & Security** → **For developers**
3. Enable **Developer Mode** toggle
4. Confirm UAC prompt

**Why:** Developer Mode diperlukan untuk debugging WinUI apps dan sideloading tanpa signed certificate.

### 1.3 Install Windows Updates

Ensure Windows fully updated:
```
Settings → Windows Update → Check for updates → Install all
```

---

## 📦 Step 2: Install Visual Studio 2022

### 2.1 Download Visual Studio 2022 Community

Download dari: https://visualstudio.microsoft.com/vs/community/

**Edition:** Community (free untuk individual developers dan open source)

### 2.2 Install Required Workloads

Saat Visual Studio Installer terbuka, pilih workloads berikut:

#### Wajib:

✅ **.NET desktop development**
- Includes: .NET 8 SDK, WPF, Windows Forms components

✅ **Windows application development**
- Includes: Windows App SDK, WinUI 3 project templates

#### Optional (Recommended):

✅ **.NET Multi-platform App UI development (MAUI)** — untuk future cross-platform
✅ **Data storage and processing** — kalau butuh SQLite integration

### 2.3 Individual Components

Klik tab **Individual components** dan pastikan installed:

- ✅ .NET 8.0 Runtime
- ✅ Windows 10/11 SDK (10.0.19041 atau lebih baru)
- ✅ Git for Windows
- ✅ NuGet package manager
- ✅ MSBuild
- ✅ Windows App SDK C# Templates

### 2.4 Verify Installation

Setelah install selesai:

```powershell
# Verify .NET SDK
dotnet --version
# Expected: 8.0.x or higher

# Verify workloads
dotnet workload list
# Should include: microsoft-windows-appsdk

# Verify Git
git --version
# Expected: git version 2.x.x
```

---

## 🔧 Step 3: Configure Visual Studio 2022

### 3.1 Set Preferences

**Tools → Options:**

#### Text Editor → C# → Advanced
- ✅ Enable "Use IntelliSense to complete braces"
- ✅ Enable "Add usings on paste"

#### Text Editor → All Languages → Tabs
- **Indenting:** Smart
- **Tab size:** 4
- **Indent size:** 4
- ✅ Insert spaces

#### Environment → Documents
- ✅ Detect when file is changed outside the environment
- ✅ Auto-load changes, if saved

#### Debugging → General
- ✅ Enable Just My Code
- ✅ Enable .NET Framework source stepping

### 3.2 Install Recommended Extensions

**Extensions → Manage Extensions:**

#### Essential:

- 🔌 **XAML Styler** — Auto-format XAML files
- 🔌 **Markdown Editor v2** — Preview markdown documents
- 🔌 **Git Extensions** — Enhanced Git integration

#### Optional (Productivity):

- 🔌 **CodeMaid** — Code cleanup automation
- 🔌 **Productivity Power Tools** — Various small enhancements
- 🔌 **GitHub Copilot** — AI code assistance (subscription required)

### 3.3 Set Solution Explorer Preferences

**View → Solution Explorer:**
- ✅ Show All Files
- ✅ Group Files by Type
- ✅ Sync with Active Document

---

## 📂 Step 4: Clone the Repository

### 4.1 Choose Development Folder

Rekomendasi struktur folder:
```
D:\Project\
└── WindowsDoctorAI\    ← Clone here
```

### 4.2 Clone via Git

```bash
cd D:\Project
git clone <repository-url> WindowsDoctorAI
cd WindowsDoctorAI
```

**Jika belum ada Git remote (proyek personal):**
```bash
# Initialize new repo
cd D:\Project\WindowsDoctorAI
git init
git add .
git commit -m "Initial commit"
```

### 4.3 Verify Repository Structure

```
D:\Project\WindowsDoctorAI\
├── .git\
├── WindowsDoctorAI.sln
├── WindowsDoctorAI\
│   ├── WindowsDoctorAI.csproj
│   ├── App.xaml + .cs
│   ├── MainWindow.xaml + .cs
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
├── CHANGELOG.md
└── .gitignore
```

---

## 🚀 Step 5: First Build

### 5.1 Open Solution in Visual Studio

**Method A - Via File Explorer:**
Double-click `WindowsDoctorAI.sln`

**Method B - Via Visual Studio:**
1. Open Visual Studio 2022
2. Click **Open a project or solution**
3. Navigate to `WindowsDoctorAI.sln`

### 5.2 Restore NuGet Packages

Visual Studio otomatis restore packages saat open solution. Kalau tidak:

**Tools → NuGet Package Manager → Manage NuGet Packages for Solution → Restore**

Atau via command line:
```bash
cd WindowsDoctorAI
dotnet restore
```

**Expected packages:**
- Microsoft.WindowsAppSDK 1.5.240627000
- Microsoft.Windows.SDK.BuildTools 10.0.26100.1
- CommunityToolkit.Mvvm 8.2.2
- System.Management 8.0.0
- System.Diagnostics.PerformanceCounter 8.0.0
- Microsoft.Win32.Registry 5.0.0
- System.ServiceProcess.ServiceController 8.0.0

### 5.3 Set Configuration

- **Solution Configuration:** `Debug`
- **Solution Platform:** `x64`
- **Startup Project:** `WindowsDoctorAI` (right-click → Set as Startup Project)

### 5.4 Build the Solution

**Menu:** Build → Build Solution (`Ctrl+Shift+B`)

**Success Criteria:**
- ✅ 0 Errors
- ✅ 0 Warnings
- ✅ Message: "Build succeeded"

### 5.5 Run the Application

Press **F5** untuk debug run, atau **Ctrl+F5** untuk run tanpa debugger.

**Expected:**
- Windows UAC prompt muncul (karena `requireAdministrator` di app.manifest)
- WindowsDoctorAI window opens
- Dashboard page ter-load

---

## 🐛 Step 6: Verify Debugging Works

### 6.1 Set Breakpoint

1. Open `MainWindow.xaml.cs`
2. Find constructor `public MainWindow()`
3. Click di left margin baris `this.InitializeComponent();` untuk set breakpoint (red dot)

### 6.2 Debug Run

Press **F5**. Application harus stop di breakpoint.

**Verify:**
- ✅ Breakpoint hit
- ✅ Variables visible di Locals window
- ✅ Call Stack menampilkan MainWindow constructor
- ✅ Bisa Step Over (F10) dan Step Into (F11)

### 6.3 Test Hot Reload

Saat app running di debugger:

1. Modify XAML (misalnya ganti text di About page)
2. Click **Hot Reload** button (🔥 icon di toolbar) atau press `Alt+F10`
3. Changes harus reflected di running app tanpa restart

---

## 📝 Step 7: (Optional) Install VS Code for Documentation

Untuk edit dokumentasi markdown lebih nyaman:

### 7.1 Download VS Code

https://code.visualstudio.com/

### 7.2 Recommended Extensions

- 📝 **Markdown All in One** — Comprehensive markdown support
- 📝 **Markdown Preview Enhanced** — Better preview dengan diagrams
- 📝 **markdownlint** — Linting untuk markdown
- 📝 **Mermaid Preview** — Preview mermaid diagrams

### 7.3 Open Documentation Folder

```bash
cd D:\Project\WindowsDoctorAI\docs
code .
```

---

## 🔍 Step 8: Verify Full Setup

Run checklist final:

- [ ] Windows 10 (17763+) atau Windows 11 installed
- [ ] Developer Mode enabled
- [ ] Visual Studio 2022 Community installed
- [ ] .NET 8 SDK installed (`dotnet --version` shows 8.0.x)
- [ ] Windows App SDK workload installed
- [ ] Git installed dan configured
- [ ] Repository cloned ke `D:\Project\WindowsDoctorAI\`
- [ ] Solution opens tanpa error di VS 2022
- [ ] NuGet packages restored
- [ ] Solution builds successfully (0 errors, 0 warnings)
- [ ] Application runs dengan F5
- [ ] UAC prompt muncul
- [ ] Debugger breakpoints berfungsi
- [ ] Hot Reload berfungsi

---

## 🎯 Development Workflow

### Typical Workflow

1. **Pull Latest Changes** (kalau kolaboratif):
   ```bash
   git pull origin main
   ```

2. **Create Feature Branch:**
   ```bash
   git checkout -b feature/my-new-feature
   ```

3. **Make Changes** di Visual Studio

4. **Test Locally:**
   - Build: `Ctrl+Shift+B`
   - Run: `F5`
   - Verify feature works
   - Check tidak ada regressions

5. **Commit Changes:**
   ```bash
   git add .
   git commit -m "Add: My new feature description"
   ```

6. **Push & Create Pull Request** (jika collaborative):
   ```bash
   git push origin feature/my-new-feature
   ```

---

## 🆘 Troubleshooting Setup Issues

### Issue: "SDK 'Microsoft.NET.Sdk' not found"

**Solution:**
```bash
# Reinstall .NET 8 SDK
winget install Microsoft.DotNet.SDK.8
```

### Issue: "Windows App SDK not found"

**Solution:**
1. Open Visual Studio Installer
2. Modify VS 2022 → Individual components
3. Search "Windows App SDK"
4. Check semua components → Modify

### Issue: NuGet restore fails

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore ulang
dotnet restore
```

### Issue: Build error "Unable to find package Microsoft.WindowsAppSDK"

**Solution:**
1. Verify internet connection
2. Check NuGet sources:
   ```
   Tools → NuGet Package Manager → Package Manager Settings → Package Sources
   ```
   Ensure `nuget.org` enabled

### Issue: Debugger tidak attach

**Solution:**
1. Close VS 2022
2. Delete folder `bin\` dan `obj\` di project
3. Reopen VS 2022
4. Rebuild solution

---

## 📚 Additional Resources

### Official Documentation

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [WinUI 3 Documentation](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)
- [Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

### Video Tutorials

- [WinUI 3 Getting Started (Microsoft Learn)](https://learn.microsoft.com/en-us/windows/apps/get-started/)
- [MVVM Pattern Tutorial](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)

### Community Resources

- [WinUI GitHub Discussions](https://github.com/microsoft/microsoft-ui-xaml/discussions)
- [Stack Overflow - WinUI 3 tag](https://stackoverflow.com/questions/tagged/winui-3)

---

## 📞 Support

Jika mengalami issue setup:

- **Developer:** RIDOLF WIDI ALFISA LUMBA
- **Documentation:** [`docs/08-Operations/TROUBLESHOOTING.md`](../08-Operations/TROUBLESHOOTING.md)
- **GitHub Issues:** Report setup issues dengan label `setup-help`

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial setup guide | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Development Setup Guide for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>