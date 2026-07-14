# 🔧 Repair Actions Catalog

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Document Overview

Dokumen ini adalah **katalog lengkap** dari semua repair actions yang tersedia di WindowsDoctorAI. Setiap action didokumentasikan dengan:
- Tujuan dan mekanisme
- Risk level dan justifikasi
- Prasyarat (admin, reboot, dll)
- Impact ke sistem
- Reversibility

---

## 🎯 Action Types Overview

WindowsDoctorAI memiliki **6 jenis** action type:

| Type | Description | User Interaction |
|------|-------------|------------------|
| **Automatic** | Fully automated dengan konfirmasi | Confirm → Execute |
| **CommandLine** | Menjalankan cmd/exe dengan arguments | Confirm → Execute |
| **SystemTool** | Membuka built-in Windows tool | Confirm → Opens tool |
| **OpenSettings** | Membuka Windows Settings page | Confirm → Opens Settings |
| **ServiceRestart** | Start/Stop Windows service | Confirm → Execute |
| **RegistryFix** | Modifikasi Windows Registry | Confirm → Execute |

---

## 🚦 Risk Levels

| Level | Description | Reversible? | Typical Actions |
|-------|-------------|-------------|-----------------|
| **None** | No system modification, hanya opens tool/settings | ✅ N/A | Open Task Manager, Open Settings |
| **Low** | Minor changes dengan minimal impact | ✅ Yes | Clear temp files, Flush DNS |
| **Medium** | Service changes atau configuration modification | ⚠️ Mostly | Service restart, Network reset |
| **High** | System-wide changes (tidak ada di v2.0.0) | ❌ May need restore | N/A |

---

## 📚 Complete Actions Catalog

### 💾 Disk Health Category

---

#### Action D1: Run Disk Cleanup

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | Low |
| **Command** | `cleanmgr.exe` |
| **Arguments** | `/d C:` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 60 seconds |
| **Reversible** | ✅ Yes (files can be recovered from Recycle Bin if not permanent delete) |

**Description:**  
Membuka Windows Disk Cleanup utility untuk menghapus file yang tidak diperlukan (temp files, cache, Recycle Bin, dll).

**When Triggered:** Free space < 5% atau < 15%

---

#### Action D2: Open Storage Settings

| Attribute | Value |
|-----------|-------|
| **Type** | OpenSettings |
| **Risk Level** | None |
| **Command** | `ms-settings:storagesense` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |
| **Reversible** | ✅ N/A |

**Description:**  
Membuka Windows Storage settings untuk melihat penggunaan disk dan mengaktifkan Storage Sense.

**When Triggered:** Free space < 5% atau < 15%

---

#### Action D3: Clear Temporary Files

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | Low |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 30 seconds |
| **Reversible** | ❌ No (files permanently deleted) |

**Description:**  
Menghapus semua files dan subdirectories di:
- `%TEMP%` (user temp)
- `C:\Windows\Temp` (system temp)

**Implementation:**
```csharp
async Task<bool> ClearTempFilesAsync()
{
    // Iterate temp paths, delete files & folders
    // Ignore locked files (silently skip)
}
```

**When Triggered:** Temp files > 500 MB

**⚠️ Warning:** Files yang sedang digunakan aplikasi lain tidak akan terhapus (silently skipped). Beberapa aplikasi mungkin perlu restart setelah temp files cleared.

---

#### Action D4: Run CHKDSK

| Attribute | Value |
|-----------|-------|
| **Type** | CommandLine |
| **Risk Level** | Medium |
| **Command** | `cmd.exe` |
| **Arguments** | `/c chkdsk C: /f /r` |
| **Requires Admin** | Yes |
| **Requires Reboot** | Yes |
| **Estimated Time** | 5+ minutes (on next boot) |
| **Reversible** | ⚠️ Cannot cancel once scheduled |

**Description:**  
Menjadwalkan Check Disk untuk scan dan repair errors pada drive C: pada boot berikutnya.

**Flags:**
- `/f` — Fix errors on disk
- `/r` — Locate bad sectors and recover readable information

**When Triggered:** File system errors detected atau SMART abnormal

