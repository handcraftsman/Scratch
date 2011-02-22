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
using System.Reflection;

using Scratch.GeneticAlgorithm.Strategies;
using Scratch.Ranges.RangeEnumeration;
using Scratch.ShuffleIEnumerable;

namespace Scratch.GeneticAlgorithm
{
    public class GeneticSolver
    {
        private const int DefaultMaxGenerationsWithoutImprovement = 16384;
        private const decimal DefaultMutationRate = 0.25m;
        private const int EliteParentCount = (int)(GaPopsize * GaElitrate);
        private const float GaElitrate = 0.10f;
        private const int GaPopsize = 2048;
        private const int MaxImprovmentsToKeepFromEachRound = 5;
        private const int RandomCitizenCount = 10;
        private const decimal SlideRate = 0.001m;

        private static decimal _slidingMutationRate = DefaultMutationRate;
        private readonly List<Pair<decimal, IChildGenerationStrategy>> _childGenerationStrategies;
        private readonly int _gaMaxGenerationsWithoutImprovement = DefaultMaxGenerationsWithoutImprovement;
        private readonly IChildGenerationStrategy _randomStrategy;
        private int _numberOfGenesInUnitOfMeaning = 1;
        private Random _random = new Random();
        private const int MinimumStrategyPercentage = 2;

        public GeneticSolver()
            : this(DefaultMaxGenerationsWithoutImprovement)
        {
        }

        public GeneticSolver(int maxGenerationsWithoutImprovement)
            : this(maxGenerationsWithoutImprovement,
                   (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.GetInterfaces().Contains(typeof(IChildGenerationStrategy))
                    where t.GetConstructor(Type.EmptyTypes) != null
                    select Activator.CreateInstance(t) as IChildGenerationStrategy).ToArray())
        {
        }

// ReSharper disable ParameterTypeCanBeEnumerable.Local
        public GeneticSolver(int maxGenerationsWithoutImprovement, ICollection<IChildGenerationStrategy> childGenerationStrategies)
// ReSharper restore ParameterTypeCanBeEnumerable.Local
        {
            _gaMaxGenerationsWithoutImprovement = maxGenerationsWithoutImprovement;

            _childGenerationStrategies = new List<Pair<decimal, IChildGenerationStrategy>>(
                childGenerationStrategies
                    .OrderBy(x => x.OrderBy)
                    .Select(x => new Pair<decimal, IChildGenerationStrategy>(100m / childGenerationStrategies.Count, x))
                );

            _randomStrategy = childGenerationStrategies.FirstOrDefault(x => x.GetType() == typeof(RandomGenes)) ?? new RandomGenes();

            DisplayGenes = (generation, fitness, genes, howCreated) => Console.WriteLine("Generation {0} fitness {1}: {2}", generation.ToString().PadLeft(_gaMaxGenerationsWithoutImprovement.ToString().Length), fitness, genes);
        }

        public Action<int, uint, string, string> DisplayGenes { get; set; }
        public bool DisplayHowCreatedPercentages { get; set; }
        public int NumberOfGenesInUnitOfMeaning
        {
            get { return _numberOfGenesInUnitOfMeaning; }
            set { _numberOfGenesInUnitOfMeaning = value; }
        }
        public int RandomSeed
        {
            set { _random = new Random(value); }
        }
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

        private void CreateNextGeneration(int numberOfGenesToUse, List<GeneSequence> population, IList<GeneSequence> buffer, Func<char> getRandomGene, IEnumerable<GeneSequence> previousBests)
        {
            CopyBestParents(population, buffer);
            CopyPreviousBests(previousBests, population);
            SortByFitness(population);

            GenerateChildren(numberOfGenesToUse, population, buffer, getRandomGene);
        }

        private static int FitnessSort(GeneSequence x, GeneSequence y)
        {
            return x.Fitness.CompareTo(y.Fitness);
        }

        private void GenerateChildren(int numberOfGenesToUse, IList<GeneSequence> population, IList<GeneSequence> buffer, Func<char> getRandomGene)
        {
            var unique = new HashSet<string>();
            var parents = population.Where(x => unique.Add(x.Genes)).Take(50).ToList();
            for (int i = 0; i < RandomCitizenCount; i++)
            {
                parents.Add(_randomStrategy.Generate(null, numberOfGenesToUse, getRandomGene, NumberOfGenesInUnitOfMeaning, 0, _random.Next));
            }

            for (int i = 0; i < GaPopsize; i++)
            {
                decimal percentage = (decimal)(100 * _random.NextDouble());
                foreach (var strategy in _childGenerationStrategies)
                {
                    if (percentage < strategy.First)
                    {
                        buffer[i] = strategy.Second.Generate(parents, numberOfGenesToUse, getRandomGene, numberOfGenesToUse, _slidingMutationRate, _random.Next);
                        break;
                    }
                    percentage -= strategy.First;
                }
            }
        }

