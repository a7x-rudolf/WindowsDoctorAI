# 🐛 Debugging Guide

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Panduan komprehensif untuk debugging WindowsDoctorAI menggunakan Visual Studio 2022. Mencakup teknik-teknik untuk debug UI issues, business logic, dan integrasi Windows system APIs.

---

## 🎯 Debugging Fundamentals

### 1.1 Debug Configuration

Selalu debug dengan configuration **Debug** (bukan Release):

- **Solution Configuration:** `Debug`
- **Platform:** `x64`
- **Optimizations:** Disabled
- **Symbols:** Full PDB generation

### 1.2 Just My Code

**Enable** untuk fokus pada kode Anda saja:
```
Tools → Options → Debugging → General → ✅ Enable Just My Code
```

**Disable** kalau perlu step into framework code atau third-party libraries.

---

## 🔴 Breakpoints

### 2.1 Basic Breakpoints

**Setting:**
- Click di left margin baris kode (F9)
- Red circle appears

**Removing:**
- Click red circle lagi
- Or `Debug → Delete All Breakpoints`

### 2.2 Conditional Breakpoints

Break hanya ketika kondisi tertentu terpenuhi.

**Setup:**
1. Set normal breakpoint
2. Right-click breakpoint → **Conditions...**
3. Add expression:

```csharp
// Break only if severity is Critical
result.Severity == Severity.Critical

// Break only after 10 iterations
i > 10

// Break for specific user
Environment.UserName == "TestUser"
```

**Use Cases:**
- Debug specific data conditions
- Skip early iterations di loops
- Focus pada edge cases

### 2.3 Hit Count Breakpoints

Break setelah hit N kali.

**Setup:**
1. Right-click breakpoint → **Conditions...**
2. Check **Hit Count**
3. Choose: "equal to", "greater than or equal to", "multiple of"

**Example:** Break di iteration ke-100 untuk debug memory leak.

### 2.4 Function Breakpoints

Break ketika any method dipanggil, tanpa perlu open file.

**Setup:**
```
Debug → New Breakpoint → Function Breakpoint (Ctrl+K, Ctrl+B)
Function Name: WindowsDoctorAI.Services.DiagnosticEngine.RunFullDiagnosticAsync
```

### 2.5 Tracepoints (Log without stopping)

Log messages ke Output window tanpa breaking execution.

**Setup:**
1. Right-click breakpoint → **Actions...**
2. Check **Log a message to Output Window**
3. Enter message:

```
Category started: {categoryName}, Time: {DateTime.Now}
```

**Use Cases:**
- Debug production-like scenarios
- Trace execution flow tanpa disrupt UI
- Log values dari hot paths

---

## 🔍 Debugging Windows

### 3.1 Locals Window

**View → Windows → Locals** (or Alt+4)

Menampilkan semua variables di current scope.

**Tips:**
- **Expand objects** dengan click arrow (▶)
- **Modify values** dengan click value dan edit
- **Pin variables** dengan pin icon untuk keep visible

### 3.2 Watch Windows

**View → Windows → Watch → Watch 1** (or Ctrl+Alt+W, 1)

Monitor specific expressions selama debugging.

**Adding Watch:**
- Type expression di empty row
- Or right-click variable → **Add Watch**

**Example Watches:**
```csharp
results.Count
results.Where(r => r.Severity == Severity.Critical).Count()
DateTime.Now.Subtract(scanStartTime).TotalSeconds
ViewModel.AllResults.Count
```

### 3.3 Call Stack

**View → Windows → Call Stack** (or Ctrl+Alt+C)

Menampilkan urutan method calls yang led ke current position.

**Actions:**
- **Double-click frame** untuk navigate ke method
- **Right-click → Show External Code** untuk see framework calls

### 3.4 Immediate Window

**View → Windows → Immediate** (or Ctrl+Alt+I)

Execute C# code langsung saat debugging.

**Examples:**
```csharp
// Query current state
> ViewModel.OverallScore
75

// Modify variables
> _isTest = true

// Call methods
> await _engine.RunFullDiagnosticAsync()

// LINQ queries
> ViewModel.AllResults.Count(r => r.Severity == Severity.Critical)
2
```

### 3.5 Autos Window

**View → Windows → Autos** (or Ctrl+Alt+V, A)

Menampilkan variables yang **used** di current dan previous statements. Berguna untuk quick inspection.

### 3.6 Diagnostic Tools

**View → Other Windows → Diagnostic Tools** (or Ctrl+Alt+F2)

Real-time monitoring:
- **CPU Usage**
- **Memory Usage** (private bytes)
- **Events** (breakpoints, exceptions)

