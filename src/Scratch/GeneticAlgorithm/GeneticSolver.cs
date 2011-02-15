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
using System.Text;

using Scratch.ShuffleIEnumerable;

namespace Scratch.GeneticAlgorithm
{
    internal class GeneSequence
    {
        public GeneSequence(string str, uint fitness)
        {
            Genes = str;
            Fitness = fitness;
        }

        public uint Fitness { get; set; }
        public string Genes { get; set; }

        public string HowCreated { get; set; }

        public GeneSequence Clone()
        {
            return new GeneSequence(Genes, Fitness);
        }

        public override string ToString()
        {
            string dispGenes = Genes ?? "";
            if (dispGenes.Length > 20)
            {
                dispGenes = dispGenes.Substring(0, 20) + " ...";
            }

            return dispGenes + " fitness: " + Fitness;
        }
    }

    public class GeneticSolver
    {
        private const decimal ChildCreationTypeStep = .5m;
        private const int DefaultActiveFillPercentage = 1;
        private const int DefaultCrossoverPercentage = 59;
        private const int DefaultMaxGenerationsWithoutImprovement = 16384;
        private const int DefaultMutationPercentage = 20;
        private const float DefaultMutationRate = 0.25f;
        private const int DefaultSplicePercentage = 20;
        private const int EliteParentCount = (int)(GaPopsize * GaElitrate);
        private const float GaElitrate = 0.10f;
        private const int GaPopsize = 2048;
        private const int MaxImprovmentsToKeepFromEachRound = 5;
        private const decimal MinActiveFillPercentage = 1m;
        private const int MinCrossoverPercentage = 5;
        private const int MinMutationPercentage = 7;
        private const int MinSplicePercentage = 5;
        private const int RandomCitizenCount = 10;

        private const float SlideRate = 0.001f;
        private static readonly int MinGaPopSize = (int)Math.Sqrt(GaPopsize);

        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
        private static float _slidingMutationRate = DefaultMutationRate;
        private readonly int _gaMaxGenerationsWithoutImprovement = DefaultMaxGenerationsWithoutImprovement;
        private decimal _activeFillPercentage;
        private decimal _crossoverPercentage;
        private decimal _mutationPercentage;
        private decimal _splicePercentage;

        public GeneticSolver()
            : this(DefaultMaxGenerationsWithoutImprovement)
        {
        }

        public GeneticSolver(int maxGenerationsWithoutImprovement)
        {
            _splicePercentage = DefaultSplicePercentage;
            _crossoverPercentage = DefaultCrossoverPercentage;
            _mutationPercentage = DefaultMutationPercentage;
            _activeFillPercentage = DefaultActiveFillPercentage;

            _gaMaxGenerationsWithoutImprovement = maxGenerationsWithoutImprovement;
            DisplayGenes = (generation, fitness, genes, howCreated) => Console.WriteLine("Generation {0} fitness {1}: {2}", generation.ToString().PadLeft(_gaMaxGenerationsWithoutImprovement.ToString().Length), fitness, genes);
        }

        public Action<int, uint, string, string> DisplayGenes { get; set; }
        public bool DisplayHowCreatedPercentages { get; set; }
        public bool UseFastSearch { get; set; }

        private static IEnumerable<GeneSequence> CalcFitness(IList<GeneSequence> population, Func<string, uint> calcDistanceFromTarget)
        {
            for (int i = 0; i < GaPopsize; i++)
            {
                var geneSequence = population[i];
                geneSequence.Fitness = calcDistanceFromTarget(geneSequence.Genes);
                yield return geneSequence;
            }
        }

        private static void CopyBestParents(IList<GeneSequence> population, IList<GeneSequence> buffer)
        {
            for (int i = 0; i < EliteParentCount; i++)
            {
                buffer[i] = population[i].Clone();
            }
        }

        private static void CopyPreviousBests(IEnumerable<GeneSequence> previousBests, IList<GeneSequence> buffer)
        {
            int lastBufferPosition = buffer.Count - 1;
            var source = previousBests.Shuffle().Take(EliteParentCount).ToList();
            for (int i = 0; i < EliteParentCount && i < source.Count; i++)
            {
                int indexFromEnd = lastBufferPosition - i;
                if (indexFromEnd < EliteParentCount)
                {
                    break;
                }
                buffer[indexFromEnd] = source[i].Clone();
            }
        }

