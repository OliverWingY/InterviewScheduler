using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Genetics
{
    public class ScheduleFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate((IScheduleChromosome)chromosome);
        }

        public double Evaluate(IScheduleChromosome chromosome)
        {
            var schedule = chromosome.GetSchedule();
            var score = GetPunishmentForBreakingRules(schedule);
            if (score < 0) return score;
            return GetRewardForEfficiency(schedule);

        }

        public double GetPunishmentForBreakingRules(Schedule schedule)
        {
            var score = 0;
            foreach(var candidate in schedule.Candidates)
            {
                var candidateSchedule = schedule.GetScheduleForCandidate(candidate.Id);
                if (CandidateIsDoubleBooked(candidateSchedule)) 
                    score = score-1;
                if (CandidateMissingRequiredSession(candidateSchedule, candidate)) 
                    score = score-1;
                if (CandidateGoesToUnnecessarySession(candidateSchedule, candidate)) 
                    score--;
                if (CandidateGoesToSameSessionTwice(candidateSchedule)) 
                    score--;
            }
            var sessions = schedule.Sessions;
            foreach (var session in sessions)
            {
                var sessionSchedule = schedule.GetScheduleForSession(session.SessionId);

                if (SessionHasLessThanMinimumCandidates(sessionSchedule, session)) 
                    score--;
            }

            return score;
        }


        public double GetRewardForEfficiency(Schedule schedule)
        {
            var score = 0.0;
            foreach (var candidate in schedule.Candidates)
            {
                var candidateSchedule = schedule.GetScheduleForCandidate(candidate.Id);
                score = score + GetBonusForShortStay(candidateSchedule);
            }
            var sessions = schedule.Sessions;
            foreach (var session in sessions)
            {
                var sessionSchedule = schedule.GetScheduleForSession(session.SessionId);
                score = score + GetBonusForCrowdedSessions(sessionSchedule, session)*10;
            }
            return score;
        }

        private double GetBonusForCrowdedSessions(Dictionary<int, List<Candidate>> sessionSchedule, Session session)
        {
            var smallestSession = sessionSchedule.Values.Where(x => x.Count() > 0).MinBy(x => x.Count());
            if (smallestSession is null) return 0;
            return smallestSession.Count();
        }

        private double GetBonusForShortStay(Dictionary<int, List<Session>> candidateSchedule)
        {
            if (candidateSchedule.Values.All(x => x.Count() == 0)) return 0;
            var fullDuration = candidateSchedule.Keys.Max();
            var dayStart = candidateSchedule.First(x => x.Value.Count > 0).Key;
            var dayEnd = candidateSchedule.Last(x => x.Value.Count > 0).Key;
            var dayDuration = dayEnd - dayStart;
            return (fullDuration - dayDuration);

        }

        private bool CandidateIsDoubleBooked(Dictionary<int, List<Session>> candidateSchedule)
        {
            return candidateSchedule.Any(kvp => kvp.Value.Count() > 1);
        }

        private bool CandidateMissingRequiredSession(Dictionary<int, List<Session>> candidateSchedule, Candidate candidate)
        {
            var necessarySessions = candidate.RequiredSessions;
            var allSessions = candidateSchedule.SelectMany(x => x.Value);
            return !necessarySessions.All(sessionId => allSessions.Any(x => x.SessionId == sessionId));            
        }

        private bool CandidateGoesToUnnecessarySession(Dictionary<int, List<Session>> candidateSchedule, Candidate candidate)
        {
            var necessarySessions = candidate.RequiredSessions;
            foreach(var kvp in candidateSchedule)
            {
                if (kvp.Value.Any(x => !necessarySessions.Contains(x.SessionId))) return true;
            }
            return false;
        }

        private bool CandidateDoesNotAttendFullSession(Dictionary<int, List<Session>> candidateSchedule)
        {
            var sessionsSeen = new HashSet<Guid>();
            if (candidateSchedule.Count() == 0) return false;
            for(int i = 0; i <= candidateSchedule.Keys.Max(); i++)
            {
                if (!candidateSchedule.ContainsKey(i) || candidateSchedule[i].Count == 0)
                    continue;
                var sessionsToCheck = candidateSchedule[i].Where(x => !sessionsSeen.Contains(x.SessionId)).ToList();
                
                foreach (var session in sessionsToCheck)
                {
                    sessionsSeen.Add(session.SessionId);
                    var sessionLength = session.Duration_15Mins;
                    for(int j = i; j < i+sessionLength; j++)
                    {
                        if (!candidateSchedule.ContainsKey(j) || candidateSchedule[j].All(x => x.SessionId != session.SessionId)) return true;
                    }
                }
            }
            return false;
        }

        private bool CandidateGoesToSameSessionTwice(Dictionary<int, List<Session>> candidateSchedule)
        {
            var sessionsSeen = new HashSet<Guid>();
            if (candidateSchedule.Count() == 0) return false;

            for (int i = 0; i <= candidateSchedule.Keys.Max(); i++)
            {
                if (!candidateSchedule.ContainsKey(i) || candidateSchedule[i].Count == 0)
                    continue;
                var sessionsToCheck = candidateSchedule[i].Where(x => !sessionsSeen.Contains(x.SessionId)).ToList();

                foreach (var session in sessionsToCheck)
                {
                    sessionsSeen.Add(session.SessionId);
                    var sessionLength = session.Duration_15Mins;
                    for (int j = i + sessionLength; j <= candidateSchedule.Keys.Max(); j++)
                    {
                        if (candidateSchedule.ContainsKey(j) && candidateSchedule[j].Any(x => x.SessionId == session.SessionId)) return true;
                    }
                }
            }
            return false;
        }

        private bool SessionHasLessThanMinimumCandidates(Dictionary<int, List<Candidate>> sessionSchedule, Session session)
        {
            var sessionsWithAnyCandidates = sessionSchedule.Values.Where(x => x.Count > 0);
            var anySessionsAreUnderResourced = sessionsWithAnyCandidates.Any(x => x.Count < session.MinNumberCandidates);
            return anySessionsAreUnderResourced;
        }
    }
}
