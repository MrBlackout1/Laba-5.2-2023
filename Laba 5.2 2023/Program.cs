using System;
using System.Threading;

class Program
{
    static bool running = true;
    static object monitor = new object();
    static int carCount = 0;
    static Semaphore semaphore = new Semaphore(2, 2);

    static void Main(string[] args)
    {
        Thread[] threads = new Thread[4];
        threads[0] = new Thread(TrafficLight);
        threads[1] = new Thread(TrafficLight);
        threads[2] = new Thread(TrafficLight);
        threads[3] = new Thread(TrafficLight);

        foreach (var thread in threads)
        {
            thread.Start();
        }

        Console.WriteLine("Press any key to stop the program...");
        Console.ReadKey();
        running = false;

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
    static void TrafficLight()
    {
        while (running)
        {
            lock (monitor)
            {
                Console.WriteLine(Thread.CurrentThread.Name + " is now GREEN");
                Thread.Sleep(2000);
                semaphore.WaitOne();
                Console.WriteLine(Thread.CurrentThread.Name + " is now YELLOW");
                Thread.Sleep(1000);
                Console.WriteLine(Thread.CurrentThread.Name + " is now RED");
                semaphore.Release();

                if (carCount > 0)
                {
                    int carsToPass = Math.Min(carCount, 2);
                    carCount -= carsToPass;
                    Console.WriteLine(Thread.CurrentThread.Name + " is now allowing " + carsToPass + " cars to pass.");
                    Thread.Sleep(5000);
                }
            }
        }
    }
    static void Car()
    {
        while (running)
        {
            lock (monitor)
            {
                carCount++;
                Console.WriteLine("Car #" + carCount + " is approaching the intersection.");

                if (semaphore.WaitOne(0))
                {
                    Console.WriteLine("Car #" + carCount + " is passing through the intersection.");
                    Thread.Sleep(2000);
                    semaphore.Release();
                }
                else
                {
                    Console.WriteLine("Car #" + carCount + " is waiting at the intersection.");
                }
            }
            Thread.Sleep(1000);
        }
    }
}
