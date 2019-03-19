using System;
using System.Threading;


namespace RiskAndPricingSolutions.Rx.SharedResources.BasicImplementations
{
    public static class MyLogger
    {
        public static void Log(this string s)
        {
            Console.WriteLine(
                $"{s as object} - Thread {Thread.CurrentThread.Name as object} {Thread.CurrentThread.ManagedThreadId as object}");
        }
    }
}
