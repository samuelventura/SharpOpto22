using System;
using System.Text;
using SharpOpto22;

public static class Program
{
    //dotnet run
    public static void Main(string[] args)
    {
        Console.WriteLine("Tryout");
        
        var brain = new IoUnit("10.77.0.10");
        brain.SendPowerupClear();
        brain.SetDigitalPointState(4 + 1, true);

        var counter = new FrequencyPoint();

        while(true)
        {
            var abank = brain.ReadAnalogBank();
            var dbank = brain.ReadDigitalBank();

            var latches0 = brain.ReadModuleLatches(0);
            var latches7 = brain.ReadModuleLatches(7);
            if (latches7[0]) counter.AddTransition();
            if (latches7[0]) counter.AddTransition();
            counter.UpdateFrequency();

            //Console.WriteLine(ToHex(dbank, 0, dbank.Length));
            //Console.WriteLine(ToHex(abank, 0, abank.Length));
            Console.WriteLine("F:{0} DM:[{1} {2} {3} {4}] AM:[{5:0.0} {6:0.0}]", 
                counter.Value, 
                brain.GetDigitalPointState(dbank, 0),
                brain.GetDigitalPointState(dbank, 1),
                brain.GetDigitalPointState(dbank, 2),
                brain.GetDigitalPointState(dbank, 3),
                brain.GetAnalogPointValue(abank, 16),
                brain.GetAnalogPointValue(abank, 17));
        }    
    }

    public static string ToHex(byte[] d, int offset, int length)
    {
        var sb = new StringBuilder();
        var end = offset + length;
        for(var i=offset;i<end;i++) sb.Append(d[i].ToString("X2"));
        return sb.ToString();
    }
}