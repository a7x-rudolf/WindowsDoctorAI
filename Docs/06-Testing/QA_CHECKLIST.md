# ✅ Pre-Release QA Checklist

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

**Comprehensive checklist** untuk quality assurance sebelum release WindowsDoctorAI. Setiap item harus diverifikasi dan di-check off oleh RIDOLF WIDI ALFISA LUMBA (developer) sebelum merilis ke publik.

**Cara Penggunaan:**
- Copy checklist ini per release
- Tandai ✅ atau ❌ untuk setiap item
- Rilis TIDAK boleh dilakukan jika ada item Critical yang ❌
- Dokumentasikan alasan untuk item yang di-skip

---

## 🎯 Release Information

**Version:** __________________
**Release Date:** __________________
**Release Type:** ☐ Major (X.0.0) ☐ Minor (X.Y.0) ☐ Patch (X.Y.Z)
**Tested By:** __________________
**Test Environment:** __________________

---

## 1. 🏗️ Build & Compilation

### 1.1 Build Verification

- [ ] Code builds tanpa errors di Debug configuration
- [ ] Code builds tanpa errors di Release configuration
- [ ] Code builds tanpa warnings
- [ ] Solution restores NuGet packages successfully
- [ ] All 45+ source files compile tanpa issues
- [ ] Output executable size: ~5-10 MB (framework-dependent) atau ~150 MB (self-contained)

### 1.2 Static Analysis

- [ ] No suppressed warnings tanpa justification
- [ ] No `TODO` comments untuk critical features
- [ ] No `HACK` comments tanpa GitHub issue reference
- [ ] Code follows [CODING_STANDARDS.md](../02-Development/CODING_STANDARDS.md)

### 1.3 Dependencies

- [ ] NuGet packages up-to-date (tidak ada critical vulnerabilities)
- [ ] Dependency versions locked (tidak ada floating versions)
- [ ] License compliance verified (semua dependencies MIT/Apache/etc.)
- [ ] No unused packages di project

---

## 2. 🚀 Application Startup

### 2.1 Launch Testing

- [ ] Aplikasi launch dari executable double-click
- [ ] UAC prompt muncul dengan proper publisher info
- [ ] Splash screen brief (< 1 detik)
- [ ] MainWindow ter-load dalam < 5 detik (cold start)
- [ ] MainWindow ter-load dalam < 3 detik (warm start)
- [ ] Default page = Dashboard
- [ ] No error dialogs saat startup
- [ ] Icon aplikasi muncul di taskbar
- [ ] Icon aplikasi muncul di title bar

### 2.2 Non-Admin Launch

- [ ] Aplikasi launches tanpa UAC (kalau di-configure)
- [ ] Status bar menampilkan "Standard User (Limited)"
- [ ] User avatar section shows warning icon
- [ ] Warning icon color = orange
- [ ] Message jelas tentang limited functionality

### 2.3 First-Run Experience

- [ ] Fresh install (no previous data) launches successfully
- [ ] No corrupted state kalau folder di-delete
- [ ] Reasonable default settings

---

## 3. 🧭 Navigation & Layout

### 3.1 Sidebar Navigation

- [ ] Dashboard menu berfungsi
- [ ] Diagnostic Scan menu berfungsi
- [ ] Scan Results menu berfungsi
- [ ] Repair Actions menu berfungsi
- [ ] Scan History menu berfungsi
- [ ] System Info menu berfungsi
- [ ] Settings menu (footer) berfungsi
- [ ] About menu (footer) berfungsi
- [ ] Selected item highlighted correctly
- [ ] Category headers ("Diagnostics", "Insights") visible

### 3.2 User Panel (Bottom Sidebar)

- [ ] User avatar shows first letter of username
- [ ] Username correct (matches Windows user)
- [ ] Role text: "Elevated Session" atau "Standard User"
- [ ] Status icon color sesuai admin state

