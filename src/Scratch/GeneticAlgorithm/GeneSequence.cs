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

using System.Linq;

namespace Scratch.GeneticAlgorithm
{
    public class GeneSequence
    {
        public static readonly FitnessResult DefaultFitness = new FitnessResult { Value = UInt32.MaxValue };
        private string _stringGenes;

        public GeneSequence(char[] genes, IChildGenerationStrategy strategy)
        {
            Genes = genes;
            Fitness = DefaultFitness;
            Strategy = strategy;
        }

        public FitnessResult Fitness { get; set; }
        public int Generation { get; set; }
        public char[] Genes { get; set; }

        public IChildGenerationStrategy Strategy { get; private set; }

        public GeneSequence Clone()
        {
            var geneSequence = new GeneSequence(Genes.ToArray(), Strategy)
                {
                    Fitness = Fitness
                };
            if (_stringGenes !=null)
            {
                geneSequence._stringGenes = _stringGenes;
            }
            return geneSequence;
        }

        public string GetStringGenes()
        {
            return _stringGenes ?? (_stringGenes = new string(Genes ?? new char[] { }));
        }

        public override string ToString()
        {
            string dispGenes = GetStringGenes();
            if (dispGenes.Length > 20)
            {
                dispGenes = dispGenes.Substring(0, 20) + " ...";
            }

            return dispGenes + " fitness: " + Fitness.Value + " strategy: " + (Strategy == null ? "none" : Strategy.Description) + " gen: " + Generation;
        }
    }
}