using System;
using SharpOpto22;

public static class Program
{
    //dotnet run
    public static void Main(string[] args)
    {
        Console.WriteLine("Tryout");
        
        var brain = new IoUnit("10.77.0.10");
        brain.SendPowerupClear();

        while(true)
        {
            if (brain.GetAndClearDigitalPointOnLatch(29)) Console.WriteLine("On");
            if (brain.GetAndClearDigitalPointOffLatch(29)) Console.WriteLine("Off");
        }
    }
}