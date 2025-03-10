using FluentAssertions;
using InterviewScheduler;

namespace InterviewSchedulerTests
{
    public class ScheduleTests
    {
        [Fact]
        public void Test_GetScheduleForCandidate()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T11:00:00");
            var sessionId = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId, 4, 0,8,4)
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            //act
            var matrix = schedule.ScheduleAsArray;
            matrix[3] = 1;
            var candidateSchedule = schedule.GetScheduleForCandidate(1);
            //assert
            candidateSchedule.Should().ContainKey(4);
            candidateSchedule[4].Should().HaveCount(1);
            candidateSchedule[4].First().SessionId.Should().Be(sessionId);
        }

        [Fact]
        public void Test_GetScheduleForSession()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T13:00:00");
            var sessionId = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId}),
                new Candidate(2, "Bob", new List<Guid>() {sessionId})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId, 2, 0,8,1),
                new Session(sessionId2, 4, 8,16,2)
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            //act
            var matrix = schedule.ScheduleAsArray;
            matrix[0] = 1;
            matrix[2] = 2;
            matrix[4] = 1;
            matrix[6] = 2;
            var sessionSchedule = schedule.GetScheduleForSession(sessionId);
            //assert

            sessionSchedule[0].Should().AllSatisfy(x => x.Name.Should().Be("Alice"));
            sessionSchedule[1].Should().AllSatisfy(x => x.Name.Should().Be("Alice"));
            sessionSchedule[2].Should().HaveCount(0);
            sessionSchedule[3].Should().HaveCount(0);
            sessionSchedule[4].Should().AllSatisfy(x => x.Name.Should().Be("Bob"));
            sessionSchedule[5].Should().AllSatisfy(x => x.Name.Should().Be("Bob"));
            sessionSchedule[6].Should().HaveCount(0);
            sessionSchedule[7].Should().HaveCount(0);

            var sessionSchedule2 = schedule.GetScheduleForSession(sessionId2);
            sessionSchedule2[8].Should().HaveCount(2); 
            sessionSchedule2[9].Should().HaveCount(2); 
            sessionSchedule2[10].Should().HaveCount(2); 
            sessionSchedule2[11].Should().HaveCount(2);
            sessionSchedule2[4].Should().HaveCount(0); 
            sessionSchedule2[5].Should().HaveCount(0);
            sessionSchedule2[6].Should().HaveCount(0);
            sessionSchedule2[7].Should().HaveCount(0);
        }
    }
}