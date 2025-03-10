**Problem:**

Imagine you are tasked with organising an interview day (or some other event) where: 
  - Candidates have a list of sessions they must attend exactly once
  - Candidates must not attend any other sessions
  - Sessions have both maximum and minimum requirements for number of attendies
  - Sessions can be different lengths
  - Sessions have constraints on availability

    
It turns out that this is a very large problem space with a relatively small number of valid solutions, blindly searching the space will not work even with clever methods such as standard genetic algorithms. Instead we must constrain the search space as much as possible, removing as many modes of failure as possible.

This is a console app that implements a genetic algorithm with a custom mutator and crossover method for creating schedules. It uses GeneticSharp for the genetic algorithm base classes and Closed.XML for excel read and writing. See tests for examples of input .xlsx files

**Algorithm**:
![alt text](/Documentation/Genome%20explanation.jpg)

![alt text](/Documentation/Genome%20explanation%202.jpg)

![alt text](/Documentation/Mutator%20explanation.jpg)

![alt text](/Documentation/Crossover%20explanation.jpg)

![alt text](/Documentation/Fitness%20explanation.jpg)


