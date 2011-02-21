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
using System.Text;

namespace Scratch.GeneticAlgorithm.Strategies
{
    public class RandomGenes : IChildGenerationStrategy
    {
        public RandomGenes()
        {
            OrderBy = Int32.MaxValue;
        }

        public int OrderBy { get; set; }

        public string Description
        {
            get { return "Random"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt)
        {
            var geneBuilder = new StringBuilder();
            for (int j = 0; j < numberOfGenesToUse; j++)
            {
                geneBuilder.Append(getRandomGene());
            }

            return new GeneSequence(geneBuilder.ToString(), this);
        }
    }
}