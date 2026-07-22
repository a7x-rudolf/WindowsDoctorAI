using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsDoctorAI.Models;

namespace WindowsDoctorAI.Services;

/// <summary>
/// Persists scan sessions and their diagnostic results to a local SQLite database.
/// This app runs unpackaged (WindowsPackageType=None), so Windows.Storage.ApplicationData
/// is NOT available. We store the database manually under
/// %LocalAppData%\WindowsDoctorAI\history.db, matching the convention used by AppSettingsStore.
/// </summary>
public class ScanHistoryService
{
    private const int MaxRetainedSessions = 200;

    private static readonly string DataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WindowsDoctorAI");

    private static readonly string DbPath = Path.Combine(DataFolder, "history.db");

    private readonly string _connectionString;
    private bool _initialized;
    private readonly object _initLock = new();

    public ScanHistoryService()
    {
        Directory.CreateDirectory(DataFolder);
        _connectionString = $"Data Source={DbPath}";
    }

    private void EnsureInitialized()
    {
        if (_initialized) return;
        lock (_initLock)
        {
            if (_initialized) return;

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS ScanSessions (
                    SessionId TEXT PRIMARY KEY,
                    Timestamp TEXT NOT NULL,
                    OverallScore REAL NOT NULL,
                    HealthRating TEXT NOT NULL,
                    TotalIssues INTEGER NOT NULL,
                    CriticalIssues INTEGER NOT NULL,
                    WarningIssues INTEGER NOT NULL,
                    InfoIssues INTEGER NOT NULL,
                    HealthyCount INTEGER NOT NULL,
                    ScanDurationSeconds REAL NOT NULL
                );

                CREATE TABLE IF NOT EXISTS ScanResults (
                    Id TEXT PRIMARY KEY,
                    SessionId TEXT NOT NULL REFERENCES ScanSessions(SessionId) ON DELETE CASCADE,
                    Category TEXT NOT NULL,
                    CategoryDisplayName TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    Details TEXT NOT NULL,
                    Severity TEXT NOT NULL,
                    Score REAL NOT NULL
                );

                CREATE INDEX IF NOT EXISTS IX_ScanResults_SessionId ON ScanResults(SessionId);
                CREATE INDEX IF NOT EXISTS IX_ScanSessions_Timestamp ON ScanSessions(Timestamp);
                """;
            cmd.ExecuteNonQuery();

            using var pragmaCmd = conn.CreateCommand();
            pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCmd.ExecuteNonQuery();

            _initialized = true;
        }
    }

    /// <summary>Saves a completed scan (summary + all results) as a new history session.</summary>
    public async Task<string> SaveScanAsync(List<DiagnosticResult> results, SystemHealthScore score)
    {
        EnsureInitialized();

        var sessionId = Guid.NewGuid().ToString();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();

        try
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = """
                    INSERT INTO ScanSessions
                        (SessionId, Timestamp, OverallScore, HealthRating, TotalIssues,
                         CriticalIssues, WarningIssues, InfoIssues, HealthyCount, ScanDurationSeconds)
                    VALUES
                        ($sessionId, $timestamp, $overallScore, $healthRating, $totalIssues,
                         $criticalIssues, $warningIssues, $infoIssues, $healthyCount, $scanDurationSeconds);
                    """;
                cmd.Parameters.AddWithValue("$sessionId", sessionId);
                cmd.Parameters.AddWithValue("$timestamp", score.ScanTime.ToString("O", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("$overallScore", score.OverallScore);
                cmd.Parameters.AddWithValue("$healthRating", score.HealthRating);
                cmd.Parameters.AddWithValue("$totalIssues", score.TotalIssues);
                cmd.Parameters.AddWithValue("$criticalIssues", score.CriticalIssues);
                cmd.Parameters.AddWithValue("$warningIssues", score.WarningIssues);
                cmd.Parameters.AddWithValue("$infoIssues", score.InfoIssues);
                cmd.Parameters.AddWithValue("$healthyCount", results.Count(r => r.Severity == Severity.Healthy));
                cmd.Parameters.AddWithValue("$scanDurationSeconds", score.ScanDuration.TotalSeconds);
                await cmd.ExecuteNonQueryAsync();
            }

            foreach (var r in results)
            {
                using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = """
                    INSERT INTO ScanResults
                        (Id, SessionId, Category, CategoryDisplayName, Title, Description, Details, Severity, Score)
                    VALUES
                        ($id, $sessionId, $category, $categoryDisplayName, $title, $description, $details, $severity, $score);
                    """;
                cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
                cmd.Parameters.AddWithValue("$sessionId", sessionId);
                cmd.Parameters.AddWithValue("$category", r.Category.ToString());
                cmd.Parameters.AddWithValue("$categoryDisplayName", r.CategoryDisplayName);
                cmd.Parameters.AddWithValue("$title", r.Title);
                cmd.Parameters.AddWithValue("$description", r.Description);
                cmd.Parameters.AddWithValue("$details", r.Details ?? string.Empty);
                cmd.Parameters.AddWithValue("$severity", r.Severity.ToString());
                cmd.Parameters.AddWithValue("$score", r.Score);
                await cmd.ExecuteNonQueryAsync();
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }

        await TrimOldSessionsAsync(conn);
        return sessionId;
    }

    /// <summary>Returns all saved scan sessions, most recent first.</summary>
    public async Task<List<ScanHistoryEntry>> GetHistoryAsync()
    {
        EnsureInitialized();

        var list = new List<ScanHistoryEntry>();
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT SessionId, Timestamp, OverallScore, HealthRating, TotalIssues,
                   CriticalIssues, WarningIssues, InfoIssues, HealthyCount, ScanDurationSeconds
            FROM ScanSessions
            ORDER BY Timestamp DESC;
            """;

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new ScanHistoryEntry
            {
                SessionId = reader.GetString(0),
                Timestamp = DateTime.Parse(reader.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                OverallScore = reader.GetDouble(2),
                HealthRating = reader.GetString(3),
                TotalIssues = reader.GetInt32(4),
                CriticalIssues = reader.GetInt32(5),
                WarningIssues = reader.GetInt32(6),
                InfoIssues = reader.GetInt32(7),
                HealthyCount = reader.GetInt32(8),
                ScanDurationSeconds = reader.GetDouble(9)
            });
        }

        return list;
    }

