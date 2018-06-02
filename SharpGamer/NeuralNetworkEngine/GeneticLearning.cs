using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace SharpGamer.NeuralNetworkEngine
{
    public static class GeneticLearning
    {
        public static int aaa = 0;

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
                    DNA parentB;
                    do
                    {
                        parentB = selectUsingPc(population, pc, rand);
                    } while (parentA.getid().Equals(parentB.getid()));
                    
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

        public static List<DNA> generateNewPopulationFromDiversityAndFitness(List<DNA> population, double crossoverRate,
    double mutationRate, double maxStepSize, double pc, Random rand = null)
        {
            int highestScore = population[0].getScore();
            int genomeSize = population[0].getGenomeSize();
            List<DNA> newPopulation = new List<DNA>(population.Count);

            float[] centroidDeltas = new float[genomeSize];
            for (int i=0; i<genomeSize; i++)
            {
                centroidDeltas[i] = 0f;
            }

            while (newPopulation.Count() < population.Count())
            {
                SharpNeuralNetwork parentA;
                SharpNeuralNetwork child;

                if (newPopulation.Count == 0)
                {
                    parentA = (SharpNeuralNetwork)population[0];
                }
                else
                {
                    float[] centroid = (float[])centroidDeltas.Clone();
                    for (int i = 0; i < centroid.Count(); i++)
                    {
                        centroid[i] = (centroid[i] / newPopulation.Count());
                    }
                    population = sortPopulationByDiversityAndFitness(population, centroid, highestScore);
                    parentA = (SharpNeuralNetwork) selectUsingPc(population, pc, rand);
                }

                // parent A added here so that parent B would be ranked for diversity on parent a too
                newPopulation.Add(parentA); 
                float[] deltasToRemove = parentA.getDnaAsArray();
                for (int i=0; i<centroidDeltas.Count(); i++)
                {
                    centroidDeltas[i] += deltasToRemove[i];
                }

                // See if crossover Happens
                if (rand.NextDouble() <= crossoverRate)
                {
                    // do crossover
                    float[] centroid = (float[])centroidDeltas.Clone();
                    for (int i = 0; i < centroid.Count(); i++)
                    {
                        centroid[i] = (centroid[i] / newPopulation.Count());
                    }
                    population = sortPopulationByDiversityAndFitness(population, centroid, highestScore);
                    SharpNeuralNetwork parentB = (SharpNeuralNetwork)selectUsingPc(population, pc, rand);

                    child = (SharpNeuralNetwork)parentA.applyCrossOver(parentB);
                    //Console.WriteLine("Crossover");
                }
                else
                {
                    // Clone one of the parents
                    child = (SharpNeuralNetwork)parentA.clone();
                }

                // mutation
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

                // Removing all traces of parentA
                newPopulation.RemoveAt(newPopulation.Count() - 1);
                for (int i = 0; i < centroidDeltas.Count(); i++)
                {
                    centroidDeltas[i] -= deltasToRemove[i];
                }

                newPopulation.Add(child);

                Console.WriteLine(aaa);
                aaa++;
            }


            aaa = 0;
            return newPopulation;
        }

        public static float getNDimensionalDistance(float[] a, float[] b)
        {
            float sum = 0;
            for (int i=0; i<a.Length; i++)
            {
                sum += (float)Math.Pow(a[0] - b[0], 2);
            }
            return (float)Math.Sqrt(sum);
        }

        // This function is not generalised TODO
        public static List<DNA> sortPopulationByDiversityAndFitness(List<DNA> population, float[] centroid, int highestScore)
        {
            List<DNA> newPop = new List<DNA>(population);

            // get distances from each member of population
            // to the centroid. That will be their diversity measure
            float maxDistance = 0;
            foreach (SharpNeuralNetwork network in newPop)
            {
                float[] dnaString = network.getDnaAsArray();
                float distance = MathNet.Numerics.Distance.Euclidean(centroid, dnaString);
                //float distance = getNDimensionalDistance(centroid, dnaString);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
                network.diversity = distance;
            }

            // now that we know the max distance we can normalise diversity measure to fitness
            // score for even weighted dimensions
            foreach (SharpNeuralNetwork network in newPop)
            {
                network.diversity = (network.diversity/maxDistance)*((float)highestScore + 0.01f);
            }

            // sort list by distance from origin on both axis
            newPop = newPop.OrderBy(i => Math.Sqrt(Math.Pow(i.getScore(),2) + Math.Pow(i.getDiversity(),2))).ToList();
            newPop.Reverse();
            return newPop;
        }
    }
}