**⚠️ Warning:** Proses ini bisa memakan waktu 30 menit hingga beberapa jam tergantung ukuran disk.

---

#### Action D5: Open Optimize Drives (Defrag)

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | Low |
| **Command** | `dfrgui.exe` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 10 minutes (defrag actual) |
| **Reversible** | ✅ N/A (defrag is safe) |

**Description:**  
Membuka Defragment and Optimize Drives utility. Untuk SSD: menjalankan TRIM. Untuk HDD: menjalankan defragmentation.

**When Triggered:** HDD detected

---

#### Action D6: Open Backup Settings

| Attribute | Value |
|-----------|-------|
| **Type** | OpenSettings |
| **Risk Level** | None |
| **Command** | `ms-settings:backup` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Windows Backup settings — direkomendasikan setelah SMART menunjukkan disk failure imminent.

**When Triggered:** SMART Status = "Pred Fail"

---

### ⚡ Performance Category

---

#### Action P1: Open Task Manager

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `taskmgr.exe` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Task Manager untuk melihat dan mengelola proses secara manual.

**When Triggered:** CPU > 70% atau RAM > 75%

---

#### Action P2: Open Resource Monitor

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `resmon.exe` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Resource Monitor untuk analisis mendalam CPU, Memory, Disk, dan Network usage.

**When Triggered:** RAM > 90% atau > 3 high-memory processes

---

#### Action P3: Set Balanced Power Plan

| Attribute | Value |
|-----------|-------|
| **Type** | CommandLine |
| **Risk Level** | Low |
| **Command** | `powercfg.exe` |
| **Arguments** | `/setactive 381b4222-f694-41f0-9685-ff5bb260df2e` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |
| **Reversible** | ✅ Yes (manual power plan change) |

**Description:**  
Set power plan ke Balanced (GUID default Windows) untuk mengurangi CPU throttling issues.

**When Triggered:** CPU > 90%

---

#### Action P4: Schedule Restart

| Attribute | Value |
|-----------|-------|
| **Type** | CommandLine |
| **Risk Level** | Medium |
| **Command** | `shutdown.exe` |
| **Arguments** | `/r /t 300 /c "WindowsDoctorAI: Scheduled restart. Save your work."` |
| **Requires Admin** | Yes |
| **Requires Reboot** | Yes |
| **Estimated Time** | 5 seconds (schedules), then 5 min countdown |
| **Reversible** | ✅ Yes (with `shutdown /a` to abort) |

**Description:**  
Menjadwalkan restart komputer dalam 5 menit dengan notifikasi ke user.

**When Triggered:** Uptime > 30 days atau pending reboot untuk Windows Update

**⚠️ Warning:** User memiliki 5 menit untuk save work sebelum restart otomatis.

---

### 🌐 Network Category

---

#### Action N1: Run Network Troubleshooter

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `msdt.exe` |
| **Arguments** | `/id NetworkDiagnosticsWeb` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 60 seconds |

**Description:**  
Menjalankan built-in Windows Network Diagnostics wizard.

**When Triggered:** No internet connectivity

---

#### Action N2: Reset Network Stack

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | Medium |
| **Requires Admin** | Yes |
| **Requires Reboot** | Yes |
| **Estimated Time** | 15 seconds |
| **Reversible** | ⚠️ Configurations reset to default |

**Description:**  
Comprehensive network stack reset. Executes berurutan:
1. `netsh winsock reset` — Reset Winsock catalog
2. `netsh int ip reset` — Reset TCP/IP stack
3. `ipconfig /flushdns` — Clear DNS resolver cache
4. `ipconfig /release` — Release IP lease
5. `ipconfig /renew` — Renew IP lease

**When Triggered:** No active adapters atau no internet

**⚠️ Warning:** Reboot diperlukan untuk complete reset. Konfigurasi network manual (VPN, custom DNS) akan hilang.

---

#### Action N3: Release and Renew IP Address

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | Low |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 10 seconds |
| **Reversible** | ✅ Yes (new IP from DHCP) |

**Description:**  
Release current IP dari DHCP, tunggu 2 detik, lalu request IP baru.

**When Triggered:** No internet connectivity

