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
    public class Remove : IChildGenerationStrategy
    {
        public Remove()
        {
            OrderBy = 60;
        }

        public string Description
        {
            get { return "Remove"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];

            if (parent.Genes.Length > 0)
            {
                return parent.Clone();
            }

            bool useHint = getRandomInt(2) == 0 &&
                parent.Fitness != null &&
                parent.Fitness.UnitOfMeaningIndexHint != null &&
                parent.Fitness.UnitOfMeaningIndexHint.Value >= freezeGenesUpTo;

            var indexes = useHint
                              ? new[] { parent.Fitness.UnitOfMeaningIndexHint.Value }
                              : Enumerable.Range(freezeGenesUpTo / numberOfGenesInUnitOfMeaning, (numberOfGenesToUse - freezeGenesUpTo) / numberOfGenesInUnitOfMeaning)
                                    .Shuffle().Take(1).ToArray();
            int index = indexes.First() * numberOfGenesInUnitOfMeaning;

            var childGenes = parent.Genes.ToList();
            childGenes.RemoveRange(index, numberOfGenesInUnitOfMeaning);

            var genes = childGenes.ToArray();
            VerifyGeneLength(parent, genes);
            return new GeneSequence(genes, this);
        }

        public int OrderBy { get; set; }

        [Conditional("DEBUG")]
        private static void VerifyGeneLength(GeneSequence parent, ICollection<char> childGenes)
        {
            if (childGenes.Count != parent.Genes.Length - 1)
            {
                throw new ArgumentException("result is different length than expected");
            }
        }
    }
}