**Use Cases:**
- Detect memory leaks
- Identify CPU bottlenecks
- Track allocation patterns

---

## ⚠️ Exception Handling

### 4.1 Exception Settings

**Debug → Windows → Exception Settings** (or Ctrl+Alt+E)

**Enable "Break When Thrown"** untuk specific exceptions:

- ✅ **Common Language Runtime Exceptions** — Semua .NET exceptions
- Or specific: `System.NullReferenceException`, `System.InvalidCastException`

**Use Case:** Debug hidden exceptions yang di-catch dan swallowed.

### 4.2 First-Chance Exceptions

Exception thrown tapi belum caught. Sering menandakan bugs meskipun app tidak crash.

**Investigation:**
1. Enable break di Exception Settings
2. Run app, reproduce issue
3. Debugger stops saat exception thrown
4. Inspect call stack dan variables
5. Determine kalau exception expected atau bug

### 4.3 Unhandled Exceptions

App akan crash. Debugger otomatis break kalau attached.

**Actions:**
1. Inspect exception details di Exception Helper popup
2. Copy stack trace untuk analysis
3. Check inner exceptions dengan `ex.InnerException`
4. Investigate root cause

---

## 🎨 XAML & UI Debugging

### 5.1 Live Visual Tree

**Debug → Windows → Live Visual Tree** (saat app running)

Menampilkan visual tree hierarchy dari UI saat runtime.

**Features:**
- **Select element di window** dengan crosshair tool
- **Highlight** element di code
- **Inspect properties** di real-time

### 5.2 Live Property Explorer

**Debug → Windows → Live Property Explorer**

Modify properties element saat runtime tanpa restart.

**Use Case:**
- Adjust margins/padding secara live
- Test warna berbeda
- Debug layout issues

### 5.3 XAML Hot Reload

Modify XAML saat app running, changes langsung reflected.

**How to Use:**
1. Run app dengan F5 (Debug mode)
2. Edit XAML file
3. Click **Hot Reload** button (🔥) di toolbar atau Alt+F10
4. Changes appear di running app

**Limitations:**
- Structural changes (adding new elements) may require restart
- Code-behind changes tidak affected

### 5.4 Layout Debugging

**Common Issues:**

**Issue: Element tidak muncul**

Check:
```csharp
// In debug console:
? element.Visibility  // Should be Visible
? element.Opacity     // Should be 1.0
? element.ActualWidth // Should be > 0
? element.ActualHeight // Should be > 0
```

**Issue: Layout terpotong**

Check:
- `MaxWidth` / `MaxHeight` constraints
- Parent `ClipToBounds` settings
- Grid row/column definitions

**Issue: Binding tidak update**

Check Output window untuk binding errors:
```
System.Windows.Data Error: 40 : BindingExpression path error
```

Common fixes:
- Verify property implements `INotifyPropertyChanged`
- Use `Mode=OneWay` atau `Mode=TwoWay` sesuai kebutuhan
- Check DataContext propagation

---

## 🔧 Debugging Specific Scenarios

### 6.1 Debug Async Code

**Problem:** Breakpoints tidak hit di async continuation.

**Solutions:**

**Enable Async Stacks:**
```
Tools → Options → Debugging → General
✅ Enable async .NET framework stepping
```

**Use Parallel Stacks Window:**
```
Debug → Windows → Parallel Stacks (Ctrl+Shift+D, S)
```

Menampilkan semua running tasks dan their stacks.

**Common Async Issues:**

**Issue: Deadlock di UI**
```csharp
// ❌ Bad (blocks UI thread)
var result = _service.GetDataAsync().Result;

// ✅ Good (proper async)
var result = await _service.GetDataAsync();
```

**Issue: Task not completing**
- Check kalau there's `await` di semua async path
- Verify cancellation token tidak stuck

### 6.2 Debug WMI Queries

WMI errors sulit di-debug karena non-descriptive messages.

**Debugging Tips:**

**Test WMI Query Standalone:**
```powershell
# Run in PowerShell as admin
Get-WmiObject -Query "SELECT * FROM Win32_DiskDrive"
```

Kalau PowerShell works tapi WindowsDoctorAI fails, issue di code.

**Enable WMI Tracing:**
```powershell
# Enable WMI trace
wevtutil sl Microsoft-Windows-WMI-Activity/Trace /e:true

# Reproduce issue

# View logs
Get-WinEvent -LogName Microsoft-Windows-WMI-Activity/Trace | Select-Object -First 20
```

**Common WMI Issues:**
- **Access Denied** → Not running as admin
- **Invalid Class** → WMI class doesn't exist di Windows version
- **Timeout** → Query terlalu complex atau WMI service issue

