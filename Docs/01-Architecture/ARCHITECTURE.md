# 🏛️ System Architecture Documentation

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Document Information

| Attribute | Value |
|-----------|-------|
| **Document Type** | Technical Architecture Reference |
| **Target Audience** | Developers, Architects, Contributors |
| **Complexity Level** | Intermediate to Advanced |
| **Prerequisites** | Understanding of MVVM, .NET, WinUI 3 |
| **Author** | RIDOLF WIDI ALFISA LUMBA |
| **Copyright** | © 2025 RIDOLF WIDI ALFISA LUMBA |

---

## 1. Executive Summary

WindowsDoctorAI mengimplementasikan **layered architecture** dengan pola **MVVM (Model-View-ViewModel)** sebagai fondasi utama. Aplikasi ini dirancang dengan prinsip:

- ✅ **Separation of Concerns** — Setiap layer memiliki tanggung jawab yang jelas
- ✅ **Loose Coupling** — Komponen dapat diganti tanpa mempengaruhi yang lain
- ✅ **High Cohesion** — Fungsi terkait dikelompokkan dalam satu module
- ✅ **Testability** — Business logic terpisah dari UI
- ✅ **Maintainability** — Struktur folder dan naming convention konsisten

---

## 2. Architectural Overview

### 2.1 High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                           │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Views (Pages) + Dialogs + Converters + Styles           │  │
│  │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐         │  │
│  │  │Dashboard│ │Diagnostic│ │Results  │ │Repair   │         │  │
│  │  │  Page   │ │  Page   │ │  Page   │ │  Page   │         │  │
│  │  └─────────┘ └─────────┘ └─────────┘ └─────────┘         │  │
│  └───────────────────────────┬───────────────────────────────┘  │
└───────────────────────────────┼─────────────────────────────────┘
                                │ (Data Binding + Commands)
┌───────────────────────────────▼─────────────────────────────────┐
│                    VIEWMODEL LAYER (MVVM)                       │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │              MainViewModel                                │  │
│  │  - ObservableProperties (Score, Results, Repairs...)     │  │
│  │  - RelayCommands (RunScan, ExecuteRepair, Export...)     │  │
│  │  - Events (ScanRequested, CategoryStarted, etc.)         │  │
│  └───────────────────────────┬───────────────────────────────┘  │
└───────────────────────────────┼─────────────────────────────────┘
                                │ (Method Calls + Events)
┌───────────────────────────────▼─────────────────────────────────┐
│                    BUSINESS LOGIC LAYER                         │
│  ┌────────────────────┐ ┌────────────────┐ ┌───────────────┐   │
│  │  DiagnosticEngine  │ │ RepairService  │ │ ReportExport  │   │
│  │  (Orchestrator)    │ │  (Executor)    │ │ Service       │   │
│  └────────┬───────────┘ └────────────────┘ └───────────────┘   │
│           │                                                     │
│  ┌────────▼──────────────────────────────────────────────┐     │
│  │            Domain Services (7 categories)              │     │
│  │  DiskHealth │ Performance │ Network │ Security │ ...  │     │
│  └────────────────────┬───────────────────────────────────┘     │
└───────────────────────┼─────────────────────────────────────────┘
                        │ (Uses)
┌───────────────────────▼─────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                         │
│  ┌──────────┐ ┌───────────┐ ┌───────────┐ ┌────────────┐      │
│  │WmiHelper │ │ProcessHelp│ │RegistryHel│ │AdminHelper │      │
│  │ (WMI)    │ │(cmd/PS)   │ │  (Reg)    │ │  (UAC)     │      │
│  └──────────┘ └───────────┘ └───────────┘ └────────────┘      │
└─────────────────────────────────────────────────────────────────┘
                        │ (System APIs)
┌───────────────────────▼─────────────────────────────────────────┐
│                    WINDOWS OPERATING SYSTEM                     │
│    WMI  │  Registry  │  Services  │  Processes  │  Network      │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 Layer Responsibilities

| Layer | Responsibility | Communicates With |
|-------|---------------|-------------------|
| **Presentation** | Rendering UI, capturing user input, data binding | ViewModel Layer |
| **ViewModel** | State management, command orchestration, event propagation | Business Logic Layer |
| **Business Logic** | Scan orchestration, repair execution, report generation | Domain Services + Infrastructure |
| **Domain Services** | Category-specific diagnostic logic | Infrastructure Layer |
| **Infrastructure** | System API abstractions (WMI, Registry, Process, Admin) | Windows OS |
| **Models** | Data structures (DTOs, Enums) | All layers |

