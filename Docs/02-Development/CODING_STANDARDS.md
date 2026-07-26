# 📏 Coding Standards & Best Practices

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Dokumen ini mendefinisikan **coding standards dan best practices** untuk pengembangan WindowsDoctorAI. Konsistensi kode adalah kunci untuk maintainability dan readability jangka panjang.

**Prinsip Utama:**
- **Consistency over preference** — Ikuti standar meskipun beda dengan preferensi pribadi
- **Readability first** — Kode dibaca 10x lebih sering dari ditulis
- **Explicit over implicit** — Jelas lebih baik dari cerdas

---

## 1. General C# Standards

### 1.1 Naming Conventions

Follow Microsoft's [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

| Element | Convention | Example |
|---------|-----------|---------|
| **Class** | PascalCase | `DiagnosticEngine` |
| **Interface** | IPascalCase | `IDiagnosticService` |
| **Method** | PascalCase | `RunFullDiagnosticAsync()` |
| **Property** | PascalCase | `OverallScore` |
| **Field (private)** | _camelCase | `_engine` |
| **Field (const)** | PascalCase | `MaxRetryCount` |
| **Local variable** | camelCase | `resultCount` |
| **Parameter** | camelCase | `cancellationToken` |
| **Enum** | PascalCase | `Severity.Critical` |
| **Namespace** | PascalCase.Dot.Notation | `WindowsDoctorAI.Services` |

### 1.2 File Organization

**Rule:** One class per file, filename matches class name.

```csharp
// ✅ Good: File "DiagnosticResult.cs" berisi class DiagnosticResult
namespace WindowsDoctorAI.Models;

public class DiagnosticResult
{
    // ...
}
```

**Exception:** Nested classes, enums yang sangat terkait boleh di file yang sama.

### 1.3 Using Directives

**Order:**
1. `System.*` namespaces
2. `Microsoft.*` namespaces
3. Third-party namespaces
4. Project namespaces (`WindowsDoctorAI.*`)

**Style:** Alphabetical dalam group.

```csharp
// ✅ Good
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using WindowsDoctorAI.Helpers;
using WindowsDoctorAI.Models;
```

### 1.4 File-Scoped Namespaces

