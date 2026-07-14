# ⚠️ Disclaimer, Warnings & Terms of Use

**Document Version:** 1.0
**Last Updated:** 15 Juli 2025
**Applies to:** WindowsDoctorAI v2.0.0 and later

---

## 🚨 READ BEFORE USING THIS SOFTWARE

By downloading, installing, or using **WindowsDoctorAI**, you acknowledge that you have read, understood, and agree to be bound by all the terms, warnings, and disclaimers described in this document.

**IF YOU DO NOT AGREE WITH ANY PART OF THIS DISCLAIMER, DO NOT USE THIS SOFTWARE.**

---

## 1. General Disclaimer

### 1.0 Software Ownership Declaration

**WindowsDoctorAI** is the exclusive intellectual property of:

> **RIDOLF WIDI ALFISA LUMBA**
> Sole Developer, Architect, and Copyright Owner
> © 2025 All Rights Reserved

This software is distributed under the [MIT License](../../LICENSE), which permits free use, modification, and distribution subject to the terms specified therein.

### 1.1 "As Is" Provision

WindowsDoctorAI is provided **"AS IS"** and **"AS AVAILABLE"** without warranty of any kind, either express or implied, including but not limited to:

- Warranties of merchantability
- Fitness for a particular purpose
- Non-infringement
- Accuracy or completeness of information
- Uninterrupted or error-free operation

### 1.2 No Professional Advice

This software is intended for **general diagnostic purposes only** and does not constitute:

- Professional IT consulting
- System administration services
- Security auditing
- Data recovery services
- Legal or compliance advice

**For critical business systems or complex issues, please consult a qualified IT professional.**

---

## 2. System Modification Warnings

### 2.1 What This Software Does

WindowsDoctorAI performs the following types of operations on your Windows system:

#### 📖 Read Operations (Safe)
- Query system information via WMI (Windows Management Instrumentation)
- Read registry values for diagnostic purposes
- Enumerate Windows services and their status
- Check network adapter configuration
- Read file system information (drive space, temp files)
- Execute read-only PowerShell commands (Get-HotFix, Get-WindowsDriver, etc.)

#### ✏️ Write Operations (User-Confirmed Only)
- Modify Windows Registry (e.g., re-enabling UAC via `EnableLUA` key)
- Start/Stop Windows Services (Firewall, Windows Update, Security services)
- Execute system commands (chkdsk, ipconfig, netsh, shutdown)
- Delete temporary files from Windows Temp directories
- Execute PowerShell scripts for repair actions
- Modify network configuration (Winsock reset, DNS flush)

### 2.2 What This Software Does NOT Do

WindowsDoctorAI does **NOT** perform any of the following:

- ❌ Send any data to external servers
- ❌ Collect personal information or telemetry
- ❌ Install or uninstall third-party software
- ❌ Modify user files or documents
- ❌ Access the internet (except for repair actions like DNS lookup)
- ❌ Perform any operation without user confirmation (for repairs)
- ❌ Bulk delete registry entries
- ❌ Scan for or remove malware

---

## 3. Risk Categorization

All repair actions in WindowsDoctorAI are categorized by risk level. **Users must review and acknowledge risk before executing any repair.**

### 3.1 Risk Levels

| Level | Description | Examples | Reversible? |
|-------|-------------|----------|-------------|
| **None** | No system modification; opens tool/settings only | Open Task Manager, Open Device Manager | ✅ N/A |
| **Low** | Minor changes with minimal impact | Clear temp files, Flush DNS cache, Update Defender definitions | ✅ Yes |
| **Medium** | Service changes or configuration modifications | Start/Stop services, Reset network stack, Change power plan | ⚠️ Mostly Yes |
| **High** | System-wide changes or destructive operations | (None currently implemented) | ❌ May require restore |

### 3.2 Actions That Require Reboot

The following actions require a system reboot to take effect:
- CHKDSK (scheduled for next boot)
- Enable UAC via Registry
- Reset Network Stack
- Windows Update installations

**A confirmation dialog will always be shown before initiating any reboot.**

---

