using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler
{
    public class Session
    {
        public Session(Guid id, int duration, int availabilityStart, int availabilityEnd, int maxNumberCandidates, int minNumberCandidates = 1, string name = null, List<string> rooms = null)
        {
            Name = name?? id.ToString();
            Rooms = rooms ?? new List<string>();
            SessionId = id;
            Duration_15Mins = duration;
            AvailabilityStart = availabilityStart;
            AvailabilityEnd = availabilityEnd;
            MaxNumberCandidates = maxNumberCandidates;
            MinNumberCandidates = minNumberCandidates;
        }
        public string Name { get; set; }
        public List<string> Rooms { get; set; }
        public Guid SessionId { get; set; }
        public int Duration_15Mins { get; set; }
        public int AvailabilityStart { get; set; }
        public int AvailabilityEnd { get; set; }
        public int MaxNumberCandidates { get; set; }
        public int MinNumberCandidates { get; set; }
    }
}