    /// <summary>Returns the full detail (session + all per-item results) for one past scan.</summary>
    public async Task<ScanHistoryDetail?> GetSessionDetailAsync(string sessionId)
    {
        EnsureInitialized();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        ScanHistoryEntry? session = null;
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                SELECT SessionId, Timestamp, OverallScore, HealthRating, TotalIssues,
                       CriticalIssues, WarningIssues, InfoIssues, HealthyCount, ScanDurationSeconds
                FROM ScanSessions WHERE SessionId = $sessionId;
                """;
            cmd.Parameters.AddWithValue("$sessionId", sessionId);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                session = new ScanHistoryEntry
                {
                    SessionId = reader.GetString(0),
                    Timestamp = DateTime.Parse(reader.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    OverallScore = reader.GetDouble(2),
                    HealthRating = reader.GetString(3),
                    TotalIssues = reader.GetInt32(4),
                    CriticalIssues = reader.GetInt32(5),
                    WarningIssues = reader.GetInt32(6),
                    InfoIssues = reader.GetInt32(7),
                    HealthyCount = reader.GetInt32(8),
                    ScanDurationSeconds = reader.GetDouble(9)
                };
            }
        }

        if (session == null) return null;

        var results = new List<ScanHistoryResult>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                SELECT Id, SessionId, Category, CategoryDisplayName, Title, Description, Details, Severity, Score
                FROM ScanResults WHERE SessionId = $sessionId
                ORDER BY CASE Severity
                    WHEN 'Critical' THEN 0
                    WHEN 'Warning' THEN 1
                    WHEN 'Info' THEN 2
                    ELSE 3
                END;
                """;
            cmd.Parameters.AddWithValue("$sessionId", sessionId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new ScanHistoryResult
                {
                    Id = reader.GetString(0),
                    SessionId = reader.GetString(1),
                    Category = Enum.Parse<DiagnosticCategory>(reader.GetString(2)),
                    CategoryDisplayName = reader.GetString(3),
                    Title = reader.GetString(4),
                    Description = reader.GetString(5),
                    Details = reader.GetString(6),
                    Severity = Enum.Parse<Severity>(reader.GetString(7)),
                    Score = reader.GetDouble(8)
                });
            }
        }

        return new ScanHistoryDetail { Session = session, Results = results };
    }

    /// <summary>Deletes one scan session and its results.</summary>
    public async Task DeleteSessionAsync(string sessionId)
    {
        EnsureInitialized();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();

        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM ScanResults WHERE SessionId = $sessionId;";
            cmd.Parameters.AddWithValue("$sessionId", sessionId);
            await cmd.ExecuteNonQueryAsync();
        }
        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM ScanSessions WHERE SessionId = $sessionId;";
            cmd.Parameters.AddWithValue("$sessionId", sessionId);
            await cmd.ExecuteNonQueryAsync();
        }

        tx.Commit();
    }

    /// <summary>Deletes all scan history.</summary>
    public async Task ClearAllAsync()
    {
        EnsureInitialized();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();

        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM ScanResults;";
            await cmd.ExecuteNonQueryAsync();
        }
        using (var cmd = conn.CreateCommand())
        {
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM ScanSessions;";
            await cmd.ExecuteNonQueryAsync();
        }

        tx.Commit();
    }

    /// <summary>Keeps the database bounded by dropping the oldest sessions beyond MaxRetainedSessions.</summary>
    private static async Task TrimOldSessionsAsync(SqliteConnection conn)
    {
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                DELETE FROM ScanSessions
                WHERE SessionId NOT IN (
                    SELECT SessionId FROM ScanSessions
                    ORDER BY Timestamp DESC
                    LIMIT $max
                );
                """;
            cmd.Parameters.AddWithValue("$max", MaxRetainedSessions);
            await cmd.ExecuteNonQueryAsync();
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                DELETE FROM ScanResults
                WHERE SessionId NOT IN (SELECT SessionId FROM ScanSessions);
                """;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
