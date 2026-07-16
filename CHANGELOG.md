# Changelog

All notable changes to **WindowsDoctorAI** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---


## 📌 Project Information

- **Developer**: RIDOLF WIDI ALFISA LUMBA
- **Copyright**: © 2025 RIDOLF WIDI ALFISA LUMBA
- **License**: MIT License
- **Project Type**: Solo Developer Independent Project
- **Started**: 2025

---

## [Unreleased]

### Planned
- Scan history persistence (SQLite)
- Parallel scan execution
- Dark mode visual audit
- Unit test coverage
- MSIX packaging for Microsoft Store

---

## [2.0.1] - 2025-07-15 - Premium Edition

### 🎉 Major Release: Complete Rewrite

This is a complete rewrite of the original WindowsDoctorAI, migrated from the
original codebase to modern .NET 8.0 and WinUI 3 with a premium Fluent Design UI.

### ✨ Added

#### Architecture
- Complete MVVM architecture with CommunityToolkit.Mvvm 8.2.2
- Clean separation of concerns: Views, ViewModels, Services, Models, Helpers
- Event-driven scan orchestration with cancellation support
- 15+ IValueConverters for reactive UI bindings
- Global exception handling with graceful degradation

#### Diagnostic Categories (7 total)
- **Disk Health**: Free space analysis, S.M.A.R.T. status, temp file detection, HDD/SSD optimization check
- **Performance**: CPU usage monitoring, RAM utilization, high-memory process detection, system uptime tracking
- **Network**: Network adapter enumeration, internet connectivity test, DNS resolution check, latency measurement
- **Security**: Antivirus detection via WMI SecurityCenter2, Windows Firewall status, UAC configuration check, security service monitoring
- **Windows Update**: Service status check, last update date detection, pending reboot detection
- **Drivers**: Problem device detection via WMI, outdated driver identification via PowerShell
- **Startup Programs**: Registry Run/RunOnce keys analysis, scheduled tasks review

#### Repair Actions (30+ actions)
- Automatic: Clear Temp Files, Reset Network Stack, Release/Renew IP, Set Google DNS, Update Defender Definitions, Enable UAC via Registry
- Command Line: chkdsk, ipconfig commands, shutdown scheduling
- System Tools: Task Manager, Resource Monitor, Device Manager, Disk Cleanup, Defragment tool
- Settings Deep Links: Storage Sense, Windows Update, Backup, Startup Apps
- Service Management: Start Firewall, Start Windows Update, Start Security services

#### User Interface
- **Dashboard Page**: Hero card with circular score gauge (0-100), quick action buttons, statistics cards, category health grid
- **Diagnostic Scan Page**: Category selection with checkbox cards, estimated duration display, start/cancel controls
- **Scan Results Page**: Responsive grid layout (2-4 columns), segmented filter chips (All/Critical/Warning/Healthy), sorted by severity
- **Repair Actions Page**: Grid layout with risk assessment tags, "Fix All Safe Issues" batch button, per-action Execute
- **Scan History Page**: Placeholder with empty state (SQLite persistence coming in v2.1.0)
- **System Info Page**: Comprehensive hardware and software details
- **Settings Page**: 2-column layout with 5 sections (Appearance, Scan Preferences, Notifications, Advanced, About)
- **About Page**: 2-column layout with app hero card, features list, tech stack

#### Interactive Dialogs
- **Scan Progress Dialog**: Live category status tracking (7 rows), real-time activity log with timestamps, LIVE indicator, progress bar with percentage
- **Confirm Repair Dialog**: Detail table showing Action Type, Risk Level, Estimated Time, Admin/Reboot requirements, contextual info callout
- **Repair Progress Dialog**: Live execution log, indeterminate progress bar, dynamic completion status (success/failure), reboot warnings
- **Result Detail Dialog**: Full technical details, category and severity metadata, available actions list with direct Execute buttons

#### Notifications & Feedback
- Toast notification system (custom implementation, no ContentDialog conflicts)
- Status bar with live status dot, admin badge, last scan timestamp
- Progress ring in status bar during operations
- Auto-dismissing toasts (3-4 seconds) with manual close option

#### Design & Icons
- Segoe Fluent Icons throughout (no emoji, no custom fonts)
- Fluent Design System compliance
- Brand accent color (#0067C0) with gradient variations
- Status color system: Healthy (#0F7B0F), Warning (#B7710D), Critical (#C42B1C), Info (#005FB8)
- Responsive UniformGridLayout for cards
- Mica-inspired background gradients

#### Reports & Export
- HTML report generation with premium styling
- Auto-save to Desktop with timestamp filename
- Includes: Health score, summary statistics, system info, all results grouped by category, available actions
- Auto-opens in default browser after generation

#### Theme System
- Light theme (default)
- Dark theme support (via ThemeService)
- System-adaptive option

### 🔒 Security
- Requires Administrator elevation via app.manifest
- Graceful degradation for non-admin sessions
- All PowerShell scripts use `-NoProfile -NonInteractive` flags
- Process timeout enforcement (120s default) with tree-kill on timeout
- No user-supplied input in system commands (all hardcoded in service layer)

### 🛡️ Reliability
- Per-category error isolation (scan failure in one category doesn't stop others)
- Try-catch on all I/O and WMI operations
- Cancellation token support for scan operations
- DispatcherQueue marshalling for thread-safe UI updates

### 📊 Performance
- Full scan completion in 20-45 seconds
- Memory footprint: ~100-170 MB
- UI remains responsive during scans (async/await + Task.Run)
- Self-contained deployment: ~150 MB

---

## [1.0.0] - Original Release

### Note
Version 1.0.0 refers to the original codebase from [a7x-rudolf/WindowsDoctorAI](https://github.com/a7x-rudolf/WindowsDoctorAI).

Version 2.0.1 represents a complete rewrite and modernization effort, transitioning from the original implementation to a modern WinUI 3 + .NET 8.0 architecture.

---

## Versioning Convention

This project follows [Semantic Versioning](https://semver.org/):

- **MAJOR** version: Incompatible API changes or major UI/UX overhauls
- **MINOR** version: New functionality added in a backward-compatible manner
- **PATCH** version: Backward-compatible bug fixes

### Version Categories in Changelog
- `Added` — New features
- `Changed` — Changes in existing functionality
- `Deprecated` — Soon-to-be removed features
- `Removed` — Removed features
- `Fixed` — Bug fixes
- `Security` — Security-related changes

---

[Unreleased]: ../../compare/v2.0.0...HEAD
[2.0.0]: ../../releases/tag/v2.0.0
[1.0.0]: https://github.com/a7x-rudolf/WindowsDoctorAI