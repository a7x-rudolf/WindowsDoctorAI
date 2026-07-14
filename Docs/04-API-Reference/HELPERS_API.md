# 🔧 Helpers API Reference

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Reference untuk semua **infrastructure helpers** yang menyediakan abstraksi ke Windows system APIs.

---

## 📋 Helpers List

| Helper | Namespace | Purpose |
|--------|-----------|---------|
| `WmiHelper` | `WindowsDoctorAI.Helpers` | WMI (Windows Management Instrumentation) queries |
| `ProcessHelper` | `WindowsDoctorAI.Helpers` | Process execution (cmd, PowerShell) |
| `RegistryHelper` | `WindowsDoctorAI.Helpers` | Windows Registry read/write |
| `AdminHelper` | `WindowsDoctorAI.Helpers` | UAC elevation checks |
| `ToastService` | `WindowsDoctorAI.Helpers` | Toast notification system |
| `IconGlyphs` | `WindowsDoctorAI.Helpers` | Segoe Fluent Icons constants |

---

## 1. WmiHelper

**File:** `Helpers/WmiHelper.cs`

Static helper untuk WMI queries dengan simplified API.

### 1.1 Class Definition

```csharp
public static class WmiHelper
```

### 1.2 Methods

#### Query

Execute WMI query dan return semua results.

```csharp
public static List<Dictionary<string, object>> Query(
    string wmiQuery,
    string scope = @"\\.\root\cimv2")
```

**Parameters:**
- `wmiQuery` — WQL query string
- `scope` (optional) — WMI namespace (default: `\\.\root\cimv2`)

**Returns:**
- `List<Dictionary<string, object>>` — Each dictionary = one WMI object

**Example:**
```csharp
// Query all disk drives
var disks = WmiHelper.Query("SELECT Model, Status, Size FROM Win32_DiskDrive");

foreach (var disk in disks)
{
    string model = disk.GetValueOrDefault("Model", "Unknown").ToString() ?? "Unknown";
    string status = disk.GetValueOrDefault("Status", "Unknown").ToString() ?? "Unknown";
    long size = Convert.ToInt64(disk.GetValueOrDefault("Size", 0));
    
    Console.WriteLine($"{model}: {status} ({size / 1024 / 1024 / 1024} GB)");
}
```

**Common WMI Scopes:**
- `\\.\root\cimv2` — Standard system info (default)
- `\\.\root\SecurityCenter2` — Security products (AV, Firewall)
- `\\.\root\WMI` — Additional WMI classes

**Error Handling:**
- Errors di-log via `Debug.WriteLine`
- Returns empty list on failure (tidak throw exception)

---

#### GetStringValue

Convenience method untuk mendapatkan single string value dari WMI query.

```csharp
public static string GetStringValue(
    string wmiQuery,
    string property,
    string scope = @"\\.\root\cimv2")
```

**Parameters:**
- `wmiQuery` — WQL query string
- `property` — Property name yang ingin di-extract
- `scope` (optional) — WMI namespace

**Returns:**
- `string` — Value of the property (empty string kalau tidak ditemukan)

**Example:**
```csharp
string osName = WmiHelper.GetStringValue(
    "SELECT Caption FROM Win32_OperatingSystem", 
    "Caption");
// Output: "Microsoft Windows 11 Pro"

string cpuName = WmiHelper.GetStringValue(
    "SELECT Name FROM Win32_Processor",
    "Name");
// Output: "Intel(R) Core(TM) i7-10700K CPU @ 3.80GHz"
```

**Use Cases:**
- Quick single-value retrieval
- System information queries
- Configuration checks

---

## 2. ProcessHelper

**File:** `Helpers/ProcessHelper.cs`

Static helper untuk executing external processes (cmd, PowerShell) dengan async support dan timeout enforcement.

### 2.1 Class Definition

```csharp
public static class ProcessHelper
```

### 2.2 Methods

#### RunCommandAsync

Execute command line application dan capture output.

```csharp
public static async Task<(int ExitCode, string Output, string Error)> RunCommandAsync(
    string fileName,
    string arguments,
    int timeoutMs = 120000)
```

**Parameters:**
- `fileName` — Path atau nama executable
- `arguments` — Command arguments
- `timeoutMs` (optional) — Timeout dalam milliseconds (default: 120 seconds)

**Returns:**
- `Tuple`:
  - `ExitCode` — Process exit code (0 = success)
  - `Output` — Standard output text
  - `Error` — Standard error text

**Behavior:**
- Process dijalankan dengan `UseShellExecute = false`
- Output/Error di-capture async
- Pada timeout: process di-kill entire tree, returns `(-1, output, "Timeout")`
- Pada exception: returns `(-1, "", ex.Message)`

