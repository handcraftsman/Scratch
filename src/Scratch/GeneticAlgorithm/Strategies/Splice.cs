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
            OrderBy = 10;
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
            var parentA = parents[i1];

            IChildGenerationStrategy type = this;
            if (numberOfGenesInUnitOfMeaning > 1 &&
                numberOfGenesToUse - freezeGenesUpTo != numberOfGenesInUnitOfMeaning &&
                getRandomInt(2) == 1)
            {
                bool useHint = getRandomInt(2) == 0 && 
                    parentA.Fitness != null && 
                    parentA.Fitness.UnitOfMeaningIndexHint != null &&
                    parentA.Fitness.UnitOfMeaningIndexHint.Value >= freezeGenesUpTo;

                sliceIndex = useHint 
                    ? parentA.Fitness.UnitOfMeaningIndexHint.Value * numberOfGenesInUnitOfMeaning
                    : sliceIndex - sliceIndex % numberOfGenesInUnitOfMeaning;
            }
            else
            {
                type = new SpliceMidUnitOfMeaning();
            }

            if (sliceIndex == 0)
            {
                return parentA.Clone();
            }

            var parentB = parents[i2];

            var childGenes = parentA.Genes.Take(sliceIndex).Concat(parentB.Genes.Skip(sliceIndex)).ToArray();
            VerifyGeneLength(parentA, childGenes);
            var child = new GeneSequence(childGenes, type);
            return child;
        }

        public int OrderBy { get; set; }

        [Conditional("DEBUG")]
        private static void VerifyGeneLength(GeneSequence parentA, ICollection<char> childGenes)
        {
            if (childGenes.Count != parentA.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
    public class SpliceMidUnitOfMeaning : IChildGenerationStrategy
    {
        public SpliceMidUnitOfMeaning()
        {
            OrderBy = 11;
        }

        public string Description
        {
            get { return "MSplice"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var indexes = Enumerable.Range(0, parents.Count)
                .Shuffle().Take(2).ToArray();

            int i1 = indexes.First();
            int i2 = indexes.Last();
            int sliceIndex = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;

            IChildGenerationStrategy type = this;
            if (numberOfGenesInUnitOfMeaning > 1 &&
                numberOfGenesToUse - freezeGenesUpTo != numberOfGenesInUnitOfMeaning &&
                getRandomInt(2) == 1)
            {
                sliceIndex = sliceIndex - sliceIndex % numberOfGenesInUnitOfMeaning;
                type = new Splice();
            }

            var parentA = parents[i1];
            if (sliceIndex == 0)
            {
                return parentA.Clone();
            }

            var parentB = parents[i2];

            var childGenes = parentA.Genes.Take(sliceIndex).Concat(parentB.Genes.Skip(sliceIndex)).ToArray();
            VerifyGeneLength(parentA, childGenes);
            var child = new GeneSequence(childGenes, type);
            return child;
        }

        public int OrderBy { get; set; }

        [Conditional("DEBUG")]
        private static void VerifyGeneLength(GeneSequence parentA, ICollection<char> childGenes)
        {
            if (childGenes.Count != parentA.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
}