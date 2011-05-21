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
    public class Move : IChildGenerationStrategy
    {
        public Move()
        {
            OrderBy = 30;
        }

        public string Description
        {
            get { return "Move"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];

            var indexes = Enumerable.Range(freezeGenesUpTo / numberOfGenesInUnitOfMeaning, (numberOfGenesToUse - freezeGenesUpTo) / numberOfGenesInUnitOfMeaning)
                .Shuffle().Take(2).ToArray();
            if (indexes.Length < 2)
            {
                return parent.Clone();
            }
            int sourceIndex = indexes.First() * numberOfGenesInUnitOfMeaning;
            int targetIndex = indexes.Last() * numberOfGenesInUnitOfMeaning;

            var genes = parent.Genes.ToList();
            var unit = genes.Skip(sourceIndex).Take(numberOfGenesInUnitOfMeaning).ToArray();
            genes.RemoveRange(sourceIndex, numberOfGenesInUnitOfMeaning);
            genes.InsertRange(targetIndex > sourceIndex ? targetIndex - 1 : targetIndex, unit);

            var childGenes = genes.ToArray();
            VerifyGeneLength(parent, childGenes);

            return new GeneSequence(childGenes, this);
        }

        public int OrderBy { get; set; }

        [Conditional("DEBUG")]
        private static void VerifyGeneLength(GeneSequence parent, ICollection<char> childGenes)
        {
            if (childGenes.Count != parent.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }

    public class MoveMidUnitOfMeaning : IChildGenerationStrategy
    {
        public MoveMidUnitOfMeaning()
        {
            OrderBy = 31;
        }

        public string Description
        {
            get { return "MMove"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];

            var indexes = Enumerable.Range(freezeGenesUpTo, (numberOfGenesToUse - freezeGenesUpTo))
                .Shuffle().Take(2).ToArray();
            if (indexes.Length < 2)
            {
                return parent.Clone();
            }
            int sourceIndex = indexes.First();
            int targetIndex = indexes.Last();
            if (sourceIndex > parent.Genes.Length - numberOfGenesInUnitOfMeaning)
            {
                sourceIndex = parent.Genes.Length - numberOfGenesInUnitOfMeaning;
            }
            if (targetIndex > parent.Genes.Length - 2 * numberOfGenesInUnitOfMeaning)
            {
                targetIndex = Math.Max(0, parent.Genes.Length - 2 * numberOfGenesInUnitOfMeaning);
            }
            if (targetIndex == sourceIndex)
            {
                return parent.Clone();
            }

            IChildGenerationStrategy type = this;
            if (sourceIndex % numberOfGenesInUnitOfMeaning == 0 &&
                targetIndex % numberOfGenesInUnitOfMeaning == 0)
            {
                type = new Move();
            }

            var genes = parent.Genes.ToList();
            var unit = genes.Skip(sourceIndex).Take(numberOfGenesInUnitOfMeaning).ToArray();
            genes.RemoveRange(sourceIndex, numberOfGenesInUnitOfMeaning);
            genes.InsertRange(targetIndex > sourceIndex ? targetIndex - 1 : targetIndex, unit);

            var childGenes = genes.ToArray();
            VerifyGeneLength(parent, childGenes);

            return new GeneSequence(childGenes.ToArray(), type);
        }

        public int OrderBy { get; set; }

        [Conditional("DEBUG")]
        private static void VerifyGeneLength(GeneSequence parent, ICollection<char> childGenes)
        {
            if (childGenes.Count != parent.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
}