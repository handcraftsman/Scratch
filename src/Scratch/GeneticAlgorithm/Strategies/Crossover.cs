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
using System.Diagnostics;
using System.Linq;

using Scratch.ShuffleIEnumerable;

namespace Scratch.GeneticAlgorithm.Strategies
{
    public class Crossover : IChildGenerationStrategy
    {
        public Crossover()
        {
            OrderBy = 2;
        }

        public string Description
        {
            get { return "Crossover"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var indexes = Enumerable.Range(0, parents.Count)
                .Shuffle().Take(2).ToArray();

            int i1 = indexes.First();
            int i2 = indexes.Last();

            string parentA = parents[i1].Genes;
            string parentB = parents[i2].Genes;
            var genes = parentA.ToArray();

            int numberOfGenesToCross = Math.Min(5, (int)(numberOfGenesToUse * slidingMutationRate));
            if (numberOfGenesInUnitOfMeaning == 1 ||
                numberOfGenesToUse - freezeGenesUpTo == numberOfGenesInUnitOfMeaning ||
                getRandomInt(2) == 0)
            {
                for (int j = 0; j < numberOfGenesToCross; j++)
                {
                    int index0 = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
                    genes[index0] = parentB[index0];
                }
            }
            else
            {
                int numberOfUnitsToCross = numberOfGenesToCross / numberOfGenesInUnitOfMeaning;
                int index = getRandomInt(numberOfUnitsToCross) * numberOfGenesInUnitOfMeaning;
                Array.Copy(parentB.ToArray(), index, genes, index, numberOfGenesInUnitOfMeaning);
            }
            string childGenes = new String(genes);
            VerifyGeneLength(parentA, childGenes);
            return new GeneSequence(childGenes, this);
        }

        public int OrderBy { get; set; }

        [Conditional("Debug")]
        private static void VerifyGeneLength(string parentA, string childGenes)
        {
            if (childGenes.Length != parentA.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
}