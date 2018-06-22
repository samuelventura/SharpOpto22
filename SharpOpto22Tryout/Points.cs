using System;
using System.Collections.Generic;

public class AnalogInput : InputPoint<decimal>
{
}

public class InputPoint<T>
{
    private readonly object locker = new object();

    private T read;

    public T Value
    {
        get { lock (locker) { return read; } }
        set { lock (locker) { read = value; } }
    }
}

public class FrequencyPoint : AnalogInput
{
    private Queue<DateTime> transitions = new Queue<DateTime>();

    public void AddTransition()
    {
        transitions.Enqueue(DateTime.Now);
    }

    public void UpdateFrequency()
    {
        while (transitions.Count > 0)
        {
            var dt = transitions.Peek();
            var ts = DateTime.Now - dt;
            if (ts.TotalSeconds > 1)
            {
                transitions.Dequeue();
            }
            else
            {
                break;
            }
        }
        Value = transitions.Count / 2.0m;
    }
}
