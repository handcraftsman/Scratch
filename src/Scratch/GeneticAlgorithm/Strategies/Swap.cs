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
            OrderBy = 50;
        }

        public int OrderBy { get; set; }

        public string Description
        {
            get { return "Swap"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];

            bool useHint = getRandomInt(2) == 0 &&
                parent.Fitness != null &&
                parent.Fitness.UnitOfMeaningIndexHint != null &&
                parent.Fitness.UnitOfMeaningIndexHint.Value >= freezeGenesUpTo;

            int pointA = useHint 
                ? parent.Fitness.UnitOfMeaningIndexHint.Value * numberOfGenesInUnitOfMeaning
                : getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            int pointB = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            if (pointA == pointB)
            {
                return parent.Clone();
            }

            var childGenes = parent.Genes.ToArray();

            IChildGenerationStrategy type = this;
            if (numberOfGenesInUnitOfMeaning == 1 ||
                numberOfGenesToUse - freezeGenesUpTo == numberOfGenesInUnitOfMeaning ||
                getRandomInt(2) == 0)
            {
                SwapTwoGenes(childGenes, pointA, pointB);
                type = new SwapMidUnitOfMeaning();
            }
            else
            {
                pointA /= numberOfGenesInUnitOfMeaning;
                pointB /= numberOfGenesInUnitOfMeaning;
                SwapTwoUnitsOfMeaning(childGenes, pointA, pointB, numberOfGenesInUnitOfMeaning);
            }

            VerifyGeneLength(parent, childGenes);

            var child = new GeneSequence(childGenes, type);

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

        [Conditional("DEBUG")]
        private static void VerifyGeneLength(GeneSequence parent, ICollection<char> childGenes)
        {
            if (childGenes.Count != parent.Genes.Length)
            {
                throw new ArgumentException("result is different length from parent");
            }
        }
    }
    public class SwapMidUnitOfMeaning : IChildGenerationStrategy
    {
        public SwapMidUnitOfMeaning()
        {
            OrderBy = 51;
        }

        public int OrderBy { get; set; }

        public string Description
        {
            get { return "MSwap"; }
        }

        public GeneSequence Generate(IList<GeneSequence> parents, int numberOfGenesToUse, Func<char> getRandomGene, int numberOfGenesInUnitOfMeaning, decimal slidingMutationRate, Func<int, int> getRandomInt, int freezeGenesUpTo)
        {
            var parent = parents[getRandomInt(parents.Count)];

            int pointA = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            int pointB = getRandomInt(numberOfGenesToUse - freezeGenesUpTo) + freezeGenesUpTo;
            if (pointA == pointB)
            {
                return parent.Clone();
            }

            var childGenes = parent.Genes.ToArray();

            IChildGenerationStrategy type = this;
            if (numberOfGenesInUnitOfMeaning == 1 ||
                numberOfGenesToUse - freezeGenesUpTo == numberOfGenesInUnitOfMeaning ||
                getRandomInt(2) == 0)
            {
                SwapTwoGenes(childGenes, pointA, pointB);
            }
            else
            {
                pointA /= numberOfGenesInUnitOfMeaning;
                pointB /= numberOfGenesInUnitOfMeaning;
                SwapTwoUnitsOfMeaning(childGenes, pointA, pointB, numberOfGenesInUnitOfMeaning);
                type = new Swap();
            }

            VerifyGeneLength(parent, childGenes);

            var child = new GeneSequence(childGenes, type);

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