**Example:**
```csharp
// Simple command
var result = await ProcessHelper.RunCommandAsync("ipconfig", "/flushdns");
if (result.ExitCode == 0)
{
    Console.WriteLine("DNS flushed successfully");
    Console.WriteLine(result.Output);
}
else
{
    Console.WriteLine($"Failed: {result.Error}");
}

// With timeout
var result = await ProcessHelper.RunCommandAsync(
    "chkdsk", 
    "C: /f", 
    timeoutMs: 300000); // 5 minutes

// Complex command
var result = await ProcessHelper.RunCommandAsync(
    "netsh",
    "int ip reset");
```

**Common Commands Used in App:**
- `ipconfig /flushdns` — Clear DNS cache
- `ipconfig /release` — Release IP
- `ipconfig /renew` — Renew IP
- `netsh winsock reset` — Reset Winsock
- `netsh int ip reset` — Reset TCP/IP
- `net start <service>` — Start service
- `net stop <service>` — Stop service
- `sc config <service> start= auto` — Set service startup type
- `shutdown /r /t 300` — Schedule reboot

---

#### RunPowerShellAsync

Execute PowerShell script dengan safe defaults.

```csharp
public static async Task<string> RunPowerShellAsync(
    string script,
    int timeoutMs = 120000)
```

**Parameters:**
- `script` — PowerShell command atau script string
- `timeoutMs` (optional) — Timeout dalam milliseconds

**Returns:**
- `string` — Standard output

**PowerShell Flags Used:**
- `-NoProfile` — Skip PowerShell profile untuk consistency
- `-NonInteractive` — Tidak prompt user input
- `-ExecutionPolicy Bypass` — Allow script execution

**Example:**
```csharp
// Simple cmdlet
string version = await ProcessHelper.RunPowerShellAsync(
    "(Get-Host).Version");

// Complex query
string updateInfo = await ProcessHelper.RunPowerShellAsync(
    "(Get-HotFix | Sort-Object InstalledOn -Descending | Select-Object -First 1).InstalledOn.ToString('yyyy-MM-dd')");

// JSON output
string json = await ProcessHelper.RunPowerShellAsync(
    "Get-NetAdapter | ConvertTo-Json");

// Execute action
await ProcessHelper.RunPowerShellAsync(
    "Update-MpSignature");
```

**Common PowerShell Commands in App:**
- `Get-HotFix` — Windows updates history
- `Get-WindowsDriver -Online -All` — Driver enumeration
- `Get-Volume` — Volume information
- `Get-MpComputerStatus` — Windows Defender status
- `Update-MpSignature` — Update AV definitions
- `Get-NetAdapter | Set-DnsClientServerAddress` — DNS configuration
- `Get-PnpDevice` — PnP devices

**Security Considerations:**
- ⚠️ Script string di-embed di command line — hati-hati dengan quoting
- ⚠️ Tidak validate script content — hanya untuk trusted internal use
- ✅ Never pass user input directly ke script

---

## 3. RegistryHelper

**File:** `Helpers/RegistryHelper.cs`

Static helper untuk Windows Registry operations dengan safe error handling.

### 3.1 Class Definition

```csharp
public static class RegistryHelper
```

### 3.2 Methods

#### GetValue

Read value dari registry.

```csharp
public static object? GetValue(
    RegistryHive hive,
    string subKey,
    string? valueName)
```

**Parameters:**
- `hive` — Registry hive (LocalMachine, CurrentUser, dll)
- `subKey` — Subkey path
- `valueName` — Value name (null untuk check kalau key exists)

**Returns:**
- `object?` — Value (typed sesuai registry type), atau `null` kalau tidak ditemukan

**Example:**
```csharp
// Read DWORD value
var uacEnabled = RegistryHelper.GetValue(
    RegistryHive.LocalMachine,
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
    "EnableLUA");

if (uacEnabled != null && Convert.ToInt32(uacEnabled) == 1)
{
    Console.WriteLine("UAC is enabled");
}

// Check kalau key exists (valueName = null)
var pendingReboot = RegistryHelper.GetValue(
    RegistryHive.LocalMachine,
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired",
    null);

if (pendingReboot != null)
{
    Console.WriteLine("Reboot pending");
}
```

---

#### SetValue

Write value ke registry.

```csharp
public static bool SetValue(
    RegistryHive hive,
    string subKey,
    string valueName,
    object value,
    RegistryValueKind kind)
```

**Parameters:**
- `hive` — Registry hive
- `subKey` — Subkey path
- `valueName` — Value name
- `value` — Value data
- `kind` — Value type (DWord, String, Binary, dll)

**Returns:**
- `bool` — `true` jika successful