## 4. Recommended Precautions

### 4.1 Before Using WindowsDoctorAI

**STRONGLY RECOMMENDED:**

1. ✅ **Create a System Restore Point**
   - Open Control Panel → System → System Protection
   - Click "Create..." and name it (e.g., "Before WindowsDoctorAI Scan")

2. ✅ **Backup Important Data**
   - Ensure critical files are backed up to external drive or cloud storage
   - Especially important before executing Medium/High risk repairs

3. ✅ **Close Other Applications**
   - Save your work and close unnecessary applications before running full scan

4. ✅ **Verify You Have Admin Rights**
   - The application requires Administrator privileges for full functionality

### 4.2 During Repair Actions

- ⚠️ **Read the confirmation dialog carefully** before clicking "Execute Repair"
- ⚠️ **Note the risk level** displayed in the dialog
- ⚠️ **Do NOT close the application** during repair execution
- ⚠️ **Do NOT execute multiple repairs simultaneously** — wait for each to complete

### 4.3 After Repair Actions

- ✅ Verify system stability by using your computer normally
- ✅ If issues arise, use System Restore to revert changes
- ✅ Check the repair completion log for any errors
- ✅ Reboot the system if the repair required it

---

## 5. Limitation of Liability

### 5.1 No Liability for Damages

To the maximum extent permitted by applicable law, the developers, contributors, and distributors of WindowsDoctorAI shall **NOT be liable** for any:

- **Direct damages** — Including but not limited to data loss, system corruption, or hardware failure
- **Indirect damages** — Including but not limited to lost profits, business interruption, or opportunity costs
- **Incidental or consequential damages** — Arising from the use or inability to use the software
- **Special or punitive damages** — Under any circumstances
- **Third-party claims** — Related to the use of this software

**This limitation applies even if the developer has been advised of the possibility of such damages.**

### 5.2 User Assumption of Risk

By using WindowsDoctorAI, you expressly acknowledge and agree that:

1. **You use the software at your own risk**
2. **You are responsible for backing up your data**
3. **You are responsible for reviewing repair actions before execution**
4. **You are responsible for any consequences of executing repairs**
5. **The developer is not obligated to provide support or fix issues**

### 5.3 Jurisdictional Limitations

Some jurisdictions do not allow the exclusion or limitation of certain warranties or damages. In such jurisdictions, the developer's liability shall be limited to the greatest extent permitted by law.

---

## 6. Privacy & Data Handling

### 6.1 No Data Collection

WindowsDoctorAI **does NOT collect, transmit, or store** any personal information, including:

- ❌ System identifiers (MAC address, Machine ID, Windows License Key)
- ❌ User information (username, email, phone)
- ❌ Scan results or diagnostic data
- ❌ Usage analytics or telemetry
- ❌ Crash reports
- ❌ Location data

### 6.2 Local Data Only

All data processed by WindowsDoctorAI remains **strictly local** on your machine:

- Scan results are held in memory during the session
- HTML reports (if generated) are saved to your Desktop as plain files
- No temporary files are created outside standard Windows Temp folders
- No configuration files are created in AppData (currently — may change in future versions)

### 6.3 Third-Party Services

WindowsDoctorAI does not integrate with any third-party services except:

- **Windows built-in APIs** (WMI, Registry, PowerShell) — Local to your system
- **DNS providers** (only when executing "Set Google DNS" repair) — User-initiated
- **Microsoft Update servers** (only when executing "Update Defender Definitions") — User-initiated

---

## 7. Software Licensing

WindowsDoctorAI is released under the **MIT License**. See [LICENSE](../../LICENSE) for full terms.

### 7.1 What You Can Do
- ✅ Use the software for personal or commercial purposes
- ✅ Modify the source code
- ✅ Distribute copies (with proper attribution)
- ✅ Sublicense (with the MIT license terms preserved)

### 7.2 What You Cannot Do
- ❌ Hold the authors liable for any damages
- ❌ Remove the copyright notice or license from copies
- ❌ Use the WindowsDoctorAI name or trademarks in ways that suggest endorsement

---