---

#### Action N4: Flush DNS Cache

| Attribute | Value |
|-----------|-------|
| **Type** | CommandLine |
| **Risk Level** | None |
| **Command** | `ipconfig.exe` |
| **Arguments** | `/flushdns` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |
| **Reversible** | ✅ N/A (cache akan re-populate) |

**Description:**  
Membersihkan DNS resolver cache lokal untuk mengatasi masalah DNS caching yang stale.

**When Triggered:** Partial internet connectivity atau DNS resolution issues

---

#### Action N5: Set DNS to Google

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | Medium |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 10 seconds |
| **Reversible** | ✅ Yes (via Network Settings) |

**Description:**  
Configure all active network adapters untuk menggunakan Google Public DNS:
- Primary: 8.8.8.8
- Secondary: 8.8.4.4

**Implementation:**
```powershell
Get-NetAdapter | Where-Object Status -eq 'Up' | 
    Set-DnsClientServerAddress -ServerAddresses ('8.8.8.8','8.8.4.4')
```

**When Triggered:** DNS resolution complete failure

**⚠️ Warning:** Akan override DNS setting yang dikonfigurasi ISP atau organisasi.

---

### 🔒 Security Category

---

#### Action S1: Open Windows Security

| Attribute | Value |
|-----------|-------|
| **Type** | OpenSettings |
| **Risk Level** | None |
| **Command** | `ms-settings:windowsdefender` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Windows Security app untuk manage antivirus, firewall, dll.

**When Triggered:** No antivirus atau AV disabled

---

#### Action S2: Update Defender Definitions

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | None |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 60 seconds |
| **Reversible** | ✅ Yes (rollback possible) |

**Description:**  
Download dan install latest antivirus signature updates via PowerShell.

**Implementation:**
```powershell
Update-MpSignature
```

**When Triggered:** Defender definitions outdated

---

#### Action S3: Start Firewall Service

| Attribute | Value |
|-----------|-------|
| **Type** | ServiceRestart |
| **Risk Level** | Low |
| **Command** | `net` |
| **Arguments** | `start MpsSvc` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 10 seconds |
| **Reversible** | ✅ Yes (net stop MpsSvc) |

**Description:**  
Start Windows Firewall service (`MpsSvc`) jika sedang stopped.

**When Triggered:** Firewall service status = Stopped

---

#### Action S4: Enable UAC via Registry

| Attribute | Value |
|-----------|-------|
| **Type** | RegistryFix |
| **Risk Level** | Low |
| **Requires Admin** | Yes |
| **Requires Reboot** | Yes |
| **Estimated Time** | 5 seconds |
| **Reversible** | ✅ Yes (manual registry edit) |

**Description:**  
Enable User Account Control dengan menulis registry value.

**Registry Change:**
- **Hive:** HKEY_LOCAL_MACHINE
- **Key:** `SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System`
- **Value Name:** `EnableLUA`
- **Value Type:** REG_DWORD
- **Value Data:** 1

**When Triggered:** UAC disabled (EnableLUA = 0)

**⚠️ Warning:** Reboot required untuk activation.

---

#### Action S5: Open UAC Settings

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `UserAccountControlSettings.exe` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka UAC configuration dialog untuk mengubah level notification.

**When Triggered:** UAC disabled

---

#### Action S6: Start Specific Security Service

| Attribute | Value |
|-----------|-------|
| **Type** | ServiceRestart |
| **Risk Level** | Low |
| **Command** | `net` |
| **Arguments** | `start <service_name>` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 10 seconds |

**Services yang bisa di-start:**
- `WinDefend` (Windows Defender Antivirus)
- `SecurityHealthService` (Windows Security Service)
- `wscsvc` (Security Center)

**When Triggered:** Security service stopped

---

### 🔄 Windows Update Category

---

#### Action WU1: Open Windows Update

| Attribute | Value |
|-----------|-------|
| **Type** | OpenSettings |
| **Risk Level** | None |
| **Command** | `ms-settings:windowsupdate` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Windows Update settings untuk manual check dan install updates.

**When Triggered:** Update overdue (> 30 days)

---

