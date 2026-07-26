# 🚀 Update & Release Procedure

**Document Version:** 1.1
**Last Updated:** 16 Juli 2025
**Author:** RIDOLF WIDI ALFISA LUMBA
**Applies to:** WindowsDoctorAI v2.0.0+

---

## 📌 Overview

Dokumen ini berisi **prosedur lengkap step-by-step** untuk merilis update WindowsDoctorAI. Ikuti urutan ini setiap kali ada update yang siap dirilis.

**Total waktu estimasi:** 15-30 menit (tergantung ukuran update dan kecepatan internet)

---

## 📋 Pre-Release Checklist

Sebelum mulai proses release, pastikan:

- [ ] Semua perubahan kode sudah **selesai dan di-test**
- [ ] Aplikasi bisa **Build tanpa error** (0 errors, 0 warnings)
- [ ] Aplikasi berjalan **normal** saat di-run via F5
- [ ] Semua fitur **berfungsi** (scan, repair, export, dll)
- [ ] **Version number** sudah di-update (lihat Step 1)
- [ ] **CHANGELOG.md** sudah di-update dengan perubahan terbaru
- [ ] **Internet connection stable** (untuk push ke GitHub)
- [ ] **Git credentials valid** (token/password sudah siap)
- [ ] **Tidak ada perubahan di remote yang belum di-pull** (cek dengan `git fetch`)

---

## STEP 1: Update Version Numbers

### 1.1 Update `WindowsDoctorAI.csproj`

Buka file dan update 3 baris ini:

```xml
<AssemblyVersion>2.0.1.0</AssemblyVersion>   <!-- Ganti sesuai versi baru -->
<FileVersion>2.0.1.0</FileVersion>            <!-- Ganti sesuai versi baru -->
<Version>2.0.1</Version>                      <!-- Ganti sesuai versi baru -->
```

### 1.2 Update `Views/AboutPage.xaml`

Cari text version dan update:

```xml
<TextBlock Text="Version 2.0.1" ... />        <!-- Ganti sesuai versi baru -->
```

### 1.3 Update `Installer/WindowsDoctorAI-Setup.iss`

Cari baris di bagian atas file:

```iss
#define AppVersion "2.0.1"                     ; Ganti sesuai versi baru
```

### 1.4 Update `CHANGELOG.md`

Tambahkan section baru untuk versi yang akan dirilis:

```markdown
## [2.0.1] - 2025-07-16

### Changed
- (tulis perubahan di sini)

### Fixed
- (tulis bug fixes di sini)

### Added
- (tulis fitur baru di sini)
```

### 1.5 Update `README.md` (Opsional)

Update badge versi di bagian atas file:

```markdown
[![Version](https://img.shields.io/badge/version-2.0.1-blue.svg)](https://github.com/a7x-rudolf/WindowsDoctorAI/releases)
```

### 1.6 Save Semua File

Tekan **Ctrl+Shift+S** di Visual Studio untuk save all.

---

## STEP 2: Build & Test Final

### 2.1 Clean Solution

Di Visual Studio:
```
Menu → Build → Clean Solution
```

### 2.2 Rebuild Solution

```
Menu → Build → Rebuild Solution
```

Pastikan output menampilkan:
```
========== Build: 1 succeeded, 0 failed ==========
```

### 2.3 Test Aplikasi

Tekan **F5** untuk run. Quick test:
- [ ] Aplikasi launch normal
- [ ] Dashboard tampil benar
- [ ] Run Full Scan berfungsi
- [ ] Semua page bisa dinavigasi
- [ ] Close aplikasi

---

## STEP 3: Publish Self-Contained Build

### 3.1 Buka PowerShell

Tekan Windows → ketik "PowerShell" → buka Windows PowerShell

### 3.2 Jalankan Publish Command

Copy-paste **seluruh command** ini:

```powershell
cd "D:\Project\WindowsDoctorAI\WindowsDoctorAI"

dotnet publish "WindowsDoctorAI.csproj" `
    -c Release `
    -r win-x64 `
    -p:Platform=x64 `
    --self-contained true `
    -p:PublishReadyToRun=true `
    -p:PublishSingleFile=false `
    -p:WindowsAppSDKSelfContained=true `
    -p:EnableMsixTooling=true `
    -o "D:\Project\WindowsDoctorAI\Publish"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ PUBLISH SUKSES!" -ForegroundColor Green
} else {
    Write-Host "❌ PUBLISH GAGAL dengan kode: $LASTEXITCODE" -ForegroundColor Red
}
Read-Host "Tekan Enter untuk keluar..."
```

### 3.3 Verify Publish

Pastikan output menampilkan:
```
Build succeeded in XX.Xs
PUBLISH SUKSES!
```

### 3.4 Test Hasil Publish

1. Buka folder: `D:\Project\WindowsDoctorAI\Publish\`
2. Klik kanan `WindowsDoctorAI.exe` → **Run as administrator**
3. Verify aplikasi berjalan normal
4. Close aplikasi setelah verify

---

## STEP 4: Build Installer (Inno Setup)

### 4.1 Pastikan Version Sudah Di-Update di Script

Buka `D:\Project\WindowsDoctorAI\Installer\WindowsDoctorAI-Setup.iss`

Verify baris ini sudah sesuai versi baru:
```iss
#define AppVersion "2.0.1"
```

### 4.2 Compile Installer

Jalankan di PowerShell:

```powershell
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" `
    "D:\Project\WindowsDoctorAI\Installer\WindowsDoctorAI-Setup.iss"

