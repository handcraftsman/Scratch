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

namespace Scratch.GeneticAlgorithm
{
	public class GeneticSolver
	{
		private const float GaElitrate = 0.10f;
		private const float GaMutationRate = 0.25f;
		private const int GaPopsize = 2048;
		private const float SlideRate = 0.001f;

		private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
		private static float _slidingMutationRate = GaMutationRate;
		private readonly int _gaMaxGenerationsWithoutImprovement = 16384;

		public GeneticSolver()
		{
		}

		public GeneticSolver(int maxGenerationsWithoutImprovement)
		{
			_gaMaxGenerationsWithoutImprovement = maxGenerationsWithoutImprovement;
		}

		private static void CalcFitness(List<ga_struct> population, Func<string, uint> calcDistanceFromTarget)
		{
			for (int i = 0; i < GaPopsize; i++)
			{
				population[i].Fitness = calcDistanceFromTarget(population[i].Genes);
			}
		}

		private static void Elitism(List<ga_struct> population, List<ga_struct> buffer, int esize)
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

		public string GetBestGenetically(int numberOfGenesToUse, string possibleGenes, Func<string, uint> calcFitness, bool orderMatters)
		{
			var popAlpha = new List<ga_struct>();
			var popBeta = new List<ga_struct>();

			Func<char> getRandomGene = () => possibleGenes[Random.Next(possibleGenes.Length)];

			InitPopulation(numberOfGenesToUse, popAlpha, popBeta, getRandomGene);
			var population = popAlpha;
			var buffer = popBeta;
			var previousBests = new HashSet<string>();
			_slidingMutationRate = GaMutationRate;
			int generation = 0;
			for (int i = 0; i < _gaMaxGenerationsWithoutImprovement; i++, generation++)
			{
				CalcFitness(population, calcFitness);
				SortByFitness(population);
				if (!previousBests.Contains(population[0].Genes))
				{
					PrintBest(generation, population);
					previousBests.Add(population[0].Genes);
					_slidingMutationRate = GaMutationRate;
					i = -1;
				}
				else
				{
					_slidingMutationRate = Math.Max(_slidingMutationRate - SlideRate, 0);
				}

				if (population[0].Fitness == 0)
				{
					break;
				}

				Mate(numberOfGenesToUse, population, buffer, getRandomGene, orderMatters);
				var temp = buffer;
				buffer = population;
				population = temp;
			}
			return population[0].Genes;
		}

		private static void InitPopulation(int numberOfGenesToUse, List<ga_struct> population, List<ga_struct> buffer, Func<char> getRandomGene)
		{
			for (int i = 0; i < GaPopsize; i++)
			{
				var citizen = new ga_struct(string.Empty, 0);

				for (int j = 0; j < numberOfGenesToUse; j++)
				{
					citizen.Genes += getRandomGene();
				}

				population.Add(citizen);
				buffer.Add(new ga_struct(string.Empty, 0));
			}
		}

		private static void Mate(int numberOfGenesToUse, List<ga_struct> population, List<ga_struct> buffer, Func<char> getRandomGene, bool orderMatters)
		{
			const int esize = (int)(GaPopsize * GaElitrate);

			Elitism(population, buffer, esize);

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
						int spos = Random.Next(numberOfGenesToUse);

						var parentB = population[i2].Genes.ToArray();
						var bufferChild = population[i1].Genes.ToArray();
						for (int j = 0; j < spos; j++)
						{
							int index0 = Random.Next(numberOfGenesToUse);
							bufferChild[index0] = parentB[index0];
						}
						buffer[i].Genes = new string(bufferChild);
						break;
					}
					case 1:
					{
						buffer[i] = Mutate(numberOfGenesToUse, population[i], getRandomGene, orderMatters);
						break;
					}
				}
			}
		}

		private static ga_struct Mutate(int numberOfGenesToUse, ga_struct member, Func<char> getRandomGene, bool orderMatters)
		{
			var mutated = member.Genes.ToCharArray();
			float mutationRate = Math.Max(1, numberOfGenesToUse * _slidingMutationRate);
			for (int i = 0; i < mutationRate; i++)
			{
				int index0 = Random.Next(numberOfGenesToUse);
				if (!orderMatters)
				{
					mutated[index0] = getRandomGene();
				}
				else
				{
					switch (Random.Next(3))
					{
						case 0: // replace
							mutated[index0] = getRandomGene();
							break;
						case 1: // shift right and insert
							for (int j = mutated.Length - 1; j > index0; j--)
							{
								mutated[j] = mutated[j - 1];
							}
							mutated[index0] = getRandomGene();
							break;
						case 2: // shift left and append
							for (int j = index0; j < mutated.Length - 1; j++)
							{
								mutated[j] = mutated[j + 1];
							}
							mutated[mutated.Length - 1] = getRandomGene();
							break;
					}
				}
			}
			return new ga_struct(new string(mutated), Int32.MaxValue);
		}

		private void PrintBest(int generation, List<ga_struct> gav)
		{
			Console.WriteLine("Generation {0} best: {1} (fitness: {2})",
			                  (1 + generation).ToString().PadLeft(_gaMaxGenerationsWithoutImprovement.ToString().Length),
			                  gav[0].Genes,
			                  gav[0].Fitness);
		}

		private static void SortByFitness(List<ga_struct> population)
		{
			population.Sort(FitnessSort);
		}
	}
}