#### Action WU2: Enable Windows Update Service

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | Low |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 10 seconds |

**Description:**  
Enable dan start Windows Update service (`wuauserv`).

**Implementation:**
```cmd
sc config wuauserv start= auto
net start wuauserv
```

**When Triggered:** Windows Update service disabled

---

#### Action WU3: Force Update Check

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | None |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 30 seconds |

**Description:**  
Trigger Windows Update scan via COM API.

**Implementation:**
```powershell
(New-Object -ComObject Microsoft.Update.AutoUpdate).DetectNow()
```

**When Triggered:** Update overdue

---

### 🛠️ Drivers Category

---

#### Action DR1: Open Device Manager

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `devmgmt.msc` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Device Manager untuk inspect dan update drivers secara manual.

**When Triggered:** Problem devices detected atau outdated drivers

---

#### Action DR2: Scan for Hardware Changes

| Attribute | Value |
|-----------|-------|
| **Type** | Automatic |
| **Risk Level** | Low |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 15 seconds |

**Description:**  
Force Windows re-detect hardware dan reinstall drivers untuk problem devices.

**Implementation:**
```powershell
Get-PnpDevice | Where-Object {$_.Status -eq 'Error'} | 
    Enable-PnpDevice -Confirm:$false -ErrorAction SilentlyContinue
```

**When Triggered:** Problem device with error code (not Code 22 - Disabled)

---

#### Action DR3: Check Optional Driver Updates

| Attribute | Value |
|-----------|-------|
| **Type** | OpenSettings |
| **Risk Level** | None |
| **Command** | `ms-settings:windowsupdate` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Windows Update untuk check optional driver updates.

**When Triggered:** Outdated drivers detected

---

### 🚀 Startup Programs Category

---

#### Action SU1: Open Task Manager Startup Tab

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `taskmgr.exe` |
| **Arguments** | `/7` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Task Manager langsung ke Startup tab untuk disable startup programs.

**When Triggered:** Too many startup programs (> 15)

---

#### Action SU2: Open Startup Apps Settings

| Attribute | Value |
|-----------|-------|
| **Type** | OpenSettings |
| **Risk Level** | None |
| **Command** | `ms-settings:startupapps` |
| **Requires Admin** | No |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Windows Settings > Startup Apps page.

**When Triggered:** Moderate to too many startup programs

---

#### Action SU3: Open Task Scheduler

| Attribute | Value |
|-----------|-------|
| **Type** | SystemTool |
| **Risk Level** | None |
| **Command** | `taskschd.msc` |
| **Requires Admin** | Yes |
| **Requires Reboot** | No |
| **Estimated Time** | 5 seconds |

**Description:**  
Membuka Task Scheduler untuk review scheduled tasks.

**When Triggered:** Scheduled tasks review recommended

---

## 📊 Statistics

### Actions by Category

| Category | Count |
|----------|-------|
| Disk Health | 6 |
| Performance | 4 |
| Network | 5 |
| Security | 6 |
| Windows Update | 3 |
| Drivers | 3 |
| Startup Programs | 3 |
| **Total** | **30** |

### Actions by Type

| Type | Count | Percentage |
|------|-------|-----------|
| Automatic | 8 | 27% |
| CommandLine | 3 | 10% |
| SystemTool | 10 | 33% |
| OpenSettings | 7 | 23% |
| ServiceRestart | 1 (+ per-service) | 3% |
| RegistryFix | 1 | 3% |

### Actions by Risk Level

| Risk | Count | Percentage |
|------|-------|-----------|
| None | 14 | 47% |
| Low | 12 | 40% |
| Medium | 4 | 13% |
| High | 0 | 0% |

---

## 🔒 Safety Guarantees

1. **All destructive actions require explicit confirmation** via `ConfirmRepairDialog`
2. **Risk level always displayed** to user before execution
3. **Reboot requirements clearly indicated** with warning icons
4. **"Fix All Safe" batch** only executes Low/None risk automatic actions
5. **No arbitrary user input** to system commands (all hardcoded)
6. **Process timeout** enforced (120s default) untuk mencegah hanging

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial repair actions catalog | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Repair Actions Catalog for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>