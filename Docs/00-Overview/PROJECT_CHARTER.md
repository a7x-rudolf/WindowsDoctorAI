# Project Charter — WindowsDoctorAI

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Status:** Active
**Owner:** Solo Developer Project

---

## 1. Ringkasan Proyek

**WindowsDoctorAI** adalah aplikasi diagnostik dan perbaikan sistem Windows yang dirancang untuk memberikan pengalaman **premium**, **informatif**, dan **user-friendly** dalam mengelola kesehatan PC Windows.

Proyek ini merupakan **hasil migrasi dan modernisasi total** dari implementasi original menjadi aplikasi native modern menggunakan **WinUI 3 + .NET 8.0**, dengan fokus pada arsitektur yang bersih, UI yang premium, dan pengalaman pengguna yang intuitif.

---

## 2. Visi (Vision)

> *"Menjadi tool diagnostik Windows yang paling elegan, informatif, dan mudah digunakan — memberdayakan pengguna dari segala tingkat keahlian untuk memahami dan merawat kesehatan sistem mereka."*

---

## 3. Misi (Mission)

1. **Menyediakan diagnostik komprehensif** dari 7 kategori kritis sistem Windows dalam satu aplikasi terintegrasi
2. **Menawarkan perbaikan otomatis** yang aman dengan sistem konfirmasi eksplisit dan penilaian risiko yang jelas
3. **Menampilkan hasil dengan cara yang informatif** melalui visualisasi modern (score gauge, category cards, progress dialogs)
4. **Menghormati waktu pengguna** dengan scan cepat (~30-45 detik) dan UI yang tidak blocking
5. **Menjaga keamanan sistem** dengan tidak melakukan operasi destruktif tanpa konfirmasi pengguna

---

## 4. Tujuan (Objectives)

### 4.1 Tujuan Fungsional

| # | Tujuan | Metrik Keberhasilan |
|---|--------|---------------------|
| 1 | Diagnostik 7 kategori sistem | Semua kategori scan tanpa error di ≥95% sistem Windows 10/11 |
| 2 | 30+ repair actions | Semua actions tereksekusi sesuai design tanpa side effect |
| 3 | UI premium Fluent Design | UI konsisten dengan design mockup yang disepakati |
| 4 | Scan time ≤ 45 detik | Full scan selesai dalam median ≤45s pada hardware modern |
| 5 | Zero crash pada normal operation | 0 unhandled exception pada test flow standar |

### 4.2 Tujuan Non-Fungsional

| # | Tujuan | Metrik Keberhasilan |
|---|--------|---------------------|
| 1 | Memory footprint efisien | < 200 MB private memory saat idle |
| 2 | UI responsif | Tidak ada UI freeze > 500ms selama operasi |
| 3 | Dokumentasi lengkap | README, LICENSE, CHANGELOG, dan docs/ folder terisi |
| 4 | Kode maintainable | Arsitektur MVVM dengan separasi layer yang jelas |
| 5 | Aksesibilitas dasar | Kompatibel dengan WinUI accessibility features |

---

## 5. Target Pengguna (Target Users)

### 5.1 Primary Users
- **Power Users**: Pengguna Windows yang ingin memantau dan mengoptimasi sistem mereka sendiri
- **IT Enthusiasts**: Hobbyist yang tertarik dengan diagnostik sistem
- **Home Users** dengan literasi teknologi menengah yang mengalami issue performa

### 5.2 Secondary Users
- **IT Support Technicians**: Alat bantu untuk quick diagnostic saat troubleshooting
- **System Administrators**: Untuk personal use di workstation
- **Students** yang mempelajari Windows internals

### 5.3 Bukan Target
- ❌ Enterprise/corporate deployment (perlu MDM, group policy, dll — di luar scope)
- ❌ Server administration (didesain untuk client OS)
- ❌ Pengguna dengan kebutuhan forensik atau security analysis mendalam

---

## 6. Ruang Lingkup (Scope)

### 6.1 In Scope ✅

- Diagnostik pada 7 kategori: Disk, Performance, Network, Security, Windows Update, Drivers, Startup
- Repair actions untuk mengatasi temuan (30+ actions)
- Health score calculation (0-100)
- HTML report export
- Light/Dark theme
- Progress dialogs dengan live logging
- Toast notifications
- System info display

### 6.2 Out of Scope ❌

- **AI/ML-based recommendations** (planned untuk v3.0.0)
- **Cloud sync** untuk scan history
- **Multi-user support** atau centralized management
- **Custom repair scripts** yang bisa diedit user (security concern)
- **Real-time monitoring** (aplikasi bersifat on-demand scanning)
- **Malware scanning** (Windows Defender sudah handle ini)
- **Registry cleaner** dengan bulk deletion (risky, tidak direkomendasikan)
- **Driver update database** (Windows Update sudah cukup)

