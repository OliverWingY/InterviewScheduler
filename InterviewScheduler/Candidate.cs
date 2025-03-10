using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler
{
    public class Candidate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Guid> RequiredSessions { get; set; }
        public Candidate(int id, string name, List<Guid> sessions)
        {
            Id = id;
            Name = name;
            RequiredSessions = sessions;
        }
    }
}