---

## 3. Design Patterns Implementation

### 3.1 MVVM (Model-View-ViewModel)

**Purpose:** Separasi UI logic dari business logic untuk testability dan maintainability.

**Implementation:**

```csharp
// Model
public class DiagnosticResult
{
    public string Title { get; set; }
    public Severity Severity { get; set; }
    public double Score { get; set; }
    public List<RepairAction> AvailableActions { get; set; }
}

// ViewModel (menggunakan CommunityToolkit.Mvvm)
public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private double _overallScore;
    
    [ObservableProperty]
    private ObservableCollection<DiagnosticResult> _allResults = new();
    
    [RelayCommand]
    private async Task RunScanAsync() { /* ... */ }
}

// View (XAML)
<TextBlock Text="{x:Bind ViewModel.OverallScore, Mode=OneWay}" />
<Button Command="{x:Bind ViewModel.RunScanCommand}" />
```

**Benefits:**
- ✅ UI thread-safe updates via `[ObservableProperty]` source generators
- ✅ Automatic INotifyPropertyChanged implementation
- ✅ Commands as first-class citizens
- ✅ Testable ViewModels without UI dependencies

---

### 3.2 Observer Pattern (Event-Driven)

**Purpose:** Loose coupling antara scan orchestration dan UI updates.

**Implementation:**

```csharp
public class DiagnosticEngine
{
    public event Action<string>? OnCategoryStarted;
    public event Action<string, int>? OnProgressUpdated;
    public event Action<DiagnosticResult>? OnResultFound;
    
    public async Task<...> RunFullDiagnosticAsync()
    {
        OnCategoryStarted?.Invoke("Disk Health");
        // ... perform scan ...
        OnResultFound?.Invoke(result);
    }
}

// Subscriber (MainViewModel)
_engine.OnCategoryStarted += category => CurrentCategory = category;
_engine.OnResultFound += result => AllResults.Add(result);
```

**Benefits:**
- ✅ ViewModel tidak perlu polling engine state
- ✅ Real-time UI updates saat scan berlangsung
- ✅ Multiple subscribers possible (Dialog + ViewModel)

---

### 3.3 Strategy Pattern

**Purpose:** Setiap kategori diagnostik memiliki algoritma berbeda tapi interface sama.

**Implementation:**

```csharp
// Common interface (implicit via method signature)
public interface IDiagnosticService
{
    Task<List<DiagnosticResult>> RunDiagnosticsAsync();
}

// Concrete strategies
public class DiskHealthService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync() { /* Disk-specific logic */ }
}

public class NetworkDiagnosticService
{
    public async Task<List<DiagnosticResult>> RunDiagnosticsAsync() { /* Network-specific logic */ }
}

// Orchestrator uses strategies uniformly
var services = new[] { _diskService, _networkService, /* ... */ };
foreach (var service in services)
{
    var results = await service.RunDiagnosticsAsync();
    allResults.AddRange(results);
}
```

**Benefits:**
- ✅ Menambah kategori baru = tambah service baru + register di engine
- ✅ Isolasi bug (error di 1 service tidak affect service lain)
- ✅ Testable per-service

---

### 3.4 Facade Pattern

**Purpose:** `DiagnosticEngine` sebagai single entry point yang menyembunyikan kompleksitas 7 services.

**Implementation:**

```csharp
public class DiagnosticEngine
{
    private readonly DiskHealthService _disk = new();
    private readonly PerformanceDiagnosticService _perf = new();
    private readonly NetworkDiagnosticService _net = new();
    // ... 4 services lain

    public async Task<(List<DiagnosticResult>, SystemHealthScore)> RunFullDiagnosticAsync()
    {
        // Client hanya memanggil 1 method, engine yang orchestrate semua
    }
}

// ViewModel usage
var (results, score) = await _engine.RunFullDiagnosticAsync();
```

**Benefits:**
- ✅ Client (ViewModel) tidak perlu tahu ada 7 services
- ✅ Perubahan internal engine tidak affect client
- ✅ Simplified API surface

---

### 3.5 Value Converter Pattern

