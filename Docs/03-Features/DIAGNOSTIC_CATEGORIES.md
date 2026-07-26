# 🔬 Diagnostic Categories Reference

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Document Overview

Dokumen ini menjelaskan secara detail **7 kategori diagnostik** yang diimplementasikan di WindowsDoctorAI, termasuk teknologi yang digunakan, checks yang dilakukan, kriteria severity, dan repair actions yang tersedia untuk setiap kategori.

---

## 📋 Categories Summary

| # | Category | Icon | Checks | Repair Actions |
|---|----------|------|--------|----------------|
| 1 | Disk Health | 💾 | 4-6 | 6+ actions |
| 2 | Performance | ⚡ | 4 | 4+ actions |
| 3 | Network | 🌐 | 4-5 | 5+ actions |
| 4 | Security | 🔒 | 4-5 | 5+ actions |
| 5 | Windows Update | 🔄 | 2-3 | 3+ actions |
| 6 | Drivers | 🛠️ | 1-3 | 3+ actions |
| 7 | Startup Programs | 🚀 | 2 | 2+ actions |
| **Total** | **7 categories** | | **~25 checks** | **~30 actions** |

---

## 1. 💾 Disk Health

### 1.1 Overview

Kategori ini memeriksa kesehatan sistem penyimpanan (HDD, SSD, NVMe) pada komputer, termasuk kapasitas, integritas hardware, dan file management.

### 1.2 Technology Stack

- **DriveInfo API** (.NET) untuk drive enumeration
- **WMI Win32_DiskDrive** untuk S.M.A.R.T. status
- **System.IO.Directory** untuk temp file scanning

### 1.3 Checks Performed

#### Check 1: Drive Space Analysis

**What is checked:**
- Total capacity per drive
- Free space per drive
- Free space percentage

**Severity Criteria:**
| Free Space | Severity | Score |
|------------|----------|-------|
| ≥ 15% | Healthy | 100 |
| 5% - 15% | Warning | 50 |
| < 5% | Critical | 10 |

**Repair Actions Triggered:**
- Run Disk Cleanup
- Open Storage Settings
- Clear Temporary Files (if < 5%)

---

#### Check 2: S.M.A.R.T. Status

**What is checked:**
- Disk hardware health via S.M.A.R.T. (Self-Monitoring, Analysis and Reporting Technology)
- Model information
- Total capacity

**Severity Criteria:**
| S.M.A.R.T. Status | Severity | Score |
|-------------------|----------|-------|
| "OK" | Healthy | 100 |
| "Pred Fail" | Critical | 5 |
| Other | Warning | 55 |

**Repair Actions Triggered:**
- Open Backup Settings (if Pred Fail)
- Run CHKDSK (if abnormal)

---

#### Check 3: Temporary Files Analysis

**What is checked:**
- File count in `%TEMP%`
- File count in `C:\Windows\Temp`
- Total size occupied

**Severity Criteria:**
| Size | Severity | Score |
|------|----------|-------|
| < 500 MB | Healthy | 100 |
| 500-2000 MB | Info | 80 |
| > 2000 MB | Warning | 60 |

**Repair Actions Triggered:**
- Clear Temporary Files (if > 500 MB)

---

#### Check 4: Disk Optimization Status

**What is checked:**
- Detection of HDD vs SSD/NVMe
- Optimization recommendation (defrag untuk HDD, TRIM automatic untuk SSD)

**Severity Criteria:**
- **SSD Detected**: Healthy (score 100) — TRIM otomatis oleh Windows
- **HDD Detected**: Info (score 80) — Defrag disarankan

**Repair Actions Triggered:**
- Open Defragment and Optimize Drives tool

---

### 1.4 WMI Queries Used

```sql
-- Query 1: List all disk drives
SELECT Model, Status, MediaType, Size FROM Win32_DiskDrive

-- Query 2: SMART status
SELECT Model, Status FROM Win32_DiskDrive
```

---

## 2. ⚡ Performance

### 2.1 Overview

Kategori ini memantau kinerja real-time sistem, mencakup penggunaan CPU, RAM, proses yang boros memory, dan durasi uptime.

