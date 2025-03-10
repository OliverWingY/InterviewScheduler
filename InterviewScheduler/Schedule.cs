using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler
{
    public class Schedule : ISchedule
    {
        public List<Candidate> Candidates { get; init; }
        public Dictionary<int, Session> SessionStartIndexes { get; set; }
        public List<Session> Sessions { get { return SessionStartIndexes.Values.Distinct().ToList(); } }
        public Dictionary<int, DateTime> SlotTimes { get; init; }
        //Dictionary<Column, row>

        public Schedule(IEnumerable<Session> sessions, IEnumerable<Candidate> candidates, DateTime dayStart, DateTime dayEnd)
        {
            SlotTimes = new Dictionary<int, DateTime>();
            int j = 0;
            for (DateTime time = dayStart; time < dayEnd; time = time + TimeSpan.FromMinutes(15))
            {
                SlotTimes.Add(j, time);
                j++;
            }
            //setup sessions
            var numberOfBlocks = 0;
            SessionStartIndexes = new Dictionary<int, Session>();
            int i = 0;
            foreach (var session in sessions)
            {
                for (int k = 0; k < session.MaxNumberCandidates; k++)
                {
                    SessionStartIndexes.Add(i, session);
                    var blocksForSession = (session.AvailabilityEnd - session.AvailabilityStart) / session.Duration_15Mins;
                    i = i + blocksForSession;
                }
            }
            ScheduleAsArray = new int[i];

            Candidates = candidates.ToList();
        }

        public Dictionary<int, List<Session>> GetScheduleForCandidate(int candidateId)
        {
            var dictionary = new Dictionary<int, List<Session>>();
            for (int i = 0; i < SlotTimes.Count; i++)
            {
                dictionary.Add(i, new List<Session>());
            }
            foreach (var kvp in SessionStartIndexes)
            {
                var session = kvp.Value;
                var schedule = ScheduleAsArray[kvp.Key..(kvp.Key + (session.AvailabilityEnd - session.AvailabilityStart) / session.Duration_15Mins)];
                for (int i = 0; i < schedule.Count(); i++)
                {
                    if (schedule[i] == candidateId)
                    {
                        for (int k = 0; k < kvp.Value.Duration_15Mins; k++)
                        {
                            dictionary[k + i * kvp.Value.Duration_15Mins + session.AvailabilityStart].Add(kvp.Value);
                        }
                    }
                }
            }
            return dictionary;
        }

        public Dictionary<int, List<Candidate>> GetScheduleForSession(Guid sessionId)
        {
            var dictionary = new Dictionary<int, List<Candidate>>();
            for (int i = 0; i < SlotTimes.Count; i++)
            {
                dictionary.Add(i, new List<Candidate>());
            }
            foreach (var kvp in SessionStartIndexes.Where(x => x.Value.SessionId == sessionId))
            {
                var schedule = ScheduleAsArray[kvp.Key..(kvp.Key + (kvp.Value.AvailabilityEnd- kvp.Value.AvailabilityStart) / kvp.Value.Duration_15Mins)];
                for (int i = 0; i < schedule.Count(); i++)
                {
                    if (schedule[i] == 0)
                        continue;
                    var candidate = Candidates.First(x => x.Id == schedule[i]);
                    for (int k = 0; k < kvp.Value.Duration_15Mins; k++)
                    {
                        var slotId = kvp.Value.AvailabilityStart + k + i * kvp.Value.Duration_15Mins;
                        if (!dictionary[slotId].Contains(candidate))
                            dictionary[slotId].Add(candidate);
                    }
                }
            }
            return dictionary;
        }

        public int[] ScheduleAsArray { get; set; }
    }
}