**Purpose:** Transformasi data model → UI-ready values di XAML binding.

**Implementation:**

```csharp
public class SeverityToColorConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        return value switch
        {
            Severity.Critical => new SolidColorBrush(Colors.Red),
            Severity.Warning => new SolidColorBrush(Colors.Orange),
            Severity.Healthy => new SolidColorBrush(Colors.Green),
            _ => new SolidColorBrush(Colors.Gray)
        };
    }
}

// XAML usage
<Border Background="{x:Bind Severity, Converter={StaticResource SeverityToColor}}" />
```

**Total Converters:** 15+ converters untuk berbagai transformasi (color, glyph, visibility, string, gradient).

---

## 4. Component Details

### 4.1 Presentation Layer

#### 4.1.1 Pages (8 total)

| Page | File | Responsibility |
|------|------|----------------|
| Dashboard | `DashboardPage.xaml` | Hero card + score gauge + category health cards |
| Diagnostic Scan | `DiagnosticPage.xaml` | Scan configuration & initiation |
| Scan Results | `ResultsPage.xaml` | Grid layout dengan filter chips |
| Repair Actions | `RepairPage.xaml` | Grid layout repair actions dengan risk badges |
| Scan History | `HistoryPage.xaml` | Placeholder untuk future feature |
| System Info | `SystemInfoPage.xaml` | Hardware/software details display |
| Settings | `SettingsPage.xaml` | 2-column layout (nav + content) |
| About | `AboutPage.xaml` | App info dengan developer credits |

#### 4.1.2 Dialogs (4 total)

| Dialog | File | Purpose |
|--------|------|---------|
| Scan Progress | `ScanProgressDialog.xaml` | Live scan progress dengan category status |
| Confirm Repair | `ConfirmRepairDialog.xaml` | Repair confirmation dengan risk detail |
| Repair Progress | `RepairProgressDialog.xaml` | Live repair execution log |
| Result Detail | `ResultDetailDialog.xaml` | Full technical detail per result |

#### 4.1.3 Converters (15+ total)

**Bool/Visibility Converters:**
- `BoolToVisibilityConverter` (dengan Invert property)
- `InverseBoolConverter`
- `CountToVisibilityConverter`

**Severity Converters:**
- `SeverityToColorConverter`
- `SeverityToBackgroundConverter`
- `SeverityToStringConverter`
- `SeverityToGlyphConverter`

**Score Converters:**
- `HealthScoreToColorConverter`
- `HealthScoreToGradientConverter`
- `HealthScoreToRatingConverter`
- `ScoreToProgressBrushConverter`
- `ScoreToWidthConverter`

**Status/Category Converters:**
- `StatusTypeToBackgroundConverter`
- `StatusTypeToForegroundConverter`
- `CategoryToGlyphConverter`
- `RepairStatusToGlyphConverter`

**String Converters:**
- `StringToBoolConverter`
- `StringToVisibilityConverter`

---

### 4.2 ViewModel Layer

#### 4.2.1 MainViewModel Structure

```csharp
public partial class MainViewModel : BaseViewModel
{
    // Dependencies
    private readonly DiagnosticEngine _engine;
    private readonly RepairService _repair;
    private readonly SystemInfoService _sysInfo;
    private readonly ReportExportService _report;

    // Observable State
    [ObservableProperty] double _overallScore;
    [ObservableProperty] string _healthRating;
    [ObservableProperty] bool _isScanning;
    [ObservableProperty] bool _hasScanResults;
    // ... more properties

    // Collections
    public ObservableCollection<DiagnosticResult> AllResults { get; }
    public ObservableCollection<RepairAction> PendingRepairs { get; }

    // Commands (auto-generated by CommunityToolkit)
    [RelayCommand] async Task RunScanAsync() { }
    [RelayCommand] async Task ExecuteRepairAsync(RepairAction? action) { }
    [RelayCommand] async Task ExecuteAllRepairsAsync() { }
    [RelayCommand] async Task ExportReportAsync() { }
    [RelayCommand] void CancelScan() { }

    // Events (untuk komunikasi ke View dialogs)
    public event Action? ScanRequested;
    public event Action<string>? CategoryStarted;
    public event Action<DiagnosticResult>? ResultFound;
    public event Action<SystemHealthScore>? ScanCompleted;
    public event Action<RepairAction, bool>? RepairCompleted;
    // ... more events
}
```