---

## 7. Deliverables

### 7.1 Software Deliverables

| Item | Format | Status |
|------|--------|--------|
| Aplikasi WindowsDoctorAI | Windows Executable (.exe) | ✅ Complete |
| Self-contained package | ZIP folder (~150 MB) | ✅ Ready |
| Source code | Git repository | ✅ Available |
| Icons & Assets | Embedded resources | ✅ Complete |

### 7.2 Documentation Deliverables

| Dokumen | Prioritas | Status |
|---------|-----------|--------|
| README.md | P0 | ✅ Complete |
| LICENSE | P0 | ✅ Complete |
| CHANGELOG.md | P0 | ✅ Complete |
| PROJECT_CHARTER.md | P0 | ✅ Complete (dokumen ini) |
| DISCLAIMER.md | P0 | ✅ Complete |
| FAQ.md | P0 | ✅ Complete |
| ARCHITECTURE.md | P1 | ⏳ Planned |
| DIAGNOSTIC_CATEGORIES.md | P1 | ⏳ Planned |
| REPAIR_ACTIONS.md | P1 | ⏳ Planned |
| BUILD_RELEASE.md | P1 | ⏳ Planned |
| TROUBLESHOOTING.md | P1 | ⏳ Planned |

---

## 8. Stakeholders

### 8.1 Project Owner & Developer

**RIDOLF WIDI ALFISA LUMBA**

- **Peran**: Sole Developer, Architect, Designer, Maintainer
- **Copyright Holder**: © 2025 RIDOLF WIDI ALFISA LUMBA
- **Tanggung Jawab**: 
  - Complete architecture design & implementation
  - Full UI/UX design & development
  - Testing, debugging & quality assurance
  - Documentation writing & maintenance
  - Release management & distribution
  - Bug fixes & feature development
  - Community support (via GitHub)

### 8.2 Extended Stakeholders

| Peran | Deskripsi | Tanggung Jawab |
|-------|-----------|----------------|
| **End Users** | Pengguna WindowsDoctorAI | Feedback, bug reports, feature requests |
| **Future Contributors** | Kontributor komunitas (jika dibuka) | Contribute code/dokumentasi via Pull Request |
| **Microsoft** | Vendor teknologi | Provide .NET, WinUI 3, Windows SDK |
| **Open Source Community** | CommunityToolkit maintainers | Provide MVVM library |

## 9. Konteks Teknologi

### 9.1 Justifikasi Pemilihan Teknologi

| Teknologi | Alasan Pemilihan |
|-----------|------------------|
| **.NET 8.0** | LTS version, performa tinggi, cross-platform capability, ekosistem matang |
| **WinUI 3** | Framework UI native modern dari Microsoft, Fluent Design built-in, future-proof |
| **C# 12** | Bahasa mature, syntax modern (pattern matching, records), tooling excellent |
| **MVVM Pattern** | Separation of concerns, testability, data binding native |
| **CommunityToolkit.Mvvm** | Source generators mengurangi boilerplate MVVM secara signifikan |
| **WMI** | Standard Microsoft API untuk system information, well-documented |
| **PowerShell** | Powerful untuk operasi sistem yang tidak tersedia via .NET API |

### 9.2 Trade-offs yang Diterima

- ✅ **WinUI 3 vs WPF**: WinUI 3 lebih modern tapi ekosistem library lebih kecil. Trade-off diterima karena aplikasi ini native Windows-only dan mengejar look modern.
- ✅ **Self-contained deployment vs framework-dependent**: Ukuran 150MB lebih besar tapi tidak perlu install .NET runtime. Trade-off diterima untuk kemudahan distribusi.
- ✅ **Solo development vs team**: Development lebih lambat tapi konsistensi arsitektur terjaga. Trade-off diterima karena scope proyek manageable oleh 1 orang.

---

## 10. Batasan & Asumsi (Constraints & Assumptions)

### 10.1 Constraints

- **Platform**: Hanya Windows 10 (1809+) dan Windows 11 x64
- **Arsitektur**: Hanya x64 (tidak support x86 atau ARM64)
- **Framework**: Terikat pada .NET 8.0 dan WinUI 3
- **Hak Akses**: Memerlukan Administrator privileges untuk sebagian besar fitur
- **Waktu Development**: Solo developer, part-time (evening/weekend)

### 10.2 Assumptions

- User memiliki Windows 10 1809+ atau Windows 11
- User memiliki hak Administrator di komputer target
- User memiliki koneksi internet untuk beberapa diagnostic checks (network, DNS)
- User membaca dan memahami disclaimer sebelum menggunakan aplikasi
- User dapat membedakan antara "Warning" dan "Critical" temuan

