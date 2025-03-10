
namespace InterviewScheduler
{
    public interface ISchedule
    {
        List<Candidate> Candidates { get;  }
        List<Session> Sessions { get; }
        Dictionary<int, DateTime> SlotTimes { get;  }

        Dictionary<int, List<Session>> GetScheduleForCandidate(int candidateId);
        Dictionary<int, List<Candidate>> GetScheduleForSession(Guid sessionId);
    }
}