**Design Decisions:**
- **Single ViewModel** untuk seluruh app (shared state)
- **Events untuk dialog communication** (bukan direct method calls)
- **CommunityToolkit source generators** untuk mengurangi boilerplate

---

### 4.3 Business Logic Layer

#### 4.3.1 DiagnosticEngine (Orchestrator)

**Responsibilities:**
1. Register semua domain services (7 categories)
2. Execute scans sequentially per category
3. Emit progress events untuk UI updates
4. Calculate final SystemHealthScore
5. Support cancellation via CancellationToken

**Execution Flow:**
```
RunFullDiagnosticAsync()
├─ For each category (Performance, Disk, Network, Security, Update, Drivers, Startup):
│   ├─ Emit OnCategoryStarted(categoryName)
│   ├─ Emit OnProgressUpdated(categoryName, percentage)
│   ├─ Await service.RunDiagnosticsAsync()
│   ├─ For each result: Emit OnResultFound(result)
│   └─ Catch exceptions per category (isolation)
├─ Calculate SystemHealthScore
└─ Return (results, score)
```

#### 4.3.2 RepairService

**Responsibilities:**
1. Execute repair actions berdasarkan `ActionType`
2. Support 6 action types: Automatic, CommandLine, SystemTool, OpenSettings, ServiceRestart, RegistryFix
3. Update `RepairStatus` (Pending → InProgress → Completed/Failed)
4. Emit progress events untuk live logging di UI

#### 4.3.3 ReportExportService

**Responsibilities:**
1. Generate HTML report dari scan results
2. Include: score, statistics, system info, all results, available actions
3. Save to Desktop dengan timestamp filename
4. Auto-open di default browser

---

### 4.4 Infrastructure Layer

#### 4.4.1 WmiHelper

```csharp
public static class WmiHelper
{
    public static List<Dictionary<string, object>> Query(string wmiQuery, string scope);
    public static string GetStringValue(string wmiQuery, string property, string scope);
}
```

**Usage:**
```csharp
var disks = WmiHelper.Query("SELECT Model, Status FROM Win32_DiskDrive");
var osName = WmiHelper.GetStringValue("SELECT Caption FROM Win32_OperatingSystem", "Caption");
```

#### 4.4.2 ProcessHelper

```csharp
public static class ProcessHelper
{
    public static async Task<(int ExitCode, string Output, string Error)> RunCommandAsync(
        string fileName, string arguments, int timeoutMs = 120000);
    
    public static async Task<string> RunPowerShellAsync(string script, int timeoutMs);
}
```

**Features:**
- Timeout enforcement (default 120s)
- Tree-kill on timeout
- Output/Error capture
- PowerShell dengan `-NoProfile -NonInteractive -ExecutionPolicy Bypass`

#### 4.4.3 RegistryHelper

```csharp
public static class RegistryHelper
{
    public static object? GetValue(RegistryHive hive, string subKey, string? valueName);
    public static bool SetValue(RegistryHive hive, string subKey, string valueName, object value, RegistryValueKind kind);
    public static bool DeleteValue(RegistryHive hive, string subKey, string valueName);
    public static List<string> GetStartupPrograms();
}
```

#### 4.4.4 AdminHelper

```csharp
public static class AdminHelper
{
    public static bool IsRunningAsAdmin();
    public static void RestartAsAdmin();
}
```

---

## 5. Data Flow

### 5.1 Scan Flow

```
User clicks "Run Full Scan"
    ↓
Button.Command → MainViewModel.RunScanCommand
    ↓
MainViewModel.RunScanAsync()
    ├─ Set IsScanning = true
    ├─ Fire ScanRequested event
    │       ↓
    │   MainWindow subscribes → Opens ScanProgressDialog
    │
    ├─ Await _engine.RunFullDiagnosticAsync(cancellationToken)
    │       ↓
    │   DiagnosticEngine iterates 7 services
    │       ├─ Emit OnCategoryStarted → UI updates dialog
    │       ├─ Await service.RunDiagnosticsAsync()
    │       │       ↓
    │       │   Service uses Infrastructure (WMI, Registry, Process)
    │       │       ↓
    │       │   Returns List<DiagnosticResult>
    │       │
    │       └─ Emit OnResultFound per result → UI logs to activity
    │
    ├─ Calculate SystemHealthScore
    ├─ Update ViewModel properties (OverallScore, HealthRating, etc.)
    ├─ Add results to ObservableCollection (auto-updates UI)
    └─ Fire ScanCompleted event
            ↓
        Dialog shows "View Results" button
```

