# 🔧 Troubleshooting Guide

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Panduan troubleshooting komprehensif untuk masalah umum yang mungkin dialami pengguna WindowsDoctorAI. Dokumen ini terorganisir berdasarkan **kategori masalah** untuk memudahkan pencarian solusi.

---

## 📚 Table of Contents

1. [Installation & Startup Issues](#1-installation--startup-issues)
2. [Scan Issues](#2-scan-issues)
3. [Repair Action Issues](#3-repair-action-issues)
4. [UI & Display Issues](#4-ui--display-issues)
5. [Performance Issues](#5-performance-issues)
6. [Report Export Issues](#6-report-export-issues)
7. [Permission & Security Issues](#7-permission--security-issues)
8. [Advanced Diagnostics](#8-advanced-diagnostics)

---

## 1. Installation & Startup Issues

### 1.1 Application won't start / crashes immediately

**Symptoms:**
- Double-click executable, nothing happens
- Brief window flash, then closes
- Error dialog pada startup

**Possible Causes & Solutions:**

#### Cause A: Windows version tidak compatible

**Check:**
```
Press Win+R → type "winver" → Enter
```

Verify: **Windows 10 build 17763 (v1809) atau lebih baru**, atau **Windows 11**.

**Solution:** Upgrade Windows atau gunakan mesin lain.

---

#### Cause B: Missing dependencies

**Symptoms:** Error "MSVCP140.dll missing" atau similar

**Solution:** Install [Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe)

---

#### Cause C: Corrupt download

**Solution:**
1. Delete existing WindowsDoctorAI folder
2. Re-download from source
3. Verify file size sesuai expected (~150 MB untuk single file)
4. Extract ulang jika dari ZIP

---

#### Cause D: Antivirus blocking

**Solution:**
1. Temporarily disable antivirus real-time protection
2. Try running WindowsDoctorAI
3. Kalau berhasil, add exclusion di antivirus untuk folder WindowsDoctorAI
4. Re-enable antivirus

**Adding Windows Defender Exclusion:**
```
Settings → Privacy & Security → Windows Security → Virus & threat protection
→ Manage settings → Exclusions → Add or remove exclusions
→ Add exclusion → Folder → Select WindowsDoctorAI folder
```

---

### 1.2 UAC prompt tidak muncul

**Symptoms:**
- Aplikasi jalan tapi banyak fitur error
- Status bar menampilkan "Standard User (Limited)"

**Cause:** Executable tidak diminta run as administrator

**Solutions:**

**Option A - Manual per-run:**
Klik kanan `WindowsDoctorAI.exe` → **Run as administrator**

**Option B - Permanent (per-user):**
1. Klik kanan executable → Properties
2. Tab **Compatibility**
3. Check **"Run this program as an administrator"**
4. OK

---

### 1.3 Error: "The application was unable to start correctly (0xc000007b)"

**Cause:** Architecture mismatch atau missing runtime

**Solution:**
1. Verify sistem Anda adalah **64-bit**:
   ```
   Settings → System → About → System type
   ```
2. Kalau 32-bit: WindowsDoctorAI **tidak support x86**, upgrade Windows atau gunakan mesin 64-bit
3. Kalau 64-bit: Re-download self-contained package (bukan framework-dependent)

---

## 2. Scan Issues

### 2.1 Scan stuck di kategori tertentu

**Symptoms:**
- Progress bar tidak bergerak
- Category status stuck di "Scanning..."
- Live log tidak update

**Common Culprits:**

#### Windows Update Scan

**Cause:** `Get-HotFix` PowerShell cmdlet slow karena banyak update history

**Solution:**
- Tunggu hingga 60 detik (normal untuk sistem lama)
- Kalau > 60 detik, klik Cancel dan restart scan
- Pertimbangkan `Clear-Host` dulu untuk WU history (advanced)

---

#### Drivers Scan

**Cause:** `Get-WindowsDriver` bisa memakan waktu 10-20 detik pada sistem dengan banyak drivers

**Solution:**
- Ini normal, tunggu hingga 30 detik
- Kalau > 30 detik, mungkin driver service issue → cek Services.msc → Windows Driver Foundation

---

### 2.2 Scan complete tapi tidak ada hasil (empty results)

**Symptoms:**
- Progress bar reach 100%
- Dialog close
- Results page kosong

**Possible Causes:**

**Cause A: Semua checks failed silently**

**Check:** Buka Event Viewer:
```
Event Viewer → Windows Logs → Application → Filter by "WindowsDoctorAI"
```

**Solution:**
- Restart aplikasi
- Coba scan ulang
- Kalau consistent, report bug via GitHub Issues

---

**Cause B: WMI service tidak aktif**

**Check & Fix:**
```cmd
net stop winmgmt
net start winmgmt
```

---

### 2.3 Score selalu 0 atau tidak berubah

**Symptoms:**
- OverallScore = 0 setelah scan
- HealthRating = "Critical" padahal sistem normal

**Possible Causes:**

**Cause A: Scan gagal partial**

**Solution:**
- Cek Results page — kalau ada hasil dengan `Status = Failed`, itu penyebabnya
- Restart aplikasi dan scan ulang
- Ensure running as administrator

---

**Cause B: Cache issue di ViewModel**

**Solution:**
1. Close aplikasi
2. Re-open
3. Run fresh scan

---

### 2.4 Cancel button tidak berfungsi

**Symptoms:**
- Klik Cancel, dialog tetap terbuka
- Scan tetap berjalan

**Cause:** Cancellation token belum di-propagate ke long-running operation

**Solution:**
- Tunggu current category selesai (max 10-20 detik)
- Kalau tetap stuck > 60 detik, force close via Task Manager

**Known Limitation:** Beberapa WMI queries dan PowerShell scripts tidak support graceful cancellation. Ini akan diperbaiki di v2.1.0.

---

## 3. Repair Action Issues

### 3.1 Repair action tidak berfungsi / gagal

**Symptoms:**
- Toast notification "Repair Failed"
- Error message di RepairProgressDialog

**Diagnosis Steps:**

**Step 1: Verify administrator elevation**
- Cek status bar bawah — harus "Administrator"
- Kalau "Standard User (Limited)", restart aplikasi as admin

**Step 2: Check specific error message**
- Baca `ResultMessage` di RepairAction
- Biasanya menampilkan reason spesifik (Access Denied, Service Not Found, dll)

**Step 3: Try manual execution**
Kalau repair action adalah CommandLine type, coba run command manual di cmd untuk debug:
```cmd
# Example untuk "Flush DNS Cache"
ipconfig /flushdns
```

Kalau manual berhasil tapi WindowsDoctorAI gagal, ada issue di app.

---

### 3.2 Repair completes tapi tidak ada efek

**Symptoms:**
- Toast "Repair Completed successfully"
- Tapi issue yang sama masih ada saat re-scan

**Possible Causes:**

**Cause A: Repair memerlukan reboot**

**Check:** Repair action harus punya `RequiresReboot = true`

**Solution:** Reboot komputer, lalu re-scan

---

**Cause B: Root cause tidak teratasi**

**Example:** "Start Firewall Service" berhasil, tapi service langsung stop lagi.

**Cause:** Group Policy atau third-party software memaksa disable

**Solution:** Cek Group Policy Editor (gpedit.msc) atau third-party security software

---

### 3.3 "Fix All Safe" tidak execute semua actions

**Symptoms:**
- Klik "Fix All Safe Issues"
- Hanya sebagian actions yang dieksekusi

**Cause:** "Fix All Safe" hanya execute actions dengan:
- `ActionType == Automatic`
- `RiskLevel == "Low"` atau `"None"`
- `Status == Pending`

**Not included:**
- SystemTool actions (butuh manual user interaction)
- OpenSettings actions
- Medium/High risk actions

**Solution:** Execute manual untuk actions yang tidak masuk safe batch.

---

### 3.4 Reboot warning ignored

**Symptoms:**
- Execute repair yang butuh reboot
- Tidak ada prompt untuk restart

**Cause:** `RequiresReboot` flag hanya menampilkan warning di dialog, tidak trigger auto-reboot

**Solution:** Manual reboot setelah repair completed. Feature auto-reboot planned untuk v2.1.0.

---

## 4. UI & Display Issues

### 4.1 UI terlihat broken / elements overlapping

**Possible Causes:**

**Cause A: Display scaling issue**

**Check:**
```
Settings → System → Display → Scale
```

**Solution:**
- Coba scaling 100% atau 125% (aplikasi tested optimal di scaling ini)
- Log out dan log in ulang setelah change scaling
- Restart aplikasi

---

**Cause B: Multi-monitor issue**

**Solution:**
- Move window ke primary monitor
- Restart aplikasi

---

**Cause C: Corrupt XAML resources**

**Solution:**
1. Close aplikasi
2. Delete file di `%LOCALAPPDATA%\WindowsDoctorAI\` (kalau ada)
3. Restart aplikasi

---

### 4.2 Icons muncul sebagai kotak kosong

**Cause:** Segoe Fluent Icons font tidak ter-install / corrupt

**Verify Font Installation:**
```
Control Panel → Fonts → Search "Segoe Fluent Icons"
```

**Solution:**
- Windows 11: Font harusnya built-in, kalau hilang re-install via [Microsoft](https://aka.ms/SegoeFluentIcons)
- Windows 10: Font mungkin fallback ke "Segoe MDL2 Assets" (masih berfungsi tapi visual berbeda)

---

### 4.3 Dark mode terlihat aneh

**Known Issue:** Dark mode di v2.0.0 belum fully audited. Beberapa element mungkin kurang visible atau kontras rendah.

**Workaround:**
1. Buka **Settings** > **Appearance**
2. Pilih **Light** untuk tema terbaik di v2.0.0

**Planned Fix:** v2.1.0 akan include comprehensive dark mode audit.

---

### 4.4 Toast notifications tidak muncul

**Symptoms:**
- Repair complete tapi tidak ada toast di kanan atas

**Possible Causes:**

**Cause A: Window minimized**

Toast muncul di **within app window**, bukan Windows notification center. Kalau window minimized, toast tidak visible.

**Solution:** Restore window sebelum execute repair

---

**Cause B: Toast host not registered**

**Solution:** Restart aplikasi. Kalau persistent, report bug.

---

### 4.5 ScrollViewer tidak scroll

**Symptoms:**
- Content lebih panjang dari screen
- Mouse wheel scroll tidak berfungsi
- Scrollbar tidak muncul

**Solution:**
1. Klik area content dulu (untuk focus)
2. Coba scroll dengan Page Up/Down keys
3. Kalau tetap tidak berfungsi, resize window

---

## 5. Performance Issues

### 5.1 Aplikasi lambat / laggy

**Symptoms:**
- Startup > 5 seconds
- UI freeze saat scan
- Slow response ke user input

**Diagnosis:**

**Check Memory Usage:**
- Task Manager → Details → WindowsDoctorAI.exe
- Normal: 100-200 MB
- Kalau > 500 MB: memory leak issue

**Solutions:**

**Solution A: Close background apps**
Free up system resources dengan close aplikasi lain yang berat.

**Solution B: Restart WindowsDoctorAI**
Kalau memory usage tinggi setelah lama running, restart aplikasi.

**Solution C: Update Windows**
Windows updates sering include performance improvements untuk WinUI apps.

---

### 5.2 Scan sangat lambat (> 2 menit)

**Normal scan time:** 20-45 seconds

**Kalau > 2 menit, possible causes:**

**Cause A: Slow HDD**
Scan Windows Update dan Drivers memerlukan disk I/O. HDD lambat menyebabkan delay signifikan.

**Solution:** Upgrade ke SSD (rekomendasi umum untuk performa Windows).

---

**Cause B: Slow network**
Network scan memerlukan ping ke servers dan DNS resolution.

**Solution:** Test koneksi internet, atau uncheck Network category kalau perlu quick scan.

---

**Cause C: Corrupt WMI database**

**Fix (Advanced):**
```cmd
# Run as administrator
net stop winmgmt
cd %windir%\system32\wbem
rd /S /Q repository
regsvr32 /s %systemroot%\system32\scecli.dll
regsvr32 /s %systemroot%\system32\userenv.dll
mofcomp cimwin32.mof
mofcomp cimwin32.mfl
mofcomp rsop.mof
mofcomp rsop.mfl
for /f %s in ('dir /b *.mof') do mofcomp %s
for /f %s in ('dir /b *.mfl') do mofcomp %s
net start winmgmt
```

**⚠️ Warning:** Perintah di atas rebuild WMI database. Gunakan hati-hati.

---

### 5.3 Memory usage terus meningkat

**Symptoms:**
- Setelah lama running, memory usage 500+ MB
- System jadi lambat

**Cause:** Possible memory leak (belum di-audit menyeluruh di v2.0.0)

**Workaround:**
- Restart aplikasi setelah beberapa jam usage
- Report ke GitHub Issues dengan screenshot Task Manager

**Planned Fix:** Memory profiling & leak fix planned untuk v2.1.0.

---

## 6. Report Export Issues

### 6.1 Export report gagal

**Symptoms:**
- Klik "Export Report"
- Error message atau nothing happens

**Possible Causes:**

**Cause A: Desktop path tidak accessible**

**Check:** Verify user memiliki write access ke Desktop
```
Right-click Desktop folder → Properties → Security
```

**Solution:** Grant write permission ke user, atau change output path (feature planned v2.1.0).

---

**Cause B: File already open**

Kalau file report dari scan sebelumnya masih terbuka di browser, write gagal.

**Solution:** Close browser tab dengan report, lalu retry export.

---

### 6.2 HTML report tidak terbuka di browser

**Symptoms:**
- File berhasil di-save
- Tapi tidak auto-open

**Cause:** No default browser configured

**Solution:**
1. Settings → Apps → Default apps → Set default browser
2. Buka file manual dari Desktop

---

## 7. Permission & Security Issues

### 7.1 "Access Denied" errors

**Symptoms:**
- Repair actions gagal dengan "Access Denied"
- Registry writes fail

**Diagnosis:**

**Verify Admin Elevation:**
```cmd
whoami /groups | findstr "S-1-16-12288"
```

Kalau output kosong, aplikasi tidak running dengan Administrator token.

**Solution:**
1. Close aplikasi
2. Right-click executable → **Run as administrator**
3. Confirm UAC prompt

---

### 7.2 Antivirus flags WindowsDoctorAI as malware

**Symptoms:**
- Antivirus notification "Threat detected"
- Aplikasi terhapus atau di-quarantine

**Cause:** False positive karena:
- Requires admin elevation
- Executes PowerShell with `-ExecutionPolicy Bypass`
- Modifies registry
- Not code-signed (v2.0.0)

**Solutions:**

**Solution A: Whitelist di Antivirus**

**Windows Defender:**
```
Settings → Privacy & Security → Windows Security 
→ Virus & threat protection → Manage settings 
→ Exclusions → Add or remove exclusions
→ Add exclusion → File → Select WindowsDoctorAI.exe
```

**Third-party AV:** Refer ke documentation antivirus masing-masing.

---

**Solution B: Submit False Positive Report**

Kalau confident aplikasi safe, submit sample ke AV vendor:
- Windows Defender: [Submit sample](https://www.microsoft.com/wdsi/filesubmission)
- Malwarebytes, Norton, etc: Cek website masing-masing

---

**Solution C: Verify Integrity**

Cek hash file:
```powershell
Get-FileHash WindowsDoctorAI.exe -Algorithm SHA256
```

Compare dengan hash di official release notes.

---

### 7.3 SmartScreen blocks execution

**Symptoms:**
- Blue screen "Windows protected your PC"
- Message "Windows SmartScreen prevented an unrecognized app from starting"

**Cause:** Aplikasi belum code-signed dengan certificate dari trusted authority

**Solutions:**

**Solution A: Bypass SmartScreen (One-Time)**
1. Klik **"More info"** di SmartScreen dialog
2. Klik **"Run anyway"**

**Solution B: Permanent Trust**
1. Right-click executable → Properties
2. Di General tab, check **"Unblock"** kalau ada checkbox
3. OK

**Planned Fix:** Code signing certificate akan ditambahkan di v2.1.0.

---

## 8. Advanced Diagnostics

### 8.1 Enable Verbose Logging

WindowsDoctorAI v2.0.0 belum memiliki dedicated logging system. Untuk debug advanced:

**Method A: Use Debug Build**
1. Build aplikasi dalam Debug configuration
2. Run dari Visual Studio dengan debugger attached
3. Cek Output window untuk debug messages

**Method B: Attach ProcessMonitor**
1. Download [ProcMon](https://learn.microsoft.com/en-us/sysinternals/downloads/procmon)
2. Filter Process Name = `WindowsDoctorAI.exe`
3. Run aplikasi, reproduce issue
4. Save log untuk analysis

**Planned:** Built-in logging system akan ditambahkan di v2.1.0.

---

### 8.2 Check Event Viewer

Windows event logs bisa memberikan clue tentang crashes:

```
Event Viewer → Windows Logs → Application
Filter Current Log:
- Event sources: ".NET Runtime", "Application Error"
- Time range: Last hour
```

Look for events yang mention `WindowsDoctorAI`.

---

### 8.3 Check .NET Runtime Version

Verify .NET 8 runtime ter-install:

```cmd
dotnet --list-runtimes
```

Expected output includes:
```
Microsoft.NETCore.App 8.0.x
Microsoft.WindowsDesktop.App 8.0.x
```

Kalau tidak ada, install .NET 8 runtime.

---

### 8.4 Reset WindowsDoctorAI to Default

**Note:** v2.0.0 belum menyimpan persistent settings, jadi reset otomatis by restart. Feature persistent settings planned untuk v2.1.0.

Kalau di v2.1.0+ ada issue karena corrupt settings:
```
Delete folder: %LOCALAPPDATA%\WindowsDoctorAI\
```

---

## 🆘 Getting Additional Help

### Before Reporting a Bug

Please gather informasi berikut:

- **Windows Version:** Run `winver` dan screenshot
- **WindowsDoctorAI Version:** Cek di About page
- **.NET Runtime:** Run `dotnet --list-runtimes`
- **Steps to Reproduce:** Detail step-by-step
- **Expected vs Actual Behavior**
- **Screenshots** kalau UI issue
- **Event Viewer logs** kalau crash

### Where to Report

- **GitHub Issues:** [Repository Issues Page](../../issues)
- **GitHub Discussions:** [Community Discussions](../../discussions) (untuk questions)

### Issue Template

```markdown
**Environment:**
- Windows: [version]
- App Version: [version]
- Running as Admin: [Yes/No]

**Steps to Reproduce:**
1. ...
2. ...
3. ...

**Expected Behavior:**
[Apa yang seharusnya terjadi]

**Actual Behavior:**
[Apa yang benar-benar terjadi]

**Screenshots:**
[Attach kalau relevant]

**Additional Context:**
[Info tambahan]
```

---

## 📞 Contact

Untuk bantuan lanjutan:

- **Developer:** RIDOLF WIDI ALFISA LUMBA
- **Issue Tracker:** [GitHub Issues](../../issues)
- **Documentation:** [`docs/`](../) folder

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial troubleshooting guide | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Troubleshooting Guide for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>