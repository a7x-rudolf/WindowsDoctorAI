# 🧪 Test Plan Document

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Document Information

| Attribute | Value |
|-----------|-------|
| **Document Type** | Test Plan & Strategy |
| **Target Audience** | Developer, QA (future), Contributors |
| **Compliance Level** | ISTQB-Inspired |
| **Author** | RIDOLF WIDI ALFISA LUMBA |
| **Copyright** | © 2025 RIDOLF WIDI ALFISA LUMBA |

---

## 1. Introduction

### 1.1 Purpose

Dokumen ini mendefinisikan **strategi pengujian komprehensif** untuk WindowsDoctorAI, mencakup:
- Test objectives dan scope
- Testing approach dan methodology
- Test cases untuk semua fitur utama
- Entry/exit criteria
- Roles dan responsibilities
- Risk assessment

### 1.2 Scope

**In Scope:**
- ✅ Manual functional testing
- ✅ UI/UX testing
- ✅ Integration testing (dengan Windows APIs)
- ✅ Performance testing (basic)
- ✅ Security testing (permissions, elevation)
- ✅ Compatibility testing (Windows 10/11)

**Out of Scope (v2.0.0):**
- ❌ Automated unit tests (planned v2.1.0)
- ❌ Load/stress testing
- ❌ Penetration testing
- ❌ Localization testing
- ❌ Accessibility deep audit

### 1.3 References

