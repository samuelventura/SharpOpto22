using System;
using System.Threading;
using NUnit.Framework;
using SharpOpto22;

namespace SharpOpto22
{
	[TestFixture]
	public class TryoutTest
	{
		//dotnet test --filter Test1 -v n
		[Test]
		public void Test1()
		{
			var iou = (IIoUnit)new IoUnit("10.77.0.10");
			iou.SendPowerupClear();
			var state = iou.GetDigitalPointState(0);
			Console.WriteLine("State {0}", state);
		}
	}
}