        private void CreateNextGeneration(int numberOfGenesToUse, List<GeneSequence> population, IList<GeneSequence> buffer, Func<char> getRandomGene, IEnumerable<GeneSequence> previousBests, string possibleGenes)
        {
            CopyBestParents(population, buffer);
            CopyPreviousBests(previousBests, population);
            SortByFitness(population);

            GenerateChildren(numberOfGenesToUse, population, buffer, getRandomGene, possibleGenes);
        }

        private static GeneSequence FindActiveFill(int numberOfGenesToUse, IEnumerable<GeneSequence> parents, string possibleGenes, Func<char> getRandomGene)
        {
            var activeWithGap = Enumerable.Range(0, numberOfGenesToUse)
                .Shuffle()
                .Select(index => new
                    {
                        index,
                        used = new HashSet<char>(parents.Select(x => x.Genes[index]))
                    })
                .FirstOrDefault(x => x.used.Count > 1 && x.used.Count < possibleGenes.Length);

            var parent = parents.Shuffle().First();
            if (activeWithGap == null)
            {
                return Mutate(numberOfGenesToUse, parent, getRandomGene);
            }

            char unusedGene = possibleGenes.Shuffle()
                .First(x => !activeWithGap.used.Contains(x));

            var child = parent.Clone();
            var genes = child.Genes.ToCharArray();
            genes[activeWithGap.index] = unusedGene;
            child.Genes = new String(genes);
            child.HowCreated = ChildCreationTypeDescription.ActiveFill;
            return child;
        }

        private static int FitnessSort(GeneSequence x, GeneSequence y)
        {
            return x.Fitness.CompareTo(y.Fitness);
        }

        private void GenerateChildren(int numberOfGenesToUse, IList<GeneSequence> population, IList<GeneSequence> buffer, Func<char> getRandomGene, string possibleGenes)
        {
            double avgFitness = population.Average(x => x.Fitness);
            int avgFitnessPoint = population.TakeWhile(x => x.Fitness <= avgFitness).Count();
            int parentCutoff = Math.Max(10, Math.Min(avgFitnessPoint, MinGaPopSize));

            var parents = population.Take(parentCutoff).ToList();
            for (int i = 0; i < RandomCitizenCount; i++)
            {
                parents.Add(GenerateRandomCitizen(numberOfGenesToUse, getRandomGene));
            }

            for (int i = 0; i < GaPopsize; i++)
            {
                decimal percentage = 1 + Random.Next(100);
                if (percentage < _splicePercentage)
                {
                    buffer[i] = SpliceTwoParents(parents, numberOfGenesToUse);
                    continue;
                }
                percentage -= _splicePercentage;
                if (percentage < _crossoverPercentage)
                {
                    buffer[i] = RandomCrossTwoParents(parents, numberOfGenesToUse);
                    continue;
                }
                percentage -= _crossoverPercentage;

                if (percentage < _mutationPercentage)
                {
                    buffer[i] = Mutate(numberOfGenesToUse, population[i], getRandomGene);
                    continue;
                }

                buffer[i] = FindActiveFill(numberOfGenesToUse, parents, possibleGenes, getRandomGene);
            }
        }

        private static GeneSequence GenerateRandomCitizen(int numberOfGenesToUse, Func<char> getRandomGene)
        {
            var geneBuilder = new StringBuilder();
            for (int j = 0; j < numberOfGenesToUse; j++)
            {
                geneBuilder.Append(getRandomGene());
            }

            return new GeneSequence(string.Empty, 0)
                {
                    Genes = geneBuilder.ToString(),
                    HowCreated = ChildCreationTypeDescription.Random
                };
        }

