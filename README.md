This is an implementation of a genetic algorithm for creating schedules for interview days. This uses GeneticSharp for the genetic algorithm base classes and Closed.XML for excel read and writing

Usage: command line application takes a filepath to an input .xlsx file and a path to an output .xlsx file, see tests for examples

This implementation uses a custom mutator and crossover method designed to limit the number of ways a solution can be invalid

![alt text](/Documentation/Genome%20explanation.jpg)

![alt text](/Documentation/Genome%20explanation%202.jpg)

![alt text](/Documentation/Mutator%20explanation.jpg)

![alt text](/Documentation/Crossover%20explanation.jpg)

![alt text](/Documentation/Fitness%20explanation.jpg)