if ($LASTEXITCODE -eq 0) {
    Write-Host "INSTALLER SUKSES!" -ForegroundColor Green
    
    $file = Get-Item "D:\Project\WindowsDoctorAI\Installer\Output\*.exe"
    Write-Host "File: $($file.Name)" -ForegroundColor Cyan
    Write-Host "Size: $([math]::Round($file.Length/1MB, 2)) MB" -ForegroundColor Cyan
} else {
    Write-Host "INSTALLER GAGAL!" -ForegroundColor Red
}
```

### 4.3 Verify Output

File installer akan ada di:
```
D:\Project\WindowsDoctorAI\Installer\Output\WindowsDoctorAI-v2.0.1-Setup-x64.exe
```

### 4.4 Test Installer (Opsional tapi Recommended)

1. Double-click installer
2. Ikuti wizard sampai selesai
3. Verify aplikasi launch setelah install
4. Uninstall via Settings → Apps (kalau hanya test)

---

## STEP 5: Push ke GitHub Repository

### ⚠️ PENTING! Baca Seluruh Step 5 Sebelum Eksekusi

Bagian ini adalah **penyebab paling umum error** saat release. Ikuti urutan dengan teliti.

---

### 5.0 Pre-Push: Cek Remote Changes

**Sebelum melakukan push**, selalu cek apakah ada perubahan di remote yang belum Anda tarik:

```powershell
cd D:\Project\WindowsDoctorAI

# Cek status remote
git fetch origin
git status
```

**Jika output menampilkan:**
```
Your branch is behind 'origin/main' by X commit(s)
```

**ARTINYA:** Ada perubahan di remote yang belum ada di lokal. **JANGAN LANGSUNG PUSH!**

**Solusi:** Lanjut ke Step 5.1 (Pull Remote Changes) dulu.

---

**Jika output menampilkan:**
```
Your branch is up to date with 'origin/main'
```

**ARTINYA:** Aman untuk push. Lanjut ke Step 5.2 (Commit & Push).

---

### 5.1 Pull Remote Changes (Jika Branch Behind)

**⚠️ HANYA JIKA** ada perubahan di remote yang belum Anda tarik:

```powershell
# Tarik perubahan dari remote
git pull origin main

# Jika muncul editor untuk merge message:
# - Di Vim: ketik :wq lalu Enter
# - Di Nano: Ctrl+X, Y, Enter
# - Atau gunakan: git pull origin main --no-edit (otomatis)

# Setelah pull selesai, cek status lagi
git status
```

**Jika terjadi konflik saat pull:**

1. Git akan menandai file yang konflik
2. Buka file tersebut dan cari tanda `<<<<<<<`, `=======`, `>>>>>>>`
3. Perbaiki konflik (pilih salah satu versi atau gabungkan)
4. Setelah selesai:

```powershell
git add .
git commit -m "merge: resolve conflicts after pull"
```

**Setelah konflik resolved**, lanjut ke Step 5.2 untuk commit dan push.

---

### 5.2 Add, Commit & Push

**⚠️ PASTIKAN** sudah tidak ada konflik dan branch sudah up-to-date:

```powershell
cd D:\Project\WindowsDoctorAI

# Add semua perubahan
git add -A

# Lihat apa saja yang berubah (opsional)
git status

# Commit dengan pesan deskriptif
git commit -m "release: WindowsDoctorAI v2.0.1

- (tulis ringkasan perubahan di sini)
- (contoh: Redesign Dashboard UI dark mode)
- (contoh: Fix category cards layout)

Developed by: RIDOLF WIDI ALFISA LUMBA"

# Push ke GitHub
git push origin main