---

## 11. Risiko Proyek (Project Risks)

| Risiko | Impact | Probability | Mitigasi |
|--------|--------|-------------|----------|
| Breaking changes di WinUI 3 versi berikutnya | Medium | Medium | Pin versi package, upgrade bertahap dengan testing |
| Windows 11 major update mengubah WMI/Registry structure | Medium | Low | Error handling per-check, graceful degradation |
| Solo developer burnout | High | Medium | Prioritize P0/P1, defer nice-to-have features |
| SmartScreen warning menghambat adoption | Medium | High | Rencana code signing di v2.1.0+ |
| Repair action menyebabkan issue di sistem user | High | Low | Explicit confirmation, risk labeling, disclaimer |

---

## 12. Success Criteria

Proyek ini dianggap **sukses** apabila memenuhi kriteria berikut:

### 12.1 Technical Success
- ✅ Aplikasi berjalan stabil pada Windows 10/11 tanpa unhandled exceptions
- ✅ Semua 7 kategori diagnostik berfungsi sesuai spesifikasi
- ✅ Semua 30+ repair actions dapat dieksekusi dengan konfirmasi
- ✅ UI premium sesuai design mockup yang disepakati
- ✅ Build tanpa errors dan warnings

### 12.2 User Success
- ⏳ User dapat menyelesaikan flow: Scan → Review → Repair dalam < 5 menit
- ⏳ User memahami severity temuan tanpa perlu baca dokumentasi
- ⏳ User merasa aplikasi "terlihat profesional dan trustworthy"

### 12.3 Project Success
- ✅ Dokumentasi P0 lengkap sebelum rilis
- ✅ Codebase maintainable untuk pengembangan lanjutan
- ⏳ Roadmap versi mendatang jelas dan terdokumentasi

---

## 13. Roadmap Singkat

### v2.0.0 (Current — Juli 2025)
- ✅ Complete rewrite ke WinUI 3
- ✅ 7 diagnostic categories
- ✅ 30+ repair actions
- ✅ Premium UI dengan Fluent Design
- ✅ P0 documentation

### v2.1.0 (Q4 2025 — Target)
- Scan history persistence (SQLite)
- Dark mode visual audit
- Code signing certificate
- README/CHANGELOG improvements
- P1 documentation

### v2.2.0 (Q1 2026 — Target)
- Parallel scan execution
- Dependency Injection migration
- Unit test coverage
- Keyboard accessibility audit

### v3.0.0 (Future — Aspirational)
- AI-powered recommendations
- MSIX packaging + Microsoft Store
- Multi-language support (i18n)
- Cloud sync (optional)

---

## 14. Referensi & Dokumen Terkait

- [README.md](../../README.md) — Overview publik proyek
- [CHANGELOG.md](../../CHANGELOG.md) — Riwayat versi
- [LICENSE](../../LICENSE) — Lisensi MIT + disclaimer
- [DISCLAIMER.md](../05-Security/DISCLAIMER.md) — Peringatan legal & keamanan
- [FAQ.md](../08-Operations/FAQ.md) — Pertanyaan umum
- [Original Repository](https://github.com/a7x-rudolf/WindowsDoctorAI) — Kode source original

---

## 15. Approval & Sign-off

Sebagai solo developer project, dokumen ini di-approve dan di-maintain sendiri oleh developer utama.

| Peran | Nama | Tanggal | Status |
|-------|------|---------|--------|
| **Project Owner** | **RIDOLF WIDI ALFISA LUMBA** | 15 Juli 2025 | ✅ Approved |
| **Lead Developer** | **RIDOLF WIDI ALFISA LUMBA** | 15 Juli 2025 | ✅ Approved |
| **Architect** | **RIDOLF WIDI ALFISA LUMBA** | 15 Juli 2025 | ✅ Approved |
| **Copyright Holder** | **RIDOLF WIDI ALFISA LUMBA** | 15 Juli 2025 | ✅ Confirmed |

---

### Copyright Declaration

> **This project and all associated intellectual property, including but not limited to source code, design assets, documentation, and derivative works, are the exclusive property of RIDOLF WIDI ALFISA LUMBA.**
>
> **Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.**
>
> Distribution and usage are governed by the MIT License, which permits free use, modification, and distribution under the conditions specified in the [LICENSE](../../LICENSE) file.

## 16. Change History

| Versi | Tanggal | Perubahan | Author |
|-------|---------|-----------|--------|
| 1.0 | 15 Juli 2025 | Initial version | **RIDOLF WIDI ALFISA LUMBA** |

**End of Project Charter**