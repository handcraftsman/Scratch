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

namespace Scratch.GeneticAlgorithm.Strategies
{
    public class Mutation : IChildGenerationStrategy
    {
        public Mutation()
        {
            OrderBy = 4;
        }

        public string Description
        {
            get { return "Mutation"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt)
        {
            var parent = parents[getRandomInt(parents.Count)];
            var mutated = parent.Genes.ToCharArray();
            int index0 = getRandomInt(numberOfGenesToUse);
            mutated[index0] = getRandomGene();
            return new GeneSequence(new string(mutated), this);
        }

        public int OrderBy { get; set; }
    }
}