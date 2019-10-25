using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
    internal class GAType
    {
        public GAType()
        {
        }

        public GAType Clone()
        {
            var gAType = new GAType();
            for (int i = 0; i < gene.Length; i++)
            {
                gAType.gene[i] = gene[i];
            }
            gAType.fitness = fitness;
            gAType.rf = rf;
            gAType.cf = cf;
            return gAType;
        }

        public int[] gene = new int[7];
        public int fitness;
        public double rf;
        public double cf;
    }

    internal class GA
    {
        public const int TEST_ROUND = 500;
        public const int OBJ_COUNT = 7;
        public const int CAPACITY = 150;
        public const int POPULATION_SIZE = 32;
        public const int MAX_GENERATIONS = 100;//500;
        public const double P_XOVER = 0.8;
        public const double P_MUTATION = 0.15;

        public int[] Weight = { 35, 30, 60, 50, 40, 10, 25 };
        public int[] Value = { 10, 40, 30, 50, 35, 40, 30 };
        public Random rd = new Random();

        public void GetRandomGene(int[] gene, int count)
        {
            for (int i = 0; i < count; i++)
            {
                gene[i] = rd.Next(0, ushort.MaxValue) % 2;
            }
        }

        public void Initialize(List<GAType> pop)
        {
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                var gAType = new GAType();
                GetRandomGene(gAType.gene, OBJ_COUNT);
                gAType.fitness = 0;
                gAType.rf = 0.0;
                gAType.cf = 0.0;
                pop.Add(gAType);
            }
        }

        public int EnvaluateFitness(List<GAType> pop)
        {
            int totalFitness = 0;
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                int tw = 0;
                pop[i].fitness = 0;
                for (int j = 0; j < OBJ_COUNT; j++)
                {
                    if (pop[i].gene[j] == 1)
                    {
                        tw += Weight[j];
                        pop[i].fitness += Value[j];
                    }
                }
                if (tw > CAPACITY) /*惩罚性措施*/
                {
                    pop[i].fitness = 1;
                }
                totalFitness += pop[i].fitness;
            }

            return totalFitness;
        }

        public int GetTotalFitness(List<GAType> pop)
        {
            int totalFitness = 0;
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                for (int j = 0; j < OBJ_COUNT; j++)
                {
                    if (pop[i].gene[j] == 1)
                    {
                        totalFitness += Value[j];
                    }
                }
            }

            return totalFitness;
        }

        public int GetBestPopulation(List<GAType> pop, out GAType bestGene)
        {
            int best = 0;
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                if (pop[i].fitness > pop[best].fitness)
                {
                    best = i;
                }
            }
            bestGene = pop[best].Clone();

            return best;
        }

        public int GetWorstPopulation(List<GAType> pop)
        {
            int worst = 0;
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                if (pop[i].fitness < pop[worst].fitness)
                {
                    worst = i;
                }
            }
            return worst;
        }

        public List<GAType> Select(int totalFitness, List<GAType> pop)
        {
            int i;
            List<GAType> newPop = new List<GAType>();
            double lastCf = 0.0;
            //计算个体的选择概率和累积概率
            for (i = 0; i < POPULATION_SIZE; i++)
            {
                pop[i].rf = (double)pop[i].fitness / totalFitness;
                pop[i].cf = lastCf + pop[i].rf;
                lastCf = pop[i].cf;
            }

            GetBestPopulation(pop, out GAType best);
            for (i = 0; i < POPULATION_SIZE; i++)
            {
                double p = (double)rd.Next(0, ushort.MaxValue) / ushort.MaxValue;
                if (p < pop[0].cf)
                {
                    newPop.Add(best.Clone());//;//pop[0];//
                }
                else
                {
                    for (int j = 0; j < POPULATION_SIZE; j++)
                    {
                        if ((p >= pop[j].cf) && (p < pop[j + 1].cf))
                        {
                            newPop.Add(pop[j + 1].Clone());
                            break;
                        }
                    }
                }
            }
            if (newPop.Count < POPULATION_SIZE)
            {
                Console.WriteLine("newpop count" + newPop.Count);
            }
            for (i = 0; i < POPULATION_SIZE; i++)
            {
                if (newPop[i].fitness == 1)
                {
                    pop[i] = best.Clone();
                }
                else
                {
                    pop[i] = newPop[i].Clone();
                }
            }
            return pop;
        }

        public void ExchangeOver(List<GAType> pop, int first, int second)
        {
            /*对随机个数的基因位进行交换*/
            int ecc = rd.Next(0, ushort.MaxValue) % OBJ_COUNT + 1;
            for (int i = 0; i < ecc; i++)
            {
                /*每个位置被交换的概率是相等的*/
                int idx = rd.Next(0, ushort.MaxValue) % OBJ_COUNT;
                int tg = pop[first].gene[idx];
                pop[first].gene[idx] = pop[second].gene[idx];
                pop[second].gene[idx] = tg;
            }
        }

        public void Crossover(List<GAType> pop)
        {
            int first = -1;//第一个个体已经选择的标识

            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                double p = (double)rd.Next(0, ushort.MaxValue) / ushort.MaxValue;
                if (p < P_XOVER)
                {
                    if (first < 0)
                    {
                        first = i; //选择第一个个体
                    }
                    else
                    {
                        ExchangeOver(pop, first, i);
                        first = -1;//清除第一个个体的选择标识
                    }
                }
            }
        }

        public void ReverseGene(List<GAType> pop, int index)
        {
            /*对随机个数的基因位进行变异*/
            int mcc = rd.Next(0, ushort.MaxValue) % OBJ_COUNT + 1;
            for (int i = 0; i < mcc; i++)
            {
                /*每个位置被交换的概率是相等的*/
                int gi = rd.Next(0, ushort.MaxValue) % OBJ_COUNT;
                pop[index].gene[gi] = 1 - pop[index].gene[gi];
            }
        }

        public void Mutation(List<GAType> pop)
        {
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                double p = (double)rd.Next(0, ushort.MaxValue) / ushort.MaxValue;
                if (p < P_MUTATION)
                {
                    ReverseGene(pop, i);
                }
            }
        }

        public void Test()
        {
            int success = 0;
            List<GAType> population = new List<GAType>();
            for (int k = 0; k < TEST_ROUND; k++)
            {
                Initialize(population);

                int totalFitness = EnvaluateFitness(population);
                for (int i = 0; i < MAX_GENERATIONS; i++)
                {
                    Select(totalFitness, population);
                    Crossover(population);
                    Mutation(population);
                    totalFitness = EnvaluateFitness(population);
                    if (i % 20 == 1)
                    {
                        Console.WriteLine($"TEST_ROUND {k}  Generations {i} totalFitness {totalFitness}");
                    }
                }
                Console.WriteLine("test round " + k);
                int index = GetBestPopulation(population, out GAType best);
                Console.WriteLine($"best Index  {index} Fitness {best.fitness}");
                if (best.fitness == 170)
                    success++;
            }

            Console.WriteLine($"success {success}");
        }
    }
}