# Verify success
if ($LASTEXITCODE -eq 0) {
    Write-Host "PUSH SUKSES!" -ForegroundColor Green
} else {
    Write-Host "PUSH GAGAL! Cek error di atas." -ForegroundColor Red
}
```

**Jika masih gagal dengan error "rejected":**

Error:
```
! [rejected] main -> main (fetch first)
```

**Penyebab:** Ada perubahan di remote yang belum di-pull (mungkin ada commit baru setelah Step 5.0).

**Solusi:** Ulangi Step 5.1 (Pull) dan Step 5.2 (Push) lagi.

---

### 5.3 Verify di GitHub

Buka browser: https://github.com/a7x-rudolf/WindowsDoctorAI

Pastikan:
- [ ] Commit terbaru muncul
- [ ] Files ter-update
- [ ] Tidak ada error

---

## STEP 6: Create GitHub Release

### 6.1 Buka Halaman Release

Buka browser:
```
https://github.com/a7x-rudolf/WindowsDoctorAI/releases/new
```

### 6.2 Isi Form Release

**Choose a tag:** Ketik versi baru (contoh: `v2.0.1`) → pilih "Create new tag on publish"

**Target:** `main`

**Release title:**
```
Windows Doctor AI v2.0.1 — (judul singkat perubahan)
```

**Description:** Tulis release notes. Contoh template:

```markdown
# 🩺 Windows Doctor AI v2.0.1 — Premium Edition

## 🎯 What's New in v2.0.1

- (tulis perubahan utama)
- (tulis perubahan utama)

## 📥 Download

| File | Description | Size |
|------|-------------|------|
| **WindowsDoctorAI-v2.0.1-Setup-x64.exe** | 🎯 Installer (Recommended) | ~120 MB |

## 💻 System Requirements
- Windows 10 (Build 17763+) or Windows 11
- x64 architecture
- Administrator privileges

---

**Developed by RIDOLF WIDI ALFISA LUMBA**
**Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.**
```

### 6.3 Upload Installer

1. Scroll ke bawah ke area **"Attach binaries by dropping them here or selecting them"**
2. Klik **"selecting them"**
3. Navigate ke: `D:\Project\WindowsDoctorAI\Installer\Output\`
4. Pilih file installer (contoh: `WindowsDoctorAI-v2.0.1-Setup-x64.exe`)
5. Tunggu upload selesai

**PENTING:** Upload di area **"Attach binaries"** (bawah), BUKAN di area description (atas)!

### 6.4 Publish

1. Pastikan **"Set as the latest release"** ter-check
2. Klik **"Publish release"**

### 6.5 Verify

Buka: `https://github.com/a7x-rudolf/WindowsDoctorAI/releases`

Pastikan:
- [ ] Release baru muncul dengan tag yang benar
- [ ] Installer file ter-attach di Assets
- [ ] Release notes tampil dengan benar

---

## ✅ Post-Release Checklist

Setelah publish release, verify:

- [ ] Release page di GitHub menampilkan versi terbaru
- [ ] Installer file ter-attach dengan benar di Assets
- [ ] Download installer berhasil dan bisa di-install
- [ ] Aplikasi hasil install berjalan normal
- [ ] CHANGELOG.md sudah ter-update di repository
- [ ] README.md badge versi sudah ter-update (jika ada)
- [ ] Tidak ada issue baru yang muncul terkait release ini
- [ ] (Opsional) Update di website atau media sosial jika ada

---

## 📊 Quick Reference: Semua Command dalam 1 Block

Untuk yang sudah hafal prosedur, ini **semua command** dalam 1 block:

```powershell
# ═══════════════════════════════════════════════════════════
# WINDOWS DOCTOR AI - FULL RELEASE PROCEDURE
# Author: RIDOLF WIDI ALFISA LUMBA
# ═══════════════════════════════════════════════════════════

# STEP 3: Publish
cd D:\Project\WindowsDoctorAI\WindowsDoctorAI
Remove-Item "D:\Project\WindowsDoctorAI\Publish" -Recurse -Force -ErrorAction SilentlyContinue
dotnet publish -c Release -r win-x64 -p:Platform=x64 --self-contained true `
    -p:PublishReadyToRun=true -p:PublishSingleFile=false `
    -p:WindowsAppSDKSelfContained=true -p:EnableMsixTooling=true `
    -o "D:\Project\WindowsDoctorAI\Publish"

# STEP 4: Build Installer
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" `
    "D:\Project\WindowsDoctorAI\Installer\WindowsDoctorAI-Setup.iss"

# STEP 5: Push ke GitHub (DENGAN SAFETY CHECK)
cd D:\Project\WindowsDoctorAI

# ⚠️ CEK REMOTE DULU!
git fetch origin
git status

# JIKA "behind" → Pull dulu
# git pull origin main

# JIKA "up to date" → Commit & Push
git add -A
git commit -m "release: WindowsDoctorAI vX.Y.Z"
git push origin main

