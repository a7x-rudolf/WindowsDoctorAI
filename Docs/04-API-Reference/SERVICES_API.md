# 📚 Services API Reference

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Reference lengkap untuk semua public API di layer **Services** WindowsDoctorAI. Setiap service didokumentasikan dengan signature, parameters, return values, exceptions, dan usage examples.

---

## 📋 Services List

| Service | Namespace | Purpose |
|---------|-----------|---------|
| `DiagnosticEngine` | `WindowsDoctorAI.Services` | Orchestrates all diagnostic scans |
| `RepairService` | `WindowsDoctorAI.Services` | Executes repair actions |
| `ReportExportService` | `WindowsDoctorAI.Services` | Generates HTML reports |
| `SystemInfoService` | `WindowsDoctorAI.Services` | Retrieves system information |
| `DiskHealthService` | `WindowsDoctorAI.Services` | Disk diagnostics |
| `PerformanceDiagnosticService` | `WindowsDoctorAI.Services` | Performance diagnostics |
| `NetworkDiagnosticService` | `WindowsDoctorAI.Services` | Network diagnostics |
| `SecurityDiagnosticService` | `WindowsDoctorAI.Services` | Security diagnostics |
| `WindowsUpdateService` | `WindowsDoctorAI.Services` | Windows Update diagnostics |
| `DriverDiagnosticService` | `WindowsDoctorAI.Services` | Driver diagnostics |
| `StartupOptimizerService` | `WindowsDoctorAI.Services` | Startup diagnostics |

---

## 1. DiagnosticEngine

**File:** `Services/DiagnosticEngine.cs`

Orchestrator utama yang mengelola semua diagnostic services dan menghasilkan aggregated results.

### 1.1 Class Definition

```csharp
public class DiagnosticEngine
```

### 1.2 Events

#### OnCategoryStarted

Fired ketika scan pada suatu category dimulai.

```csharp
public event Action<string>? OnCategoryStarted;
```

**Parameters:**
- `string` — Name of the category yang starting (e.g., "Disk Health")

**Usage:**
```csharp
_engine.OnCategoryStarted += (categoryName) =>
{
    Console.WriteLine($"Scanning: {categoryName}");
};
```

---

#### OnProgressUpdated

Fired saat progress percentage berubah.

```csharp
public event Action<string, int>? OnProgressUpdated;
```

**Parameters:**
- `string` — Current category name
- `int` — Progress percentage (0-100)

---

#### OnResultFound

Fired setiap kali diagnostic result ditemukan (real-time update).

```csharp
public event Action<DiagnosticResult>? OnResultFound;
```

**Parameters:**
- `DiagnosticResult` — Complete diagnostic result object

**Usage:**
```csharp
_engine.OnResultFound += (result) =>
{
    if (result.Severity == Severity.Critical)
    {
        LogCritical(result);
    }
};
```

### 1.3 Methods

#### RunFullDiagnosticAsync

Menjalankan full diagnostic scan pada semua kategori.

```csharp
public async Task<(List<DiagnosticResult> Results, SystemHealthScore Score)> RunFullDiagnosticAsync(
    CancellationToken cancellationToken = default)
```

**Parameters:**
- `cancellationToken` (optional) — Token untuk cancel operation

**Returns:**
- `Tuple` berisi:
  - `List<DiagnosticResult>` — Semua hasil scan
  - `SystemHealthScore` — Aggregated health score

**Exceptions:**
- `OperationCanceledException` — Kalau cancellation requested

**Example:**
```csharp
var engine = new DiagnosticEngine();

engine.OnCategoryStarted += (cat) => Console.WriteLine($"Starting: {cat}");
engine.OnProgressUpdated += (cat, pct) => Console.WriteLine($"{pct}%");

var cts = new CancellationTokenSource();
try
{
    var (results, score) = await engine.RunFullDiagnosticAsync(cts.Token);
    
    Console.WriteLine($"Total results: {results.Count}");
    Console.WriteLine($"Health score: {score.OverallScore}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Scan cancelled");
}
```

**Performance:**
- Typical duration: 20-45 seconds
- Sequential execution per category
- Non-blocking (UI thread free)

---

## 2. RepairService

**File:** `Services/RepairService.cs`

Mengeksekusi repair actions dengan berbagai jenis (Automatic, CommandLine, SystemTool, dll).

### 2.1 Class Definition

```csharp
public class RepairService
```

### 2.2 Events

#### OnProgress

Fired saat repair action progressing.

```csharp
public event Action<RepairAction, string>? OnProgress;
```

**Parameters:**
- `RepairAction` — Action yang sedang dieksekusi
- `string` — Progress message

**Usage:**
```csharp
_repair.OnProgress += (action, msg) =>
{
    Console.WriteLine($"[{action.Name}] {msg}");
};
```

### 2.3 Methods

