using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Genetics
{
    public class ScheduleMutation : MutationBase
    {
        private List<int> _segmentIndecies;
        public ScheduleMutation(Schedule schedule)
        {
            _segmentIndecies = new List<int>();
            HashSet<Guid> seenSchedules = new();
            foreach(var kvp in schedule.SessionStartIndexes)
            {
                if(!seenSchedules.Contains(kvp.Value.SessionId))
                {
                    _segmentIndecies.Add(kvp.Key);
                    seenSchedules.Add(kvp.Value.SessionId);
                }
            }
            
        }
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            var rnd = new Random();
            for(int i = 0; i < _segmentIndecies.Count; i++)
            {
                if(rnd.NextDouble() < probability)
                {
                    var startIndex = _segmentIndecies[i];
                    int endIndex;
                    if (i == _segmentIndecies.Count - 1)
                        endIndex = chromosome.Length;
                    else endIndex = _segmentIndecies[i + 1];

                    var genesToShuffle = chromosome.GetGenes()[startIndex..endIndex];
                    List<Gene> shuffledGene = genesToShuffle.OrderBy(x => rnd.Next()).ToList();
                    chromosome.ReplaceGenes(startIndex, shuffledGene.ToArray());
                }
            }
        }
    }
}
