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
    public class Splice : IChildGenerationStrategy
    {
        public Splice()
        {
            OrderBy = 1;
        }

        public string Description
        {
            get { return "Splice"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt)
        {
            var indexes = Enumerable.Range(0, parents.Count)
                .Shuffle().Take(2).ToArray();

            int i1 = indexes.First();
            int i2 = indexes.Last();
            int sliceIndex = getRandomInt(numberOfGenesToUse);

            if (numberOfGenesInUnitOfMeaning > 1 && getRandomInt(2) == 1)
            {
                sliceIndex = sliceIndex - sliceIndex % numberOfGenesInUnitOfMeaning;
            }

            var parentA = parents[i1];
            var parentB = parents[i2];
            var child = new GeneSequence(parentA.Genes.Substring(0, sliceIndex) + parentB.Genes.Substring(sliceIndex), this);
            return child;
        }

        public int OrderBy { get; set; }
    }
}