### 3.3 Layout Responsiveness

- [ ] Window resize 800×600: functional
- [ ] Window resize 1400×900: optimal
- [ ] Window resize 1920×1080: content well-distributed
- [ ] Window maximize: no layout breakage
- [ ] Content tidak stretch berlebihan di fullscreen
- [ ] Grid layouts adjust kolom (2/3/4 cols)
- [ ] Cards resize proportionally
- [ ] Text tidak terpotong
- [ ] Scrollbars muncul when needed

### 3.4 Page Transitions

- [ ] Navigasi antar page smooth (< 300ms)
- [ ] Previous page state preserved (NavigationCacheMode="Required")
- [ ] Frame content updates correctly
- [ ] No flicker atau visual glitches

---

## 4. 🔍 Diagnostic Scan

### 4.1 Scan Initiation

- [ ] "Run Full Scan" button di Dashboard berfungsi
- [ ] "Start Diagnostic Scan" button di Diagnostic page berfungsi
- [ ] Scan Progress Dialog muncul immediately
- [ ] Dialog shows 7 category rows
- [ ] Progress bar animates dengan shimmer effect
- [ ] LIVE indicator (red dot) berkedip

### 4.2 Scan Execution

- [ ] Category status transitions: Waiting → Scanning → Completed
- [ ] Icon status changes sesuai state (empty circle → spinner → checkmark)
- [ ] Live activity log menampilkan messages real-time
- [ ] Log timestamps akurat (mm:ss format)
- [ ] Log auto-scroll ke bottom
- [ ] Progress percentage updates smoothly
- [ ] Current category name displayed di header

### 4.3 Scan Categories

- [ ] Performance scan runs (2-3 detik)
- [ ] Disk Health scan runs (1-2 detik)
- [ ] Network scan runs (3-8 detik)
- [ ] Security scan runs (1-3 detik)
- [ ] Windows Update scan runs (3-5 detik)
- [ ] Drivers scan runs (5-20 detik)
- [ ] Startup Programs scan runs (< 1 detik)

### 4.4 Scan Completion

- [ ] Total scan time: 20-45 detik (typical)
- [ ] "View Results" button muncul setelah selesai
- [ ] Title dialog: "Scan Complete"
- [ ] Summary menampilkan total issues found
- [ ] Klik "View Results" navigates ke Scan Results page

### 4.5 Scan Cancellation

- [ ] "Cancel Scan" button responsive
- [ ] Cancellation happens dalam < 10 detik
- [ ] Dialog title berubah ke "Scan Cancelled"
- [ ] Partial results tersedia setelah cancel
- [ ] IsScanning state properly reset

### 4.6 Error Handling

- [ ] Error di single category tidak menghentikan scan
- [ ] Error message meaningful (not just stack trace)
- [ ] Failed checks di-mark dengan Status=Failed
- [ ] Aplikasi tidak crash saat scan error

---

## 5. 📊 Results Display

### 5.1 Dashboard After Scan

- [ ] Score gauge menampilkan angka correct (0-100)
- [ ] Score gauge warna sesuai rating (green/yellow/red)
- [ ] Health rating text muncul ("EXCELLENT", "GOOD", dll)
- [ ] Health description accurate
- [ ] Statistic cards menampilkan counts correct:
  - [ ] Critical count
  - [ ] Warning count
  - [ ] Healthy count
  - [ ] Total Issues count
- [ ] Category cards populated dengan data:
  - [ ] Category icon
  - [ ] Category name
  - [ ] Status pill (Healthy/Fair/Attention)
  - [ ] Description
  - [ ] Progress bar dengan color sesuai score
  - [ ] Score number

### 5.2 Scan Results Page

- [ ] Empty state muncul jika belum scan
- [ ] Grid layout responsive (2/3/4 cols)
- [ ] Setiap card menampilkan:
  - [ ] Severity icon dengan background color
  - [ ] Score besar (colored)
  - [ ] Severity badge (colored)
  - [ ] Title (semi-bold, max 2 lines)
  - [ ] Description (max 2 lines dengan ellipsis)
  - [ ] Category name dengan icon
  - [ ] Available actions count