### 2.2 Technology Stack

- **PerformanceCounter** untuk CPU monitoring
- **WMI Win32_OperatingSystem** untuk memory info
- **System.Diagnostics.Process** untuk process enumeration
- **Environment.TickCount64** untuk uptime

### 2.3 Checks Performed

#### Check 1: CPU Usage

**What is checked:**
- Sampled CPU usage (1-second average) via `PerformanceCounter("Processor", "% Processor Time", "_Total")`

**Severity Criteria:**
| CPU Usage | Severity | Score |
|-----------|----------|-------|
| < 70% | Healthy | 100 |
| 70-90% | Warning | 50 |
| > 90% | Critical | 15 |

**Repair Actions Triggered:**
- Open Task Manager (if > 70%)
- Set Balanced Power Plan (if > 90%)

---

#### Check 2: RAM Usage

**What is checked:**
- Total physical memory
- Free physical memory
- Usage percentage

**Severity Criteria:**
| RAM Usage | Severity | Score |
|-----------|----------|-------|
| < 75% | Healthy | 100 |
| 75-90% | Warning | 55 |
| > 90% | Critical | 15 |

**Repair Actions Triggered:**
- Open Task Manager
- Open Resource Monitor (if > 90%)

---

#### Check 3: High Memory Processes

**What is checked:**
- Enumerasi semua processes
- Filter processes yang menggunakan > 500 MB RAM
- Top 5 highest memory consumers

**Severity Criteria:**
| Count | Severity | Score |
|-------|----------|-------|
| 0-3 | Info | 85 |
| > 3 | Warning | 60 |

**Repair Actions Triggered:**
- Open Resource Monitor

---

#### Check 4: System Uptime

**What is checked:**
- Duration since last boot (via `Environment.TickCount64`)

**Severity Criteria:**
| Uptime | Severity | Score |
|--------|----------|-------|
| < 7 days | Healthy | 100 |
| 7-30 days | Info | 80 |
| > 30 days | Warning | 50 |

**Repair Actions Triggered:**
- Schedule Restart (if > 30 days)

---

## 3. 🌐 Network

### 3.1 Overview

Kategori ini memeriksa konektivitas jaringan, konfigurasi adapter, resolusi DNS, dan latency ke server publik.

### 3.2 Technology Stack

- **System.Net.NetworkInformation.NetworkInterface** untuk adapter enumeration
- **System.Net.NetworkInformation.Ping** untuk latency test
- **System.Net.Dns** untuk DNS resolution

### 3.3 Checks Performed

#### Check 1: Network Adapters

**What is checked:**
- Enumerasi semua network interfaces (exclude Loopback, Tunnel)
- Status (Up/Down)
- Speed (Mbps)
- IPv4 Address
- Gateway Address

**Severity Criteria:**
| Active Adapters | Severity | Score |
|-----------------|----------|-------|
| ≥ 1 | Healthy (per adapter) | 100 |
| 0 | Critical | 0 |

**Repair Actions Triggered:**
- Open Network Settings (if no active)
- Reset Network Stack (if no active)

---

#### Check 2: Internet Connectivity

**What is checked:**
- Ping test ke 3 DNS servers publik:
  - 8.8.8.8 (Google)
  - 1.1.1.1 (Cloudflare)
  - 208.67.222.222 (OpenDNS)
- Timeout: 3 seconds per host

**Severity Criteria:**
| Reachable Hosts | Severity | Score |
|-----------------|----------|-------|
| 3/3 | Healthy | 100 |
| 1-2/3 | Warning | 60 |
| 0/3 | Critical | 0 |

**Repair Actions Triggered:**
- Run Network Troubleshooter
- Reset Network Stack
- Release and Renew IP
- Flush DNS Cache

---

#### Check 3: DNS Resolution

**What is checked:**
- Resolve 3 domain names:
  - www.google.com
  - www.microsoft.com
  - www.cloudflare.com

**Severity Criteria:**
| Resolved Domains | Severity | Score |
|------------------|----------|-------|
| 3/3 | Healthy | 100 |
| 1-2/3 | Warning | 65 |
| 0/3 | Critical | 10 |

