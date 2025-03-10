using FluentAssertions;
using InterviewScheduler;
using InterviewScheduler.Genetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewSchedulerTests
{
    public class ScheduleFitnessTests
    {
        [Fact]
        public void Test_DoubleBookedCandidateLosesScore()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T10:00:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId1, sessionId2})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 1, 0,3,1 ),
                new Session(sessionId2, 1, 0,3,1 )
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var matrix = schedule.ScheduleAsArray;
            matrix[0] = 1;
            matrix[2] = 1;

            var fitness = new ScheduleFitness();
            //act
            var score = fitness.GetPunishmentForBreakingRules(schedule);

            //assert
            score.Should().BeLessThan(0);
        }

        [Fact]
        public void Test_CandidateMissingSessionsLosesScore()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T10:00:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId1, sessionId2})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 1, 0,3,1 ),
                new Session(sessionId2, 1, 0,3,1 )
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var matrix = schedule.ScheduleAsArray;
            matrix[1] = 1;

            var fitness = new ScheduleFitness();
            //act
            var score = fitness.GetPunishmentForBreakingRules(schedule);

            //assert
            score.Should().BeLessThan(0);
        }

        [Fact]
        public void Test_CandidateGoingToWrongSessionsLosesScore()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T10:00:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 1, 0,3,1 ),
                new Session(sessionId2, 1, 0,3,1 )
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var matrix = schedule.ScheduleAsArray;
            matrix[0] = 1;

            var fitness = new ScheduleFitness();
            //act
            var score = fitness.GetPunishmentForBreakingRules(schedule);

            //assert
            score.Should().BeLessThan(0);
        }

        [Fact]
        public void Test_CandidateGoingToSameSessionTwiceLosesScore()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T10:00:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId1})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 1, 0,4,1 ),
                new Session(sessionId2, 1, 0,4,1 )
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var matrix = schedule.ScheduleAsArray;
            matrix[0] = 1;
            matrix[3] = 1;


            var fitness = new ScheduleFitness();
            //act
            var score = fitness.GetPunishmentForBreakingRules(schedule);

            //assert
            score.Should().Be(-1);
        }


        [Fact]
        public void Test_SessionNotFullLosesScore()
        {
            //arrange
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T10:00:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId1})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 1, 0,4,2,2 )
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var matrix = schedule.ScheduleAsArray;
            matrix[0] = 1;


            var fitness = new ScheduleFitness();
            //act
            var score = fitness.GetPunishmentForBreakingRules(schedule);

            //assert
            score.Should().Be(-1);
        }

        //[Fact]
        //public void Test_SessionBeingUsedWhenUnavailableLosesScore()
        //{
        //    //arrange
        //    var dayStart = DateTime.Parse("2025-01-01T09:00:00");
        //    var dayEnd = DateTime.Parse("2025-01-01T10:00:00");
        //    var sessionId1 = Guid.NewGuid();
        //    var sessionId2 = Guid.NewGuid();
        //    var candidates = new List<Candidate>()
        //    {
        //        new Candidate(1, "Alice", new List<Guid>() {sessionId1})
        //    };
        //    var sessions = new List<Session>()
        //    {
        //        new Session(sessionId1, 1, 2,32,1 ),
        //        new Session(sessionId2, 1, 0,32,1 )
        //    };

        //    var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
        //    var matrix = schedule.ScheduleAsArray;
        //    matrix[0, 0] = 1;


        //    var fitness = new ScheduleFitness();
        //    //act
        //    var score = fitness.GetPunishmentForBreakingRules(schedule);

        //    //assert
        //    score.Should().Be(-1);
        //}
    }
}
