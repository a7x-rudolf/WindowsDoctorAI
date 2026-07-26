using System;
using System.Collections.Generic;

namespace WindowsDoctorAI.Models;

public class SystemHealthScore
{
    public double OverallScore { get; set; } = 100.0;
    public Dictionary<DiagnosticCategory, double> CategoryScores { get; set; } = new();
    public int TotalIssues { get; set; }
    public int CriticalIssues { get; set; }
    public int WarningIssues { get; set; }
    public int InfoIssues { get; set; }
    public DateTime ScanTime { get; set; } = DateTime.Now;
    public TimeSpan ScanDuration { get; set; }

    public string HealthRating => OverallScore switch
    {
        >= 90 => "Excellent",
        >= 75 => "Good",
        >= 60 => "Fair",
        >= 40 => "Poor",
        _ => "Critical"
    };

    public string HealthDescription => OverallScore switch
    {
        >= 90 => "Your system is in excellent condition.",
        >= 75 => "Your system is in good shape with minor issues.",
        >= 60 => "Your system has some issues that should be addressed.",
        >= 40 => "Your system has significant problems. Action is recommended.",
        _ => "Your system has critical issues needing immediate attention."
    };
}