        public string GetBestGenetically(int numberOfGenesToUse, string possibleGenes, Func<string, uint> calcFitness, bool orderMatters)
        {
            var popAlpha = new List<GeneSequence>();
            var popBeta = new List<GeneSequence>();

            Func<char> getRandomGene = () => possibleGenes[Random.Next(possibleGenes.Length)];

            InitPopulation(numberOfGenesToUse, popAlpha, popBeta, getRandomGene);
            var population = popAlpha;
            var spare = popBeta;
            var previousBests = new List<GeneSequence>();
            _slidingMutationRate = DefaultMutationRate;
            int generation = 0;
            for (int i = 0; i < _gaMaxGenerationsWithoutImprovement; i++, generation++)
            {
                var previousBestLookup = new HashSet<string>(previousBests.Select(x => x.Genes));
                var populationWithFitness = CalcFitness(population, calcFitness);

                var first = populationWithFitness.First();
                uint previousBestFitness = previousBests.Any()
                                               ? previousBests.Last().Fitness
                                               : population.First().Fitness;
                var newSequences = UseFastSearch && i < 20
                                       ? populationWithFitness
                                             .TakeWhile(x => x.Fitness <= previousBestFitness)
                                             .Where(x => !previousBestLookup.Contains(x.Genes))
                                             .Take(MaxImprovmentsToKeepFromEachRound)
                                             .ToList()
                                       : populationWithFitness
                                             .Where(x => x.Fitness <= previousBestFitness)
                                             .Where(x => !previousBestLookup.Contains(x.Genes))
                                             .Take(UseFastSearch ? MaxImprovmentsToKeepFromEachRound : GaPopsize)
                                             .ToList();

                if (newSequences.Any())
                {
                    SortByFitness(newSequences);
                    if (!previousBests.Any() || newSequences.First().Fitness < previousBests.First().Fitness)
                    {
                        UpdateChildCreationTypePercentages(newSequences.First());
                        PrintBest(generation, newSequences.First());
                        i = -1;
                    }
                    previousBests.AddRange(newSequences.Select(geneSequence => geneSequence.Clone()));
                    int numberToKeep = Math.Max(100, previousBests.Count(x => x.Fitness == first.Fitness));
                    SortByFitness(previousBests);
                    if (numberToKeep < previousBests.Count)
                    {
                        previousBests = previousBests.Take(numberToKeep).ToList();
                    }
                    _slidingMutationRate = DefaultMutationRate;
                }
                else
                {
                    _slidingMutationRate = Math.Max(_slidingMutationRate - SlideRate, 0);
                }

                if (first.Fitness == 0)
                {
                    break;
                }

                CreateNextGeneration(numberOfGenesToUse, population, spare, getRandomGene, previousBests, possibleGenes);
                var temp = spare;
                spare = population;
                population = temp;
            }
            return previousBests.First().Genes;
        }

        private static void InitPopulation(int numberOfGenesToUse, List<GeneSequence> population, List<GeneSequence> buffer, Func<char> getRandomGene)
        {
            population.AddRange(Enumerable.Range(0, GaPopsize).Select(x => GenerateRandomCitizen(numberOfGenesToUse, getRandomGene)));
            buffer.AddRange(Enumerable.Range(0, GaPopsize).Select(x => new GeneSequence(string.Empty, 0)));
        }

        private static GeneSequence Mutate(int numberOfGenesToUse, GeneSequence member, Func<char> getRandomGene)
        {
            var mutated = member.Genes.ToCharArray();
            int index0 = Random.Next(numberOfGenesToUse);
            mutated[index0] = getRandomGene();
            return new GeneSequence(new string(mutated), Int32.MaxValue)
                {
                    HowCreated = ChildCreationTypeDescription.Mutation
                };
        }

        private void PrintBest(int generation, GeneSequence geneSequence)
        {
            DisplayGenes(1 + generation, geneSequence.Fitness, geneSequence.Genes, geneSequence.HowCreated);
        }

        private static GeneSequence RandomCrossTwoParents(IList<GeneSequence> parents, int numberOfGenesToUse)
        {
            int i1 = Random.Next(parents.Count);
            int i2 = Random.Next(parents.Count);
            int numberOfGenesToCross = (int)(numberOfGenesToUse * _slidingMutationRate);

            var childGenes = parents[i1].Genes.ToArray();
            var parentB = parents[i2].Genes.ToArray();
            for (int j = 0; j < numberOfGenesToCross; j++)
            {
                int index0 = Random.Next(numberOfGenesToUse);
                childGenes[index0] = parentB[index0];
            }
            return new GeneSequence(new string(childGenes), Int32.MaxValue)
                {
                    HowCreated = ChildCreationTypeDescription.Crossover
                };
        }

        private static void SortByFitness(List<GeneSequence> population)
        {
            population.Sort(FitnessSort);
        }

        private static GeneSequence SpliceTwoParents(IList<GeneSequence> parents, int numberOfGenesToUse)
        {
            int i1 = Random.Next(parents.Count);
            int i2 = Random.Next(parents.Count);
            int split = Random.Next(numberOfGenesToUse);

            var parentA = parents[i1];
            var parentB = parents[i2];
            var child = new GeneSequence(parentA.Genes.Substring(0, split) + parentB.Genes.Substring(split), Int32.MaxValue)
                {
                    HowCreated = ChildCreationTypeDescription.Splice
                };
            return child;
        }

