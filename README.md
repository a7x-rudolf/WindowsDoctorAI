<div align="center">

# 🩺 WindowsDoctorAI

**Comprehensive Windows System Diagnostic & Repair Tool**

[![Version](https://img.shields.io/badge/version-2.0.0-blue.svg)](CHANGELOG.md)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D4.svg)]()
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4.svg)]()
[![WinUI 3](https://img.shields.io/badge/WinUI-3.0-purple.svg)]()
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/status-Production%20Ready-brightgreen.svg)]()

*A premium diagnostic tool that helps users identify and fix Windows system issues with ease.*

[Features](#-features) • [Installation](#-installation) • [Screenshots](#-screenshots) • [Documentation](#-documentation) • [License](#-license)

</div>

---

<div align="center">

### 👨‍💻 Developer & Copyright Owner

**RIDOLF WIDI ALFISA LUMBA**

*Sole Developer, Architect & Copyright Holder*

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.
Licensed under the [MIT License](LICENSE).

</div>

---

## 📖 Overview

**WindowsDoctorAI** is a modern, premium-grade diagnostic and repair tool for Windows 10/11 systems. Built with the latest **WinUI 3** framework, it provides a beautiful Fluent Design interface for scanning your PC across 7 major categories and offers actionable repair recommendations with one-click fixes.

Whether you're a power user troubleshooting performance issues or an IT professional maintaining multiple systems, WindowsDoctorAI provides comprehensive insights into your system's health.

---

## ✨ Features

### 🔍 Comprehensive Diagnostics
- **💾 Disk Health** — Space analysis, S.M.A.R.T. monitoring, temp file detection, optimization status
- **⚡ Performance** — CPU usage, RAM utilization, high-memory processes, system uptime
- **🌐 Network** — Adapter status, internet connectivity, DNS resolution, latency tests
- **🔒 Security** — Antivirus status, Windows Firewall, UAC configuration, security services
- **🔄 Windows Update** — Service status, last update date, pending reboot detection
- **🛠️ Drivers** — Problem device detection, outdated driver identification
- **🚀 Startup Programs** — Registry entries analysis, scheduled tasks review

### 🔧 30+ Automated Repair Actions
- Disk cleanup and temp file removal
- Network stack reset (Winsock, TCP/IP, DNS flush)
- Windows Defender definitions update
- Service restart (Firewall, Windows Update, Security Center)
- UAC enable/disable
- Registry-based fixes
- System tool launchers (Task Manager, Device Manager, etc.)

### 💎 Premium User Experience
- **Fluent Design UI** — Modern Windows 11 aesthetic with Mica-inspired backgrounds
- **Live Progress Dialogs** — Real-time scan progress with category-by-category updates
- **Interactive Notifications** — Toast notifications for repair completion
- **Grid Layout Results** — Responsive card-based results with severity indicators
- **HTML Report Export** — Beautiful standalone reports saved to Desktop
- **Dark/Light Theme** — System-adaptive or manual selection

---

## 🖼️ Screenshots

> **Note:** Screenshots akan ditambahkan di sini. Lihat `docs/images/` untuk gallery lengkap.

| Dashboard | Scan Progress | Repair Actions |
|-----------|---------------|----------------|
| *Score gauge + category health* | *Live activity log* | *Grid layout dengan risk assessment* |

---

## 💻 System Requirements

### Minimum Requirements
- **OS**: Windows 10 version 1809 (Build 17763) or later
- **Architecture**: x64
- **RAM**: 4 GB
- **Disk Space**: 200 MB
- **Permissions**: Administrator rights (for full functionality)

### Recommended
- **OS**: Windows 11 (any version)
- **RAM**: 8 GB or more
- **Display**: 1400×900 or higher resolution
- **.NET Runtime**: .NET 8.0 (bundled with self-contained builds)

---

## 📥 Installation

### Option 1: Portable (Recommended)
1. Download the latest release from [Releases](../../releases)
2. Extract the ZIP to any folder
3. Right-click `WindowsDoctorAI.exe` → **Run as administrator**

### Option 2: Build from Source
See [Build Instructions](docs/07-Deployment/BUILD_RELEASE.md) for detailed steps.

**Quick Start:**
```bash
git clone <repository-url>
cd WindowsDoctorAI
dotnet restore
dotnet build -c Release
```

Open `WindowsDoctorAI.sln` in Visual Studio 2022 and press F5.

---

## 🚀 Quick Start

1. **Launch the app** (as Administrator for full features)
2. Navigate to **Dashboard**
3. Click **"Run Full Scan"** — takes ~30-45 seconds
4. Review results in **Scan Results** page
5. Click any result to see technical details and available repair actions
6. Navigate to **Repair Actions** to execute fixes with confirmation
7. Optionally click **"Export Report"** to save an HTML report to your Desktop

---

## 🏗️ Technology Stack

| Component | Technology |
|-----------|------------|
| **Runtime** | .NET 8.0 |
| **UI Framework** | WinUI 3 (Windows App SDK 1.5) |
| **Language** | C# 12 |
| **Pattern** | MVVM (Model-View-ViewModel) |
| **MVVM Library** | CommunityToolkit.Mvvm 8.2.2 |
| **System APIs** | WMI, Registry, PowerShell, ServiceController |
| **IDE** | Visual Studio 2022 Community |
| **Design System** | Fluent Design + Segoe Fluent Icons |

---

## 📚 Documentation

Comprehensive documentation is available in the [`docs/`](docs/) folder:

- 📋 [Project Charter](docs/00-Overview/PROJECT_CHARTER.md) — Vision, mission, objectives
- 🏛️ [Architecture Overview](docs/01-Architecture/ARCHITECTURE.md) *(coming soon)*
- 🔬 [Diagnostic Categories](docs/03-Features/DIAGNOSTIC_CATEGORIES.md) *(coming soon)*
- 🔧 [Repair Actions Catalog](docs/03-Features/REPAIR_ACTIONS.md) *(coming soon)*
- 🛡️ [Security Disclaimer](docs/05-Security/DISCLAIMER.md)
- ❓ [FAQ](docs/08-Operations/FAQ.md)
- 📝 [Changelog](CHANGELOG.md)

---

## ⚠️ Important Notice

WindowsDoctorAI performs system-level operations including:
- Registry read/write
- Service management
- Process execution
- Network configuration changes

**Please read the [Disclaimer](docs/05-Security/DISCLAIMER.md) before use.**

While all repair actions are confirmed via dialogs and marked with risk levels, we recommend creating a **System Restore Point** before executing medium/high-risk repairs.

---

## 🤝 Contributing

This is currently a solo developer project. Contributing guidelines will be published when the project opens to community contributions.

For bug reports and feature requests, please use the [Issues](../../issues) section.

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙏 Credits & Acknowledgments

### Development
- **Sole Developer & Architect**: **RIDOLF WIDI ALFISA LUMBA**
- **Copyright Owner**: RIDOLF WIDI ALFISA LUMBA © 2025
- **Project Type**: Independent Solo Developer Project

### Technologies & Libraries
- **UI Framework**: Microsoft WinUI 3 Team
- **MVVM Library**: [CommunityToolkit](https://github.com/CommunityToolkit/dotnet)
- **Icons**: Segoe Fluent Icons (Microsoft)
- **Runtime**: .NET 8.0 (Microsoft)

### Inspiration
- **Original Concept**: [a7x-rudolf/WindowsDoctorAI](https://github.com/a7x-rudolf/WindowsDoctorAI) — This project is a complete rewrite and modernization## 🙏 Credits & Acknowledgments

### Development
- **Sole Developer & Architect**: **RIDOLF WIDI ALFISA LUMBA**
- **Copyright Owner**: RIDOLF WIDI ALFISA LUMBA © 2025
- **Project Type**: Independent Solo Developer Project

### Technologies & Libraries
- **UI Framework**: Microsoft WinUI 3 Team
- **MVVM Library**: [CommunityToolkit](https://github.com/CommunityToolkit/dotnet)
- **Icons**: Segoe Fluent Icons (Microsoft)
- **Runtime**: .NET 8.0 (Microsoft)

### Inspiration
- **Original Concept**: [a7x-rudolf/WindowsDoctorAI](https://github.com/a7x-rudolf/WindowsDoctorAI) — This project is a complete rewrite and modernization

## 📮 Contact & Support

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Documentation**: [`docs/`](docs/) folder

---

<div align="center">

**Made with ❤️ by RIDOLF WIDI ALFISA LUMBA**

*Version 2.0.0 - Premium Edition*

*Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.*

*Licensed under the MIT License.*

</div>