#### ExecuteAsync

Menjalankan repair action.

```csharp
public async Task<bool> ExecuteAsync(RepairAction action)
```

**Parameters:**
- `action` — RepairAction untuk dieksekusi

**Returns:**
- `bool` — `true` jika successful, `false` jika failed

**Side Effects:**
- Updates `action.Status` (Pending → InProgress → Completed/Failed)
- Updates `action.ResultMessage`
- Fires `OnProgress` events

**Example:**
```csharp
var repair = new RepairService();

repair.OnProgress += (action, msg) => Console.WriteLine(msg);

var action = new RepairAction
{
    Name = "Clear Temp Files",
    ActionType = RepairActionType.Automatic,
    ExecuteAsync = async () => { /* implementation */ return true; }
};

bool success = await repair.ExecuteAsync(action);
Console.WriteLine($"Result: {(success ? "Success" : "Failed")}");
Console.WriteLine($"Status: {action.Status}");
```

**Supported Action Types:**
- `Automatic` — Executes `action.ExecuteAsync` delegate
- `CommandLine` — Runs command dengan `ProcessHelper.RunCommandAsync`
- `SystemTool` — Opens tool dengan `Process.Start(useShellExecute: true)`
- `OpenSettings` — Opens Windows Settings deep link
- `ServiceRestart` — Runs `net start/stop` commands
- `RegistryFix` — Modifies Windows Registry

---

## 3. ReportExportService

**File:** `Services/ReportExportService.cs`

Generate HTML reports dari scan results.

### 3.1 Class Definition

```csharp
public class ReportExportService
```

### 3.2 Methods

#### ExportToHtmlAsync

Membuat HTML report file dan menyimpan ke Desktop.

```csharp
public async Task<string> ExportToHtmlAsync(
    List<DiagnosticResult> results,
    SystemHealthScore score,
    Dictionary<string, string> sysInfo)
```

**Parameters:**
- `results` — List of diagnostic results
- `score` — Aggregated health score
- `sysInfo` — System information key-value pairs

**Returns:**
- `string` — Absolute path file yang disimpan

**File Location:**
```
%USERPROFILE%\Desktop\WindowsDoctorAI_Report_YYYYMMDD_HHMMSS.html
```

**Example:**
```csharp
var report = new ReportExportService();
var sysInfo = new SystemInfoService().GetSystemInfo();

string filePath = await report.ExportToHtmlAsync(results, score, sysInfo);
Console.WriteLine($"Report saved: {filePath}");

// Auto-open
Process.Start(new ProcessStartInfo
{
    FileName = filePath,
    UseShellExecute = true
});
```

**Report Contents:**
- Overall health score dan rating
- Summary statistics (Critical, Warning, Info, Healthy counts)
- System information section
- Results grouped by category
- Available repair actions per result

---

## 4. SystemInfoService

**File:** `Services/SystemInfoService.cs`

Mengambil informasi sistem hardware dan software.

### 4.1 Class Definition

```csharp
public class SystemInfoService
```

### 4.2 Methods

#### GetSystemInfo

Retrieves comprehensive system information.

```csharp
public Dictionary<string, string> GetSystemInfo()
```

**Returns:**
- `Dictionary<string, string>` — Key-value pairs of system info

**Example Return Value:**
```csharp
{
    ["OS Name"] = "Microsoft Windows 11 Pro",
    ["OS Version"] = "Microsoft Windows NT 10.0.22631.0",
    ["OS Build"] = "22631",
    ["Architecture"] = "X64",
    ["Computer Name"] = "DESKTOP-ABC123",
    ["User Name"] = "RIDOLF",
    ["Processor"] = "Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz",
    ["CPU Cores"] = "8",
    ["Total RAM"] = "32.0 GB",
    ["BIOS Version"] = "F5",
    ["Motherboard"] = "Z490 AORUS ELITE",
    ["Manufacturer"] = "Gigabyte Technology Co., Ltd.",
    ["Last Boot"] = "2025-07-15 08:30:15",
    ["Uptime"] = "0.05:23:45"
}
```

**Example:**
```csharp
var sysInfo = new SystemInfoService();
var info = sysInfo.GetSystemInfo();

foreach (var (key, value) in info)
{
    Console.WriteLine($"{key}: {value}");
}
```

**Data Sources:**
- WMI: `Win32_OperatingSystem`, `Win32_Processor`, `Win32_BIOS`, `Win32_BaseBoard`
- .NET APIs: `Environment.OSVersion`, `Environment.UserName`, `RuntimeInformation.OSArchitecture`

---

## 5. Domain Services (Common Pattern)

Semua domain services (DiskHealthService, PerformanceDiagnosticService, dll) mengikuti pattern yang sama.

### 5.1 Common Interface (Implicit)

Meskipun tidak ada explicit interface, semua services implement method signature:

