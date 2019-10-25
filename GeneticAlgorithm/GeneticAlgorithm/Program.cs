using System;

namespace GeneticAlgorithm
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GA gA = new GA();
            gA.Test();

            Console.WriteLine("按任意键结束");
            Console.ReadKey();
        }
    }
}