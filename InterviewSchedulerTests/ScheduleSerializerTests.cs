using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentAssertions;
using InterviewScheduler;
using InterviewScheduler.Serialization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewSchedulerTests
{
    public class ScheduleSerializerTests
    {
        [Fact]
        public void Test_CanReadCandidatesAndSessionsFromXlsx()
        {
            //arrange
            var expectedCandidateNames = new List<string>()
            {
                "Alice",
                "Bob",
                "Charlie",
                "Daniel",
                "Ella",
                "Frank",
                "Gretta",
                "Harry",
                "Imogen",
                "Jack"
            };
            var expectedSessionNames = new List<string>()
            {
                "Business fit",
                "Group",
                "Mechanical technical",
                "Materials technical",
                "Expectations",
                "Preparation"
            };

            var filePath = "./TestData/TestSchedule.xlsx";
            var scheduleSerializer = new ScheduleSerializer();
            //act
            var schedule = scheduleSerializer.ReadFromXlsx(filePath);
            //assert
            schedule.Candidates.Select(x => x.Name).Should().BeEquivalentTo(expectedCandidateNames);
            schedule.Sessions.Select(x => x.Name).Should().BeEquivalentTo(expectedSessionNames);
            schedule.Candidates.Should().AllSatisfy(x => x.RequiredSessions.Should().HaveCount(5));
            var mechanicalTechnicalId = schedule.Sessions.Single(x => x.Name == "Mechanical technical").SessionId;
            var materialsTechnicalId = schedule.Sessions.Single(x => x.Name == "Materials technical").SessionId;
            var mechCandidates = schedule.Candidates.Where(x => x.RequiredSessions.Contains(materialsTechnicalId));
            var matCandidates = schedule.Candidates.Where(x => x.RequiredSessions.Contains(mechanicalTechnicalId));
            mechCandidates.Should().HaveCount(5);
            matCandidates.Should().HaveCount(5);
            matCandidates.Intersect(mechCandidates).Should().HaveCount(0);
        }

        [Fact]
        public void Test_CanSerializeCandidatesToXlsx()
        {
            //arrange
            var expectedFilePath = "./TestData/ExpectedSchedule.xlsx";
            var candidate = new Candidate(1, "alice", new List<Guid>());
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();
            var sessions = new List<Session>()
            {
                new Session(sessionId1, 4, 0, 8, 1,1, "Session 1", new List<string>() {"Apache", "Meteor" }),
                new Session(sessionId2, 4, 0, 8, 1,1, "Session 2", new List<string>() {"Comet" })

            };
            var times = new Dictionary<int, DateTime>()
            {
                {0, DateTime.Parse("2025-01-01T09:00:00") },
                {1, DateTime.Parse("2025-01-01T09:15:00") },
                {2, DateTime.Parse("2025-01-01T09:30:00") },
                {3, DateTime.Parse("2025-01-01T09:45:00") },
                {4, DateTime.Parse("2025-01-01T10:00:00") },
                {5, DateTime.Parse("2025-01-01T10:15:00") },
                {6, DateTime.Parse("2025-01-01T10:30:00") },
                {7, DateTime.Parse("2025-01-01T10:45:00") },
                {8, DateTime.Parse("2025-01-01T11:00:00") }
            };

            var candidateSchedule = new Dictionary<int, List<Session>>()
            {
                {0, new List<Session>() { sessions.First() }  },
                {1, new List<Session>() { sessions.First() } },
                {2, new List<Session>() { sessions.First() } },
                {3, new List<Session>() { sessions.First() } },
                {4, new List<Session>() { sessions.Last() } },
                {5, new List<Session>() { sessions.Last() } },
                {6, new List<Session>() { sessions.Last() } },
                {7, new List<Session>() { sessions.Last() } },
                {8, new List<Session>() { } }
            };

            var session1Schedule = new Dictionary<int, List<Candidate>>()
            {
                {0, new List<Candidate>() { candidate }  },
                {1, new List<Candidate>() { candidate } },
                {2, new List<Candidate>() { candidate } },
                {3, new List<Candidate>() { candidate } },
                {4, new List<Candidate>() { } },
                {5, new List<Candidate>() { } },
                {6, new List<Candidate>() { } },
                {7, new List<Candidate>() { } },
                {8, new List<Candidate>() { } }
            };

            var session2Schedule = new Dictionary<int, List<Candidate>>()
            {
                {0, new List<Candidate>() {  }  },
                {1, new List<Candidate>() {  } },
                {2, new List<Candidate>() {  } },
                {3, new List<Candidate>() {  } },
                {4, new List<Candidate>() {candidate} },
                {5, new List<Candidate>() {candidate} },
                {6, new List<Candidate>() {candidate} },
                {7, new List<Candidate>() { candidate } },
                {8, new List<Candidate>() { } }
            };

            var mockSchedule = new Mock<ISchedule>();
            mockSchedule.SetupGet(x => x.Sessions).Returns(sessions);
            mockSchedule.SetupGet(x => x.SlotTimes).Returns(times);
            mockSchedule.SetupGet(x => x.Candidates).Returns(new List<Candidate>() { candidate });
            mockSchedule.Setup(x => x.GetScheduleForCandidate(It.IsAny<int>())).Returns(candidateSchedule);
            mockSchedule.Setup(x => x.GetScheduleForSession(It.Is<Guid>(x => x == sessionId1))).Returns(session1Schedule);
            mockSchedule.Setup(x => x.GetScheduleForSession(It.Is<Guid>(x => x == sessionId2))).Returns(session2Schedule);
            var filePathToWriteTo = "./output.xlsx";
            var scheduleSerializer = new ScheduleSerializer();
            //act
            scheduleSerializer.WriteToXlsx(filePathToWriteTo, mockSchedule.Object);
            //assert
            using (var stream = File.Open(expectedFilePath, FileMode.Open))
            {
                var expectedWorkbook = new XLWorkbook(stream);
                expectedWorkbook.TryGetWorksheet("Full Schedule", out var expectedFullScheduleWorkSheet).Should().BeTrue();
                expectedWorkbook.TryGetWorksheet("Alice", out var expectedCandidateWorkSheet).Should().BeTrue();
                using (var actualFile = File.Open(filePathToWriteTo, FileMode.Open))
                {
                    var actualWorkbook = new XLWorkbook(actualFile);

                    actualWorkbook.TryGetWorksheet("Full Schedule", out var actualFullScheduleWorkSheet).Should().BeTrue();
                    actualWorkbook.TryGetWorksheet("Alice", out var actualCandidateWorkSheet).Should().BeTrue();

                    for(int i = 1; i < 5; i++)
                    {
                        for(int j = 1; j < 12; j++)
                        {
                            actualCandidateWorkSheet.Cell(j, i).GetString().Should().BeEquivalentTo(expectedCandidateWorkSheet.Cell(j, i).GetString());
                            actualFullScheduleWorkSheet.Cell(j, i).GetString().Should().BeEquivalentTo(expectedFullScheduleWorkSheet.Cell(j, i).GetString());

                        }
                    }
                }

            }

        }
    }
}
