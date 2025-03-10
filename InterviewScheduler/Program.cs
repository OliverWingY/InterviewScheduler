using InterviewScheduler.Genetics;
using InterviewScheduler.Serialization;
using System;

namespace InterviewScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Auto-Scheduler <CandidatesAndRoomsFilePath> <OutputFilePath> <Number of iterations (recommended 100~500)> <Score threshold (recommended 50~100)>");
                return;
            }
            int iterations = 500;
            int maxScore = 100;
            string inputFilePath = args[0];
            string outputFilePath = args[1];
            if (args.Count() > 2)
                iterations = int.Parse(args[2]);
            if (args.Count() > 3)
                iterations = int.Parse(args[3]);
            var scheduleSerializer = new ScheduleSerializer();
            Console.WriteLine("Reading input");
            var schedule = scheduleSerializer.ReadFromXlsx(inputFilePath);
            Console.WriteLine("Finding best schedule");
            var chromosome = new ScheduleChromosome(schedule);
            var bestIndividual = ScheduleHelper.Eval(chromosome, 100, maxScore, iterations);
            var fitness = new ScheduleFitness();
            if (fitness.GetPunishmentForBreakingRules(bestIndividual) < 0)
            {
                Console.WriteLine("Failed to find valid schedule, writing nearest");
                scheduleSerializer.WriteToXlsx(outputFilePath, bestIndividual);
            }
            else
            {
                scheduleSerializer.WriteToXlsx(outputFilePath, bestIndividual);
                Console.WriteLine("Finished!");
            }
        }
    }
}