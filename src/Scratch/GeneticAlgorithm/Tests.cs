//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace Scratch.GeneticAlgorithm
{
    internal class ga_struct
    {
        public ga_struct(string str, uint fitness)
        {
            Genes = str;
            Fitness = fitness;
        }

        public uint Fitness { get; set; }
        public string Genes { get; set; }
    }

    /// <summary>
    /// http://stackoverflow.com/questions/948691/genetic-programming-implementation
    /// 
    /// example:
    /// Generation     1 best: Ho*fSOv@rG)! (9)
    /// Generation     2 best: HelfSOvArGV! (7)
    /// Generation     3 best: Hel@S!/@rld! (5)
    /// Generation     4 best: HelloOuRrld! (3)
    /// Generation     6 best: Hello!$@rld! (3)
    /// Generation     8 best: Hello!u@rld! (3)
    /// Generation     9 best: HelloO/@rld! (3)
    /// Generation    10 best: Hello t@rld! (2)
    /// Generation    11 best: Hello tRrld! (2)
    /// Generation    12 best: Hello uRrld! (2)
    /// Generation    16 best: Hello u@rld! (2)
    /// Generation    30 best: Hello w@rld! (1)
    /// Generation   127 best: Hello whrld! (1)
    /// Generation   133 best: Hello wprld! (1)
    /// Generation   261 best: Hello wYrld! (1)
    /// Generation   287 best: Hello world! (0)
    /// </summary>
    [TestFixture]
    public class Tests
    {
        private const float GaElitrate = 0.10f;
        private const int GaMaxGenerations = 16384;
        private const float GaMutationRate = 0.25f;
        private const int GaPopsize = 2048;
        private const string GaTarget = "Hello world!";
        private const float SlideRate = 0.001f;

        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private static float _slidingMutationRate = GaMutationRate;

        [Test]
        public void Show()
        {
            var popAlpha = new List<ga_struct>();
            var popBeta = new List<ga_struct>();

            const string genes = @"`1234567890-=~!@#$%^&*()_+qwertyuiop[]\QWERTYUIOP{}|asdfghjkl;'ASDFGHJKL:""zxcvbnm,./ZXCVBNM<>? ";
            Func<char> getRandomGene = () => genes[Random.Next(genes.Length)];

            InitPopulation(ref popAlpha, ref popBeta, getRandomGene);
            var population = popAlpha;
            var buffer = popBeta;
            Func<char, char, int> calcDistanceFromTarget = (curr, target) => (curr == target ? 0 : 1);
            var previousBests = new HashSet<string>();
            _slidingMutationRate = GaMutationRate;
            for (int i = 0; i < GaMaxGenerations; i++)
            {
                CalcFitness(ref population, calcDistanceFromTarget);
                SortByFitness(ref population);
                if (!previousBests.Contains(population[0].Genes))
                {
                    PrintBest(i, ref population);
                    previousBests.Add(population[0].Genes);
                    _slidingMutationRate = GaMutationRate;
                }
                else
                {
                    _slidingMutationRate = Math.Max(_slidingMutationRate - SlideRate, 0);
                }

                if (population[0].Fitness == 0)
                {
                    break;
                }

                Mate(ref population, ref buffer, getRandomGene);
                var temp = buffer;
                buffer = population;
                population = temp;
            }
        }

        private static void CalcFitness(ref List<ga_struct> population, Func<char, char, int> calcDistanceFromTarget)
        {
            const string target = GaTarget;
            int tsize = target.Length;

            for (int i = 0; i < GaPopsize; i++)
            {
                uint fitness = 0;
                var gaStruct = population[i];
                for (int j = 0; j < tsize; j++)
                {
                    try
                    {
                        string str = gaStruct.Genes;
                        fitness += (uint)calcDistanceFromTarget(str[j], target[j]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                gaStruct.Fitness = fitness;
            }
        }

        private static void Elitism(ref List<ga_struct> population, ref List<ga_struct> buffer, int esize)
        {
            for (int i = 0; i < esize; i++)
            {
                buffer[i].Genes = population[i].Genes;
                buffer[i].Fitness = population[i].Fitness;
            }
        }

        private static int FitnessSort(ga_struct x, ga_struct y)
        {
            return x.Fitness.CompareTo(y.Fitness);
        }

        private static void InitPopulation(ref List<ga_struct> population, ref List<ga_struct> buffer, Func<char> getRandomGene)
        {
            int tsize = GaTarget.Length;
            for (int i = 0; i < GaPopsize; i++)
            {
                var citizen = new ga_struct(string.Empty, 0);

                for (int j = 0; j < tsize; j++)
                {
                    citizen.Genes += getRandomGene();
                }

                population.Add(citizen);
                buffer.Add(new ga_struct(string.Empty, 0));
            }
        }

        private static void Mate(ref List<ga_struct> population, ref List<ga_struct> buffer, Func<char> getRandomGene)
        {
            const int esize = (int)(GaPopsize * GaElitrate);
            int tsize = GaTarget.Length;

            Elitism(ref population, ref buffer, esize);

            double avgFitness = population.Average(x => x.Fitness);
            int avgFitnessPoint = population.TakeWhile(x => x.Fitness <= avgFitness).Count();
            int parentCutoff = Math.Max(10, Math.Min(avgFitnessPoint / 10, 10));

            for (int i = esize; i < GaPopsize; i++)
            {
                int option = Random.Next(2);
                switch (option)
                {
                    case 0:
                    {
                        int i1 = Random.Next(parentCutoff);
                        int i2 = Random.Next(parentCutoff);
                        int spos = Random.Next(tsize);

                        var parentB = population[i2].Genes.ToArray();
                        var bufferChild = population[i1].Genes.ToArray();
                        for (int j = 0; j < spos; j++)
                        {
                            int index0 = Random.Next(tsize);
                            bufferChild[index0] = parentB[index0];
                        }
                        buffer[i].Genes = new string(bufferChild);
                        break;
                    }
                    case 1:
                    {
                        buffer[i] = Mutate(population[i], getRandomGene);
                        break;
                    }
                }
            }
        }

        private static ga_struct Mutate(ga_struct member, Func<char> getRandomGene)
        {
            int tsize = GaTarget.Length;
            var mutated = member.Genes.ToCharArray();
            float mutationRate = Math.Max(1, tsize * _slidingMutationRate);
            for (int i = 0; i < mutationRate; i++)
            {
                try
                {
                    mutated[Random.Next(tsize)] = getRandomGene();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return new ga_struct(new string(mutated), Int32.MaxValue);
        }

        private static void PrintBest(int generation, ref List<ga_struct> gav)
        {
            Console.WriteLine("Generation {0} best: {1} ({2})",
                              (1 + generation).ToString().PadLeft(GaMaxGenerations.ToString().Length),
                              gav[0].Genes,
                              gav[0].Fitness);
        }

        private static void SortByFitness(ref List<ga_struct> population)
        {
            population.Sort(FitnessSort);
        }
    }
}