- [ ] Cards sorted: Critical → Warning → Info → Healthy
- [ ] Filter chips berfungsi:
  - [ ] "All" shows semua
  - [ ] "Critical" filter accurate
  - [ ] "Warning" filter accurate
  - [ ] "Healthy" filter accurate
- [ ] Active filter chip highlighted
- [ ] Count text updates ("N result(s)")

### 5.3 Result Detail Dialog

- [ ] Click pada card membuka dialog
- [ ] Dialog menampilkan:
  - [ ] Severity icon dengan color
  - [ ] Title
  - [ ] Category + severity + score meta
  - [ ] Description section
  - [ ] Technical details (jika ada)
  - [ ] Available repair actions
  - [ ] "No actions needed" pesan jika healthy
- [ ] Klik "Execute" di action opens confirm dialog
- [ ] Close button berfungsi

---

## 6. 🔧 Repair Actions

### 6.1 Repair Actions Page

- [ ] Empty state muncul jika belum scan
- [ ] Hero header menampilkan:
  - [ ] Total repair actions count
  - [ ] "Fix All Safe Issues" button
- [ ] Warning InfoBar tentang review before executing
- [ ] Grid layout responsive
- [ ] Setiap card menampilkan:
  - [ ] Icon dengan background
  - [ ] Name (semi-bold)
  - [ ] Description (max 3 lines)
  - [ ] Tags: Risk, Type, Estimated time
  - [ ] Execute button

### 6.2 Confirm Repair Dialog

- [ ] Klik "Execute" opens confirm dialog
- [ ] Dialog menampilkan:
  - [ ] Warning icon
  - [ ] Title: "Confirm Repair Action"
  - [ ] Action name (semi-bold)
  - [ ] Action description
  - [ ] Detail table dengan icons:
    - [ ] Action Type
    - [ ] Risk Level (colored)
    - [ ] Estimated Time
    - [ ] Requires Admin
    - [ ] Requires Reboot
  - [ ] Info callout (Success/Info/Warning/Error)
- [ ] "Cancel" button closes dialog tanpa execute
- [ ] "Execute Repair" button starts execution

### 6.3 Repair Progress Dialog

- [ ] Progress dialog muncul setelah confirm
- [ ] Dialog menampilkan:
  - [ ] Hero icon (repair)
  - [ ] Title: "Executing Repair"
  - [ ] Action name subtitle
  - [ ] Indeterminate progress bar
  - [ ] Status text
  - [ ] LIVE indicator
  - [ ] Execution log dengan timestamps
- [ ] Log entries appear real-time
- [ ] Log auto-scroll

### 6.4 Repair Completion

- [ ] Success case:
  - [ ] Header icon berubah ke green checkmark
  - [ ] Title: "Repair Completed"
  - [ ] Success message di subtitle
  - [ ] Progress bar reaches 100%
  - [ ] "Close" button muncul
  - [ ] Success toast notification muncul
- [ ] Failure case:
  - [ ] Header icon berubah ke red critical
  - [ ] Title: "Repair Failed"
  - [ ] Error message di subtitle
  - [ ] Error toast notification muncul
- [ ] Reboot required case:
  - [ ] Warning message tentang reboot

### 6.5 Fix All Safe

- [ ] "Fix All Safe Issues" button berfungsi
- [ ] Confirmation dialog explains scope
- [ ] Hanya safe actions (Automatic + Low/None risk) dieksekusi
- [ ] Manual/Medium/High risk actions skipped
- [ ] Progress indicator di status bar
- [ ] Final message: "Success: X, Failed: Y"

### 6.6 Individual Repair Actions

