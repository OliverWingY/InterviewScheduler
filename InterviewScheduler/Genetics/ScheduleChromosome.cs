using GeneticSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Genetics
{
    public class ScheduleChromosome : ChromosomeBase, IScheduleChromosome
    {
        private readonly Schedule _targetSchedule;
        private readonly int _maxCandidateId;
        public ScheduleChromosome(Schedule schedule) : base((int)schedule.ScheduleAsArray.Length)
        {
            _targetSchedule = schedule;
            _maxCandidateId = _targetSchedule.Candidates.MaxBy(x => x.Id).Id;

            //replace this with something that has a high chance of creating a valid schedule.
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i,0));
            }
            foreach(var session in schedule.Sessions)
            {
                var sessionIndexStart = schedule.SessionStartIndexes.First(x => x.Value == session).Key;
                int sessionIndexEnd;
                if (schedule.SessionStartIndexes.Any(x => x.Value != session && x.Key > sessionIndexStart))
                    sessionIndexEnd = schedule.SessionStartIndexes.FirstOrDefault(x => x.Value != session && x.Key > sessionIndexStart).Key;
                else
                    sessionIndexEnd = schedule.ScheduleAsArray.Length;
                HashSet<int> genesForSession = new HashSet<int>();

                for (int i = sessionIndexStart; i < sessionIndexEnd; i++)
                {
                    genesForSession.Add(i);
                }
                var candidateIds = schedule.Candidates.Where(x => x.RequiredSessions.Contains(session.SessionId)).Select(x => x.Id);

                Random random = new Random();
                foreach(var candidateId in candidateIds)
                {
                    int setIndex = random.Next(genesForSession.Count);
                    var geneIndex = genesForSession.ElementAt(setIndex);
                    ReplaceGene(geneIndex, GenerateGene(geneIndex, candidateId));
                    genesForSession.Remove(geneIndex);
                }
            }
        }

        public override IChromosome CreateNew()
        {
            return new ScheduleChromosome(_targetSchedule);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = RandomizationProvider.Current;
            var id = rnd.GetInt(0, _maxCandidateId + 1);
            return new Gene(id);
        }

        public Gene GenerateGene(int geneIndex, int value)
        {
            return new Gene(value);
        }

        public Schedule GetSchedule()
        {
            var schedule = new Schedule(_targetSchedule.Sessions, _targetSchedule.Candidates, _targetSchedule.SlotTimes[0], _targetSchedule.SlotTimes.Values.Max()+TimeSpan.FromMinutes(1));
            var genes = GetGenes();
            for (int i = 0; i < genes.Length; i++)
            {
                schedule.ScheduleAsArray[i] = (int)genes[i].Value;
            }

            return schedule;
        }
    }
}
