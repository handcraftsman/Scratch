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

namespace Scratch.GeneticAlgorithm.Strategies
{
    public class Mutation : IChildGenerationStrategy
    {
        private RandomGenes _randomStrategy;

        public Mutation()
        {
            OrderBy = 40;
            _randomStrategy = new RandomGenes();
        }

        public string Description
        {
            get { return "Mutation"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];
            char[] childGenes = parent.Genes.ToArray();

            bool useHint = getRandomInt(2) == 0 &&
                parent.Fitness != null &&
                parent.Fitness.UnitOfMeaningIndexHint != null &&
                parent.Fitness.UnitOfMeaningIndexHint.Value >= freezeGenesUpTo;

            int index0 = useHint 
                ? parent.Fitness.UnitOfMeaningIndexHint.Value 
                : getRandomInt((numberOfGenesToUse - freezeGenesUpTo) / numberOfGenesInUnitOfMeaning - freezeGenesUpTo / numberOfGenesInUnitOfMeaning) + freezeGenesUpTo / numberOfGenesInUnitOfMeaning;
            index0 *= numberOfGenesInUnitOfMeaning;
            var random = _randomStrategy.Generate(null, numberOfGenesInUnitOfMeaning, getRandomGene, numberOfGenesInUnitOfMeaning, slidingMutationRate, getRandomInt, 0);
            CopyUnitOfMeaningToGenesAtOffset(random.Genes, childGenes, index0);
            VerifyGeneLength(parent, childGenes);
            return new GeneSequence(childGenes, this);
        }


        private static void CopyUnitOfMeaningToGenesAtOffset(char[] unit, char[] genes, int byteOffset)
        {
            Array.Copy(unit, 0, genes, byteOffset, unit.Length);
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
    public class MutationMidUnitOfMeaning : IChildGenerationStrategy
    {
        public MutationMidUnitOfMeaning()
        {
            OrderBy = 41;
        }

        public string Description
        {
            get { return "MMutation"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];
            char[] childGenes = parent.Genes.ToArray();
            int index0 = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            childGenes[index0] = getRandomGene();
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
}