### 5.2 Repair Flow

```
User clicks "Execute" on repair card
    ↓
RepairPage.Execute_Click(sender, e)
    ├─ Show ConfirmRepairDialog
    ├─ If user confirms:
    │   ├─ Show RepairProgressDialog
    │   ├─ Subscribe to ViewModel events
    │   └─ Await MainViewModel.ExecuteRepairCommand.ExecuteAsync(action)
    │           ↓
    │       MainViewModel.ExecuteRepairAsync(action)
    │           ├─ Fire RepairStarted event
    │           ├─ Await _repair.ExecuteAsync(action)
    │           │       ↓
    │           │   RepairService switches on ActionType
    │           │       ├─ Automatic: action.ExecuteAsync()
    │           │       ├─ CommandLine: ProcessHelper.RunCommandAsync()
    │           │       ├─ SystemTool: Process.Start()
    │           │       ├─ OpenSettings: Process.Start("ms-settings:...")
    │           │       ├─ ServiceRestart: net start/stop
    │           │       └─ RegistryFix: RegistryHelper.SetValue()
    │           │
    │           └─ Fire RepairCompleted event
    │                   ↓
    │               RepairPage receives → updates dialog
    │               MainWindow receives → shows Toast notification
```

---

## 6. Threading Model

### 6.1 Thread Considerations

| Operation | Thread |
|-----------|--------|
| UI Rendering | UI Thread (Main) |
| User Input Handling | UI Thread |
| ObservableCollection Updates | UI Thread (via DispatcherQueue) |
| WMI Queries | Background Thread (via Task.Run) |
| PowerShell Execution | Background Thread (async Process) |
| File I/O | Background Thread (async File operations) |
| HTTP Requests (Ping, DNS) | Background Thread (async) |

### 6.2 UI Thread Marshalling

```csharp
// In event handler triggered from background thread
_engine.OnResultFound += result =>
{
    DispatcherQueue.TryEnqueue(() =>
    {
        AllResults.Add(result); // Safe UI update
    });
};
```

**Rules:**
- ❌ Never update `ObservableCollection` or UI properties from background thread directly
- ✅ Always use `DispatcherQueue.TryEnqueue()` for UI updates from background
- ✅ Use `ConfigureAwait(true)` (default) untuk maintain UI context

---

## 7. Error Handling Strategy

### 7.1 Layered Error Handling

```
Domain Service (per-check try-catch)
    ├─ Catch → Return DiagnosticResult with Status=Failed
    └─ Never throw to caller
        ↓
Diagnostic Engine (per-category try-catch)
    ├─ Catch → Add error DiagnosticResult
    └─ Continue to next category (isolation)
        ↓
ViewModel (per-operation try-catch)
    ├─ Catch → Update StatusMessage
    ├─ Fire ScanFailed event
    └─ UI shows error toast
        ↓
UI Layer (dialog error display)
    └─ Show user-friendly error message
```

### 7.2 Cancellation Support

```csharp
private CancellationTokenSource? _cts;

async Task RunScanAsync()
{
    _cts = new CancellationTokenSource();
    try
    {
        await _engine.RunFullDiagnosticAsync(_cts.Token);
    }
    catch (OperationCanceledException)
    {
        // Handle cancellation gracefully
    }
}

void CancelScan() => _cts?.Cancel();
```

---

## 8. Extension Points

### 8.1 Adding New Diagnostic Category

**Steps:**

1. **Create Service** di `Services/`:
   ```csharp
   public class MyNewCategoryService
   {
       public async Task<List<DiagnosticResult>> RunDiagnosticsAsync()
       {
           // Implementation
       }
   }
   ```

2. **Add Category Enum** di `Models/DiagnosticCategory.cs`:
   ```csharp
   public enum DiagnosticCategory
   {
       // ... existing categories
       MyNewCategory
   }
   ```

3. **Register in Engine** di `Services/DiagnosticEngine.cs`:
   ```csharp
   private readonly MyNewCategoryService _myNew = new();
   
   var cats = new (string Name, Func<Task<...>> Fn)[]
   {
       // ... existing
       ("My New Category", _myNew.RunDiagnosticsAsync),
   };
   ```