**Repair Actions Triggered:**
- Flush DNS Cache
- Set DNS to Google (8.8.8.8, 8.8.4.4)

---

#### Check 4: Network Latency

**What is checked:**
- Ping ke 8.8.8.8, timeout 5 seconds
- Roundtrip time (ms)

**Severity Criteria:**
| Latency | Severity | Score |
|---------|----------|-------|
| < 100 ms | Healthy | 100 |
| 100-200 ms | Info | 75 |
| > 200 ms | Warning | 50 |

---

## 4. 🔒 Security

### 4.1 Overview

Kategori ini memeriksa postur keamanan sistem, mencakup antivirus, firewall, UAC, dan security services.

### 4.2 Technology Stack

- **WMI SecurityCenter2** untuk antivirus detection
- **ServiceController** untuk service status
- **Registry** untuk UAC configuration

### 4.3 Checks Performed

#### Check 1: Antivirus Status

**What is checked:**
- WMI Query pada `\\.\root\SecurityCenter2` namespace
- Detect installed antivirus products
- Enabled status (bit-level parsing of `productState`)
- Definitions up-to-date status

**Severity Criteria:**
| Condition | Severity | Score |
|-----------|----------|-------|
| Installed, Enabled, Up-to-date | Healthy | 100 |
| Installed, Enabled, Outdated | Warning | 50 |
| Installed, NOT Enabled | Critical | 10 |
| No AV Detected | Critical | 0 |

**Repair Actions Triggered:**
- Open Windows Security
- Update Defender Definitions

---

#### Check 2: Windows Firewall

**What is checked:**
- Status of `MpsSvc` (Windows Firewall Service) via ServiceController

**Severity Criteria:**
| Status | Severity | Score |
|--------|----------|-------|
| Running | Healthy | 100 |
| Stopped | Critical | 10 |

**Repair Actions Triggered:**
- Start Firewall Service
- Open Firewall Settings

---

#### Check 3: User Account Control (UAC)

**What is checked:**
- Registry key: `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System`
- Value name: `EnableLUA`

**Severity Criteria:**
| EnableLUA | Severity | Score |
|-----------|----------|-------|
| 1 (Enabled) | Healthy | 100 |
| 0 or missing | Critical | 15 |

**Repair Actions Triggered:**
- Enable UAC via Registry (writes EnableLUA = 1)
- Open UAC Settings

---

#### Check 4: Security Services

**What is checked:**
- Status of critical security services:
  - `WinDefend` (Windows Defender Antivirus)
  - `SecurityHealthService` (Windows Security Service)
  - `wscsvc` (Security Center)

**Severity Criteria (per service):**
| Status | Severity | Score |
|--------|----------|-------|
| Running | Healthy | 100 |
| Stopped | Warning | 40 |

**Repair Actions Triggered:**
- Start Specific Service (per stopped service)

---

## 5. 🔄 Windows Update

### 5.1 Overview

Kategori ini memeriksa status Windows Update service, tanggal update terakhir, dan pending reboot.

### 5.2 Technology Stack

- **ServiceController** untuk `wuauserv` status
- **PowerShell Get-HotFix** untuk update history
- **Registry** untuk pending reboot detection

### 5.3 Checks Performed

#### Check 1: Windows Update Service

**What is checked:**
- Service status `wuauserv`
- Start type (Automatic/Manual/Disabled)

**Severity Criteria:**
| Configuration | Severity | Score |
|---------------|----------|-------|
| Running or Auto | Healthy | 100 |
| Stopped, Manual | Warning | 60 |
| Disabled | Critical | 10 |

**Repair Actions Triggered:**
- Start Windows Update Service
- Enable Windows Update Service (if Disabled)

---

#### Check 2: Last Update Date

**What is checked:**
- PowerShell command: `(Get-HotFix | Sort InstalledOn -Descending | Select -First 1).InstalledOn`

**Severity Criteria:**
| Days Since Update | Severity | Score |
|-------------------|----------|-------|
| < 30 days | Healthy | 100 |
| 30-60 days | Warning | 55 |
| > 60 days | Critical | 15 |