```csharp
public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
```

### 5.2 Individual Service Details

#### DiskHealthService

**Purpose:** Memeriksa disk space, S.M.A.R.T. status, temp files, optimization.

```csharp
public class DiskHealthService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- Free space per drive
- S.M.A.R.T. status via WMI
- Temp file accumulation
- HDD vs SSD detection

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#1--disk-health)

---

#### PerformanceDiagnosticService

**Purpose:** Memeriksa CPU, RAM, high-memory processes, uptime.

```csharp
public class PerformanceDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- CPU usage (via PerformanceCounter)
- RAM usage (via WMI)
- Top 5 memory processes
- System uptime

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#2--performance)

---

#### NetworkDiagnosticService

**Purpose:** Memeriksa adapters, connectivity, DNS, latency.

```csharp
public class NetworkDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- Active network adapters
- Ping ke DNS servers (8.8.8.8, 1.1.1.1, 208.67.222.222)
- DNS resolution test
- Network latency

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#3--network)

---

#### SecurityDiagnosticService

**Purpose:** Memeriksa antivirus, firewall, UAC, security services.

```csharp
public class SecurityDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- Antivirus detection (WMI SecurityCenter2)
- Windows Firewall status
- UAC configuration
- Security services status

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#4--security)

---

#### WindowsUpdateService

**Purpose:** Memeriksa Windows Update service, last update, pending reboot.

```csharp
public class WindowsUpdateService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- `wuauserv` service status
- Last hotfix installation date
- Pending reboot detection

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#5--windows-update)

---

#### DriverDiagnosticService

**Purpose:** Memeriksa problem devices dan outdated drivers.

```csharp
public class DriverDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- Problem devices via WMI (`Win32_PnPEntity` with error codes)
- Outdated drivers (> 2 years) via PowerShell

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#6--drivers)

---

#### StartupOptimizerService

**Purpose:** Memeriksa startup programs dan scheduled tasks.

```csharp
public class StartupOptimizerService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}
```

**Checks Performed:**
- Registry Run/RunOnce keys (HKLM + HKCU)
- Scheduled tasks overview

**Detail:** Lihat [DIAGNOSTIC_CATEGORIES.md](../03-Features/DIAGNOSTIC_CATEGORIES.md#7--startup-programs)

---

## 6. Service Composition Example

Contoh kompleks menggunakan multiple services:

```csharp
public async Task<ScanResult> PerformCustomScanAsync()
{
    // Initialize services
    var diskService = new DiskHealthService();
    var perfService = new PerformanceDiagnosticService();
    var sysInfoService = new SystemInfoService();
    var reportService = new ReportExportService();
    
    // Get system context
    var sysInfo = sysInfoService.GetSystemInfo();
    Console.WriteLine($"Scanning: {sysInfo["Computer Name"]}");
    
    // Run selected scans
    var diskResults = await diskService.RunDiagnosticsAsync();
    var perfResults = await perfService.RunDiagnosticsAsync();
    
    // Aggregate results
    var allResults = new List<DiagnosticResult>();
    allResults.AddRange(diskResults);
    allResults.AddRange(perfResults);
    
    // Calculate score
    var avgScore = allResults.Average(r => r.Score);
    var score = new SystemHealthScore
    {
        OverallScore = avgScore,
        TotalIssues = allResults.Count(r => r.Severity != Severity.Healthy),
        CriticalIssues = allResults.Count(r => r.Severity == Severity.Critical),
        WarningIssues = allResults.Count(r => r.Severity == Severity.Warning),
    };
    
    // Export report
    string reportPath = await reportService.ExportToHtmlAsync(
        allResults, score, sysInfo);
    
    return new ScanResult
    {
        Results = allResults,
        Score = score,
        ReportPath = reportPath
    };
}
```

---

## 7. Best Practices

### 7.1 Service Instantiation

**Current (v2.0.0):** Direct instantiation
```csharp
var engine = new DiagnosticEngine();
```

**Future (v2.2.0+):** Dependency Injection
```csharp
services.AddSingleton<DiagnosticEngine>();
services.AddTransient<RepairService>();

// Consumer
public class MyViewModel(DiagnosticEngine engine, RepairService repair) { }
```

### 7.2 Error Handling

Services **tidak throw exceptions** untuk normal errors. Instead:
- Return `DiagnosticResult` dengan `Status = Failed`
- Include error info di `Description`

```csharp
try
{
    var results = await service.RunDiagnosticsAsync();
    // Process results
}
catch (Exception ex)
{
    // Only truly unexpected errors bubble up
    _logger.LogError(ex, "Unexpected error");
}
```

### 7.3 Cancellation

Always pass CancellationToken untuk long-running operations:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

try
{
    await engine.RunFullDiagnosticAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Handle gracefully
}
```

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial services API reference | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Services API Reference for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>