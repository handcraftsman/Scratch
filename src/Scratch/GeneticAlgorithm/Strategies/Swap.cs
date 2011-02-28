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
    public class Swap : IChildGenerationStrategy
    {
        public Swap()
        {
            OrderBy = 5;
        }

        public int OrderBy { get; set; }

        public string Description
        {
            get { return "Swap"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];
            var genes = parent.Genes.ToCharArray();

            int pointA = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            int pointB = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            if (pointA == pointB)
            {
                return parent.Clone();
            }
            if (numberOfGenesInUnitOfMeaning == 1 ||
                numberOfGenesToUse - freezeGenesUpTo == numberOfGenesInUnitOfMeaning ||
                getRandomInt(2) == 0)
            {
                SwapTwoGenes(genes, pointA, pointB);
            }
            else
            {
                pointA /= numberOfGenesInUnitOfMeaning;
                pointB /= numberOfGenesInUnitOfMeaning;
                SwapTwoUnitsOfMeaning(genes, pointA, pointB, numberOfGenesInUnitOfMeaning);
            }

            string childGenes = new String(genes);
            VerifyGeneLength(parent, childGenes);

            var child = new GeneSequence(childGenes, this);

            return child;
        }

        private static void CopyUnitOfMeaningToGenesAtOffset(char[] unit, char[] genes, int byteOffset)
        {
            Array.Copy(unit, 0, genes, byteOffset, unit.Length);
        }

        private static void SwapTwoGenes(IList<char> genes, int pointA, int pointB)
        {
            char temp = genes[pointA];
            genes[pointA] = genes[pointB];
            genes[pointB] = temp;
        }

        private static void SwapTwoUnitsOfMeaning(char[] genes, int pointA, int pointB, int numberOfGenesInUnitOfMeaning)
        {
            int offsetA = pointA * numberOfGenesInUnitOfMeaning;
            var unitA = genes.Skip(offsetA).Take(numberOfGenesInUnitOfMeaning).ToArray();
            int offsetB = pointB & numberOfGenesInUnitOfMeaning;
            var unitB = genes.Skip(offsetB).Take(numberOfGenesInUnitOfMeaning).ToArray();
            CopyUnitOfMeaningToGenesAtOffset(unitB, genes, offsetA);
            CopyUnitOfMeaningToGenesAtOffset(unitA, genes, offsetB);
        }

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