- [Project Charter](../00-Overview/PROJECT_CHARTER.md)
- [Architecture Documentation](../01-Architecture/ARCHITECTURE.md)
- [Diagnostic Categories](../03-Features/DIAGNOSTIC_CATEGORIES.md)
- [Repair Actions Catalog](../03-Features/REPAIR_ACTIONS.md)
- [ISTQB Foundation Level Syllabus](https://www.istqb.org/)

---

## 2. Test Strategy

### 2.1 Testing Levels

WindowsDoctorAI v2.0.0 mengimplementasikan **4 testing levels**:

```
┌─────────────────────────────────────────┐
│  Level 4: User Acceptance Testing       │  ← End-user validation
├─────────────────────────────────────────┤
│  Level 3: System Testing                │  ← Full app testing
├─────────────────────────────────────────┤
│  Level 2: Integration Testing           │  ← Multi-component
├─────────────────────────────────────────┤
│  Level 1: Component Testing (Manual)   │  ← Individual features
└─────────────────────────────────────────┘
```

### 2.2 Testing Types

| Type | Coverage | Priority |
|------|----------|----------|
| **Functional Testing** | All features work as specified | 🔴 High |
| **UI Testing** | Layout, responsiveness, theming | 🔴 High |
| **Integration Testing** | Windows APIs interaction | 🔴 High |
| **Performance Testing** | Startup time, scan duration | 🟡 Medium |
| **Security Testing** | Elevation, permissions | 🔴 High |
| **Compatibility Testing** | Different Windows versions | 🟡 Medium |
| **Usability Testing** | User flows, ease of use | 🟡 Medium |
| **Regression Testing** | Existing features tidak broken | 🔴 High |

### 2.3 Testing Approach

**Current Approach (v2.0.0):** Manual Testing by Developer

**Future Approach (v2.1.0+):**
- Automated unit tests (xUnit)
- Integration tests dengan mock Windows APIs
- UI automation dengan WinAppDriver

---

## 3. Test Environment

### 3.1 Hardware Requirements

**Primary Test Machine:**
- OS: Windows 11 Pro (latest build)
- CPU: Intel/AMD x64 (4+ cores)
- RAM: 8+ GB
- Storage: SSD dengan 50+ GB free
- Network: Ethernet dan/atau WiFi

**Secondary Test Machines:**
- Windows 10 22H2 (Home & Pro editions)
- Windows 11 21H2 (initial release)
- VM dengan clean Windows install (untuk regression testing)

### 3.2 Software Requirements

- Visual Studio 2022 Community
- .NET 8 SDK
- Windows App SDK 1.5+
- Git untuk source control
- Multiple browsers untuk HTML report testing (Chrome, Edge, Firefox)

### 3.3 Test Data

**System State Variations untuk Testing:**

| Scenario | Setup |
|----------|-------|
| Healthy System | Fresh Windows install, all services running |
| Full Disk | Fill C: drive hingga > 95% |
| Low RAM | Run memory-intensive apps |
| No Internet | Disconnect network |
| Antivirus Disabled | Temporarily disable Windows Defender |
| UAC Disabled | Set EnableLUA = 0 |
| Firewall Stopped | Stop MpsSvc service |
| Many Startup Apps | Add 20+ items ke startup |
| Outdated System | Skip Windows updates untuk 60+ days |

---

## 4. Test Cases

### 4.1 Application Startup Tests

#### TC-STARTUP-001: First Launch

**Objective:** Verify aplikasi berhasil launch pertama kali

**Prerequisites:**
- WindowsDoctorAI.exe available
- No previous installation

**Steps:**
1. Double-click WindowsDoctorAI.exe
2. Observe UAC prompt

**Expected Result:**
- ✅ UAC prompt muncul dengan publisher info
- ✅ Setelah accept, splash screen brief (< 1 detik)
- ✅ MainWindow ter-load dengan Dashboard page
- ✅ No error dialogs
- ✅ Sidebar terlihat dengan 8 navigation items
- ✅ Status bar menampilkan "System Ready" dan "Administrator"

**Status:** ✅ Pass / ❌ Fail

---

#### TC-STARTUP-002: Launch Without Admin

**Objective:** Verify graceful degradation tanpa admin privileges

**Steps:**
1. Right-click WindowsDoctorAI.exe → Properties → Compatibility
2. Uncheck "Run as administrator" jika enabled
3. Double-click executable

**Expected Result:**
- ✅ Aplikasi ter-load tanpa UAC prompt
- ✅ Status bar menampilkan "Standard User (Limited)"
- ✅ User avatar section menampilkan warning icon
- ✅ Warning icon color = orange

**Status:** ✅ Pass / ❌ Fail

---

#### TC-STARTUP-003: Startup Time Measurement

**Objective:** Verify startup time acceptable

**Steps:**
1. Close aplikasi
2. Wait 10 detik
3. Start timer, launch aplikasi
4. Stop timer saat MainWindow visible

**Expected Result:**
- ✅ Cold start: < 5 detik
- ✅ Warm start: < 3 detik

**Status:** ✅ Pass / ❌ Fail

---

### 4.2 Navigation Tests

#### TC-NAV-001: Sidebar Navigation

**Objective:** Verify semua navigation items berfungsi

**Steps:**
1. Klik "Dashboard" di sidebar
2. Klik "Diagnostic Scan"
3. Klik "Scan Results"
4. Klik "Repair Actions"
5. Klik "Scan History"
6. Klik "System Info"
7. Klik "Settings" (footer)
8. Klik "About" (footer)

**Expected Result untuk setiap step:**
- ✅ Selected item highlighted di sidebar
- ✅ Content area menampilkan page yang correct
- ✅ Page title matches menu label
- ✅ No navigation delay (< 300ms)
- ✅ Previous page state preserved (NavigationCacheMode)

**Status:** ✅ Pass / ❌ Fail

---

### 4.3 Diagnostic Scan Tests

#### TC-SCAN-001: Full Scan Execution

**Objective:** Verify full diagnostic scan completes successfully

**Prerequisites:**
- App launched as administrator
- Internet connection available

**Steps:**
1. Navigate ke Dashboard
2. Klik "Run Full Scan" di hero card
3. Observe scan progress dialog
4. Wait until scan completes

**Expected Result:**
- ✅ ScanProgressDialog muncul immediately
- ✅ Dialog menampilkan 7 category rows
- ✅ Progress bar animasi shimmer
- ✅ Live activity log menampilkan timestamps
- ✅ Category status transitions: Waiting → Scanning → Completed
- ✅ Total scan time: 20-45 detik
- ✅ Setelah selesai, tombol "View Results" muncul
- ✅ Klik "View Results" navigates ke Scan Results page
- ✅ Score gauge updates dengan hasil aktual

**Status:** ✅ Pass / ❌ Fail

---

#### TC-SCAN-002: Scan Cancellation

**Objective:** Verify user bisa cancel scan yang running

**Steps:**
1. Klik "Run Full Scan"
2. Setelah ~5 detik, klik "Cancel Scan"

**Expected Result:**
- ✅ Cancel button responsive
- ✅ Dialog title berubah ke "Scan Cancelled"
- ✅ Activity log menampilkan "Scan cancelled by user"
- ✅ Tombol "Close" muncul (bukan "View Results")
- ✅ IsScanning = false setelah cancel
- ✅ Partial results tersedia di Scan Results page

**Status:** ✅ Pass / ❌ Fail

---

#### TC-SCAN-003: Scan Result Accuracy

**Objective:** Verify scan results akurat sesuai kondisi sistem

**Test Matrix:**

| Test Condition | Expected Result |
|----------------|-----------------|
| System healthy | Overall score ≥ 90 |
| Fill disk hingga 95% | Critical warning untuk disk space |
| Stop MpsSvc | Critical warning untuk Firewall |
| Set EnableLUA = 0 | Critical warning untuk UAC |
| Disconnect internet | Critical untuk internet connectivity |
| Add 20 startup apps | Warning untuk too many startups |

**Steps untuk setiap kondisi:**
1. Setup test condition
2. Run full scan
3. Verify specific result muncul dengan severity yang benar

**Status:** ✅ Pass / ❌ Fail (per condition)

---

#### TC-SCAN-004: Empty State Handling

**Objective:** Verify empty states menampilkan message yang correct

**Steps:**
1. Fresh app launch (belum pernah scan)
2. Navigate ke "Scan Results" page

**Expected Result:**
- ✅ Empty state dialog muncul
- ✅ Icon besar (search icon)
- ✅ Text: "No Scan Results Yet"
- ✅ Description: "Run a diagnostic scan to see results here"

**Repeat untuk:**
- Repair Actions page → "No Repair Actions Available"
- Scan History page → "No Scan History Yet"

**Status:** ✅ Pass / ❌ Fail

---

### 4.4 Repair Action Tests

#### TC-REPAIR-001: Simple Repair Execution

**Objective:** Verify safe repair action executes successfully

**Prerequisites:**
- Full scan completed dengan minimal 1 warning

**Steps:**
1. Navigate ke Repair Actions
2. Cari action "Clear Temporary Files"
3. Klik "Execute" button
4. Confirm dialog muncul → klik "Execute Repair"
5. Observe progress dialog
6. Wait until complete

**Expected Result:**
- ✅ Confirm dialog menampilkan detail:
  - Action Type: Automatic
  - Risk Level: Low
  - Estimated Time: ~30s
  - Requires Admin: Yes
  - Requires Reboot: No
- ✅ Progress dialog menampilkan live log
- ✅ Icon dialog berubah ke green checkmark saat complete
- ✅ Title berubah ke "Repair Completed"
- ✅ Toast notification muncul di kanan atas
- ✅ Toast auto-dismiss setelah 3-4 detik

**Status:** ✅ Pass / ❌ Fail

---

#### TC-REPAIR-002: Repair Cancellation di Confirm Dialog

**Objective:** Verify user bisa cancel di confirm dialog

**Steps:**
1. Klik "Execute" di any repair action
2. Confirm dialog muncul
3. Klik "Cancel"

**Expected Result:**
- ✅ Dialog closes
- ✅ Repair action TIDAK dieksekusi
- ✅ Action status tetap "Pending"
- ✅ No toast notification muncul

**Status:** ✅ Pass / ❌ Fail

---

#### TC-REPAIR-003: Failed Repair Handling

**Objective:** Verify graceful handling saat repair gagal

**Setup:**
- Simulate failure (misalnya: stop service yang required untuk repair)

**Steps:**
1. Execute repair yang bakal gagal
2. Observe error handling

**Expected Result:**
- ✅ Progress dialog title berubah ke "Repair Failed"
- ✅ Icon dialog berubah ke red critical icon
- ✅ Error message tampil di dialog
- ✅ Toast notification tipe Error muncul
- ✅ Aplikasi TIDAK crash
- ✅ User bisa retry atau close dialog

**Status:** ✅ Pass / ❌ Fail

---

#### TC-REPAIR-004: Fix All Safe Issues

**Objective:** Verify batch repair execution

**Prerequisites:**
- Scan completed dengan multiple warnings
- Ada minimal 2 automatic + low risk actions

**Steps:**
1. Navigate ke Repair Actions
2. Klik "Fix All Safe Issues"
3. Confirm dialog muncul → "Continue"
4. Wait until batch completes

**Expected Result:**
- ✅ Konfirmasi dialog menjelaskan batch scope
- ✅ Hanya actions dengan ActionType=Automatic DAN RiskLevel=Low/None yang dieksekusi
- ✅ Manual actions (SystemTool, OpenSettings) tidak dieksekusi
- ✅ Medium/High risk actions tidak dieksekusi
- ✅ Status bar menampilkan progress
- ✅ Final message menampilkan success/fail count

**Status:** ✅ Pass / ❌ Fail

---

### 4.5 Report Export Tests

#### TC-REPORT-001: HTML Report Generation

**Objective:** Verify HTML report berhasil di-generate

**Prerequisites:**
- Scan completed

**Steps:**
1. Klik "Export Report" (dari Dashboard atau Results page)
2. Verify file tersimpan di Desktop
3. Verify file otomatis buka di browser default

**Expected Result:**
- ✅ File dengan format `WindowsDoctorAI_Report_YYYYMMDD_HHMMSS.html` tersimpan
- ✅ File auto-open di default browser
- ✅ Report berisi:
  - Overall health score
  - System information
  - All scan results grouped by category
  - Available repair actions per result
  - Timestamp scan
- ✅ Styling professional (CSS embedded)
- ✅ File standalone (tidak butuh internet)

**Status:** ✅ Pass / ❌ Fail

---

#### TC-REPORT-002: Report dengan Empty Results

**Objective:** Verify report handling ketika belum ada scan

**Steps:**
1. Fresh app launch
2. Coba klik "Export Report" (harusnya disabled)

**Expected Result:**
- ✅ Button "Export Report" disabled
- ✅ Tooltip menjelaskan perlu scan dulu

**Status:** ✅ Pass / ❌ Fail

---

### 4.6 UI/UX Tests

#### TC-UI-001: Window Resizing

**Objective:** Verify layout responsive terhadap window size

**Steps:**
1. Launch app (default size 1400×900)
2. Resize window ke:
   - 800×600 (small)
   - 1200×800 (medium)
   - 1920×1080 (fullscreen)
   - Maximize
3. Navigate through pages

**Expected Result untuk setiap size:**
- ✅ Sidebar tetap functional
- ✅ Content area tidak overflow
- ✅ Grid layouts adjust kolom (2/3/4 cols)
- ✅ Cards resize proportionally
- ✅ Text tidak terpotong
- ✅ Scrollbars muncul jika needed

**Status:** ✅ Pass / ❌ Fail

---

#### TC-UI-002: Theme Switching

**Objective:** Verify light/dark theme switching

**Steps:**
1. Navigate ke Settings → Appearance
2. Ganti theme ke "Dark"
3. Verify UI berubah
4. Ganti ke "Light"
5. Ganti ke "Use System Setting"

**Expected Result:**
- ✅ Theme change instant (no restart required)
- ✅ Semua pages menggunakan theme baru
- ✅ Colors sesuai theme
- ✅ Contrast readable di kedua theme
- ✅ System setting mengikuti Windows theme

**Known Issues (v2.0.0):**
- ⚠️ Dark mode belum fully audited (planned v2.1.0)

**Status:** ✅ Pass / ⚠️ Partial / ❌ Fail

---

#### TC-UI-003: Icon Rendering

**Objective:** Verify Segoe Fluent Icons render correctly

**Steps:**
1. Verify semua pages untuk icon consistency
2. Check icons di:
   - Sidebar navigation
   - Status bar
   - Cards dan buttons
   - Dialogs
   - Category identifiers

**Expected Result:**
- ✅ Semua icons render sebagai proper glyphs (bukan kotak kosong)
- ✅ Icon sizes consistent per context
- ✅ Icons align dengan text properly
- ✅ Colors sesuai severity/status

**Status:** ✅ Pass / ❌ Fail

---

#### TC-UI-004: Toast Notifications

**Objective:** Verify toast system berfungsi

**Steps:**
1. Execute repair action successful → Verify success toast
2. Execute repair yang gagal → Verify error toast
3. Trigger multiple toasts rapidly

**Expected Result:**
- ✅ Toast muncul di top-right area
- ✅ Icon sesuai type (green check, red X, dll)
- ✅ Auto-dismiss setelah 3-4 detik
- ✅ Manual close button (X) berfungsi
- ✅ Multiple toasts stack vertically
- ✅ Tidak overlap dengan UI elements

**Status:** ✅ Pass / ❌ Fail

---

### 4.7 Integration Tests

#### TC-INT-001: WMI Integration

**Objective:** Verify WMI queries berfungsi

**Steps:**
1. Run full scan
2. Verify data dari WMI-based checks:
   - Disk info (Win32_DiskDrive)
   - OS info (Win32_OperatingSystem)
   - CPU info (Win32_Processor)
   - Antivirus (SecurityCenter2)

**Expected Result:**
- ✅ Semua data ter-retrieve dengan benar
- ✅ No WMI errors di logs
- ✅ Data akurat sesuai sistem

**Status:** ✅ Pass / ❌ Fail

---

#### TC-INT-002: PowerShell Integration

**Objective:** Verify PowerShell commands execute

**Steps:**
1. Trigger scans yang menggunakan PowerShell:
   - Windows Update (Get-HotFix)
   - Drivers (Get-WindowsDriver)
2. Trigger repairs yang menggunakan PowerShell:
   - Update Defender Definitions
   - Set DNS

**Expected Result:**
- ✅ PowerShell scripts execute tanpa error
- ✅ Output di-parse correctly
- ✅ Execution time reasonable (< 30s per operation)

**Status:** ✅ Pass / ❌ Fail

---

#### TC-INT-003: Registry Operations

**Objective:** Verify registry read/write

**Steps:**
1. Trigger UAC check (registry read)
2. Trigger "Enable UAC" repair (registry write)
3. Verify perubahan di registry

**Expected Result:**
- ✅ Registry read returns correct value
- ✅ Registry write successful
- ✅ Value verified via regedit
- ✅ No permission errors

**Status:** ✅ Pass / ❌ Fail

---

### 4.8 Performance Tests

#### TC-PERF-001: Scan Duration

**Objective:** Measure scan performance

**Metrics:**
- Total scan time
- Per-category duration
- CPU usage during scan
- Memory usage during scan

**Steps:**
1. Fresh restart Windows
2. Launch app
3. Run full scan dengan stopwatch
4. Record metrics

**Expected Result:**
- ✅ Full scan: 20-45 detik
- ✅ CPU usage: < 30% average
- ✅ Memory usage: < 200 MB during scan
- ✅ No UI freezing

**Status:** ✅ Pass / ❌ Fail

---

#### TC-PERF-002: Memory Leak Detection

**Objective:** Verify no memory leaks

**Steps:**
1. Launch app, record baseline memory (Task Manager)
2. Run scan 10x consecutively
3. Execute 10x repair actions
4. Navigate all pages multiple times
5. Record final memory

**Expected Result:**
- ✅ Memory growth: < 50 MB setelah 30 minutes usage
- ✅ Memory dapat di-collect (GC)
- ✅ No unbounded growth

**Status:** ✅ Pass / ❌ Fail

---

### 4.9 Compatibility Tests

#### TC-COMPAT-001: Windows Version Compatibility

**Test Matrix:**

| Windows Version | Build | Test Status |
|----------------|-------|-------------|
| Windows 10 21H2 | 19044 | ✅ Pass |
| Windows 10 22H2 | 19045 | ✅ Pass |
| Windows 11 21H2 | 22000 | ✅ Pass |
| Windows 11 22H2 | 22621 | ✅ Pass |
| Windows 11 23H2 | 22631 | ✅ Pass |

**Steps per version:**
1. Fresh install target Windows version
2. Copy WindowsDoctorAI standalone build
3. Run all TC-STARTUP, TC-SCAN, TC-REPAIR tests
4. Record any incompatibilities

**Expected Result:**
- ✅ App launches successfully
- ✅ All features functional
- ✅ No compatibility warnings

**Status per version:** ✅ Pass / ❌ Fail

---

### 4.10 Security Tests

#### TC-SEC-001: Elevation Verification

**Objective:** Verify UAC elevation properly handled

**Steps:**
1. Launch app
2. Verify UAC prompt appearance
3. Check running process privileges (Task Manager → Details → "Elevated" column)

**Expected Result:**
- ✅ UAC prompt muncul (kecuali sudah di-configure "Always run as admin")
- ✅ Process shows "Yes" di Elevated column
- ✅ Admin badge di status bar shows "Administrator"

**Status:** ✅ Pass / ❌ Fail

---

#### TC-SEC-002: Command Injection Prevention

**Objective:** Verify no user input injected ke system commands

**Review:**
- Semua ProcessHelper calls harus punya hardcoded arguments
- Tidak boleh ada user input concatenated ke command strings

**Steps:**
1. Code review semua RepairAction executions
2. Search codebase untuk pattern: `arguments = $"..."` dengan user input

**Expected Result:**
- ✅ Semua command arguments hardcoded
- ✅ Tidak ada user input injection points

**Status:** ✅ Pass / ❌ Fail

---

## 5. Entry & Exit Criteria

### 5.1 Entry Criteria (Sebelum Testing)

- ✅ Code builds tanpa errors dan warnings
- ✅ Latest version di-deploy ke test environment
- ✅ Test environment sesuai spec
- ✅ Test data prepared
- ✅ Previous test cases resolved (untuk regression)

### 5.2 Exit Criteria (Ready untuk Release)

**Must Have (Blocking):**
- ✅ 100% test cases untuk critical features passed
- ✅ 0 critical bugs
- ✅ 0 high-priority bugs
- ✅ All P0 documentation complete

**Should Have:**
- ✅ ≥ 90% test cases passed
- ✅ Known issues documented
- ✅ Performance benchmarks met

**Nice to Have:**
- ✅ 100% test cases passed
- ✅ 0 medium-priority bugs

---

## 6. Test Deliverables

### 6.1 Documents

- ✅ Test Plan (dokumen ini)
- ✅ QA Checklist ([QA_CHECKLIST.md](QA_CHECKLIST.md))
- 📝 Test Execution Report (per release)
- 📝 Bug Report (kalau ada issues)

### 6.2 Test Artifacts

- Test execution logs
- Screenshots evidence
- Video recordings (untuk complex flows)
- Performance metrics

---

## 7. Roles & Responsibilities

**Solo Developer Project — RIDOLF WIDI ALFISA LUMBA menghandle semua peran:**

| Role | Responsibility |
|------|----------------|
| **Test Lead** | Plan testing strategy |
| **Test Designer** | Create test cases |
| **Tester** | Execute tests |
| **Bug Fixer** | Fix identified issues |
| **QA Reviewer** | Review test results |

---

## 8. Risk Assessment

### 8.1 Testing Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Solo developer bias | Medium | High | Follow structured test cases |
| Limited test environments | Medium | Medium | Use VMs untuk different Windows versions |
| Manual testing time-consuming | High | High | Automate di v2.1.0+ |
| Regression not caught | High | Medium | Systematic re-testing per release |
| Edge cases missed | Medium | High | Community feedback via GitHub |

### 8.2 Product Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Repair actions break system | High | Extensive testing di VMs sebelum release |
| Data loss dari cleanup | High | Warning dialogs, backup recommendations |
| Windows API changes | Medium | Test per Windows update |
| Antivirus false positive | Medium | Code signing planned v2.1.0 |

---

## 9. Test Metrics

### 9.1 Metrics Tracked

**Per Release:**
- Total test cases: [Count]
- Passed: [Count] ([%])
- Failed: [Count] ([%])
- Blocked: [Count] ([%])
- Not Executed: [Count] ([%])

**Bug Metrics:**
- Total bugs: [Count]
- Critical: [Count]
- High: [Count]
- Medium: [Count]
- Low: [Count]

**Coverage:**
- Feature coverage: [%]
- Category coverage: [%] (7 diagnostic categories)
- Platform coverage: [%] (Windows versions)

### 9.2 Quality Gates

**Release Approved jika:**
- ✅ Feature coverage ≥ 95%
- ✅ 0 Critical bugs
- ✅ ≤ 2 High priority bugs (with workarounds)
- ✅ Category coverage 100% (all 7 tested)

---

## 10. Test Execution Schedule

### 10.1 Per Release Cycle

**Estimated Duration:** 2-3 hari untuk full test suite

| Day | Activities |
|-----|-----------|
| Day 1 | Startup + Navigation + Diagnostic Scan tests |
| Day 2 | Repair Actions + Report + UI tests |
| Day 3 | Integration + Performance + Compatibility tests |

### 10.2 Frequency

- **Full test suite:** Sebelum setiap MAJOR/MINOR release
- **Smoke tests:** Sebelum setiap PATCH release
- **Ad-hoc tests:** Sesuai perubahan feature

---

## 11. Automation Roadmap

### 11.1 v2.1.0 Planned

- Unit tests untuk domain services
- Integration tests dengan mock Windows APIs
- Basic UI smoke tests

**Framework:**
- xUnit untuk unit tests
- Moq untuk mocking
- WinAppDriver untuk UI automation

### 11.2 v2.2.0 Planned

- Comprehensive unit test coverage (≥ 70%)
- E2E test scenarios
- CI/CD integration dengan GitHub Actions

---

## 12. Bug Reporting Template

```markdown
**Bug ID:** BUG-YYYYMMDD-NNN
**Title:** [Brief description]
**Severity:** Critical / High / Medium / Low
**Priority:** P0 / P1 / P2 / P3
**Component:** [Dashboard / Scan / Repair / Report / UI]

**Environment:**
- Windows Version: [10/11 + build]
- App Version: [2.0.0]
- Running as Admin: [Yes/No]

**Steps to Reproduce:**
1. ...
2. ...

**Expected Result:**
[What should happen]

**Actual Result:**
[What actually happened]

**Screenshots/Logs:**
[Attach evidence]

**Workaround:**
[If any]

**Notes:**
[Additional context]
```

---

## 13. Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial test plan | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Test Plan for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>