**Repair Actions Triggered:**
- Open Windows Update
- Force Update Check

---

#### Check 3: Pending Reboot Detection

**What is checked:**
- Registry key existence: `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired`

**Severity Criteria:**
| Reboot Pending | Severity | Score |
|----------------|----------|-------|
| No | (not reported) | N/A |
| Yes | Warning | 60 |

**Repair Actions Triggered:**
- Schedule Restart (5-minute countdown)

---

## 6. 🛠️ Drivers

### 6.1 Overview

Kategori ini memeriksa driver perangkat yang bermasalah (Device Manager errors) dan driver yang sudah usang.

### 6.2 Technology Stack

- **WMI Win32_PnPEntity** untuk problem devices
- **PowerShell Get-WindowsDriver** untuk driver age analysis

### 6.3 Checks Performed

#### Check 1: Problem Devices

**What is checked:**
- WMI Query: `SELECT ... FROM Win32_PnPEntity WHERE ConfigManagerErrorCode <> 0`
- Error codes mapped ke deskripsi (Code 22 = Disabled, Code 43 = Reported problem, etc.)

**Severity Criteria (per device):**
| Error Code | Severity | Score |
|-----------|----------|-------|
| 22 (Disabled) | Info | 80 |
| Others | Warning | 40 |
| None found | Healthy | 100 |

**Repair Actions Triggered:**
- Open Device Manager
- Scan for Hardware Changes (if not Code 22)

---

#### Check 2: Outdated Drivers

**What is checked:**
- PowerShell: `Get-WindowsDriver -Online -All | Where {$_.Date -lt (Get-Date).AddYears(-2)}`
- Count drivers older than 2 years

**Severity Criteria:**
| Outdated Count | Severity | Score |
|----------------|----------|-------|
| 0 | (not reported) | N/A |
| 1-5 | Info | 75 |
| > 5 | Warning | 60 |

**Repair Actions Triggered:**
- Check Optional Driver Updates
- Open Device Manager

---

## 7. 🚀 Startup Programs

### 7.1 Overview

Kategori ini menganalisis program yang berjalan saat Windows startup, yang bisa mempengaruhi boot time.

### 7.2 Technology Stack

- **Registry** untuk Run/RunOnce keys
- **Task Scheduler** (placeholder, deep scan planned)

### 7.3 Checks Performed

#### Check 1: Startup Programs Count

**What is checked:**
- HKLM Registry keys:
  - `SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
  - `SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`
  - `SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run`
- HKCU Registry keys:
  - `SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
  - `SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`

**Severity Criteria:**
| Startup Count | Severity | Score |
|---------------|----------|-------|
| ≤ 8 | Healthy | 100 |
| 9-15 | Info | 70 |
| > 15 | Warning | 40 |

**Repair Actions Triggered:**
- Open Task Manager (Startup tab)
- Open Startup Apps Settings

---

#### Check 2: Scheduled Tasks Overview

**What is checked:**
- Currently a placeholder yang menampilkan informational message
- Deep task scanning planned untuk v2.1.0

**Severity:** Info (score 90)

**Repair Actions Triggered:**
- Open Task Scheduler

---

## 8. Scoring Algorithm

### 8.1 Individual Result Scoring

Setiap `DiagnosticResult` memiliki skor 0-100 berdasarkan kriteria severity di atas.

### 8.2 Category Scoring

```csharp
CategoryScore = Average(ResultScores[] per category)
```

### 8.3 Overall Health Score

```csharp
OverallScore = Average(CategoryScores[])
OverallScore -= CriticalIssues * 5  // Penalty
OverallScore = Clamp(0, 100, OverallScore)
```

### 8.4 Rating Mapping

| Score Range | Rating | Color |
|-------------|--------|-------|
| 90-100 | Excellent | 🟢 Green |
| 75-89 | Good | 🔵 Blue |
| 60-74 | Fair | 🟡 Yellow |
| 40-59 | Poor | 🟠 Orange |
| 0-39 | Critical | 🔴 Red |

---

## 9. Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial diagnostic categories documentation | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Diagnostic Categories Documentation for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>