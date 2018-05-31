using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.Neural_Network_Engine
{
    public static class GeneticLearning
    {
        public static  List<DNA> getMatingPoolUsingFitnessScore(List<DNA> population, Random rand = null)
        {
            if (rand == null) rand = new Random();
            int fitnessSum = 0;
            foreach (DNA d in population)
            {
                fitnessSum += d.getScore();
            }
            

            List <DNA> matingPool = new List<DNA>();
            foreach (DNA d in population)
            {
                double probability = (double)d.getScore() / (double)fitnessSum;
                int numEntries = (int)(probability * 100000);
                Console.WriteLine($"Probability of selection = {d.getScore()}/{fitnessSum} = {probability}, NumEntries = {numEntries}");
                for (int i=0; i<numEntries; i++)
                {
                    matingPool.Add(d);
                }
            }

            return matingPool;
        }

        public static List<DNA> generateNewPopulationFromMatingPool(List<DNA> matingPool, int popSize,
            double crossoverRate, double mutationRate, double maxStepSize, Random rand = null)
        {
            if (rand == null) rand = new Random();
            List<DNA> population = new List<DNA>(popSize);

            while (population.Count() < popSize)
            {
                // Pick 2 parents
                DNA parentA = matingPool[rand.Next(0, matingPool.Count)];
                DNA parentB = matingPool[rand.Next(0, matingPool.Count)];
                DNA child;

                // See if crossover Happens
                if (rand.NextDouble() <= crossoverRate)
                {
                    // do crossover
                    child = parentA.applyCrossOver(parentB);
                    //Console.WriteLine("Crossover");
                }
                else
                {
                    // Clone one of the parents
                    child = parentA.clone();
                }

                // mutation
                int genomeSize = child.getGenomeSize();
                for (int i = 0; i < genomeSize; i++)
                {
                    if (rand.NextDouble() < mutationRate)
                    {
                        double stepSize = rand.NextDouble() * maxStepSize;
                        bool positiveLeap = (rand.Next(0, 2) == 1);
                        child.mutateAt(i, stepSize, positiveLeap);
                        //Console.WriteLine($"Mutation at {i} of {genomeSize} with stepsize of {stepSize}");
                    }
                }

                population.Add(child);
            }

            return population;
        }

        public static DNA selectUsingPc(List<DNA> sortedPopulation, double pc, Random rand = null)
        {
            if (rand == null) rand = new Random();
            
            for (int i = 0; i < sortedPopulation.Count(); i++)
            {
                if (rand.NextDouble() < pc)
                {
                    return sortedPopulation[i];
                }
            }
            return sortedPopulation[sortedPopulation.Count - 1];
        }

        public static List<DNA> generateNewPopulationFromPcSelection(List<DNA> population, double crossoverRate,
            double mutationRate, double maxStepSize, double pc, Random rand = null)
        {
            if (rand == null) rand = new Random();

            List<DNA> newPopulation = new List<DNA>(population.Count);
            while (newPopulation.Count() < population.Count())
            {
                // Pick 2 parents
                DNA parentA = selectUsingPc(population, pc, rand);
                DNA child;

                // See if crossover Happens
                if (rand.NextDouble() <= crossoverRate)
                {
                    // do crossover
                    DNA parentB = selectUsingPc(population, pc, rand);
                    child = parentA.applyCrossOver(parentB);
                    //Console.WriteLine("Crossover");
                }
                else
                {
                    // Clone one of the parents
                    child = parentA.clone();
                }

                // mutation
                int genomeSize = child.getGenomeSize();
                for (int i = 0; i < genomeSize; i++)
                {
                    if (rand.NextDouble() < mutationRate)
                    {
                        double stepSize = rand.NextDouble() * maxStepSize;
                        bool positiveLeap = (rand.Next(0, 2) == 1);
                        child.mutateAt(i, stepSize, positiveLeap);
                        //Console.WriteLine($"Mutation at {i} of {genomeSize} with stepsize of {stepSize}");
                    }
                }

                newPopulation.Add(child);
            }

            return newPopulation;
        }
    }
}
