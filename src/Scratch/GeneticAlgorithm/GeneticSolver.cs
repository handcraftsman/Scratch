using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch.GeneticAlgorithm
{
	public class GeneticSolver
	{
		private const float GaElitrate = 0.10f;
		private const int GaMaxGenerations = 16384;
		private const float GaMutationRate = 0.25f;
		private const int GaPopsize = 2048;
		private const string GaTarget = "Hello world!";
		private const float SlideRate = 0.001f;

		private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
		private static float _slidingMutationRate = GaMutationRate;
		private static void CalcFitness(List<ga_struct> population, Func<string, uint> calcDistanceFromTarget)
		{
			for (int i = 0; i < GaPopsize; i++)
			{
				population[i].Fitness = calcDistanceFromTarget(population[i].Genes);
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

		public string GetBestGenetically(int numberOfGenesToUse, string possibleGenes, Func<string, uint> calcFitness)
		{
			var popAlpha = new List<ga_struct>();
			var popBeta = new List<ga_struct>();

			Func<char> getRandomGene = () => possibleGenes[Random.Next(possibleGenes.Length)];

			InitPopulation(popAlpha, popBeta, getRandomGene);
			var population = popAlpha;
			var buffer = popBeta;
			var previousBests = new HashSet<string>();
			_slidingMutationRate = GaMutationRate;
			for (int i = 0; i < GaMaxGenerations; i++)
			{
				CalcFitness(population, calcFitness);
				SortByFitness(population);
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

				Mate(population, buffer, getRandomGene);
				var temp = buffer;
				buffer = population;
				population = temp;
			}
			return population[0].Genes;
		}

		private static void InitPopulation(List<ga_struct> population, List<ga_struct> buffer, Func<char> getRandomGene)
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

		private static void Mate(List<ga_struct> population, List<ga_struct> buffer, Func<char> getRandomGene)
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

		private static void SortByFitness(List<ga_struct> population)
		{
			population.Sort(FitnessSort);
		}
	}
}