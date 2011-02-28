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

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var indexes = Enumerable.Range(0, parents.Count)
                .Shuffle().Take(2).ToArray();

            int i1 = indexes.First();
            int i2 = indexes.Last();
            int sliceIndex = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;

            if (numberOfGenesInUnitOfMeaning > 1 &&
                numberOfGenesToUse - freezeGenesUpTo != numberOfGenesInUnitOfMeaning &&
                getRandomInt(2) == 1)
            {
                sliceIndex = sliceIndex - sliceIndex % numberOfGenesInUnitOfMeaning;
            }

            var parentA = parents[i1];
            if (sliceIndex == 0)
            {
                return parentA.Clone();
            }

            var parentB = parents[i2];

            string childGenes = parentA.Genes.Substring(0, sliceIndex) + parentB.Genes.Substring(sliceIndex);
            VerifyGeneLength(parentA, childGenes);
            var child = new GeneSequence(childGenes, this);
            return child;
        }

        public int OrderBy { get; set; }

        [Conditional("Debug")]
        private static void VerifyGeneLength(GeneSequence parentA, string childGenes)
        {
            if (childGenes.Length != parentA.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
}