Verify semua 30+ actions dari [REPAIR_ACTIONS.md](../03-Features/REPAIR_ACTIONS.md):

#### Disk Health
- [ ] Run Disk Cleanup
- [ ] Open Storage Settings
- [ ] Clear Temporary Files
- [ ] Run CHKDSK (test di VM only!)
- [ ] Open Optimize Drives
- [ ] Open Backup Settings

#### Performance
- [ ] Open Task Manager
- [ ] Open Resource Monitor
- [ ] Set Balanced Power Plan
- [ ] Schedule Restart (test di VM only!)

#### Network
- [ ] Run Network Troubleshooter
- [ ] Reset Network Stack (test di VM only!)
- [ ] Release and Renew IP
- [ ] Flush DNS Cache
- [ ] Set DNS to Google

#### Security
- [ ] Open Windows Security
- [ ] Update Defender Definitions
- [ ] Start Firewall Service
- [ ] Enable UAC (test di VM only!)
- [ ] Open UAC Settings
- [ ] Start Specific Security Service

#### Windows Update
- [ ] Open Windows Update
- [ ] Enable Windows Update Service
- [ ] Force Update Check

#### Drivers
- [ ] Open Device Manager
- [ ] Scan for Hardware Changes
- [ ] Check Optional Driver Updates

#### Startup
- [ ] Open Task Manager Startup Tab
- [ ] Open Startup Apps Settings
- [ ] Open Task Scheduler

---

## 7. 📄 Report Export

### 7.1 Export Functionality

- [ ] "Export Report" button di Dashboard berfungsi
- [ ] "Export Report" button di Results page berfungsi
- [ ] Buttons disabled jika belum scan
- [ ] Report file tersimpan ke Desktop
- [ ] Filename format: `WindowsDoctorAI_Report_YYYYMMDD_HHMMSS.html`
- [ ] File auto-open di default browser

### 7.2 Report Content

- [ ] Report title correct
- [ ] Generated timestamp
- [ ] Scan duration displayed
- [ ] Health score visible dan colored
- [ ] Summary table:
  - [ ] Total Issues
  - [ ] Critical count
  - [ ] Warnings count
  - [ ] Info count
- [ ] System Information section lengkap
- [ ] Results grouped by category
- [ ] Each result menampilkan:
  - [ ] Severity indicator
  - [ ] Title
  - [ ] Description
  - [ ] Details (jika ada)
  - [ ] Available actions
- [ ] Footer dengan copyright

### 7.3 Report Styling

- [ ] CSS embedded (standalone file)
- [ ] Warna severity correct
- [ ] Layout professional
- [ ] Readable di semua browsers (Chrome, Edge, Firefox)
- [ ] Print-friendly (untuk save as PDF)
- [ ] No broken links atau missing images

---

## 8. 🎨 UI/UX

### 8.1 Visual Design

