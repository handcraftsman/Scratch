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

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt)
        {
            var parent = parents[getRandomInt(parents.Count)];

            var indexes = Enumerable.Range(0, numberOfGenesToUse / numberOfGenesInUnitOfMeaning)
                .Shuffle().Take(2).ToArray();
            int sourceIndex = indexes.First() * numberOfGenesInUnitOfMeaning;
            int targetIndex = indexes.Last() * numberOfGenesInUnitOfMeaning;

            var genes = parent.Genes.ToList();
            var unit = genes.Skip(sourceIndex).Take(numberOfGenesToUse).ToArray();
            genes.RemoveRange(sourceIndex, numberOfGenesInUnitOfMeaning);
            genes.InsertRange(targetIndex > sourceIndex ? targetIndex - 1 : targetIndex, unit);

            return new GeneSequence(new String(genes.ToArray()), this);
        }

        public int OrderBy { get; set; }
    }
}