        private void UpdateChildCreationTypePercentages(GeneSequence geneSequence)
        {
            string howCreated = geneSequence.HowCreated;
            if (howCreated != ChildCreationTypeDescription.Mutation &&
                howCreated != ChildCreationTypeDescription.Splice &&
                howCreated != ChildCreationTypeDescription.Crossover &&
                howCreated != ChildCreationTypeDescription.ActiveFill)
            {
                return;
            }
            bool reduceMutation = false;
            bool reduceSplice = false;
            bool reduceCrossover = false;
            bool reduceActiveFill = false;

            int changeCount = 0;
            if (howCreated != ChildCreationTypeDescription.Mutation && _mutationPercentage > MinMutationPercentage)
            {
                changeCount++;
                reduceMutation = true;
            }
            if (howCreated != ChildCreationTypeDescription.Splice && _splicePercentage > MinSplicePercentage)
            {
                changeCount++;
                reduceSplice = true;
            }
            if (howCreated != ChildCreationTypeDescription.Crossover && _crossoverPercentage > MinCrossoverPercentage)
            {
                changeCount++;
                reduceCrossover = true;
            }
            if (howCreated != ChildCreationTypeDescription.ActiveFill && _activeFillPercentage > MinActiveFillPercentage)
            {
                changeCount++;
                reduceActiveFill = true;
            }

            if (changeCount == 0)
            {
                return;
            }

            bool changedCount;
            do
            {
                changedCount = false;
                if (reduceMutation && (_mutationPercentage - ChildCreationTypeStep / changeCount) < MinMutationPercentage)
                {
                    changeCount--;
                    changedCount = true;
                    reduceMutation = false;
                }
                if (reduceSplice && (_splicePercentage - ChildCreationTypeStep / changeCount) < MinSplicePercentage)
                {
                    changeCount--;
                    changedCount = true;
                    reduceSplice = false;
                }
                if (reduceCrossover && (_crossoverPercentage - ChildCreationTypeStep / changeCount) < MinCrossoverPercentage)
                {
                    changeCount--;
                    changedCount = true;
                    reduceCrossover = false;
                }
                if (reduceActiveFill && (_activeFillPercentage - ChildCreationTypeStep / changeCount) < MinActiveFillPercentage)
                {
                    changeCount--;
                    changedCount = true;
                    reduceActiveFill = false;
                }
            } while (changedCount);

            if (changeCount == 0)
            {
                return;
            }

            decimal amountToRemove = ChildCreationTypeStep / changeCount;

            if (howCreated != ChildCreationTypeDescription.Mutation)
            {
                if (reduceMutation)
                {
                    _mutationPercentage -= amountToRemove;
                }
            }
            else
            {
                _mutationPercentage += ChildCreationTypeStep;
            }
            if (howCreated != ChildCreationTypeDescription.Splice)
            {
                if (reduceSplice)
                {
                    _splicePercentage -= amountToRemove;
                }
            }
            else
            {
                _splicePercentage += ChildCreationTypeStep;
            }
            if (howCreated != ChildCreationTypeDescription.Crossover)
            {
                if (reduceCrossover)
                {
                    _crossoverPercentage -= amountToRemove;
                }
            }
            else
            {
                _crossoverPercentage += ChildCreationTypeStep;
            }
            if (howCreated != ChildCreationTypeDescription.ActiveFill)
            {
                if (reduceActiveFill)
                {
                    _activeFillPercentage -= amountToRemove;
                }
            }
            else
            {
                _activeFillPercentage += ChildCreationTypeStep;
            }

            if (DisplayHowCreatedPercentages && Random.Next(7) == 0)
            {
                Console.WriteLine("> " + ChildCreationTypeDescription.Splice + " " + Math.Round(_splicePercentage, 1)
                                  + " " + ChildCreationTypeDescription.Crossover + " " + Math.Round(_crossoverPercentage, 1)
                                  + " " + ChildCreationTypeDescription.Mutation + " " + Math.Round(_mutationPercentage, 1)
                                  + " " + ChildCreationTypeDescription.ActiveFill + " " + Math.Round(_activeFillPercentage, 1));
            }
        }

        private static class ChildCreationTypeDescription
        {
            public const string ActiveFill = "Active Fill";
            public const string Crossover = "Crossover";
            public const string Mutation = "Mutation";
            public const string Random = "Random";
            public const string Splice = "Splice";
        }
    }
}