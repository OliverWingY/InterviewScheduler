using GeneticSharp;
using InterviewScheduler;
using InterviewScheduler.Genetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewScheduler.Genetics
{
    public static class ScheduleHelper
    {
        public static Schedule Eval(IChromosome chromosome, int populationSize, double fitnessThreshold, int generationNb)
        {
            var schedule = ((IScheduleChromosome)chromosome).GetSchedule();
            var fitness = new ScheduleFitness();
            var selection = new TruncationSelection();
            var crossover = new ScheduleCrossover(schedule);
            var mutation = new ScheduleMutation(schedule);

            var population = new Population(populationSize, populationSize, chromosome);
            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new OrTermination(new ITermination[]
                {
                    new FitnessThresholdTermination(fitnessThreshold),
                    new GenerationNumberTermination(generationNb)
                })
            };

            ga.Start();

            var bestIndividual = ((IScheduleChromosome)ga.Population.BestChromosome);
            return bestIndividual.GetSchedule();
        }
    }
}