        public string GetBestGenetically(int numberOfGenesToUse, string possibleGenes, Func<string, uint> calcFitness, bool orderMatters)
        {
            var popAlpha = new List<GeneSequence>(GaPopsize);
            var popBeta = new List<GeneSequence>(GaPopsize);

            Func<char> getRandomGene = () => possibleGenes[_random.Next(possibleGenes.Length)];

            InitPopulation(numberOfGenesToUse, popAlpha, popBeta, getRandomGene);
            var population = popAlpha;
            var spare = popBeta;
            var previousBests = new List<GeneSequence>
                {
                    population.First()
                };
            previousBests[0].Fitness = calcFitness(previousBests[0].Genes);
            _slidingMutationRate = DefaultMutationRate;
            int generation = 0;
            for (int i = 0; i < _gaMaxGenerationsWithoutImprovement; i++, generation++)
            {
                var previousBestLookup = new HashSet<string>(previousBests.Select(x => x.Genes));
                var populationWithFitness = CalcFitness(population, calcFitness);

                var first = populationWithFitness.First();
                uint worstFitness = previousBests[previousBests.Count / 2].Fitness;
                var newSequences = populationWithFitness
                    .Take(UseFastSearch && i < 20 ? GaPopsize / 10 : GaPopsize)
                    .Where(x => x.Fitness <= worstFitness)
                    .Where(x => previousBestLookup.Add(x.Genes))
                    .Take(UseFastSearch ? MaxImprovmentsToKeepFromEachRound : (int)((1-_slidingMutationRate)*GaPopsize))
                    .ToList();

                if (newSequences.Any())
                {
                    SortByFitness(newSequences);
                    uint previousBestFitness = previousBests.First().Fitness;
                    if (newSequences.First().Fitness < previousBestFitness)
                    {
                        PrintBest(generation, newSequences.First());
                        i = -1;
                    }
                    previousBests.AddRange(newSequences.Select(x => x.Clone()));
                    int numberToKeep = Math.Max(100, previousBests.Count(x => x.Fitness == first.Fitness));
                    SortByFitness(previousBests);
                    if (numberToKeep < previousBests.Count)
                    {
                        previousBests = previousBests.Take(numberToKeep).ToList();
                    }
                    UpdateStrategyPercentages(previousBests, generation);

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

                CreateNextGeneration(numberOfGenesToUse, population, spare, getRandomGene, previousBests);
                var temp = spare;
                spare = population;
                population = temp;
            }
            return previousBests.First().Genes;
        }

        private void UpdateStrategyPercentages(ICollection<GeneSequence> previousBests, int generation)
        {
            var minimumStrategyPercentageValue = (int)Math.Ceiling(MinimumStrategyPercentage/100m*previousBests.Count);
            var strategiesInUse = previousBests
                .Select(x => x.Strategy)
                .GroupBy(x => x)
                .Where(x=>x.Count() >= minimumStrategyPercentageValue)
                .ToList();
            int adjustedPreviousBestsCount = previousBests.Count 
                                             + (_childGenerationStrategies.Count - strategiesInUse.Count) * minimumStrategyPercentageValue;

            foreach (var strategy in _childGenerationStrategies)
            {
                bool found = false;
                foreach (var strategyInUse in strategiesInUse)
                {
                    if (strategy.Second == strategyInUse.Key)
                    {
                        strategy.First = 100.0m * Math.Max(minimumStrategyPercentageValue,strategyInUse.Count()) / adjustedPreviousBestsCount;
                        found = true;
                    }
                }
                if (!found)
                {
                    strategy.First = 0;
                }
            }

            // normalize to 100 %
            var strategySum = _childGenerationStrategies.Sum(x => Math.Max(MinimumStrategyPercentage,x.First));
            foreach (var strategy in _childGenerationStrategies)
            {
                strategy.First = 100.0m * Math.Max(MinimumStrategyPercentage,strategy.First) / strategySum;
            }

            if (generation%100 == 0)
            {
                var strategyPercentages = _childGenerationStrategies.Select(x => x.Second.Description + " " + (x.First < 10 ? " " : "") + Math.Round(x.First, 1).ToString().PadRight(x.First < 10 ? 3 : 4)).ToArray();
                Console.WriteLine("% " + String.Join(" ", strategyPercentages));
            }
        }

        private void InitPopulation(int numberOfGenesToUse, ICollection<GeneSequence> population, List<GeneSequence> buffer, Func<char> getRandomGene)
        {
            for (int i = 0; i < GaPopsize; i++)
            {
                population.Add(_randomStrategy.Generate(null, numberOfGenesToUse, getRandomGene, NumberOfGenesInUnitOfMeaning, _slidingMutationRate, _random.Next));
            }

            buffer.AddRange(population.Select(x => x.Clone()));
        }

        private void PrintBest(int generation, GeneSequence geneSequence)
        {
            DisplayGenes(1 + generation, geneSequence.Fitness, geneSequence.Genes, geneSequence.Strategy.Description);
        }

        private static void SortByFitness(List<GeneSequence> population)
        {
            population.Sort(FitnessSort);
        }
    }
}