**Rule:** Gunakan file-scoped namespaces (C# 10+ syntax).

```csharp
// ✅ Good (file-scoped)
namespace WindowsDoctorAI.Services;

public class DiagnosticEngine
{
    // ...
}

// ❌ Bad (block-scoped)
namespace WindowsDoctorAI.Services
{
    public class DiagnosticEngine
    {
        // ...
    }
}
```

### 1.5 Nullable Reference Types

**Rule:** Enable dan gunakan nullable annotations.

```csharp
// Di WindowsDoctorAI.csproj:
<Nullable>enable</Nullable>

// ✅ Good
public class ExampleClass
{
    private readonly string _requiredField;      // Non-nullable
    private string? _optionalField;              // Nullable
    
    public ExampleClass(string required)
    {
        _requiredField = required ?? throw new ArgumentNullException(nameof(required));
    }
    
    public string? TryGetValue() => _optionalField;
}
```

---

## 2. Formatting & Style

### 2.1 Indentation

- **4 spaces** (bukan tabs)
- **No trailing whitespace**
- **LF line endings** (Unix style)

### 2.2 Braces

**Style:** Allman (opening brace on new line).

```csharp
// ✅ Good
public class MyClass
{
    public void MyMethod()
    {
        if (condition)
        {
            DoSomething();
        }
    }
}

// ❌ Bad (K&R style)
public class MyClass {
    public void MyMethod() {
        if (condition) {
            DoSomething();
        }
    }
}
```

**Exception:** Single-line properties dan lambdas.

```csharp
// ✅ Good (single-line property)
public string Name { get; set; }

// ✅ Good (expression-bodied member)
public string FullName => $"{FirstName} {LastName}";

// ✅ Good (short lambda)
list.Where(x => x.IsActive);
```

### 2.3 Line Length

- **Max 120 characters** per line (soft limit)
- Break long lines untuk readability

```csharp
// ✅ Good (readable multi-line)
var result = await _diagnosticEngine
    .RunFullDiagnosticAsync(cancellationToken)
    .ConfigureAwait(true);

// ❌ Bad (single very long line)
var result = await _diagnosticEngine.RunFullDiagnosticAsync(cancellationToken).ConfigureAwait(true);
```

### 2.4 Blank Lines

- **1 blank line** between methods
- **1 blank line** between logical groups within methods
- **2 blank lines** between class definitions (rare)
- **No blank line** at start/end of class

---

## 3. Language Features

### 3.1 var vs Explicit Type

**Rule:** Gunakan `var` ketika tipe jelas dari context, explicit ketika ambigu.

```csharp
// ✅ Good (type obvious)
var results = new List<DiagnosticResult>();
var count = results.Count;
var user = GetCurrentUser();

// ✅ Good (explicit for clarity)
IDiagnosticService service = new DiskHealthService();
int severity = 0;

// ❌ Bad (ambiguous)
var x = ProcessData(); // What's the type?
```

### 3.2 String Interpolation

**Rule:** Gunakan `$"..."` untuk string concatenation.

```csharp
// ✅ Good
var message = $"Found {count} issues in {category}";

// ❌ Bad
var message = "Found " + count + " issues in " + category;
var message = string.Format("Found {0} issues in {1}", count, category);
```

### 3.3 Expression-Bodied Members

Gunakan untuk simple members.

```csharp
// ✅ Good
public class Example
{
    public string Name => "Test";
    public int Count => _list.Count;
    public bool IsValid() => _value > 0;
    public override string ToString() => $"Example({Name})";
}
```

### 3.4 Pattern Matching

Gunakan modern pattern matching untuk clarity.

```csharp
// ✅ Good (switch expression)
var color = severity switch
{
    Severity.Critical => Colors.Red,
    Severity.Warning => Colors.Orange,
    Severity.Healthy => Colors.Green,
    _ => Colors.Gray
};

// ❌ Bad (traditional switch)
Color color;
switch (severity)
{
    case Severity.Critical:
        color = Colors.Red;
        break;
    // ...
}

// ✅ Good (property pattern)
if (result is { Severity: Severity.Critical, Score: < 20 })
{
    HandleCriticalIssue(result);
}
```

### 3.5 Null Handling

```csharp
// ✅ Good (null coalescing)
var name = user?.Name ?? "Unknown";

// ✅ Good (null conditional invocation)
_engine?.OnCategoryStarted?.Invoke(categoryName);

// ✅ Good (throw expression)
public void SetValue(string value)
{
    _value = value ?? throw new ArgumentNullException(nameof(value));
}

// ✅ Good (is not null)
if (result is not null)
{
    Process(result);
}
```

### 3.6 Async/Await

**Rules:**
1. Selalu suffix async methods dengan `Async`
2. Selalu return `Task` atau `Task<T>`, never `void` (except event handlers)
3. Gunakan `ConfigureAwait(true)` di UI code (default)
4. Handle `OperationCanceledException` gracefully

```csharp
// ✅ Good
public async Task<List<DiagnosticResult>> RunScanAsync(CancellationToken ct = default)
{
    try
    {
        var result = await _service.ExecuteAsync(ct);
        return result;
    }
    catch (OperationCanceledException)
    {
        // Handle cancellation
        return new List<DiagnosticResult>();
    }
}

// ❌ Bad (async void, no cancellation)
public async void RunScan()
{
    var result = await _service.ExecuteAsync();
}
```

---

## 4. XAML Standards

### 4.1 Attribute Ordering

**Order per element:**
1. `x:Name` / `x:Key`
2. `x:Class`
3. `Grid.Row` / `Grid.Column`
4. Layout properties (Width, Height, Margin, Padding)
5. Alignment (Horizontal/VerticalAlignment)
6. Appearance (Background, Foreground, BorderBrush, CornerRadius)
7. Content (Text, Content)
8. Behavior (IsEnabled, Visibility)
9. Events (Click, SelectionChanged)

```xml
<!-- ✅ Good -->
<Button x:Name="ExecuteButton"
        Grid.Column="2"
        Width="100" Height="32"
        Margin="10,0"
        HorizontalAlignment="Right"
        Background="{StaticResource AccentBrush}"
        Content="Execute"
        IsEnabled="{x:Bind CanExecute, Mode=OneWay}"
        Click="Execute_Click" />
```

### 4.2 One Attribute Per Line

Untuk elements dengan > 3 attributes, satu per line.

```xml
<!-- ✅ Good -->
<Border Background="{ThemeResource LayerFillColorDefaultBrush}"
        BorderBrush="{ThemeResource CardStrokeColorDefaultBrush}"
        BorderThickness="1"
        CornerRadius="8"
        Padding="16"
        Margin="0,3">
    <!-- content -->
</Border>

<!-- ❌ Bad (single line, hard to read) -->
<Border Background="{ThemeResource LayerFillColorDefaultBrush}" BorderBrush="{ThemeResource CardStrokeColorDefaultBrush}" BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,3">
```

### 4.3 Binding Preferences

**Rule:** Prefer `{x:Bind}` over `{Binding}` untuk performance dan compile-time checking.

```xml
<!-- ✅ Good (x:Bind - compile-time checked, faster) -->
<TextBlock Text="{x:Bind ViewModel.Title, Mode=OneWay}" />

<!-- ⚠️ Acceptable (Binding - untuk DataTemplate scenarios) -->
<TextBlock Text="{Binding Name}" />
```

### 4.4 Resource References

```xml
<!-- Static resource (compile-time) -->
<Button Background="{StaticResource AccentBrush}" />

<!-- Theme resource (theme-aware, light/dark) -->
<Border Background="{ThemeResource LayerFillColorDefaultBrush}" />

<!-- ✅ Prefer ThemeResource untuk theme-aware colors -->
```

### 4.5 XAML File Organization

Struktur file XAML:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Page
    x:Class="WindowsDoctorAI.Views.MyPage"
    xmlns="..."
    xmlns:x="..."
    xmlns:local="..."
    xmlns:converters="..."
    NavigationCacheMode="Required">

    <!-- 1. Page Resources -->
    <Page.Resources>
        <converters:MyConverter x:Key="MyConverter" />
    </Page.Resources>

    <!-- 2. Root Layout -->
    <Grid Padding="32,24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <!-- 3. Content Sections -->
        <!-- Section 1: Header -->
        <StackPanel Grid.Row="0">
            <!-- ... -->
        </StackPanel>

        <!-- Section 2: Main Content -->
        <ScrollViewer Grid.Row="1">
            <!-- ... -->
        </ScrollViewer>
    </Grid>
</Page>
```

---

## 5. MVVM Patterns

### 5.1 ViewModel Structure

```csharp
public partial class MyViewModel : BaseViewModel
{
    // 1. Dependencies (constructor injection)
    private readonly IMyService _service;
    
    // 2. Private fields
    private CancellationTokenSource? _cts;
    
    // 3. Observable properties (public via source generator)
    [ObservableProperty]
    private bool _isBusy;
    
    [ObservableProperty]
    private ObservableCollection<MyItem> _items = new();
    
    // 4. Events (untuk view-viewmodel communication)
    public event Action? OperationCompleted;
    
    // 5. Constructor
    public MyViewModel(IMyService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    // 6. Commands (auto-generated via source generator)
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        IsBusy = true;
        try
        {
            var data = await _service.GetDataAsync();
            Items.Clear();
            foreach (var item in data) Items.Add(item);
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    // 7. Private helper methods
    private void HandleError(Exception ex)
    {
        // ...
    }
}
```

### 5.2 ObservableCollection Updates

**Rule:** Update dari UI thread only.

```csharp
// ✅ Good (from UI thread)
Items.Add(newItem);

// ✅ Good (from background thread with dispatcher)
DispatcherQueue.TryEnqueue(() =>
{
    Items.Add(newItem);
});

// ❌ Bad (from background thread directly)
await Task.Run(() =>
{
    Items.Add(newItem); // Will throw!
});
```

### 5.3 Command Naming

**Convention:** `[Action][Noun]Command`

```csharp
// ✅ Good
[RelayCommand]
private async Task RunScanAsync() { /* ... */ }
// Auto-generates: RunScanCommand

[RelayCommand]
private async Task ExportReportAsync() { /* ... */ }
// Auto-generates: ExportReportCommand
```

---

## 6. Error Handling

### 6.1 Exception Handling Rules

**Do:**
- ✅ Catch specific exceptions
- ✅ Log exceptions dengan context
- ✅ Provide fallback behavior
- ✅ Let exceptions bubble up jika tidak bisa handle

**Don't:**
- ❌ Catch dan swallow exceptions tanpa logging
- ❌ Catch `Exception` base class kecuali di top-level
- ❌ Throw exceptions untuk control flow

```csharp
// ✅ Good
try
{
    var result = await _service.QueryWmiAsync(query);
    return result;
}
catch (ManagementException ex)
{
    // Specific WMI exception
    _logger.LogError(ex, "WMI query failed: {Query}", query);
    return new List<DiagnosticResult>(); // Fallback
}
catch (UnauthorizedAccessException)
{
    // Expected untuk non-admin scenarios
    return CreateAdminRequiredResult();
}

// ❌ Bad
try
{
    // ...
}
catch (Exception ex)
{
    // Silent swallow
}
```

### 6.2 Guard Clauses

Fail fast dengan validation di method entry.

```csharp
// ✅ Good
public async Task<Result> ProcessAsync(Request request)
{
    ArgumentNullException.ThrowIfNull(request);
    
    if (string.IsNullOrEmpty(request.Data))
        throw new ArgumentException("Data cannot be empty", nameof(request));
    
    // Main logic
    return await DoProcessAsync(request);
}
```

---

## 7. Comments & Documentation

### 7.1 When to Comment

**Do comment:**
- ✅ **Why** (business logic reasoning)
- ✅ Complex algorithms
- ✅ Workarounds untuk bugs (dengan reference)
- ✅ Public API (XML docs)
- ✅ TODO / FIXME dengan context

**Don't comment:**
- ❌ **What** (kode sudah jelaskan)
- ❌ Obvious code
- ❌ Redundant with method name

```csharp
// ✅ Good comment (explains WHY)
// Windows caches DNS entries for 24 hours by default,
// so we force flush to ensure fresh resolution.
await ProcessHelper.RunCommandAsync("ipconfig", "/flushdns");

// ❌ Bad comment (redundant)
// Flush DNS
await ProcessHelper.RunCommandAsync("ipconfig", "/flushdns");
```

### 7.2 XML Documentation

Untuk public API, gunakan XML docs.

```csharp
/// <summary>
/// Executes a full system diagnostic scan across all registered categories.
/// </summary>
/// <param name="cancellationToken">Token untuk cancel operation.</param>
/// <returns>
/// Tuple berisi list hasil diagnostik dan aggregated health score.
/// </returns>
/// <exception cref="OperationCanceledException">
/// Thrown ketika scan di-cancel via cancellation token.
/// </exception>
/// <remarks>
/// Method ini menjalankan scan secara sequential per category.
/// Untuk parallel execution, planned di v2.2.0.
/// </remarks>
public async Task<(List<DiagnosticResult>, SystemHealthScore)> RunFullDiagnosticAsync(
    CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### 7.3 TODO Comments

**Format:** `// TODO: [Description] (Owner: XYZ, Target: vX.Y.Z)`

```csharp
// TODO: Add unit tests for edge cases (Owner: RIDOLF, Target: v2.1.0)
// FIXME: Race condition when multiple scans triggered rapidly (Issue #42)
// HACK: Workaround for WinUI 3 binding bug (see https://github.com/microsoft/microsoft-ui-xaml/issues/xxxx)
```

---

## 8. File & Folder Organization

### 8.1 Folder Structure

```
WindowsDoctorAI/
├── App.xaml + .cs                    # Application entry
├── MainWindow.xaml + .cs             # Main window
├── Models/                           # Data structures
│   ├── DiagnosticResult.cs
│   └── RepairAction.cs
├── ViewModels/                       # MVVM view models
│   ├── BaseViewModel.cs
│   └── MainViewModel.cs
├── Services/                         # Business logic
│   ├── DiagnosticEngine.cs
│   ├── RepairService.cs
│   └── [Category]Service.cs
├── Views/                            # Pages (UI)
│   └── [Name]Page.xaml + .cs
├── Dialogs/                          # Modal dialogs
│   └── [Name]Dialog.xaml + .cs
├── Converters/                       # Value converters
│   └── [Purpose]Converter.cs
├── Helpers/                          # Infrastructure
│   ├── WmiHelper.cs
│   └── ProcessHelper.cs
├── Styles/                           # Shared XAML styles
│   └── AppStyles.xaml
└── Assets/                           # Icons, images
```

### 8.2 File Naming

- **PascalCase** untuk C# dan XAML files
- **Suffix based on type:**
  - `[Name]Page.xaml` untuk pages
  - `[Name]Dialog.xaml` untuk dialogs
  - `[Name]Service.cs` untuk services
  - `[Name]Converter.cs` untuk converters
  - `[Name]Helper.cs` untuk helpers
  - `[Name]ViewModel.cs` untuk view models

---

## 9. Git Commit Standards

### 9.1 Commit Message Format

```
<type>: <subject>

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only
- `style`: Formatting, no code change
- `refactor`: Code restructuring
- `perf`: Performance improvement
- `test`: Adding tests
- `chore`: Maintenance tasks

**Examples:**

```
feat: Add scan history persistence with SQLite

Implements local database untuk menyimpan hasil scan sebelumnya.
Users bisa melihat trend health score over time.

Closes #42
```

```
fix: Prevent NullReferenceException in ScanProgressDialog

Fixed race condition ketika scan di-cancel sebelum dialog fully loaded.

Refs #58
```

### 9.2 Commit Best Practices

- ✅ **Atomic commits** — Satu commit = satu logical change
- ✅ **Descriptive messages** — Bukan "fix stuff"
- ✅ **Present tense** — "Add feature" not "Added feature"
- ✅ **50 chars** untuk subject line
- ✅ **72 chars** untuk body lines

---

## 10. Code Review Checklist

Sebelum submit PR (atau self-review):

### Functionality
- [ ] Code melakukan apa yang diintended
- [ ] Edge cases handled
- [ ] Error handling appropriate
- [ ] No hardcoded values yang seharusnya configurable

### Style
- [ ] Follows naming conventions
- [ ] Proper indentation dan formatting
- [ ] No commented-out code
- [ ] No debug statements (Console.WriteLine, dll)

### Performance
- [ ] No obvious performance issues
- [ ] Async/await used correctly
- [ ] Collections sized appropriately
- [ ] No memory leaks

### Testing
- [ ] Manual testing performed
- [ ] Build succeeds tanpa warnings
- [ ] Feature works di Debug dan Release

### Documentation
- [ ] Public API documented
- [ ] README updated jika needed
- [ ] CHANGELOG updated
- [ ] Complex logic explained dengan comments

---

## 📚 References

- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Framework Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [.NET Naming Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines)
- [XAML Style Guide (Microsoft)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml-services/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial coding standards | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Coding Standards for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>