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
            OrderBy = 3;
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
                return parent;
            }
            int sourceIndex = indexes.First() * numberOfGenesInUnitOfMeaning;
            int targetIndex = indexes.Last() * numberOfGenesInUnitOfMeaning;

            var genes = parent.Genes.ToList();
            var unit = genes.Skip(sourceIndex).Take(numberOfGenesInUnitOfMeaning).ToArray();
            genes.RemoveRange(sourceIndex, numberOfGenesInUnitOfMeaning);
            genes.InsertRange(targetIndex > sourceIndex ? targetIndex - 1 : targetIndex, unit);

            string childGenes = new String(genes.ToArray());
            VerifyGeneLength(parent, childGenes);

            return new GeneSequence(childGenes, this);
        }

        public int OrderBy { get; set; }

        [Conditional("Debug")]
        private static void VerifyGeneLength(GeneSequence parent, string childGenes)
        {
            if (childGenes.Length != parent.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
}