**Example:**
```csharp
// Enable UAC
bool success = RegistryHelper.SetValue(
    RegistryHive.LocalMachine,
    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System",
    "EnableLUA",
    1,
    RegistryValueKind.DWord);

if (success)
{
    Console.WriteLine("UAC enabled (reboot required)");
}
```

**⚠️ Warning:**
- Requires Administrator privileges
- Reboot mungkin diperlukan untuk take effect
- Use dengan hati-hati — invalid registry changes can break Windows

---

#### DeleteValue

Delete registry value.

```csharp
public static bool DeleteValue(
    RegistryHive hive,
    string subKey,
    string valueName)
```

**Parameters:**
- `hive` — Registry hive
- `subKey` — Subkey path
- `valueName` — Value name to delete

**Returns:**
- `bool` — `true` jika successful (atau value tidak exist)

**Example:**
```csharp
bool deleted = RegistryHelper.DeleteValue(
    RegistryHive.CurrentUser,
    @"Software\MyApp",
    "OldSetting");
```

---

#### GetStartupPrograms

Convenience method untuk enumerate semua startup programs.

```csharp
public static List<string> GetStartupPrograms()
```

**Returns:**
- `List<string>` — Formatted strings `[HKLM/HKCU] Name: Value`

**Registry Locations Checked:**
- HKLM: `SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
- HKLM: `SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`
- HKLM: `SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run`
- HKCU: `SOFTWARE\Microsoft\Windows\CurrentVersion\Run`
- HKCU: `SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce`

**Example:**
```csharp
var startups = RegistryHelper.GetStartupPrograms();

Console.WriteLine($"Found {startups.Count} startup programs:");
foreach (var program in startups)
{
    Console.WriteLine($"  {program}");
}

// Output:
// [HKLM] OneDriveSetup: C:\Windows\SysWOW64\OneDriveSetup.exe /thfirstsetup
// [HKCU] Discord: C:\Users\User\AppData\Local\Discord\Update.exe --processStart Discord.exe
```

---

## 4. AdminHelper

**File:** `Helpers/AdminHelper.cs`

Static helper untuk UAC elevation checks dan restart-as-admin.

### 4.1 Class Definition

```csharp
public static class AdminHelper
```

### 4.2 Methods

#### IsRunningAsAdmin

Check apakah current process running dengan Administrator privileges.

```csharp
public static bool IsRunningAsAdmin()
```

**Returns:**
- `bool` — `true` jika running as admin

**Example:**
```csharp
if (!AdminHelper.IsRunningAsAdmin())
{
    Console.WriteLine("⚠️ Not running as administrator");
    Console.WriteLine("Some features will be limited");
}
else
{
    Console.WriteLine("✅ Running with elevated privileges");
}
```

**Implementation Detail:**
Uses `WindowsIdentity.GetCurrent()` dan `WindowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)`.

---

#### RestartAsAdmin

Restart current process dengan UAC elevation prompt.

```csharp
public static void RestartAsAdmin()
```

**Behavior:**
- Launches new process dengan `Verb = "runas"` (triggers UAC)
- Terminates current process dengan `Environment.Exit(0)`
- Kalau user cancels UAC prompt, current process continues (no exit)

**Example:**
```csharp
if (!AdminHelper.IsRunningAsAdmin())
{
    var dialog = new ContentDialog
    {
        Title = "Administrator Required",
        Content = "This action requires administrator privileges. Restart as admin?",
        PrimaryButtonText = "Restart",
        CloseButtonText = "Cancel"
    };
    
    if (await dialog.ShowAsync() == ContentDialogResult.Primary)
    {
        AdminHelper.RestartAsAdmin();
        // Process akan restart, code di bawah tidak execute
    }
}
```

---

## 5. ToastService

**File:** `Helpers/ToastService.cs`

Static service untuk in-app toast notifications.

### 5.1 Class Definition

```csharp
public static class ToastService
```

### 5.2 Enums

#### ToastType

```csharp
public enum ToastType
{
    Info,     // Blue accent
    Success,  // Green accent
    Warning,  // Orange accent
    Error     // Red accent
}
```

### 5.3 Methods

#### RegisterHost

Register root panel untuk toast display. **Must be called once at app startup.**

```csharp
public static void RegisterHost(Panel host)
```

**Parameters:**
- `host` — Panel container (typically MainWindow's root Grid)

**Example:**
```csharp
// In MainWindow constructor
public MainWindow()
{
    this.InitializeComponent();
    ToastService.RegisterHost(RootGrid);
    // ...
}
```

---

#### Show

Display toast notification.

```csharp
public static void Show(
    XamlRoot? xamlRoot,
    ToastType type,
    string title,
    string message,
    int durationMs = 3500)