### 6.3 Debug PowerShell Execution

WindowsDoctorAI runs PowerShell via `ProcessHelper.RunPowerShellAsync()`.

**Debugging Steps:**

**Step 1: Log actual command**
```csharp
public static async Task<string> RunPowerShellAsync(string script, int timeoutMs = 120000)
{
    // TEMPORARY: Log command untuk debugging
    Debug.WriteLine($"[PS] Executing: {script}");
    
    var result = await RunCommandAsync("powershell.exe",
        $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"{script}\"",
        timeoutMs);
    
    Debug.WriteLine($"[PS] Output: {result.Output}");
    Debug.WriteLine($"[PS] Error: {result.Error}");
    
    return result.Output;
}
```

**Step 2: Test command manual**
Copy command dari debug output, run di PowerShell dengan `-NoProfile -NonInteractive`:

```powershell
powershell.exe -NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "Get-HotFix | Sort-Object InstalledOn -Descending | Select-Object -First 1"
```

Kalau berhasil di PowerShell tapi fail di app → issue di argument escaping.

**Common PowerShell Issues:**
- **Quote escaping** — Nested quotes di command string
- **Encoding** — Non-ASCII characters
- **Timeout** — Long-running cmdlets

### 6.4 Debug Registry Operations

**Verify Registry Value:**

**Via Registry Editor:**
```
Win+R → regedit → Navigate ke key
```

**Via PowerShell:**
```powershell
Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" -Name "EnableLUA"
```

**Common Registry Issues:**
- **Access Denied** → Not admin atau protected key
- **Value Not Found** → Wrong key path
- **Wrong Type** → REG_DWORD vs REG_SZ mismatch

### 6.5 Debug Multi-Threading

**Show Threads Window:**
```
Debug → Windows → Threads (Ctrl+Alt+H)
```

**Switch between threads** dengan double-click.

**Debugging Deadlocks:**

1. **Pause execution** saat app hang (Debug → Break All)
2. **Threads window** menampilkan semua threads dan states
3. **Look for threads di "Wait"** state
4. **Check Call Stack** each thread
5. **Identify circular wait** patterns

**Debugging Race Conditions:**

Use **Concurrency Visualizer** (extension):
```
Analyze → Concurrency Visualizer
```

Menampilkan timeline execution semua threads.

---

## 📊 Performance Debugging

### 7.1 Performance Profiler

**Debug → Performance Profiler** (Alt+F2)

**Available Tools:**

- **CPU Usage** — Identify hot methods
- **Memory Usage** — Find allocations dan leaks
- **.NET Object Allocation Tracking** — GC pressure analysis

### 7.2 CPU Profiling

**Steps:**

1. Debug → Performance Profiler
2. Check ☑ **CPU Usage**
3. Click **Start**
4. Perform actions di app
5. Click **Stop collection**
6. Analyze report:
   - **Hot Path** — Methods yang consume most CPU
   - **Function Call Tree** — Detailed breakdown

**Look For:**
- Methods dengan > 10% CPU time
- Unexpected recursion
- Blocking I/O di UI thread

### 7.3 Memory Profiling

**Detecting Memory Leaks:**

1. Take initial snapshot (before action)
2. Perform action yang suspect leak
3. Take snapshot ke-2
4. Perform action lagi
5. Take snapshot ke-3
6. Compare snapshots:
   - **Diff Size** menandakan growth
   - **Diff Count** menandakan object accumulation

**Common Leak Patterns:**

**Event Handler Not Unsubscribed:**
```csharp
// ❌ Bad (leak)
public class MyViewModel
{
    public MyViewModel()
    {
        _service.SomeEvent += OnEvent; // Never unsubscribed
    }
}

// ✅ Good
public class MyViewModel : IDisposable
{
    public MyViewModel()
    {
        _service.SomeEvent += OnEvent;
    }
    
    public void Dispose()
    {
        _service.SomeEvent -= OnEvent;
    }
}
```

**Static Collections:**
```csharp
// ❌ Bad (grows forever)
private static List<DiagnosticResult> _allTimeResults = new();

// ✅ Good
private List<DiagnosticResult> _sessionResults = new();
```

---

## 🎬 Debugging Scenarios by Feature

### 8.1 Debug Scan Flow

**Set Breakpoints:**
1. `MainViewModel.RunScanAsync()` — Entry point
2. `DiagnosticEngine.RunFullDiagnosticAsync()` — Orchestration
3. Each service's `RunDiagnosticsAsync()` — Per-category
4. `MainViewModel.OverallScore` setter — Score calculation