# STEP 6: Create release via browser
# https://github.com/a7x-rudolf/WindowsDoctorAI/releases/new
```

---

## ⚠️ Troubleshooting

### Publish Gagal: "SDK not found"

**Cause:** File `global.json` muncul lagi

**Fix:**
```powershell
Remove-Item "D:\Project\WindowsDoctorAI\global.json" -Force -ErrorAction SilentlyContinue
Remove-Item "D:\Project\WindowsDoctorAI\WindowsDoctorAI\global.json" -Force -ErrorAction SilentlyContinue
```

### Publish Gagal: "AnyCPU not supported"

**Cause:** Missing `-p:Platform=x64` di command

**Fix:** Pastikan command publish ada parameter `-p:Platform=x64`

### Publish Gagal: "EnableMsixTooling required"

**Cause:** Property belum ada di `.csproj`

**Fix:** Pastikan `.csproj` ada baris:
```xml
<EnableMsixTooling>true</EnableMsixTooling>
```

### Installer Error: "File not found"

**Cause:** Folder Publish kosong atau path salah di script `.iss`

**Fix:** 
1. Pastikan Step 3 (Publish) sukses dulu
2. Verify file ada di `D:\Project\WindowsDoctorAI\Publish\`
3. Verify path di `.iss` file benar

### Git Push Rejected: "fetch first" ⚠️ PALING SERING TERJADI

**Error:**
```
! [rejected]        main -> main (fetch first)
error: failed to push some refs to 'https://github.com/...'
```

**Penyebab:** Remote repository memiliki commit baru yang tidak ada di lokal Anda.

**Solusi:**

1. **JANGAN** gunakan `git push --force` (bisa menghapus perubahan orang lain!)
2. Tarik perubahan terlebih dahulu:
   ```powershell
   git pull origin main
   ```
3. Jika ada konflik, selesaikan (lihat cara di Step 5.1)
4. Setelah pull sukses, push lagi:
   ```powershell
   git push origin main
   ```

**Pencegahan:** Selalu jalankan `git fetch origin` dan `git status` sebelum push untuk cek apakah branch sudah up-to-date (lihat Step 5.0).

### Git Push Gagal: "Authentication failed"

**Cause:** Token expired atau credential issue

**Fix:**
1. Buka: https://github.com/settings/tokens
2. Generate new token (classic)
3. Scope: centang `repo`
4. Copy token
5. Jalankan push lagi, paste token sebagai password

### GitHub Release: File Upload Gagal

**Cause:** File terlalu besar atau network timeout

**Fix:**
1. Pastikan koneksi internet stabil
2. Upload di area **"Attach binaries"** (bukan di description area)
3. Tunggu upload selesai sebelum klik Publish
4. Kalau timeout, coba lagi

---

## 📁 File Locations Reference

| Item | Location |
|------|----------|
| **Source Code** | `D:\Project\WindowsDoctorAI\WindowsDoctorAI\` |
| **Solution File** | `D:\Project\WindowsDoctorAI\WindowsDoctorAI.sln` |
| **Project File** | `D:\Project\WindowsDoctorAI\WindowsDoctorAI\WindowsDoctorAI.csproj` |
| **Installer Script** | `D:\Project\WindowsDoctorAI\Installer\WindowsDoctorAI-Setup.iss` |
| **Publish Output** | `D:\Project\WindowsDoctorAI\Publish\` |
| **Installer Output** | `D:\Project\WindowsDoctorAI\Installer\Output\` |
| **CHANGELOG** | `D:\Project\WindowsDoctorAI\CHANGELOG.md` |
| **About Page** | `D:\Project\WindowsDoctorAI\WindowsDoctorAI\Views\AboutPage.xaml` |
| **App Icon** | `D:\Project\WindowsDoctorAI\WindowsDoctorAI\Assets\AppIcon.ico` |
| **Inno Setup Compiler** | `C:\Program Files (x86)\Inno Setup 6\ISCC.exe` |

---

## 🔢 Version History Tracker

| Version | Release Date | Notes |
|---------|-------------|-------|
| v2.0.0 | 15 Juli 2025 | Initial release - Complete rewrite to WinUI 3 |
| v2.0.1 | 16 Juli 2025 | UI Redesign - Dark mode default, blue accent, localization foundation |
| v2.1.0 | (Planned) | Scan History, Settings persistence |

---

## Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial release procedure document | RIDOLF WIDI ALFISA LUMBA |
| 1.1 | 16 Juli 2025 | Added git safety checks & troubleshooting for push rejected | RIDOLF WIDI ALFISA LUMBA |

---

<div align="center">

**Update & Release Procedure for WindowsDoctorAI**

Authored by **RIDOLF WIDI ALFISA LUMBA**

Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.

</div>