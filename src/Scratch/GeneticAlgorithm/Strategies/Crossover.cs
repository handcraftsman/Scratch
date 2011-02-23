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

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt)
        {
            var indexes = Enumerable.Range(0, parents.Count)
                .Shuffle().Take(2).ToArray();

            int i1 = indexes.First();
            int i2 = indexes.Last();

            string parentA = parents[i1].Genes;
            string parentB = parents[i2].Genes;
            var childGenes = parentA.ToArray();

            int numberOfGenesToCross = Math.Min(5,(int)(numberOfGenesToUse * slidingMutationRate));
            if (numberOfGenesInUnitOfMeaning == 1 || getRandomInt(2) == 0)
            {
                for (int j = 0; j < numberOfGenesToCross; j++)
                {
                    int index0 = getRandomInt(numberOfGenesToUse);
                    childGenes[index0] = parentB[index0];
                }
            }
            else
            {
                int numberOfUnitsToCross = numberOfGenesToCross / numberOfGenesInUnitOfMeaning;
//                for (int j = 0; j < numberOfUnitsToCross; j++)
                {
                    int index = getRandomInt(numberOfUnitsToCross) * numberOfGenesInUnitOfMeaning;
                    Array.Copy(parentB.ToArray(), index, childGenes, index, numberOfGenesInUnitOfMeaning);
                }
            }
            return new GeneSequence(new string(childGenes), this);
        }

        public int OrderBy { get; set; }
    }
}