## 8. Third-Party Components

WindowsDoctorAI includes the following third-party components, each with their own licenses:

| Component | License | Purpose |
|-----------|---------|---------|
| Microsoft.WindowsAppSDK | MIT | WinUI 3 runtime |
| Microsoft.Windows.SDK.BuildTools | MIT | Windows SDK build tools |
| CommunityToolkit.Mvvm | MIT | MVVM helper library |
| System.Management | MIT | WMI access |
| System.Diagnostics.PerformanceCounter | MIT | Performance monitoring |
| Microsoft.Win32.Registry | MIT | Registry access |
| System.ServiceProcess.ServiceController | MIT | Service management |

All third-party components are used in compliance with their respective licenses.

---

## 9. Software Updates & Support

### 9.1 No Warranty of Updates

- Software updates are provided at the sole discretion of the developer
- No guarantee of ongoing maintenance, bug fixes, or feature additions
- Users are responsible for checking for updates periodically

### 9.2 Community Support Only

- No official support channels are provided
- Users may seek assistance through:
  - GitHub Issues (bug reports and feature requests)
  - GitHub Discussions (general questions)
  - Community forums (unofficial)

---

## 10. Reporting Security Vulnerabilities

If you discover a security vulnerability in WindowsDoctorAI, please:

1. **DO NOT** disclose it publicly until it has been addressed
2. Report it via GitHub Issues with the label "security" (or via email if provided)
3. Provide as much detail as possible: reproduction steps, impact, potential mitigation
4. Allow reasonable time for the developer to investigate and respond

The developer will make best efforts to address security issues but makes no guarantees of response time or fixes.

---

## 11. Age & Legal Capacity

By using WindowsDoctorAI, you represent and warrant that:

- You are at least 18 years old, OR
- You are using the software with the supervision and consent of a parent or legal guardian
- You have the legal capacity to accept these terms in your jurisdiction

---

## 12. Termination

The developer reserves the right to:

- Cease development or maintenance at any time without notice
- Remove the software from distribution channels
- Modify these terms and conditions in future versions

Users may stop using the software at any time. Uninstallation instructions:
1. Delete the WindowsDoctorAI folder from your system
2. Optionally delete any HTML reports saved to Desktop
3. No registry cleanup is required (no persistent settings written currently)

---

## 13. Acknowledgment

**BY USING WINDOWSDOCTORAI, YOU ACKNOWLEDGE THAT:**

1. ✅ You have read this disclaimer in its entirety
2. ✅ You understand the risks associated with system modification software
3. ✅ You accept full responsibility for how you use the software
4. ✅ You will create a System Restore Point before executing Medium/High risk repairs
5. ✅ You will not hold the developer liable for any damages resulting from software use
6. ✅ You accept the MIT License terms

---

## 14. Contact Information

### 14.1 Developer & Copyright Owner

**RIDOLF WIDI ALFISA LUMBA**
- Role: Sole Developer, Architect, Copyright Holder
- Project: WindowsDoctorAI (Solo Developer Project)

### 14.2 Support Channels

For questions or concerns about this disclaimer:

- **Bug Reports & Issues:** [GitHub Issues](../../issues)
- **General Discussion:** [GitHub Discussions](../../discussions)
- **License Questions:** See [LICENSE](../../LICENSE)
- **Legal Inquiries:** Contact developer via GitHub

### 14.3 Intellectual Property

All intellectual property rights, including copyrights, trademarks, and design rights, are held exclusively by **RIDOLF WIDI ALFISA LUMBA**. The MIT License grants permission for use and modification but does not transfer ownership of intellectual property rights.

---

## 15. Change History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | 15 Juli 2025 | Initial version for WindowsDoctorAI v2.0.0 | **RIDOLF WIDI ALFISA LUMBA** |

---

**End of Disclaimer**

*By continuing to use WindowsDoctorAI, you confirm your acceptance of all terms outlined in this document and the accompanying MIT License.*

---

**Software Copyright:** © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.
**License:** MIT License
**Document Author:** RIDOLF WIDI ALFISA LUMBA