**Watch Variables:**
- `_cts.Token.IsCancellationRequested`
- `AllResults.Count`
- `_engine.OnResultFound` (event subscribers)

**Verify Flow:**
1. Scan starts → `IsScanning = true`
2. Each category → `OnCategoryStarted` fires
3. Results found → `AllResults` grows
4. Score calculated → `OverallScore` updated
5. Scan complete → `IsScanning = false`

### 8.2 Debug Repair Execution

**Set Breakpoints:**
1. `MainViewModel.ExecuteRepairAsync()` — Entry
2. `RepairService.ExecuteAsync()` — Dispatch
3. Specific action delegate (`action.ExecuteAsync`)
4. `RepairAction.Status` setter — State changes

**Watch:**
- `action.ActionType`
- `action.Status` (Pending → InProgress → Completed/Failed)
- `action.ResultMessage`

### 8.3 Debug Dialog Interactions

**Common Issue:** Dialog tidak muncul atau crash.

**Debugging:**

1. Set breakpoint di dialog constructor
2. Verify `XamlRoot` set correctly:
   ```csharp
   var dialog = new MyDialog
   {
       XamlRoot = this.XamlRoot // Must not be null
   };
   ```

3. Verify tidak ada existing ContentDialog:
   ```
   Only one ContentDialog can be open at a time
   ```

4. Check DispatcherQueue:
   ```csharp
   DispatcherQueue.TryEnqueue(async () =>
   {
       await dialog.ShowAsync();
   });
   ```

---

## 🔬 Advanced Debugging Techniques

### 9.1 Symbol Loading

**Enable Microsoft Symbol Server:**
```
Tools → Options → Debugging → Symbols
✅ Microsoft Symbol Servers
Cache: C:\SymbolCache
```

Allows stepping into .NET framework source.

### 9.2 Source Server

**Enable Source Server:**
```
Tools → Options → Debugging → General
✅ Enable source server support
```

Downloads source code untuk external libraries saat step into.

### 9.3 IntelliTrace (VS Enterprise)

Records execution history untuk time-travel debugging.

**Note:** Only available di Visual Studio Enterprise edition.

### 9.4 Remote Debugging

Debug app running di another machine.

**Setup:**
1. Install **Remote Debugger** di target machine
2. Run `msvsmon.exe` on target
3. VS → Debug → Attach to Process
4. Enter target machine name
5. Select WindowsDoctorAI.exe

---

## 📋 Debugging Checklist

**Before starting debug session:**

- [ ] Build in Debug configuration
- [ ] Set relevant breakpoints
- [ ] Enable "Just My Code" untuk clarity
- [ ] Open needed debug windows (Locals, Watch, Call Stack)
- [ ] Configure exception settings jika debugging exceptions
- [ ] Clear Output window
- [ ] Save current state (git commit) sebelum destructive testing

**During debug session:**

- [ ] Take notes tentang findings
- [ ] Screenshot important states
- [ ] Copy stack traces untuk complex issues
- [ ] Test hypothesis dengan conditional breakpoints
- [ ] Verify fixes tidak break other features

**After debug session:**

- [ ] Remove temporary breakpoints
- [ ] Remove debug print statements
- [ ] Commit fixes dengan descriptive message
- [ ] Document root cause di code atau issue tracker

---

## 🎓 Debugging Tips & Tricks

### Tip 1: Use Debug.WriteLine

```csharp
using System.Diagnostics;

Debug.WriteLine($"[MyClass] State: {_state}, Count: {_count}");
```

Output ke Output window (Debug configuration only).

### Tip 2: Assert Statements

```csharp
Debug.Assert(result != null, "Result should not be null here");
```

Break jika condition false. Only in Debug builds.

### Tip 3: Debugger.Break()

Programmatic breakpoint:

```csharp
if (someUnexpectedCondition)
{
    Debugger.Break();
}
```

### Tip 4: DebuggerDisplay Attribute

Customize how objects displayed di debugger:

```csharp
[DebuggerDisplay("Result: {Title} - {Severity}")]
public class DiagnosticResult
{
    public string Title { get; set; }
    public Severity Severity { get; set; }
    // ...
}
```

### Tip 5: Use Object IDs

Right-click variable di debugger → **Make Object ID**

Track specific instance dengan ID (e.g., `$1`) even setelah scope changes.

---

## 📚 References

- [Visual Studio Debugger Documentation](https://learn.microsoft.com/en-us/visualstudio/debugger/)
- [.NET Debugging Tutorial](https://learn.microsoft.com/en-us/dotnet/core/tutorials/debugging-with-visual-studio)
- [WinUI 3 Debugging Guide](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial debugging guide | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Debugging Guide for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>