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
    public class ScheduleChromosomeTests
    {
        [Fact]
        public void Test_CanCreateSimpleValidSchedule()
        {
            //arrange
            var fitness = new ScheduleFitness();
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T09:30:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var sessionId3 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId1, sessionId3}),
                new Candidate(2, "Bob", new List<Guid>() {sessionId2, sessionId3})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 1, 0,2,1 ),
                new Session(sessionId2, 1, 0,2,1 ),
                new Session(sessionId3, 1, 0,2,2 ),
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var chromosome = new ScheduleChromosome(schedule);
            //act
            var bestIndividual = ScheduleHelper.Eval(chromosome, 100, 0, 10000);
            //assert
            fitness.GetPunishmentForBreakingRules(bestIndividual).Should().Be(0);
        }

        [Fact]
        public void Test_CanCreateMediumComplexityValidSchedule()
        {
            //arrange
            var fitness = new ScheduleFitness();
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T11:00:00");
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var sessionId3 = Guid.NewGuid();
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() {sessionId1, sessionId3}),
                new Candidate(2, "Bob", new List<Guid>() {sessionId2, sessionId3})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 4, 0,8,1 ),
                new Session(sessionId2, 4, 0,8,1 ),
                new Session(sessionId3, 4, 0,8,2 ),
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var chromosome = new ScheduleChromosome(schedule);
            //act
            var bestIndividual = ScheduleHelper.Eval(chromosome, 100, 0, 10000);
            //assert
            fitness.GetPunishmentForBreakingRules(bestIndividual).Should().Be(0);
        }

        [Fact]
        public void Test_CanCreateHighComplexityValidSchedule()
        {
            //arrange
            var fitness = new ScheduleFitness();
            var dayStart = DateTime.Parse("2025-01-01T09:00:00");
            var dayEnd = DateTime.Parse("2025-01-01T17:00:00");
            var sessionIds = new List<Guid>();
            for (int i = 0; i < 6; i++)
            {
                sessionIds.Add(Guid.NewGuid());
            }
            var candidates = new List<Candidate>()
            {
                new Candidate(1, "Alice", new List<Guid>() { sessionIds[0], sessionIds[1], sessionIds[2], sessionIds[5]}),
                new Candidate(2, "Bob", new List<Guid>() {sessionIds[0], sessionIds[1], sessionIds[2],  sessionIds[5]}),
                new Candidate(3, "Charlie", new List<Guid>() {sessionIds[0], sessionIds[1], sessionIds[2],  sessionIds[5]}),
                new Candidate(4, "Dave", new List<Guid>() {sessionIds[0], sessionIds[1], sessionIds[2],  sessionIds[5]}),
                new Candidate(5, "Ella", new List<Guid>() {sessionIds[0], sessionIds[4], sessionIds[3], sessionIds[5]}),
                new Candidate(6, "Frank", new List<Guid>() {sessionIds[0], sessionIds[4], sessionIds[3], sessionIds[5]}),
                new Candidate(7, "Grace", new List<Guid>() {sessionIds[0], sessionIds[4], sessionIds[3], sessionIds[5]}),
                new Candidate(8, "Harry", new List<Guid>() {sessionIds[0], sessionIds[4], sessionIds[3], sessionIds[5]})
            };
            var sessions = new List<Session>()
            {
                new Session(sessionIds[0], 4, 0,32,4 ),
                new Session(sessionIds[1], 4, 0,32,1 ),
                new Session(sessionIds[2], 4, 0,32,2 ),
                new Session(sessionIds[3], 4, 0,32,1 ),
                new Session(sessionIds[4], 4, 0,32,2 ),
                new Session(sessionIds[5], 4, 0,32,4 ),
            };

            var schedule = new Schedule(sessions, candidates, dayStart, dayEnd);
            var chromosome = new ScheduleChromosome(schedule);
            //act
            var bestIndividual = ScheduleHelper.Eval(chromosome, 1000, 10, 100);
            //assert
            fitness.GetPunishmentForBreakingRules(bestIndividual).Should().Be(0);
        }

    }
}
