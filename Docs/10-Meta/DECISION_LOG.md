# 📋 Architecture Decision Records (ADR)

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+
**Author:** RIDOLF WIDI ALFISA LUMBA

---

## 📌 Overview

Dokumen ini mencatat **keputusan arsitektur dan desain penting** yang diambil selama development WindowsDoctorAI, beserta konteks, alternatif yang dipertimbangkan, dan konsekuensi.

**Manfaat ADR (Architecture Decision Records):**
- 📚 **Historical Context** — Memahami mengapa keputusan tertentu diambil
- 🔄 **Onboarding** — New developers/maintainers cepat catch up
- 🤔 **Avoid Re-litigation** — Tidak perlu diskusi ulang decisions yang sudah dibuat
- 📈 **Learning** — Refleksi dari past decisions untuk future improvements

---

## 📋 ADR Format

Setiap ADR mengikuti template:

```markdown
## ADR-XXX: [Title]

**Date:** [Date]
**Status:** Proposed / Accepted / Deprecated / Superseded
**Deciders:** [Who made decision]

### Context
[Situasi yang memerlukan keputusan]

### Decision
[Apa yang diputuskan]

### Consequences
[Konsekuensi positif dan negatif]

### Alternatives Considered
[Opsi lain yang dipertimbangkan]
```

---

## 📖 Table of Contents

