using System.Linq;

namespace QueuingModel.UI.Console
{
    using System;
    internal class Program
    {
        private static void Main(string[] args)
        {
            float ro, pi1, pi2;

            Console.WriteLine("Enter values: (print nothing to set ro = 0.75, pi1 = 0.7, pi2 = 0.65)");
            Console.Write("ro = ");

            string result = Console.ReadLine();

            if (result == string.Empty)
            {
                ro = 0.75f;
                pi1 = 0.7f;
                pi2 = 0.65f;
            }
            else
            {
                ro = Convert.ToSingle(result);
                Console.Write("pi1 = ");
                pi1 = Convert.ToSingle(Console.ReadLine());
                Console.Write("pi2 = ");
                pi2 = Convert.ToSingle(Console.ReadLine());
            }
            
            Console.WriteLine("Calculating...");
            var model = new Model(ro, pi1, pi2);
            
            for (int i = 0; i < 1000000; i++)
                model.NextTick();

            var allStatesCount = model.StatesCount.Sum();

            Console.WriteLine("\n========= Probabilities: ===========");
            // Вероятности состояний
            for (int i = 0; i < 8; i++)
                Console.WriteLine($"P{State.IndexToStateStr(i)} = {model.GetStateProbability(i)}");
            Console.WriteLine("\n=========== Parameters: ============");
            // Средняя длина очереди
            var queueAverageLength = model.QueueAverageLength;
            // Относительная пропускная способность
            var relativeThroughput = model.RelativeThroughput;
            // Среднее время пребывания заявки в системе
            var wc = model.AverageRequestTimeInSystem;

            Console.WriteLine($"L queue = {queueAverageLength}");
            Console.WriteLine($"Q = {relativeThroughput}");
            Console.WriteLine($"Wc = {wc}");
            Console.ReadLine();  
        }
    }
}
