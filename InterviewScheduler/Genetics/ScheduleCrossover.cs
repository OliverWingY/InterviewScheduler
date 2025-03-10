using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Genetics
{
    public class ScheduleCrossover : CrossoverBase
    {
        private List<int> _segmentIndecies;
        public ScheduleCrossover(Schedule schedule) : base(2,2)
        {
            _segmentIndecies = new List<int>();
            HashSet<Guid> seenSchedules = new();
            foreach (var kvp in schedule.SessionStartIndexes)
            {
                if (!seenSchedules.Contains(kvp.Value.SessionId))
                {
                    _segmentIndecies.Add(kvp.Key);
                    seenSchedules.Add(kvp.Value.SessionId);
                }
            }
        }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var rnd = new Random();
            var parent1 = parents[0];
            var parent2 = parents[1];
            var child1 = parent1.Clone();
            var child2 = parent2.Clone();
            for(int i =0; i < _segmentIndecies.Count; i++)
            {
                var startIndex = _segmentIndecies[i];
                int endIndex;
                if (i == _segmentIndecies.Count - 1)
                    endIndex = parent1.Length;
                else endIndex = _segmentIndecies[i + 1];

                var geneSet1 = parent1.GetGenes()[startIndex..endIndex];
                var geneSet2 = parent2.GetGenes()[startIndex..endIndex];
                if(rnd.NextDouble() > 0.5)
                {
                    child1.ReplaceGenes(startIndex, geneSet2);
                }
                if (rnd.NextDouble() > 0.5)
                {
                    child2.ReplaceGenes(startIndex, geneSet1);
                }
            }
            return new List<IChromosome> { child1, child2 };
        }
    }
}