4. **Update Category Converter** di `Converters/GlyphConverters.cs`:
   ```csharp
   public class CategoryToGlyphConverter : IValueConverter
   {
       // Add case for MyNewCategory
   }
   ```

5. **Update ScanProgressDialog** untuk display new category row.

### 8.2 Adding New Repair Action Type

**Steps:**

1. Add value ke `RepairActionType` enum di `Models/RepairAction.cs`
2. Update switch case di `RepairService.ExecuteAsync()`
3. Add UI representation di dialogs jika perlu

---

## 9. Performance Characteristics

### 9.1 Scan Performance

| Category | Typical Duration | Bottleneck |
|----------|-----------------|------------|
| Performance | 2-3s | PerformanceCounter sampling (1s delay) |
| Disk Health | 1-2s | WMI query + Directory enumeration |
| Network | 3-8s | Ping timeout (up to 5s), DNS resolution |
| Security | 1-3s | WMI query + ServiceController |
| Windows Update | 3-5s | PowerShell startup + Get-HotFix |
| Drivers | 5-20s | PowerShell Get-WindowsDriver (slow) |
| Startup | <1s | Registry read (fast) |
| **Total** | **20-45s** | Sequential execution |

### 9.2 Optimization Opportunities

- ⚡ **Parallel Category Execution** (planned v2.2.0)
- ⚡ **Caching** untuk static data (system info)
- ⚡ **Incremental Scanning** (only re-check changed items)

---

## 10. Security Considerations

### 10.1 Trust Boundaries

```
User Input → UI → ViewModel (safe)
                ↓
        Repair Actions → Confirmation Dialog → RepairService
                                                    ↓
                            Infrastructure Layer → Windows OS
                                (⚠️ Trust boundary crossed)
```

### 10.2 Security Controls

- ✅ **UAC Elevation** required via app.manifest
- ✅ **User Confirmation** for all destructive operations
- ✅ **Hardcoded Command Arguments** (no user input to shell)
- ✅ **PowerShell Safe Flags** (`-NoProfile -NonInteractive`)
- ✅ **Process Timeout** enforced (120s default)
- ✅ **Registry Whitelist** (only specific keys modified)

---

## 11. Testing Strategy

### 11.1 Current State (v2.0.0)

- ❌ **No Unit Tests** (planned v2.1.0)
- ✅ **Manual Testing** performed by developer
- ✅ **Build Validation** (0 warnings, 0 errors)

### 11.2 Planned Testing (v2.1.0+)

- **Unit Tests** untuk Domain Services (mock Infrastructure)
- **Integration Tests** untuk DiagnosticEngine (test real WMI/Registry)
- **UI Tests** dengan WinAppDriver (E2E scenarios)

---

## 12. Deployment Architecture

### 12.1 Distribution Format

**Current (v2.0.0):**
- **Self-Contained EXE** dengan .NET 8 runtime bundled
- Ukuran: ~150 MB (compressed) / ~250 MB (extracted)
- Portable (no installer)

**Future (v3.0.0+):**
- **MSIX Package** untuk Microsoft Store
- **Code Signed** untuk hilangkan SmartScreen warning

### 12.2 Runtime Requirements

- Windows 10 build 17763+ atau Windows 11
- x64 architecture
- ~200 MB free disk space
- Administrator privileges (recommended)

---

## 13. Future Architecture Evolution

### 13.1 v2.1.0 Planned Changes

- **Scan History Persistence** dengan SQLite
- **Settings Persistence** dengan ApplicationData.LocalSettings
- **Dark Mode Audit** untuk visual consistency

### 13.2 v2.2.0 Planned Changes

- **Dependency Injection** (Microsoft.Extensions.DependencyInjection)
- **Parallel Scan Execution** (Task.WhenAll for independent categories)
- **Unit Test Coverage** (xUnit + Moq)

### 13.3 v3.0.0 Aspirational

- **AI/ML Integration** untuk smart recommendations
- **Multi-Language Support** (i18n framework)
- **Plugin Architecture** untuk custom diagnostics

---

## 14. Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial architecture documentation | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Architecture Documentation for WindowsDoctorAI v2.0.0**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

Licensed under the MIT License.

</div>