```

**Parameters:**
- `xamlRoot` — Optional XamlRoot untuk positioning
- `type` — Toast severity type
- `title` — Toast title (bold text)
- `message` — Toast message body
- `durationMs` (optional) — Auto-dismiss duration (default: 3500ms)

**Example:**
```csharp
// Success toast
ToastService.Show(
    this.XamlRoot,
    ToastType.Success,
    "Repair Completed",
    "Windows Defender definitions updated successfully");

// Error toast
ToastService.Show(
    this.XamlRoot,
    ToastType.Error,
    "Scan Failed",
    "Unable to complete diagnostic scan. Please try again.");

// Custom duration
ToastService.Show(
    this.XamlRoot,
    ToastType.Warning,
    "Important",
    "System reboot recommended",
    durationMs: 5000);
```

**Toast Positioning:**
- Top-right corner of registered host panel
- Multiple toasts stack vertically
- Auto-dismiss dengan timer
- Manual close via X button

---

## 6. IconGlyphs

**File:** `Helpers/IconGlyphs.cs`

Static constants untuk Segoe Fluent Icons Unicode characters.

### 6.1 Class Definition

```csharp
public static class IconGlyphs
```

### 6.2 Constants

#### Navigation Icons
```csharp
public const string Home = "\uE80F";
public const string Search = "\uE773";
public const string List = "\uE9F9";
public const string Repair = "\uE90F";
public const string History = "\uE81C";
public const string SystemInfo = "\uE770";
public const string Settings = "\uE713";
public const string Info = "\uE946";
```

#### App
```csharp
public const string AppLogo = "\uE95E";  // Health icon
```

#### Status Icons
```csharp
public const string CheckMark = "\uE73E";
public const string Warning = "\uE7BA";
public const string Critical = "\uEA39";
public const string Cancel = "\uE711";
public const string Close = "\uE8BB";
```

#### Category Icons
```csharp
public const string Disk = "\uEDA2";
public const string Performance = "\uE945";
public const string Network = "\uE968";
public const string Security = "\uE72E";
public const string Update = "\uE895";
public const string Startup = "\uE7C1";
```

#### Action Icons
```csharp
public const string Play = "\uE768";
public const string OpenNew = "\uE8A7";
public const string Export = "\uE8A5";
public const string Refresh = "\uE777";
public const string Filter = "\uE71C";
public const string Clean = "\uE74D";
```

#### Meta Icons
```csharp
public const string Time = "\uE121";
public const string Timer = "\uE916";
public const string Admin = "\uE7EF";
public const string TrendUp = "\uE96D";
public const string TrendDown = "\uE96E";
public const string Empty = "\uE712";
```

### 6.3 Usage

**In XAML:**
```xml
<FontIcon Glyph="&#xE73E;" 
          FontFamily="{StaticResource SymbolThemeFontFamily}" />
```

**In C#:**
```csharp
var icon = new FontIcon
{
    Glyph = IconGlyphs.CheckMark,
    FontSize = 16
};

// Or use in converters
public class SeverityToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        return value switch
        {
            Severity.Healthy => IconGlyphs.CheckMark,
            Severity.Warning => IconGlyphs.Warning,
            Severity.Critical => IconGlyphs.Critical,
            _ => IconGlyphs.Info
        };
    }
}
```

**Reference:** [Segoe Fluent Icons Font](https://learn.microsoft.com/en-us/windows/apps/design/style/segoe-fluent-icons-font)

---

## 7. Usage Best Practices

### 7.1 Error Handling

Semua helpers menggunakan **safe defaults** dan tidak throw untuk expected errors:

```csharp
// ✅ Good pattern
var results = WmiHelper.Query("SELECT * FROM Win32_DiskDrive");
if (results.Count == 0)
{
    // Handle empty gracefully
    return CreateEmptyResult();
}

// ❌ Anti-pattern
try
{
    var results = WmiHelper.Query("...");  // Doesn't throw
}
catch { }  // Unnecessary
```

### 7.2 Async Usage

`ProcessHelper` methods async — always await:

```csharp
// ✅ Good
var result = await ProcessHelper.RunCommandAsync("ipconfig", "/flushdns");

// ❌ Bad (fire and forget, no error handling)
_ = ProcessHelper.RunCommandAsync("ipconfig", "/flushdns");
```

### 7.3 Registry Operations

Always check return value untuk critical operations:

```csharp
// ✅ Good
if (!RegistryHelper.SetValue(hive, key, "EnableLUA", 1, RegistryValueKind.DWord))
{
    // Handle failure
    ShowError("Failed to update registry");
}

// ❌ Bad (ignore result)
RegistryHelper.SetValue(hive, key, "EnableLUA", 1, RegistryValueKind.DWord);
```

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial helpers API reference | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Helpers API Reference for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>