- [ADR-001: Adopt WinUI 3 over WPF](#adr-001-adopt-winui-3-over-wpf)
- [ADR-002: Use MVVM Pattern with CommunityToolkit.Mvvm](#adr-002-use-mvvm-pattern-with-communitytoolkitmvvm)
- [ADR-003: Sequential Scan Execution (not Parallel)](#adr-003-sequential-scan-execution-not-parallel)
- [ADR-004: Self-Contained Deployment](#adr-004-self-contained-deployment)
- [ADR-005: No Unit Tests in v2.0.0](#adr-005-no-unit-tests-in-v200)
- [ADR-006: Segoe Fluent Icons over Emoji](#adr-006-segoe-fluent-icons-over-emoji)
- [ADR-007: Static Helper Classes over DI](#adr-007-static-helper-classes-over-di)
- [ADR-008: Event-Driven Communication for Dialogs](#adr-008-event-driven-communication-for-dialogs)
- [ADR-009: Grid Layout for Results Display](#adr-009-grid-layout-for-results-display)
- [ADR-010: HTML Report Format](#adr-010-html-report-format)
- [ADR-011: Require Administrator Privileges](#adr-011-require-administrator-privileges)
- [ADR-012: MIT License](#adr-012-mit-license)

---

## ADR-001: Adopt WinUI 3 over WPF

**Date:** 2025 (Project Inception)
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Original WindowsDoctorAI menggunakan WPF (Windows Presentation Foundation) yang sudah legacy meskipun masih supported. Perlu keputusan tentang UI framework untuk complete rewrite.

**Options Considered:**
1. **WPF** — Mature, banyak resources, .NET Framework/Core support
2. **WinUI 3** — Modern Fluent Design, native Windows 11 look, Microsoft's current recommendation
3. **WinForms** — Simple, tapi outdated visual style
4. **Avalonia UI** — Cross-platform, tapi bukan native Microsoft
5. **MAUI** — Cross-platform, overkill untuk Windows-only app

### Decision

**Adopt WinUI 3** dengan Windows App SDK 1.5.

### Rationale

1. **Modern Design** — Native Fluent Design System, mengikuti Windows 11 aesthetic
2. **Future-Proof** — Microsoft's active development focus untuk Windows apps
3. **Native Performance** — Better performance dari WPF untuk modern UI
4. **New Features** — Access ke modern controls (NavigationView, ContentDialog, InfoBar)
5. **Long-term Support** — Roadmap Microsoft jelas untuk Windows App SDK

### Consequences

**Positive:**
- ✅ Beautiful modern UI out of the box
- ✅ Consistent dengan Windows 11 apps
- ✅ Access ke latest UI controls
- ✅ Better animations dan transitions

**Negative:**
- ❌ Smaller ecosystem dibanding WPF
- ❌ Fewer third-party libraries
- ❌ Learning curve untuk yang biasa WPF
- ❌ Some quirks (StrokeDashArray issues, dark mode audit needed)
- ❌ Requires Windows 10 build 17763+

### Alternatives Considered

**WPF (Rejected):**
- Pro: Mature, banyak resources
- Con: Legacy, tidak modern look, tidak future-proof

**WinForms (Rejected):**
- Pro: Simplest
- Con: Terlalu outdated untuk premium app

**Avalonia (Rejected):**
- Pro: Cross-platform
- Con: Bukan native Microsoft, potential compatibility issues dengan Windows-specific features

---

## ADR-002: Use MVVM Pattern with CommunityToolkit.Mvvm

**Date:** 2025 (Project Inception)
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Perlu pilih pattern untuk memisahkan UI dari business logic. WinUI 3 support MVVM natively, tapi implementation approach ada beberapa options.

### Decision

**Adopt MVVM pattern** dengan **CommunityToolkit.Mvvm 8.2.2** untuk source generators.

### Rationale

1. **Separation of Concerns** — UI dan logic terpisah jelas
2. **Testability** — ViewModels bisa di-test tanpa UI
3. **Data Binding** — Native WinUI support
4. **Source Generators** — CommunityToolkit mengurangi boilerplate signifikan:
   - `[ObservableProperty]` → auto-generates INotifyPropertyChanged
   - `[RelayCommand]` → auto-generates ICommand implementations
5. **Standard Pattern** — Well-known, familiar untuk developer .NET

### Consequences

**Positive:**
- ✅ Clean code separation
- ✅ Reduced boilerplate (~50% less code untuk MVVM plumbing)
- ✅ Type-safe commands
- ✅ Compile-time checks

**Negative:**
- ❌ Learning curve untuk MVVM newcomers
- ❌ Debugging source generators bisa tricky
- ❌ Additional dependency

### Alternatives Considered

**Manual MVVM (No Library) — Rejected:**
- Too much boilerplate untuk properties dan commands

**Prism / MVVM Light — Rejected:**
- Heavier framework, overkill untuk single-window app
- CommunityToolkit lebih modern dan lightweight

**No MVVM (Code-Behind Only) — Rejected:**
- Not testable
- Hard to maintain untuk complex UI

---

## ADR-003: Sequential Scan Execution (not Parallel)

**Date:** 2025
**Status:** ✅ Accepted (untuk v2.0.0), 📅 Planned Change (v2.2.0)
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Diagnostic engine menjalankan 7 kategori scan. Bisa sequential (satu-satu) atau parallel (bersamaan).

### Decision

**Sequential execution** untuk v2.0.0. Parallel execution ditunda ke v2.2.0.

### Rationale

**Untuk v2.0.0 (Sequential):**
1. **Simplicity** — Easier untuk implement dan debug
2. **Predictable Progress** — Live progress dialog lebih mudah design
3. **Better Error Isolation** — Error di satu category tidak affect lain
4. **Windows API Concerns** — Beberapa WMI queries tidak thread-safe
5. **Time Adequate** — 20-45 detik masih acceptable

**Untuk v2.2.0 (Parallel Planned):**
- Beberapa categories bisa berjalan parallel (Disk + Network tidak conflict)
- Potential speedup: 30-50%

### Consequences

**Positive (Sequential):**
- ✅ Predictable execution flow
- ✅ Easier debugging
- ✅ Simpler UI updates

**Negative (Sequential):**
- ❌ Slower total scan time
- ❌ Not utilizing multi-core CPUs efficiently

### Alternatives Considered

**Full Parallel (All 7 Categories) — Rejected for v2.0.0:**
- Complexity: UI updates dari multiple threads
- Risk: WMI resource contention
- Debug: Harder to trace issues

**Selective Parallel (Independent Categories) — Planned v2.2.0:**
- Group: Disk + Network + Startup (independent)
- Sequential: Performance + Security + Update + Drivers

---

## ADR-004: Self-Contained Deployment

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

.NET 8 apps bisa di-distribute dengan 2 cara:
1. **Framework-dependent** — Membutuhkan .NET runtime terinstall di target machine
2. **Self-contained** — Bundle .NET runtime dengan aplikasi

### Decision

**Self-contained deployment** sebagai default.

### Rationale

1. **User Convenience** — No prerequisites untuk end users
2. **Portable** — Copy folder, run, no installation
3. **Version Consistency** — Guaranteed .NET runtime version
4. **Distribution Simplicity** — Single ZIP file
5. **Isolation** — Tidak affected by other .NET apps di system

### Consequences

**Positive:**
- ✅ Zero setup untuk users
- ✅ Portable (bisa run dari USB drive)
- ✅ Version isolation
- ✅ Simpler support (known runtime version)

**Negative:**
- ❌ Larger file size (~150 MB vs ~5 MB framework-dependent)
- ❌ Need re-publish untuk .NET runtime updates
- ❌ Duplicate .NET runtime kalau user punya multiple .NET apps

### Alternatives Considered

**Framework-dependent — Rejected:**
- Pro: Smaller (5-10 MB)
- Con: User harus install .NET 8 runtime terlebih dahulu (barrier untuk adoption)

**MSIX Packaging — Deferred to v3.0.0:**
- Pro: Modern packaging, Microsoft Store ready
- Con: Complexity, requires signing certificate

---

## ADR-005: No Unit Tests in v2.0.0

**Date:** 2025
**Status:** ⚠️ Accepted (Trade-off), 📅 Reversal Planned (v2.1.0+)
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Best practice suggests unit tests dari awal. Namun sebagai solo developer dengan waktu terbatas, perlu prioritize.

### Decision

**Skip unit tests untuk v2.0.0** — Prioritize feature completeness dan manual testing. Unit tests ditambahkan bertahap dari v2.1.0.

### Rationale

1. **Time Constraint** — Solo developer, limited time
2. **Rapid Prototyping** — Focus pada MVP delivery
3. **UI-Heavy App** — UI testing lebih valuable dari unit tests untuk desktop apps
4. **Manual Testing Adequate** — For solo project di initial release
5. **Refactoring Expected** — Architecture masih evolve, tests jadi churn

### Consequences

**Positive:**
- ✅ Faster initial delivery
- ✅ Focus pada UX polish
- ✅ Less code to maintain initially

**Negative:**
- ❌ Higher risk regressions
- ❌ Manual testing time-consuming
- ❌ Refactoring lebih risky
- ❌ Not "professional" practice

### Alternatives Considered

**Full TDD dari Awal — Rejected:**
- Too slow untuk solo developer MVP

**Integration Tests Only — Rejected:**
- Still requires infrastructure setup
- Manual testing sufficient untuk v2.0.0

### Reversal Plan

**v2.1.0:**
- Unit tests untuk Domain Services (mock Infrastructure)
- Test coverage target: 40%

**v2.2.0:**
- Integration tests dengan test WMI/Registry
- Test coverage target: 70%

**v3.0.0:**
- Full test suite dengan UI automation
- Test coverage target: 85%+

---

## ADR-006: Segoe Fluent Icons over Emoji

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Icons untuk UI bisa menggunakan:
1. Emoji (native Windows/Unicode support)
2. Segoe Fluent Icons font (Microsoft's icon font)
3. Custom SVG/PNG assets
4. Third-party icon libraries

### Decision

**Exclusive use Segoe Fluent Icons** — No emoji digunakan di production UI.

### Rationale

1. **Consistency** — Semua icons dari single source (Microsoft Fluent design)
2. **Windows Native** — Built-in font di Windows 10/11 (no external dependency)
3. **Scalable** — Vector font, sharp di semua DPI
4. **Theme-Aware** — Otomatis adapt dengan light/dark theme via `Foreground`
5. **Professional** — Emoji terlihat casual/informal untuk system diagnostic tool
6. **No Miss-Match** — Emoji rendering bervariasi antar Windows versions

### Consequences

**Positive:**
- ✅ Professional appearance
- ✅ Consistent visual language
- ✅ No dependency management
- ✅ Sharp rendering di semua screen sizes

**Negative:**
- ❌ Harus memorize Unicode code points
- ❌ Limited icon selection (dibanding emoji + custom)
- ❌ Requires font installed (available di Win 10 1809+)

### Implementation

- Central `IconGlyphs.cs` class dengan semua Unicode constants
- XAML uses: `<FontIcon Glyph="&#xE73E;" />`
- No emoji di source code atau UI text

---

## ADR-007: Static Helper Classes over DI

**Date:** 2025
**Status:** ⚠️ Accepted (Trade-off), 📅 Reversal Planned (v2.2.0)
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Infrastructure helpers (WmiHelper, ProcessHelper, dll) bisa:
1. **Static classes** — Simple, direct usage
2. **Instance classes dengan DI** — Testable, decoupled

### Decision

**Static helper classes untuk v2.0.0**. DI migration planned di v2.2.0.

### Rationale

1. **Simplicity** — No DI container setup needed
2. **Rapid Development** — Direct call site usage
3. **Infrastructure Nature** — WMI, Registry, Process are truly static concerns
4. **No Multiple Implementations** — Only one way to query WMI
5. **Solo Developer** — DI overhead not justified untuk initial version

### Consequences

**Positive:**
- ✅ Simple call sites: `WmiHelper.Query("...")`
- ✅ No boilerplate untuk registration
- ✅ Easy untuk understand

**Negative:**
- ❌ Hard to mock untuk unit testing
- ❌ Tight coupling
- ❌ Global state concerns (ToastService)

### Alternatives Considered

**Full DI dari Awal — Rejected:**
- Complexity untuk solo developer
- Overkill untuk single-window app

**Instance dengan Manual Injection — Rejected:**
- Pass instances everywhere = boilerplate
- Same as DI tanpa benefit container

### Migration Plan (v2.2.0)

1. Introduce interfaces: `IWmiHelper`, `IProcessHelper`, `IRegistryHelper`
2. Convert helpers ke instance classes
3. Register di `Microsoft.Extensions.DependencyInjection` container
4. Update services untuk consume via constructor injection
5. Update tests untuk use mocks

---

## ADR-008: Event-Driven Communication for Dialogs

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

ScanProgressDialog perlu real-time updates dari ViewModel. Pilihan communication pattern:
1. **Direct method calls** dari ViewModel ke Dialog
2. **Events** — Dialog subscribes to ViewModel events
3. **Messaging** — Publish/subscribe dengan Messenger

### Decision

**Event-driven** dengan events di MainViewModel.

### Rationale

1. **Loose Coupling** — ViewModel tidak reference Dialog directly
2. **Multiple Subscribers** — Dialog + MainWindow bisa subscribe same event
3. **Simplicity** — .NET events are native
4. **No Extra Library** — No need untuk Messenger pattern

**Events di MainViewModel:**
```csharp
public event Action? ScanRequested;
public event Action<string>? CategoryStarted;
public event Action<DiagnosticResult>? ResultFound;
public event Action<SystemHealthScore>? ScanCompleted;
```

### Consequences

**Positive:**
- ✅ Decoupled ViewModel dari Dialog
- ✅ Multiple subscribers supported
- ✅ Standard .NET pattern

**Negative:**
- ❌ Manual subscribe/unsubscribe (memory leak risk kalau lupa)
- ❌ Event handlers scattered di multiple files

### Alternatives Considered

**Messenger Pattern (CommunityToolkit) — Rejected untuk v2.0.0:**
- Overhead: Registering message types
- Not needed untuk single-window app

**Direct Coupling — Rejected:**
- ViewModel tidak boleh reference View components (MVVM violation)

---

## ADR-009: Grid Layout for Results Display

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Scan Results dan Repair Actions pages menampilkan multiple items. Pilihan layout:
1. **Vertical List** — Traditional, satu per baris
2. **Grid Layout** — Multiple kolom, responsive
3. **Table** — Structured columns

### Decision

**Responsive grid layout** dengan `ItemsRepeater + UniformGridLayout`.

### Rationale

1. **Better Information Density** — Multiple items visible sekaligus tanpa scroll
2. **Modern Look** — Dashboard-style presentation
3. **Responsive** — Adjust kolom (2/3/4) sesuai window width
4. **Card-Based UI** — Consistent dengan Fluent Design

**Layout Config:**
```xml
<UniformGridLayout MinItemWidth="380" MinItemHeight="140"
                   MinColumnSpacing="10" MinRowSpacing="10" />
```

### Consequences

**Positive:**
- ✅ More items visible per screen
- ✅ Responsive to window resize
- ✅ Modern premium look
- ✅ Card-based visual

**Negative:**
- ❌ Slightly more complex layout logic
- ❌ Card content constraint (limited space)

### Alternatives Considered

**Vertical List — Rejected:**
- Wastes horizontal space di large screens
- Feels dated

**Table — Rejected:**
- Not aligned dengan modern UI
- Poor untuk cards dengan varying content

---

## ADR-010: HTML Report Format

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Report export bisa dalam berbagai formats:
1. **HTML** — Web-based, universal viewing
2. **PDF** — Print-friendly, professional
3. **JSON** — Machine-readable
4. **CSV** — Spreadsheet-friendly
5. **Plain Text** — Simple

### Decision

**HTML format** sebagai primary export (v2.0.0). Additional formats planned untuk masa depan.

### Rationale

1. **Universal** — Semua PC punya browser
2. **Rich Formatting** — CSS untuk styling, colors, layout
3. **Standalone** — Embedded CSS, no external dependencies
4. **Print-Friendly** — User bisa Ctrl+P → Save as PDF
5. **Shareable** — Attach ke email, view di mobile browser
6. **Easy Implementation** — String template generation

### Consequences

**Positive:**
- ✅ Beautiful visual presentation
- ✅ No PDF library dependency
- ✅ User bisa convert ke PDF sendiri
- ✅ Copy-paste friendly

**Negative:**
- ❌ Not directly a "report" format (secara traditional)
- ❌ Larger file size dari plain text

### Alternatives Considered

**PDF Direct — Deferred:**
- Requires library (iTextSharp, PdfSharp)
- Additional dependency dan licensing considerations

**JSON/CSV — Planned v2.1.0+:**
- Untuk data analysis use cases
- Complementary ke HTML, bukan replacement

---

## ADR-011: Require Administrator Privileges

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Aplikasi bisa berjalan:
1. **Always as admin** — Force UAC elevation
2. **Standard user** — Limited functionality
3. **Optional elevation** — Elevate saat needed

### Decision

**Require administrator elevation** via `app.manifest` dengan `requireAdministrator` level.

### Rationale

1. **Feature Completeness** — Most repair actions require admin
2. **User Clarity** — Better UX: users tahu upfront butuh admin
3. **Security Best Practice** — Elevated once, not repeatedly
4. **Windows Standard** — Diagnostic tools umumnya require admin

**Alternative Handling:**
- Aplikasi masih launches jika user cancel UAC
- Status bar menampilkan "Standard User (Limited)"
- Repair actions disabled/limited

### Consequences

**Positive:**
- ✅ Full functionality out of the box
- ✅ Clear user expectation
- ✅ Consistent behavior

**Negative:**
- ❌ UAC prompt setiap launch (kecuali user configure otherwise)
- ❌ May deter some users
- ❌ Cannot be run di Restricted Group Policy scenarios

### Alternatives Considered

**Optional Elevation — Rejected:**
- Complex UX (UAC prompts saat action, bukan startup)
- Confusing untuk users

**Standard User Only — Rejected:**
- Would remove 70%+ of functionality

---

## ADR-012: MIT License

**Date:** 2025
**Status:** ✅ Accepted
**Deciders:** RIDOLF WIDI ALFISA LUMBA

### Context

Sebagai solo developer project, perlu pilih license untuk:
- Legal protection
- Allow atau restrict usage
- Community contribution possibility

### Decision

**MIT License** dengan additional disclaimer untuk system modification warnings.

### Rationale

1. **Permissive** — Allow wide adoption
2. **Simple** — Easy to understand
3. **Widely Used** — Familiar untuk developers
4. **Commercial Friendly** — Users bisa use untuk projects komersial
5. **Attribution** — Copyright RIDOLF WIDI ALFISA LUMBA preserved
6. **Legal Protection** — "AS IS" disclaimer untuk liability

### Consequences

**Positive:**
- ✅ Maximum adoption potential
- ✅ Encourages community contributions
- ✅ Portfolio-friendly (open source contribution)
- ✅ Copyright ownership maintained

**Negative:**
- ❌ Others can create commercial derivative works
- ❌ No obligation untuk contribute back (unlike GPL)

### Alternatives Considered

**Apache 2.0 — Alternative:**
- Similar to MIT
- Includes patent grant
- Slightly more verbose

**GPL v3 — Rejected:**
- Copyleft: Derivative works must be open source
- May deter commercial adoption

**Proprietary — Rejected:**
- Not aligned dengan open source spirit
- Reduces community potential

---

## 📝 Adding New ADRs

Ketika membuat keputusan arsitektur baru:

1. Copy template di bagian atas dokumen
2. Beri nomor sequential: ADR-013, ADR-014, dst.
3. Add ke Table of Contents
4. Fill in all sections
5. Update status kalau berubah (Deprecated, Superseded)

**Kapan buat ADR:**
- ✅ Keputusan yang affect multiple components
- ✅ Trade-offs yang signifikan
- ✅ Technology/library selections
- ✅ Pattern/approach choices
- ✅ Reversible decisions dengan future implications

**Kapan TIDAK perlu ADR:**
- ❌ Simple bug fixes
- ❌ Refactoring internal implementation
- ❌ Naming decisions
- ❌ Individual feature additions (unless significant)

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial ADR document with 12 records | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Architecture Decision Records for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>