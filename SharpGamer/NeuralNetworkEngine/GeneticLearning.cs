﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGamer.NeuralNetworkEngine
{
    static class GeneticLearning
    {

        /*
         * Given a list of sorted by fitness ([0] is the fittest) neural networks:
         * This function will select one of these neural networks baced on the double "pc"
         * given. Essentially the fittest element has a (pc) chance of being selected. If
         * he is not selected the next element has a (pc) chance of being selected.
         * If none are selected the last one is used
        */
        public static NeuralNetwork SelectUsingPc(List<NeuralNetwork> sortedPopulation, double pc, Random rand)
        {
            for (var i = 0; i < sortedPopulation.Count(); i++)
            {
                if (rand.NextDouble() < pc)
                {
                    return sortedPopulation[i];
                }
            }
            return sortedPopulation[sortedPopulation.Count - 1];
        }

        /*
         * Given a population sorted by fitness ([0] is the fittest) neural networks:
         * This function will generate a new population based on "pc" selection.
         * See above function for explenation of how this selection works
        */ 
        public static List<NeuralNetwork> GeneratePopulationFromFitnessPC(List<NeuralNetwork> sortedPopulation, double crossoverRate,
            double mutationRate, double maxStepSize, double pc, Random rand)
        {
            List<NeuralNetwork> newPopulation = new List<NeuralNetwork>(sortedPopulation.Count);
            while (newPopulation.Count() < sortedPopulation.Count())
            {
                // Pick 2 parents
                NeuralNetwork parentA = SelectUsingPc(sortedPopulation, pc, rand);
                NeuralNetwork child;

                // See if crossover Happens
                if (rand.NextDouble() <= crossoverRate)
                {
                    // do crossover
                    NeuralNetwork parentB;
                    do
                    {
                        parentB = SelectUsingPc(sortedPopulation, pc, rand);
                    } while (parentA.Id.Equals(parentB.Id));

                    // selecting cutoff point for the crossover.
                    // using complete random for now. Subject to change
                    double cutOff = rand.NextDouble();
                    child = parentA.ApplyCrossoverWith(parentB, cutOff);
                }
                else
                {
                    // Clone one of the parents
                    child = parentA.Clone();
                }

                // mutation
                ApplyMutations(ref child, mutationRate, maxStepSize, rand);

                newPopulation.Add(child);
            }

            return newPopulation;
        }

        /*
         * Given a population sorted by fitness ([0] is the fittest) neural networks:
         * This function will generate a new population by sorting the population
         * by fitness AND diversity upon every selection. This will make sure that the
         * new population is as fit and diverse as possible.
        */
        public static List<NeuralNetwork> GeneratePopulationFromFitnessAndDiversityPC(List<NeuralNetwork> population, double crossoverRate,
            double mutationRate, double maxStepSize, double pc, Random rand = null)
        {
            var highestScore = population[0].Score;
            var genomeSize = population[0].GenomeSize;
            List<NeuralNetwork> newPopulation = new List<NeuralNetwork>(population.Count);

            // The centroid is basically the average point in N dimensional space
            // of all of the neural networks. This gives an average distance needed
            // to compute the diversity of a selected network to the current population
            float[] centroidDeltas = new float[genomeSize];
            for (var i=0; i<genomeSize; i++)
            {
                centroidDeltas[i] = 0f;
            }

            while (newPopulation.Count() < population.Count())
            {
                NeuralNetwork parentA;
                NeuralNetwork child;

                if (newPopulation.Count == 0)
                {
                    parentA = population[0];
                }
                else
                {
                    // averaging out the centroid deltas to get the average disntace
                    float[] centroid = (float[])centroidDeltas.Clone();
                    for (int i = 0; i < centroid.Count(); i++)
                    {
                        centroid[i] = (centroid[i] / newPopulation.Count());
                    }
                    population = SortPopulationByDiversityAndFitness(population, centroid, highestScore);
                    parentA = SelectUsingPc(population, pc, rand);
                }

                // parent A added here so that parent B would be ranked for diversity from parent A too
                newPopulation.Add(parentA);
                // will be removing these from the deltas after parent B selection (if any)
                float[] deltasToRemove = parentA.GetDNAStrand();
                for (var i = 0; i < centroidDeltas.Count(); i++)
                {
                    centroidDeltas[i] += deltasToRemove[i];
                }

                // See if crossover Happens
                if (rand.NextDouble() <= crossoverRate)
                {
                    // do crossover
                    float[] centroid = (float[])centroidDeltas.Clone();
                    for (var i = 0; i < centroid.Count(); i++)
                    {
                        centroid[i] = (centroid[i] / newPopulation.Count());
                    }
                    population = SortPopulationByDiversityAndFitness(population, centroid, highestScore);
                    NeuralNetwork parentB = SelectUsingPc(population, pc, rand);

                    child = parentA.ApplyCrossoverWith(parentB, rand.NextDouble());
                }
                else
                {
                    // Clone one of the parents
                    child = parentA.Clone();
                }
                
                // Applying mutations
                ApplyMutations(ref child, mutationRate, maxStepSize, rand);

                // Removing all traces of parentA
                newPopulation.RemoveAt(newPopulation.Count() - 1);
                for (var i = 0; i < genomeSize; i++)
                {
                    centroidDeltas[i] -= deltasToRemove[i];
                }

                // Adding child deltas
                float[] childDeltas = ((NeuralNetwork)child).GetDNAStrand();
                for (var i = 0; i < genomeSize; i++)
                {
                    centroidDeltas[i] += childDeltas[i];
                }

                newPopulation.Add(child);
            }
            
            return newPopulation;
        }

        /*
         * This function sorts the given population by fitness AND diversity.
         * i.e Math.Sqrt(Math.Pow(i.getScore(),2) + Math.Pow(i.getDiversity(),2))
        */
        public static List<NeuralNetwork> SortPopulationByDiversityAndFitness(List<NeuralNetwork> population, float[] centroid, int highestScore)
        {
            List<NeuralNetwork> newPop = new List<NeuralNetwork>(population);

            // get distances from each member of population
            // to the centroid. That will be their diversity measure
            float maxDistance = 0;
            foreach (NeuralNetwork network in newPop)
            {
                float[] dnaString = network.GetDNAStrand();

                // Euclidean distance is cheapest of N dimensional distance functions.
                float distance = MathNet.Numerics.Distance.Euclidean(centroid, dnaString);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
                network.Diversity = distance;
            }

            // now that we know the max distance we can normalise diversity measure to fitness
            // score for even weighted dimensions
            foreach (NeuralNetwork network in newPop)
            {
                network.Diversity = (network.Diversity/maxDistance)*((float)highestScore + 0.01f);
            }

            // sort list by distance from origin on both axis
            newPop = newPop.OrderBy(i => Math.Sqrt(Math.Pow(i.Score,2) + Math.Pow(i.Diversity,2))).ToList();
            newPop.Reverse();
            return newPop;
        }

        /*
         * Applies mutations to a subject NeuralNetwork based on the parameters.
         * mutationRate is the fraction of the genome that will be mutated.
         * maxStepSize is the maximum fraction of the Gene at the edit index
         * to be added or subtracted to the current value.
        */
        public static void ApplyMutations(ref NeuralNetwork subject, double mutationRate, double maxStepSize, Random rand)
        {
            int numMutations = 0;
            var genomeSize = subject.GenomeSize;
            for (var i = 0; i < genomeSize; i++)
            {
                if (rand.NextDouble() < mutationRate)
                {
                    double stepSize = rand.NextDouble() * maxStepSize;
                    bool positiveLeap = (rand.Next(0, 2) == 1);
                    subject.MutateGeneAtIndex(i, stepSize, positiveLeap);
                    numMutations++;
                }
            }

            Console.WriteLine($"Num mutations: {numMutations}.");
        }
    }
}
