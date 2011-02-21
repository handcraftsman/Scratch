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

using Scratch.GeneticAlgorithm.Strategies;

namespace Scratch.GeneticAlgorithm
{
    public class GeneSequence
    {
        public GeneSequence(string genes, IChildGenerationStrategy strategy)
        {
            Genes = genes;
            Fitness = Int32.MaxValue;
            Strategy = strategy;
        }

        public uint Fitness { get; set; }
        public string Genes { get; set; }

        public IChildGenerationStrategy Strategy { get; private set; }

        public GeneSequence Clone()
        {
            return new GeneSequence(Genes, Strategy)
                {
                    Fitness = Fitness
                };
        }

        public override string ToString()
        {
            string dispGenes = Genes ?? "";
            if (dispGenes.Length > 20)
            {
                dispGenes = dispGenes.Substring(0, 20) + " ...";
            }

            return dispGenes + " fitness: " + Fitness + " strategy: " + Strategy.Description;
        }
    }
}