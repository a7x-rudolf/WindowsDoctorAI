using System.Diagnostics.CodeAnalysis;

// Suppress CsWinRT AOT/trimming messages - app is not published with NativeAOT
[assembly: SuppressMessage("Interoperability", "CsWinRT1028",
    Justification = "App does not use NativeAOT or trimming.",
    Scope = "namespaceanddescendants", Target = "~N:WindowsDoctorAI")]

[assembly: SuppressMessage("Interoperability", "CsWinRT1029",
    Justification = "App does not use NativeAOT or trimming.",
    Scope = "namespaceanddescendants", Target = "~N:WindowsDoctorAI")]

[assembly: SuppressMessage("Interoperability", "CsWinRT1030",
    Justification = "App does not use NativeAOT or trimming.",
    Scope = "namespaceanddescendants", Target = "~N:WindowsDoctorAI")]