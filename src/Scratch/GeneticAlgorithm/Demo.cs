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

using NUnit.Framework;

namespace Scratch.GeneticAlgorithm
{

    /// <summary>
    ///     http://stackoverflow.com/questions/948691/genetic-programming-implementation
    /// 
    ///     example:
    ///     Generation     1 best: Ho*fSOv@rG)! (9)
    ///     Generation     2 best: HelfSOvArGV! (7)
    ///     Generation     3 best: Hel@S!/@rld! (5)
    ///     Generation     4 best: HelloOuRrld! (3)
    ///     Generation     6 best: Hello!$@rld! (3)
    ///     Generation     8 best: Hello!u@rld! (3)
    ///     Generation     9 best: HelloO/@rld! (3)
    ///     Generation    10 best: Hello t@rld! (2)
    ///     Generation    11 best: Hello tRrld! (2)
    ///     Generation    12 best: Hello uRrld! (2)
    ///     Generation    16 best: Hello u@rld! (2)
    ///     Generation    30 best: Hello w@rld! (1)
    ///     Generation   127 best: Hello whrld! (1)
    ///     Generation   133 best: Hello wprld! (1)
    ///     Generation   261 best: Hello wYrld! (1)
    ///     Generation   287 best: Hello world! (0)
    /// </summary>
    [TestFixture]
    public class Demo
    {
        [Test]
        public void Hello_World()
        {
            const string genes = @"`1234567890-=~!@#$%^&*()_+qwertyuiop[]\QWERTYUIOP{}|asdfghjkl;'ASDFGHJKL:""zxcvbnm,./ZXCVBNM<>? ";
            string target = "Hello world!";
            Func<string, uint> calcFitness = str =>
                {
                    uint fitness = 0;
                    for (int j = 0; j < target.Length; j++)
                    {
                        try
                        {
                            fitness += str[j] == target[j] ? 0U : 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    return fitness;
                };
            string best = new GeneticSolver().GetBestGenetically(target.Length, genes, calcFitness);
            Console.WriteLine(best);
        }
    }
}