- [ ] Fluent Design compliance
- [ ] Consistent spacing (8/12/16/20/24 grid)
- [ ] Proper typography hierarchy
- [ ] Brand accent color (#0067C0) applied consistently
- [ ] Status colors correct:
  - [ ] Healthy: #0F7B0F (green)
  - [ ] Warning: #B7710D (orange)
  - [ ] Critical: #C42B1C (red)
  - [ ] Info: #005FB8 (blue)
- [ ] Cards dengan proper corner radius (8px)
- [ ] Subtle shadows dan borders

### 8.2 Icons

- [ ] Segoe Fluent Icons render properly (bukan kotak)
- [ ] Icon sizes consistent per context
- [ ] Icons align dengan text
- [ ] No emoji digunakan (semua Fluent Icons)

### 8.3 Toast Notifications

- [ ] Success toast muncul (green accent)
- [ ] Error toast muncul (red accent)
- [ ] Warning toast muncul (orange accent)
- [ ] Info toast muncul (blue accent)
- [ ] Position: top-right area
- [ ] Auto-dismiss setelah 3-4 detik
- [ ] Manual close (X) button berfungsi
- [ ] Multiple toasts stack vertically
- [ ] No overlap dengan critical UI elements

### 8.4 Status Bar

- [ ] Live status dot dengan pulse animation
- [ ] Dot color: green (idle), blue (busy)
- [ ] Status text updates dinamis
- [ ] Progress ring active saat scan
- [ ] Last scan timestamp updates
- [ ] Admin badge dengan color yang correct

### 8.5 Theme Support

- [ ] Light theme (default) looks good
- [ ] Dark theme functional (perhatikan known issues)
- [ ] "Use System Setting" mengikuti Windows theme
- [ ] Theme switch instant tanpa restart

---

## 9. ⚙️ Settings

### 9.1 Settings Page

- [ ] 2-column layout (nav kiri, content kanan)
- [ ] Section navigation berfungsi:
  - [ ] Appearance
  - [ ] Scan Preferences
  - [ ] Notifications
  - [ ] Advanced
  - [ ] About
- [ ] Active section highlighted di nav

### 9.2 Appearance Section

- [ ] Theme selector combobox
- [ ] Theme change apply immediately
- [ ] Compact mode toggle (placeholder)

### 9.3 Toggle Switches

- [ ] All toggle switches respond ke click
- [ ] Visual feedback saat toggle
- [ ] State persisted (untuk v2.1.0+)

**Note untuk v2.0.0:** Settings belum persistent, akan di-implement di v2.1.0.

---

## 10. 📈 Performance

### 10.1 Startup Performance

- [ ] Cold start: < 5 detik
- [ ] Warm start: < 3 detik
- [ ] No visible lag saat launch

### 10.2 Runtime Performance

- [ ] Full scan: 20-45 detik
- [ ] UI responsive selama scan
- [ ] No UI freeze > 500ms
- [ ] Smooth animations (60fps)
- [ ] Memory usage < 200 MB idle
- [ ] Memory usage < 250 MB during scan

### 10.3 Memory Management

- [ ] No memory leak setelah 30 minutes usage
- [ ] Memory dapat di-collect (GC)
- [ ] No unbounded growth

---

## 11. 🔒 Security & Permissions

### 11.1 Elevation

- [ ] UAC prompt muncul saat launch (kalau not configured)
- [ ] Process runs dengan elevated privileges
- [ ] Admin badge menampilkan status correct

### 11.2 Sensitive Operations

- [ ] Semua repair actions require confirmation
- [ ] Registry writes hanya untuk specific known keys
- [ ] PowerShell scripts menggunakan safe flags
- [ ] No user input di-inject ke commands
- [ ] Process timeout enforced (120s default)

### 11.3 Data Privacy

- [ ] No data sent ke external servers
- [ ] No telemetry collection
- [ ] Reports disimpan lokal (Desktop)
- [ ] No personal info di logs

---

## 12. 🔗 Integration

### 12.1 Windows APIs

- [ ] WMI queries execute successfully
- [ ] PowerShell commands execute successfully
- [ ] Registry operations succeed
- [ ] Service management works
- [ ] Process execution works

### 12.2 System Integration

- [ ] `ms-settings:` deep links open correctly
- [ ] `.msc` snap-ins launch properly
- [ ] External tools (dfrgui, cleanmgr, dll) launch

---

## 13. 📱 Compatibility

### 13.1 Windows Versions

- [ ] Tested di Windows 10 22H2
- [ ] Tested di Windows 11 22H2
- [ ] Tested di Windows 11 23H2
- [ ] Tested di latest Windows 11

### 13.2 Display Configurations

- [ ] Tested at 100% DPI scaling
- [ ] Tested at 125% DPI scaling
- [ ] Tested at 150% DPI scaling
- [ ] Tested di 1080p display
- [ ] Tested di 4K display (kalau available)

---

## 14. 📚 Documentation

### 14.1 In-App Documentation

- [ ] About page menampilkan:
  - [ ] App logo
  - [ ] App name
  - [ ] Version number
  - [ ] Developer credit: "RIDOLF WIDI ALFISA LUMBA"
  - [ ] Copyright notice
  - [ ] License info
  - [ ] Tech stack
- [ ] Settings page memiliki About section
- [ ] Tooltips untuk complex UI elements

### 14.2 External Documentation

- [ ] README.md up-to-date dengan latest version
- [ ] CHANGELOG.md updated dengan release notes
- [ ] LICENSE file present
- [ ] Semua docs di `docs/` folder
- [ ] Screenshots current (kalau ada)
- [ ] Links dalam docs berfungsi

### 14.3 Code Documentation

- [ ] Public API punya XML comments
- [ ] Complex logic explained
- [ ] TODOs punya context (owner + target version)

---

## 15. 📦 Distribution

### 15.1 Build Preparation

- [ ] Version numbers updated di `.csproj`
- [ ] Version di About page updated
- [ ] Release notes drafted di CHANGELOG
- [ ] Git commit dengan version tag
- [ ] Publish self-contained build

### 15.2 Package Verification

- [ ] Published folder ~150 MB
- [ ] Executable runs standalone (no VS needed)
- [ ] Test di clean VM (no dev tools)
- [ ] No missing DLL errors
- [ ] UAC prompt muncul
- [ ] Full functionality available

### 15.3 Distribution Files

- [ ] ZIP file dibuat dengan proper naming: `WindowsDoctorAI-v2.0.0-x64.zip`
- [ ] ZIP tidak corrupt (test extract)
- [ ] Include README dalam ZIP
- [ ] Include LICENSE dalam ZIP
- [ ] Include CHANGELOG dalam ZIP

---

## 16. 🌐 Release Process

### 16.1 Pre-Release

- [ ] All checklist items above completed
- [ ] Known issues documented di CHANGELOG
- [ ] Release notes proofread
- [ ] Screenshots up-to-date

### 16.2 Release

- [ ] Git tag created: `git tag v2.0.0`
- [ ] Tag pushed: `git push origin v2.0.0`
- [ ] GitHub Release created (jika applicable)
- [ ] Release notes copied dari CHANGELOG
- [ ] Distribution ZIP uploaded

### 16.3 Post-Release

- [ ] Announcement (kalau ada channels)
- [ ] Monitor GitHub Issues untuk bug reports
- [ ] Update download stats (kalau tracking)
- [ ] Plan next release cycle

---

## 17. 🚨 Known Issues Documentation

### Format untuk Document Known Issues:

```markdown
### Issue: [Title]

**Severity:** Critical / High / Medium / Low
**Component:** [Component name]
**Description:** [What's the issue]
**Workaround:** [How to work around it]
**Target Fix:** vX.Y.Z
```

### Known Issues untuk v2.0.0:

- ⚠️ **Dark mode belum fully audited** — Beberapa elements mungkin low contrast di dark mode. Target fix: v2.1.0
- ⚠️ **Settings tidak persistent** — Preferences reset setelah restart. Target: v2.1.0
- ⚠️ **Scan history tidak tersimpan** — History placeholder only. Target: v2.1.0
- ⚠️ **Scan berjalan sequential** — Bisa dipercepat dengan parallel execution. Target: v2.2.0

---

## 18. ✅ Sign-Off

**Release Approved untuk Distribution:**

- [ ] Semua critical items ✅
- [ ] High priority items ✅
- [ ] Documentation updated
- [ ] Known issues documented

**Tested & Approved By:**

**Name:** RIDOLF WIDI ALFISA LUMBA
**Role:** Developer & QA
**Date:** __________________
**Signature:** __________________

---

## 19. 📝 Test Execution Notes

**Space untuk notes selama testing:**

```
[Add any observations, unexpected behaviors, or notes here]




```

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial QA checklist | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**QA Checklist for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>