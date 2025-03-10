using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Serialization
{
    public class ScheduleSerializer : IScheduleSerializer
    {
        public Schedule ReadFromXlsx(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open))
            {
                var day = DateTime.Parse("2025-01-01");
                var dayStartTime = DateTime.Parse("2025-01-01T09:00:00");
                var workbook = new XLWorkbook(stream);
                var gotWorkSheet = workbook.TryGetWorksheet("Sessions", out var sessionWorksheet);
                var sessions = new List<Session>();
                var rowCount = sessionWorksheet.RowsUsed().Count();
                for(int i = 2; i <= rowCount; i++)
                {
                    var duration = (int)(sessionWorksheet.Cell(i, 5).GetTimeSpan() / TimeSpan.FromMinutes(15));
                    var startTime = (int)((sessionWorksheet.Cell(i, 6).GetTimeSpan() - dayStartTime.TimeOfDay)/ TimeSpan.FromMinutes(15));
                    var endTime = (int)((sessionWorksheet.Cell(i, 7).GetTimeSpan() - dayStartTime.TimeOfDay) / TimeSpan.FromMinutes(15));
                    var sessionName = sessionWorksheet.Cell(i, 1).GetString().Trim();
                    var maxCandidates = sessionWorksheet.Cell(i, 3).GetValue<int>();
                    var minCandidates = sessionWorksheet.Cell(i, 4).GetValue<int>();
                    var rooms = sessionWorksheet.Cell(i, 2).GetString().Split(",").Select(x => x.Trim()).ToList();
                    sessions.Add(new Session(Guid.NewGuid(), duration, startTime, endTime, maxCandidates, minCandidates, sessionName, rooms));
                }

                var groups = new Dictionary<string, List<string>>();
                var getGroupWorksheet = workbook.TryGetWorksheet("Groups", out var groupWorksheet);
                rowCount = groupWorksheet.RowsUsed().Count();
                for (int i = 2; i <= rowCount; i++)
                {
                    var groupName = groupWorksheet.Cell(i, 1).GetString();
                    var sessionsNames = groupWorksheet.Cell(i, 2).GetString().Split(",").Select(x => x.Trim()).ToList();
                    groups.Add(groupName, sessionsNames);
                }

                var candidates = new List<Candidate>();
                var gotCandidateworksheet = workbook.TryGetWorksheet("Candidates", out var candidateWorksheet);
                rowCount = candidateWorksheet.RowsUsed().Count();
                for (int i = 2; i <= rowCount; i++)
                {
                    var name = candidateWorksheet.Cell(i, 1).GetString();
                    var group = candidateWorksheet.Cell(i, 2).GetString().Trim();
                    var sessionNames = groups[group].ToList();
                    var sessionIds = new List<Guid>();
                    foreach(var sessionName in sessionNames)
                    {
                        var session = sessions.SingleOrDefault(x => x.Name == sessionName);
                        if (session is null) 
                            throw new Exception($"Cant find session named {sessionName}");
                        sessionIds.Add(session.SessionId);
                    }
                    candidates.Add(new Candidate(i, name, sessionIds));

                }


                return new Schedule(sessions, candidates, dayStartTime, dayStartTime + TimeSpan.FromHours(8));
            }
        }

        public void WriteToXlsx(string filePath, ISchedule schedule)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                foreach(var candidate in schedule.Candidates)
                {
                    CreateAndPopulateWorkBooksForCandidate(schedule, workbook, candidate);
                }
                CreateAndPopulateFullSchedule(schedule, workbook);

                workbook.SaveAs(filePath);                
            }

        }

        private void CreateAndPopulateFullSchedule(ISchedule schedule, XLWorkbook workbook)
        {
            workbook.AddWorksheet("Full Schedule");
            var workSheet = workbook.Worksheets.First(x => x.Name == "Full Schedule");
            workSheet.Cell(3, 1).SetValue("Time");
            workSheet.Cell(1, 2).SetValue("Session");
            workSheet.Cell(2, 2).SetValue("Room");
            var times = schedule.SlotTimes;
            foreach(var kvp in schedule.SlotTimes)
            {
                workSheet.Cell(kvp.Key + 3, 2).SetValue(kvp.Value.TimeOfDay.ToString().TrimStart('0'));
            }
            var currentColumn = 3;
            foreach(var session in schedule.Sessions)
            {
                workSheet.Cell(1, currentColumn).SetValue(session.Name);
                for(int i = 0; i < session.Rooms.Count; i++)
                {
                    workSheet.Cell(2, currentColumn + i).SetValue(session.Rooms[i]);
                }
                var candidatesPerRoom = session.Rooms.Count / session.MaxNumberCandidates;
                var sessionSchedule = schedule.GetScheduleForSession(session.SessionId);
                foreach(var kvp in sessionSchedule)
                {
                    var row = kvp.Key + 3;
                    var candidates = kvp.Value;
                    if(candidatesPerRoom == 0)
                    {
                        workSheet.Cell(row, currentColumn).SetValue(string.Join(", ", candidates.Select(x=> x.Name)));
                    }
                    else
                    {
                        for (int i = 0; i < candidates.Count; i++)
                        {
                            var roomAssignment = i / candidatesPerRoom;
                            var cell = workSheet.Cell(row, roomAssignment + currentColumn);
                            var currentString = cell.GetString();
                            var newString = currentString.Trim().Length == 0 ? candidates[i].Name : $"{currentString}, {candidates[i].Name}";
                            cell.SetValue(newString);
                        }
                    }

                }
                currentColumn = currentColumn + session.Rooms.Count;
            }

        }

        private void CreateAndPopulateWorkBooksForCandidate(ISchedule schedule, XLWorkbook workbook, Candidate candidate)
        {
            workbook.AddWorksheet(candidate.Name);            
            var workSheet = workbook.Worksheets.First(x => x.Name == candidate.Name);
            PopulateCandidateSchedule(workSheet, schedule, candidate.Id);
        }

        private void PopulateCandidateSchedule(IXLWorksheet worksheet, ISchedule schedule, int candidateId)
        {
            worksheet.Cell(1, 1).SetValue("Time");
            worksheet.Cell(1, 2).SetValue("Session");
            worksheet.Cell(1, 3).SetValue("Room");

            var times = schedule.SlotTimes;
            var candidateSchedule = schedule.GetScheduleForCandidate(candidateId);
            foreach(var kvp in candidateSchedule)
            {
                var row = kvp.Key + 2;
                var time = times[kvp.Key];
                worksheet.Cell(row, 1).SetValue(time.TimeOfDay.ToString().TrimStart('0'));
                if(kvp.Value.Count == 1)
                {
                    var session = kvp.Value.First();
                    worksheet.Cell(row, 2).SetValue(session.Name);
                    if(session.Rooms.Count == 1)
                        worksheet.Cell(row, 3).SetValue(session.Rooms.First());
                    else
                    {
                        var candidatesPerRoom = session.Rooms.Count / session.MaxNumberCandidates;
                        var allCandidates = schedule.GetScheduleForSession(session.SessionId)[kvp.Key];
                        var thisCandidate = allCandidates.First(x => x.Id == candidateId);
                        var thisCandidateIndex = allCandidates.IndexOf(thisCandidate);
                        var room = session.Rooms[thisCandidateIndex / candidatesPerRoom];
                        worksheet.Cell(row, 3).SetValue(session.Rooms.First());
                    }
                }
            }
        }
    }
}
