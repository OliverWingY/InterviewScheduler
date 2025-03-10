using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Serialization
{
    public interface IScheduleSerializer
    {
        public Schedule ReadFromXlsx(string filePath);

        public void WriteToXlsx(string filePath, ISchedule schedule);
    }
}
