using System;
using System.Collections.Concurrent;
using System.Linq;
using Corno.Web.Areas.Kitchen.Models;

namespace Corno.Web.Areas.Kitchen.Services
{
    public class ImportSessionService
    {
        private static readonly ConcurrentDictionary<string, ImportSession> _sessions = new ConcurrentDictionary<string, ImportSession>();
        private static readonly object _lockObject = new object();

        public ImportSession CreateSession(string userId, string fileName)
        {
            lock (_lockObject)
            {
                // Check if user already has an active import
                var existingSession = GetActiveSessionByUser(userId);
                if (existingSession != null)
                {
                    throw new InvalidOperationException($"User already has an active import in progress. Session: {existingSession.SessionId}");
                }

                var sessionId = $"{userId}_{Guid.NewGuid():N}";
                var session = new ImportSession
                {
                    SessionId = sessionId,
                    UserId = userId,
                    FileName = fileName,
                    StartTime = DateTime.Now,
                    Status = ImportStatus.Pending
                };

                _sessions.TryAdd(sessionId, session);
                return session;
            }
        }

        public ImportSession GetSession(string sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public ImportSession GetActiveSessionByUser(string userId)
        {
            return _sessions.Values
                .FirstOrDefault(s => s.UserId == userId && 
                    (s.Status == ImportStatus.Pending || 
                     s.Status == ImportStatus.Reading || 
                     s.Status == ImportStatus.Validating || 
                     s.Status == ImportStatus.Processing) &&
                    !s.IsCancelled);
        }

        public bool HasActiveImport(string userId)
        {
            return GetActiveSessionByUser(userId) != null;
        }

        public void UpdateSession(string sessionId, Action<ImportSession> updateAction)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                lock (_lockObject)
                {
                    updateAction(session);
                }
            }
        }

        public bool CancelSession(string sessionId, string userId)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                if (session.UserId != userId)
                {
                    return false; // User can only cancel their own imports
                }

                lock (_lockObject)
                {
                    session.IsCancelled = true;
                    session.Status = ImportStatus.Cancelled;
                    session.EndTime = DateTime.Now;
                    session.CurrentMessage = "Import cancelled by user";
                    session.CurrentStep = "Cancelled";
                }
                return true;
            }
            return false;
        }

        public void CompleteSession(string sessionId, ImportSummary summary)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                lock (_lockObject)
                {
                    session.Status = ImportStatus.Completed;
                    session.EndTime = DateTime.Now;
                    session.Summary = summary;
                    session.PercentComplete = 100;
                    session.CurrentMessage = "Import completed successfully";
                }
            }
        }

        public void FailSession(string sessionId, string errorMessage)
        {
            if (_sessions.TryGetValue(sessionId, out var session))
            {
                lock (_lockObject)
                {
                    session.Status = ImportStatus.Failed;
                    session.EndTime = DateTime.Now;
                    session.CurrentMessage = errorMessage;
                    session.ErrorMessages.Add(errorMessage);
                }
            }
        }

        public void RemoveSession(string sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
        }

        public void CleanupOldSessions(TimeSpan maxAge)
        {
            var cutoffTime = DateTime.Now.Subtract(maxAge);
            var sessionsToRemove = _sessions.Values
                .Where(s => s.EndTime.HasValue && s.EndTime.Value < cutoffTime)
                .Select(s => s.SessionId)
                .ToList();

            foreach (var sessionId in sessionsToRemove)
            {
                _sessions.TryRemove(sessionId, out _);
            }
        }
    }
}

