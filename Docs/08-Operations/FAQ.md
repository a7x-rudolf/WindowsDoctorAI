# ❓ Frequently Asked Questions (FAQ)

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0+

---

## 📌 Quick Navigation

- [General Questions](#general-questions)
- [Installation & Setup](#installation--setup)
- [Using the Application](#using-the-application)
- [Diagnostic Scans](#diagnostic-scans)
- [Repair Actions](#repair-actions)
- [Reports & Export](#reports--export)
- [Privacy & Security](#privacy--security)
- [Troubleshooting](#troubleshooting)
- [Development & Contribution](#development--contribution)

---

## General Questions

### Q1: Apa itu WindowsDoctorAI?

**A:** WindowsDoctorAI adalah aplikasi diagnostik dan perbaikan sistem Windows yang membantu Anda:
- Mendeteksi masalah pada 7 kategori sistem (disk, performa, network, keamanan, dll)
- Memberikan skor kesehatan sistem (0-100)
- Menyediakan 30+ aksi perbaikan otomatis dengan konfirmasi
- Menghasilkan laporan HTML yang komprehensif

Aplikasi ini dibuat dengan teknologi modern (.NET 8 + WinUI 3) untuk memberikan pengalaman premium yang sesuai dengan Windows 11 Fluent Design.

---

### Q2: Apakah aplikasi ini gratis?

**A:** **Ya, sepenuhnya gratis.** WindowsDoctorAI adalah karya **RIDOLF WIDI ALFISA LUMBA** (solo developer) yang dilisensikan di bawah **MIT License**, yang berarti:
- ✅ Gratis untuk penggunaan personal maupun komersial
- ✅ Bebas untuk dimodifikasi
- ✅ Bebas untuk didistribusikan ulang (dengan atribusi)
- ✅ Copyright tetap milik RIDOLF WIDI ALFISA LUMBA

Lihat file [LICENSE](../../LICENSE) untuk detail lengkap.

---

### Q3: Apakah aplikasi ini aman untuk sistem saya?

**A:** **Ya**, dengan catatan berikut:

- ✅ Semua aksi perbaikan **selalu memerlukan konfirmasi eksplisit** dari user
- ✅ Setiap aksi memiliki **level risiko** yang ditampilkan (None/Low/Medium/High)
- ✅ Tidak ada operasi destruktif otomatis
- ✅ Aplikasi tidak mengirim data ke internet (kecuali user request eksplisit)
- ✅ Source code tersedia untuk audit

**Namun**, karena aplikasi ini melakukan operasi sistem, kami **sangat merekomendasikan** membuat **System Restore Point** sebelum menggunakan repair actions dengan level risiko Medium atau High.

Baca [DISCLAIMER](../05-Security/DISCLAIMER.md) untuk info lengkap.

---

### Q4: Apa bedanya dengan tool bawaan Windows seperti Task Manager atau Performance Monitor?

**A:** WindowsDoctorAI memberikan **overview terpadu** dalam satu aplikasi:

| Fitur | Windows Tools | WindowsDoctorAI |
|-------|---------------|-----------------|
| Multi-kategori scan | ❌ (Terpisah per tool) | ✅ (7 kategori dalam 1 scan) |
| Health score | ❌ | ✅ (0-100) |
| Recommendation actions | ❌ | ✅ (30+ actions) |
| Modern UI | ⚠️ (Bervariasi) | ✅ (Fluent Design) |
| HTML report | ❌ | ✅ |
| Confirmation-based repairs | ❌ | ✅ |

WindowsDoctorAI **tidak menggantikan** tool bawaan Windows, tapi memberikan lapisan orkestrasi yang lebih user-friendly.
### Q4a: Siapa developer WindowsDoctorAI?

**A:** WindowsDoctorAI dikembangkan sepenuhnya oleh:

> **RIDOLF WIDI ALFISA LUMBA**
> Sole Developer, Architect & Copyright Owner

Project ini adalah **independent solo developer project**, artinya:
- 🎨 Design, architecture, coding, testing, dan dokumentasi dilakukan sendiri oleh developer
- 💼 Hak cipta (copyright) sepenuhnya dimiliki RIDOLF WIDI ALFISA LUMBA
- 📜 Dilisensikan dengan MIT License untuk penggunaan bebas oleh komunitas
- 🛠️ Maintenance dan support dilakukan berdasarkan availability developer

Detail lebih lanjut ada di [PROJECT_CHARTER.md](../00-Overview/PROJECT_CHARTER.md).

---

## Installation & Setup

### Q5: Bagaimana cara install WindowsDoctorAI?

**A:** WindowsDoctorAI adalah aplikasi **portable**, tidak perlu proses instalasi:

1. Download file ZIP dari [Releases](../../releases)
2. Extract ke folder pilihan Anda (misal: `C:\Tools\WindowsDoctorAI\`)
3. Klik kanan `WindowsDoctorAI.exe` → **Run as administrator**

Lihat [BUILD_RELEASE.md](../07-Deployment/BUILD_RELEASE.md) *(coming soon)* untuk build dari source.

---

### Q6: Kenapa harus dijalankan sebagai Administrator?

**A:** Karena aplikasi perlu akses ke:

- Windows Management Instrumentation (WMI) untuk system info
- Windows Registry (untuk beberapa checks dan repair)
- Service management (Start/Stop services seperti Firewall)
- File operations di folder sistem (Temp cleanup)
- PowerShell dengan Execution Policy bypass

Tanpa hak Administrator, sebagian besar diagnostic dan **semua** repair actions tidak akan berfungsi. Aplikasi akan tetap jalan namun dengan **fungsionalitas terbatas**.

---

### Q7: Apakah bisa dijalankan tanpa Administrator?

**A:** **Ya, tapi terbatas.** Aplikasi akan menampilkan badge "Standard User (Limited)" di status bar dan sidebar.

Fitur yang tetap berfungsi tanpa admin:
- ✅ Melihat System Information
- ✅ Sebagian scan (Performance, Network dasar)
- ✅ Melihat UI dan navigasi

Fitur yang **tidak** berfungsi:
- ❌ Semua repair actions
- ❌ Security scan lengkap
- ❌ Registry-based checks
- ❌ Service management

---

### Q8: Apa system requirements minimum?

**A:**

| Komponen | Minimum | Recommended |
|----------|---------|-------------|
| OS | Windows 10 v1809 (Build 17763) | Windows 11 |
| Arsitektur | x64 | x64 |
| RAM | 4 GB | 8 GB+ |
| Disk Space | 200 MB | 500 MB+ |
| Display | 1280×720 | 1400×900+ |
| .NET Runtime | (Bundled) | (Bundled) |

**Tidak support**: Windows 7/8, Windows Server, ARM64 architecture, x86.

---

## Using the Application

### Q9: Bagaimana cara menjalankan diagnostic scan?

**A:** Ada 2 cara:

**Cara 1: Dari Dashboard**
1. Buka aplikasi
2. Klik tombol besar **"Run Full Scan"** di hero card
3. Popup Scan Progress akan muncul dengan live update

**Cara 2: Dari Menu Diagnostic Scan**
1. Klik menu **"Diagnostic Scan"** di sidebar
2. (Opsional) Pilih kategori yang ingin di-scan
3. Klik **"Start Diagnostic Scan"**

Estimasi waktu: **30-45 detik** untuk full scan.

---

### Q10: Bagaimana cara membaca hasil scan?

**A:** Setelah scan selesai, hasil ditampilkan di **Scan Results** page dengan:

- **Severity Badge**: Warna menunjukkan tingkat keparahan
  - 🔴 **Critical** — Perlu perhatian segera
  - 🟡 **Warning** — Perlu diperhatikan
  - 🔵 **Info** — Informasi tambahan
  - 🟢 **Healthy** — Kondisi baik

- **Score (0-100)**: Skor per temuan
- **Category Icon**: Menunjukkan kategori diagnostik
- **Action Count**: Jumlah repair actions yang tersedia

**Klik pada card manapun** untuk melihat detail teknis lengkap.

---

### Q11: Apa arti "Score" di dashboard?

**A:** Score adalah **health score keseluruhan sistem** Anda, dihitung dari:

```
Overall Score = Rata-rata skor semua kategori
Penalty: -5 poin per Critical issue
Range: 0 (worst) - 100 (perfect)
```

**Rating**:
- **90-100**: Excellent (hijau)
- **75-89**: Good (biru)
- **60-74**: Fair (kuning)
- **40-59**: Poor (oranye)
- **0-39**: Critical (merah)

---

### Q12: Bagaimana cara filter hasil scan?

**A:** Di **Scan Results** page, gunakan **segmented filter chips** di atas grid:

- **All** — Tampilkan semua hasil
- **Critical** — Hanya critical issues
- **Warning** — Hanya warnings
- **Healthy** — Hanya kondisi baik

Filter otomatis update jumlah results yang ditampilkan.

---

## Diagnostic Scans

### Q13: Apa saja yang di-scan?

**A:** **7 kategori** diagnostik:

| Kategori | Yang Dicek |
|----------|------------|
| 💾 **Disk Health** | Space, S.M.A.R.T., temp files, optimization (HDD/SSD) |
| ⚡ **Performance** | CPU usage, RAM usage, high-memory processes, uptime |
| 🌐 **Network** | Adapters, internet connectivity, DNS resolution, latency |
| 🔒 **Security** | Antivirus, Firewall, UAC, security services |
| 🔄 **Windows Update** | Service status, last update date, pending reboot |
| 🛠️ **Drivers** | Problem devices, outdated drivers |
| 🚀 **Startup Programs** | Registry entries, scheduled tasks |

Total **25-30 individual checks** per full scan.

---

### Q14: Apakah scan bisa dijalankan otomatis?

**A:** **Belum di v2.0.0**. Fitur automatic scheduled scans akan hadir di **v2.1.0** (planned Q4 2025). Toggle switch untuk fitur ini sudah tersedia di Settings > Scan Preferences namun belum functional.

Workaround: Anda bisa membuat Windows Scheduled Task manual yang menjalankan `WindowsDoctorAI.exe` pada interval tertentu.

---

### Q15: Kenapa hasil scan berbeda antara satu waktu dengan waktu lain?

**A:** Ini **normal** dan diharapkan karena:

- CPU dan RAM usage berubah sesuai aktivitas
- Temp files akumulasi seiring waktu
- Network conditions (latency, DNS) fluktuatif
- Update Windows menghasilkan pending reboot temporer

Untuk mendapatkan baseline yang konsisten, scan pada kondisi sistem idle.

---

### Q16: Bisakah saya scan hanya kategori tertentu?

**A:** **Ya**, di halaman **Diagnostic Scan** Anda bisa uncheck kategori yang tidak ingin di-scan. 

**Catatan**: Saat ini feature checkbox masih bersifat UI-only (tidak affect scan logic). Full scan tetap dijalankan. Selective scan akan diimplementasi di v2.1.0.

---

### Q17: Berapa lama full scan?

**A:** Rata-rata **20-45 detik**, tergantung:

- Jumlah drive di sistem (lebih banyak = lebih lama untuk disk scan)
- Kecepatan disk (SSD lebih cepat dari HDD)
- Kualitas koneksi internet (untuk network ping test)
- Jumlah driver (untuk driver scan via PowerShell)

Sistem modern dengan SSD dan koneksi baik biasanya scan dalam **~25 detik**.

---

## Repair Actions

### Q18: Bagaimana cara menjalankan repair action?

**A:**

1. Setelah scan selesai, buka menu **"Repair Actions"**
2. Cari action yang ingin dijalankan
3. Klik tombol **"Execute"** di card action
4. **Baca dengan teliti** dialog konfirmasi (risk level, requirements)
5. Klik **"Execute Repair"** untuk memulai
6. Tunggu progress dialog selesai
7. Cek toast notification untuk konfirmasi berhasil/gagal

---

### Q19: Apa itu "Fix All Safe Issues"?

**A:** Tombol batch di header Repair Actions page yang otomatis menjalankan **semua repair actions** yang memenuhi kriteria:

- `ActionType` = **Automatic** (bukan manual)
- `RiskLevel` = **Low** atau **None**
- Status = **Pending** (belum dieksekusi)

Actions dengan risk **Medium** atau **High** **tidak** akan dijalankan otomatis — harus manual dengan konfirmasi.

---

### Q20: Apa saja repair actions yang tersedia?

**A:** Total **30+ actions** dikategorikan sebagai berikut:

**Automatic (Auto-execute dengan konfirmasi):**
- Clear Temporary Files
- Reset Network Stack
- Release and Renew IP
- Set DNS to Google (8.8.8.8)
- Update Defender Definitions
- Enable UAC via Registry
- Start Windows Services (Firewall, WU, etc.)

**System Tools (Buka tool bawaan Windows):**
- Task Manager
- Resource Monitor
- Device Manager
- Disk Cleanup
- Defragment and Optimize Drives
- Task Scheduler

**Settings Deep Links:**
- Windows Update settings
- Storage Sense
- Startup Apps
- Windows Security
- Backup settings

Detail lengkap: [REPAIR_ACTIONS.md](../03-Features/REPAIR_ACTIONS.md) *(coming soon)*

---

### Q21: Apakah repair actions bisa di-undo?

**A:** Tergantung jenis action:

| Action | Reversible? |
|--------|-------------|
| Clear Temp Files | ❌ (Files terhapus permanen) |
| Reset Network Stack | ✅ (Tapi konfigurasi network default) |
| Enable UAC | ✅ (Bisa disable manual) |
| Update Defender Definitions | ✅ (Definitions bisa rollback) |
| Change DNS | ✅ (Bisa ganti manual di Network Settings) |
| Registry changes | ⚠️ (Perlu manual edit atau System Restore) |

**Rekomendasi**: Buat **System Restore Point** sebelum menjalankan repair Medium/High risk.

---

### Q22: Kenapa ada repair action yang tidak berfungsi?

**A:** Beberapa kemungkinan:

1. **Tidak dijalankan sebagai Admin** → Restart app as administrator
2. **Windows service dependencies tidak aktif** → Cek Services.msc
3. **PowerShell Execution Policy restricted** → App handle otomatis dengan Bypass flag
4. **Antivirus memblokir** → Whitelist WindowsDoctorAI.exe di antivirus
5. **Windows version tidak support** → Beberapa PowerShell cmdlets hanya ada di Windows 10 1903+

Jika masalah berlanjut, cek [TROUBLESHOOTING.md](TROUBLESHOOTING.md) *(coming soon)*.

---

## Reports & Export

### Q23: Bagaimana cara export hasil scan?

**A:**

1. Setelah scan selesai, klik tombol **"Export Report"** (tersedia di Dashboard, Results page, atau Repair Actions page)
2. File HTML akan otomatis:
   - Disimpan ke **Desktop** dengan nama `WindowsDoctorAI_Report_YYYYMMDD_HHMMSS.html`
   - Otomatis dibuka di browser default

Report berisi:
- Health score dan rating
- Summary statistik (Critical/Warning/Healthy)
- System information lengkap
- Semua hasil scan per kategori
- Available repair actions per temuan

---

### Q24: Apakah report bisa di-share?

**A:** **Ya.** File HTML adalah standalone (self-contained CSS), bisa:
- ✅ Dibuka di browser manapun (Chrome, Edge, Firefox)
- ✅ Dikirim via email sebagai attachment
- ✅ Di-print sebagai PDF (Ctrl+P → Save as PDF)
- ✅ Di-share ke teknisi IT untuk troubleshooting

**Catatan Privacy**: Report **mencantumkan informasi sistem** seperti computer name, username, hardware info. Hati-hati saat share dengan pihak yang tidak dipercaya.

---

### Q25: Apakah bisa export dalam format lain (PDF, JSON, CSV)?

**A:** **Belum di v2.0.0**. Hanya format HTML yang tersedia.

**Workaround**:
- **PDF**: Buka HTML report di browser → Print → Save as PDF
- **JSON/CSV**: Belum ada, direncanakan untuk versi mendatang

Feature request bisa dibuat via [GitHub Issues](../../issues).

---

## Privacy & Security

### Q26: Apakah WindowsDoctorAI mengumpulkan data saya?

**A:** **Tidak.** Aplikasi ini:

- ❌ **Tidak** mengirim data ke server manapun
- ❌ **Tidak** mengumpulkan telemetry
- ❌ **Tidak** menyimpan personal information
- ❌ **Tidak** mengakses file pribadi user
- ❌ **Tidak** ada tracking atau analytics

Semua data scan **hanya ada di memory** selama sesi aplikasi. Kalau di-export ke HTML report, file disimpan lokal di Desktop Anda.

---

### Q27: Apakah kode source terbuka untuk diaudit?

**A:** **Ya.** Source code tersedia di [GitHub repository](../../). Anda bisa:
- ✅ Baca semua kode C#/XAML
- ✅ Compile sendiri dari source
- ✅ Modify sesuai kebutuhan
- ✅ Contribute perbaikan (via Pull Request)

Lisensi **MIT** memberikan kebebasan penuh untuk audit dan modifikasi.

---

### Q28: Apakah aplikasi memerlukan koneksi internet?

**A:** **Tidak untuk operasi normal.** Namun beberapa repair actions memerlukan internet:

- ✅ Update Defender Definitions (download dari Microsoft)
- ✅ Force Windows Update Check
- ✅ Ping test ke 8.8.8.8, 1.1.1.1 (untuk internet connectivity check)
- ✅ DNS resolution test (query ke DNS server)

Semua koneksi ini **user-initiated** dan **transparan** (bisa dilihat dari action name).

---

### Q29: Apakah antivirus akan mendeteksi WindowsDoctorAI sebagai malware?

**A:** **Sangat mungkin false positive** karena aplikasi:

- ⚠️ Requires Administrator elevation
- ⚠️ Menjalankan PowerShell dengan `-ExecutionPolicy Bypass`
- ⚠️ Modify Registry
- ⚠️ Belum di-code sign (v2.0.0)

**Solusi**:
1. Whitelist `WindowsDoctorAI.exe` di antivirus
2. Whitelist folder aplikasi
3. Verifikasi source code di GitHub untuk memastikan tidak ada malicious code

Code signing certificate akan ditambahkan di **v2.1.0** untuk mengurangi false positive.

---

## Troubleshooting

### Q30: Aplikasi tidak mau jalan / crash saat startup

**A:** Cek berikut:

1. **Windows version**: Minimum Windows 10 build 17763. Cek dengan `winver`
2. **Architecture**: Harus x64. Cek dengan Settings → System → About
3. **File integrity**: Re-download ZIP dan extract ulang
4. **Administrator**: Klik kanan → Run as administrator
5. **Antivirus**: Sementara disable atau whitelist app
6. **Windows App SDK Runtime**: Sudah di-bundle, tapi jika issue, install manual dari Microsoft

Jika masih crash, lihat log di Event Viewer → Windows Logs → Application → filter untuk `WindowsDoctorAI`.

---

### Q31: Scan berhenti di tengah jalan

**A:** Kemungkinan penyebab:

1. **Kategori tertentu error** → Cek Live Activity Log di dialog untuk melihat pesan error
2. **Timeout PowerShell** → Beberapa cmdlet seperti `Get-WindowsDriver` bisa lama; default timeout 30-120s
3. **WMI service issues** → Restart WMI service via `net stop winmgmt` && `net start winmgmt`
4. **Cancel button ditekan** → User membatalkan scan

Solusi: Restart aplikasi dan coba lagi. Jika konsisten error di kategori tertentu, report bug via GitHub Issues.

---

### Q32: Score selalu 0 atau tidak berubah

**A:** Kemungkinan:

1. **Scan belum dijalankan** → Klik "Run Full Scan"
2. **Semua checks error** → Cek permission (Administrator?)
3. **Bug di calculation** → Report via GitHub Issues dengan screenshot

---

### Q33: Toast notification tidak muncul

**A:** Toast di WindowsDoctorAI adalah **in-app notification** (bukan Windows notification center). Muncul di **pojok kanan atas** area aplikasi.

Kalau tidak muncul:
1. Cek window aplikasi tidak minimized
2. Restart aplikasi
3. Report bug jika masih terjadi

---

### Q34: UI terlihat aneh / broken

**A:** Kemungkinan:

1. **Windows theme issue** → Ganti theme di Settings → Personalization → Colors
2. **Display scaling** → Ganti scaling ke 100% atau 125% di Display Settings
3. **DPI awareness** → Aplikasi seharusnya support high DPI, kalau bermasalah restart
4. **Fluent Icons tidak ter-load** → Restart Windows (Segoe Fluent Icons built-in)

Screenshot bug dan report ke GitHub Issues sangat membantu debug.

---

### Q35: Dark mode tidak berfungsi dengan baik

**A:** **Diketahui issue** di v2.0.0. Dark mode functional tapi beberapa elemen visual belum ter-audit optimal. Fix planned di **v2.1.0**.

Workaround: Gunakan Light theme untuk pengalaman terbaik di v2.0.0.

---

## Development & Contribution

### Q36: Bagaimana cara build dari source?

**A:** Requirements:

- Visual Studio 2022 Community (atau higher)
- Workload: **.NET Desktop Development**
- Windows App SDK components

Steps:
```bash
git clone <repository-url>
cd WindowsDoctorAI
```

1. Buka `WindowsDoctorAI.sln` di Visual Studio 2022
2. Restore NuGet packages (otomatis saat load solution)
3. Set platform ke **x64**
4. Set configuration ke **Debug** atau **Release**
5. Tekan **F5** untuk build & run

Detail lengkap: [BUILD_RELEASE.md](../07-Deployment/BUILD_RELEASE.md) *(coming soon)*

---

### Q37: Bisakah saya contribute ke project?

**A:** **Belum secara resmi.** Project saat ini dikelola oleh solo developer.

Namun Anda bisa:
- ✅ **Report bugs** via GitHub Issues
- ✅ **Suggest features** via GitHub Discussions
- ✅ **Fork the repo** untuk personal modifications
- ✅ **Share the project** untuk membantu awareness

Contribution guidelines akan dipublikasi saat project siap untuk community contribution (target v2.2.0+).

---

### Q38: Apakah aplikasi ini akan terus dikembangkan?

**A:** **Ya**, roadmap sudah dipublikasikan di [PROJECT_CHARTER.md](../00-Overview/PROJECT_CHARTER.md#13-roadmap-singkat).

Highlights:
- **v2.1.0** (Q4 2025): Scan history, dark mode audit, code signing
- **v2.2.0** (Q1 2026): Parallel scans, unit tests, DI migration
- **v3.0.0** (Future): AI recommendations, MSIX packaging, i18n

---

### Q39: Bahasa pemrograman apa yang digunakan?

**A:** **C# 12** dengan **.NET 8.0**, menggunakan **WinUI 3** untuk UI.

Tech stack detail:
- **Language**: C# 12
- **Runtime**: .NET 8.0 (LTS)
- **UI Framework**: WinUI 3 (Windows App SDK 1.5)
- **Pattern**: MVVM (Model-View-ViewModel)
- **MVVM Library**: CommunityToolkit.Mvvm 8.2.2
- **IDE**: Visual Studio 2022 Community

---

### Q40: Bagaimana cara request fitur baru?

**A:**

1. Cek di [GitHub Issues](../../issues) apakah fitur sudah pernah di-request
2. Kalau belum, buat issue baru dengan template:
   - **Title**: `[Feature Request] Nama fitur singkat`
   - **Description**: Deskripsi fitur, use case, dan benefit
   - **Label**: `enhancement`, `feature-request`
3. Developer akan review dan add ke roadmap jika sesuai

---

## Masih Punya Pertanyaan?

Jika pertanyaan Anda tidak terjawab di FAQ ini:

- 💬 **General questions**: [GitHub Discussions](../../discussions)
- 🐛 **Bug reports**: [GitHub Issues](../../issues)
- 📚 **Documentation**: Browse [`docs/`](../) folder
- 📖 **Learn more**: [Project Charter](../00-Overview/PROJECT_CHARTER.md)

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial FAQ for WindowsDoctorAI v2.0.0 | **RIDOLF WIDI ALFISA LUMBA** |

---

**End of FAQ**

*Dokumen ini akan di-update secara berkala berdasarkan pertanyaan yang sering muncul dari pengguna.*

---

**Document Author:** RIDOLF WIDI ALFISA LUMBA
**Copyright:** © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.
**License:** MIT License