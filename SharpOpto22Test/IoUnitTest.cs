using System;
using System.Threading;
using NUnit.Framework;
using SharpOpto22;

namespace SharpOpto22
{
	[TestFixture]
	public class IoUnitTest
	{
		[Test]
		public void ConnectTimeoutTest()
		{
			var dl = DateTime.Now.AddMilliseconds(600);
			try {
				var iou = new IoUnit("10.99.99.99", 2001, 400);
				Assert.Fail("Timeout expected");
			} catch (Exception) {
			}
			Assert.Less(DateTime.Now, dl);
		}
		
		[Test]
		public void RackSweepTest()
		{
			// required otg included
			// m0.0 = 24V
			// m0.1 = 0V
			// m0.2 = m1.2
			// m0.3 = m1.3
			// m2.1 = m3.1
			// m2.2 = m3.2
			var iou = (IIoUnit)new IoUnit("10.77.0.2");
			iou.SendPowerupClear();
			
			Assert.AreEqual("78h - SNAP-PAC-R2", iou.GetDeviceType().ToString());
			Assert.AreEqual("SNAP-PAC-R2", iou.GetDevicePartNumber());
			Assert.AreEqual("11/30/2015", iou.GetFirmwareDate());
			Assert.AreEqual("R9.4c", iou.GetFirmwareRevision());
			Assert.AreEqual("13:18:20", iou.GetFirmwareTime());
			
			Assert.AreEqual("di0", iou.GetPointName(0, 0));
			Assert.AreEqual("di1", iou.GetPointName(0, 1));
			Assert.AreEqual("di2", iou.GetPointName(0, 2));
			Assert.AreEqual("di3", iou.GetPointName(0, 3));
			Assert.AreEqual("do0", iou.GetPointName(1, 0));
			Assert.AreEqual("do1", iou.GetPointName(1, 1));
			Assert.AreEqual("do2", iou.GetPointName(1, 2));
			Assert.AreEqual("do3", iou.GetPointName(1, 3));
			Assert.AreEqual("ai0", iou.GetPointName(2, 0));
			Assert.AreEqual("ai1", iou.GetPointName(2, 1));
			Assert.AreEqual("ao0", iou.GetPointName(3, 0));
			Assert.AreEqual("ao1", iou.GetPointName(3, 1));
			
			Assert.AreEqual("0h - 4-channel digital or empty*", iou.GetModuleType(0).ToString());
			Assert.AreEqual("0h - 4-channel digital or empty*", iou.GetModuleType(1).ToString());
			Assert.AreEqual("12h - SNAP-AIV", iou.GetModuleType(2).ToString());
			Assert.AreEqual("A7h - SNAP-AOV27", iou.GetModuleType(3).ToString());
			
			iou.SetDigitalPointState(1, 2, false);
			iou.SetDigitalPointState(1, 3, false);
			Thread.Sleep(50);
			iou.GetAndClearDigitalPointOffLatch(0, 0);
			iou.GetAndClearDigitalPointOffLatch(0, 1);
			iou.GetAndClearDigitalPointOffLatch(0, 2);
			iou.GetAndClearDigitalPointOffLatch(0, 3);
			iou.GetAndClearDigitalPointOnLatch(0, 0);
			iou.GetAndClearDigitalPointOnLatch(0, 1);
			iou.GetAndClearDigitalPointOnLatch(0, 2);
			iou.GetAndClearDigitalPointOnLatch(0, 3);
			
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 1));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 1));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 3));
			
			Assert.AreEqual(true, iou.GetDigitalPointState(0, 0));
			Assert.AreEqual(false, iou.GetDigitalPointState(0, 1));
			
			iou.SetDigitalPointState(1, 2, true);
			iou.SetDigitalPointState(1, 3, false);
			Assert.AreEqual(true, iou.GetDigitalPointState(1, 2));
			Assert.AreEqual(false, iou.GetDigitalPointState(1, 3));
			Thread.Sleep(50);
			Assert.AreEqual(true, iou.GetDigitalPointState(0, 2));
			Assert.AreEqual(false, iou.GetDigitalPointState(0, 3));
			Assert.AreEqual(true, iou.GetAndClearDigitalPointOnLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 3));
			
			iou.SetDigitalPointState(1, 2, false);
			iou.SetDigitalPointState(1, 3, true);
			Assert.AreEqual(false, iou.GetDigitalPointState(1, 2));
			Assert.AreEqual(true, iou.GetDigitalPointState(1, 3));
			Thread.Sleep(50);
			Assert.AreEqual(false, iou.GetDigitalPointState(0, 2));
			Assert.AreEqual(true, iou.GetDigitalPointState(0, 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 2));
			Assert.AreEqual(true, iou.GetAndClearDigitalPointOnLatch(0, 3));
			Assert.AreEqual(true, iou.GetAndClearDigitalPointOffLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 3));			
			
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 1));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0, 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 1));			
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0, 3));			
			
			iou.SetAnalogPointValue(3, 0, 0f);
			iou.SetAnalogPointValue(3, 1, 5f);
			Assert.AreEqual(0f, iou.GetAnalogPointValue(3, 0));
			Assert.AreEqual(5f, iou.GetAnalogPointValue(3, 1));
			Thread.Sleep(50);
			Assert.That(0f, Is.EqualTo(iou.GetAnalogPointValue(2, 0)).Within(0.1));
			Assert.That(5f, Is.EqualTo(iou.GetAnalogPointValue(2, 1)).Within(0.1));

			iou.SetAnalogPointValue(3, 0, 5f);
			iou.SetAnalogPointValue(3, 1, 10f);
			Assert.AreEqual(5f, iou.GetAnalogPointValue(3, 0));
			Assert.AreEqual(10f, iou.GetAnalogPointValue(3, 1));
			Thread.Sleep(50);
			Assert.That(5f, Is.EqualTo(iou.GetAnalogPointValue(2, 0)).Within(0.1));
			Assert.That(10f, Is.EqualTo(iou.GetAnalogPointValue(2, 1)).Within(0.1));

			iou.SetAnalogPointValue(3, 0, -10f);
			iou.SetAnalogPointValue(3, 1, -5f);
			Assert.AreEqual(-10f, iou.GetAnalogPointValue(3, 0));
			Assert.AreEqual(-5f, iou.GetAnalogPointValue(3, 1));
			Thread.Sleep(50);
			Assert.That(-10f, Is.EqualTo(iou.GetAnalogPointValue(2, 0)).Within(0.1));
			Assert.That(-5f, Is.EqualTo(iou.GetAnalogPointValue(2, 1)).Within(0.1));
		}
		
		[Test]
		public void RackSweep2Test()
		{
			// required otg included
			// m0.0 = 24V
			// m0.1 = 0V
			// m0.2 = m1.2
			// m0.3 = m1.3
			// m2.1 = m3.1
			// m2.2 = m3.2
			var iou = (IIoUnit)new IoUnit("10.77.0.2");
			iou.SendPowerupClear();
			
			Assert.AreEqual("78h - SNAP-PAC-R2", iou.GetDeviceType().ToString());
			Assert.AreEqual("SNAP-PAC-R2", iou.GetDevicePartNumber());
			Assert.AreEqual("11/30/2015", iou.GetFirmwareDate());
			Assert.AreEqual("R9.4c", iou.GetFirmwareRevision());
			Assert.AreEqual("13:18:20", iou.GetFirmwareTime());
			
			Assert.AreEqual("di0", iou.GetPointName(0 * 4 + 0));
			Assert.AreEqual("di1", iou.GetPointName(0 * 4 + 1));
			Assert.AreEqual("di2", iou.GetPointName(0 * 4 + 2));
			Assert.AreEqual("di3", iou.GetPointName(0 * 4 + 3));
			Assert.AreEqual("do0", iou.GetPointName(1 * 4 + 0));
			Assert.AreEqual("do1", iou.GetPointName(1 * 4 + 1));
			Assert.AreEqual("do2", iou.GetPointName(1 * 4 + 2));
			Assert.AreEqual("do3", iou.GetPointName(1 * 4 + 3));
			Assert.AreEqual("ai0", iou.GetPointName(2 * 4 + 0));
			Assert.AreEqual("ai1", iou.GetPointName(2 * 4 + 1));
			Assert.AreEqual("ao0", iou.GetPointName(3 * 4 + 0));
			Assert.AreEqual("ao1", iou.GetPointName(3 * 4 + 1));
			
			Assert.AreEqual("0h - 4-channel digital or empty*", iou.GetModuleType(0).ToString());
			Assert.AreEqual("0h - 4-channel digital or empty*", iou.GetModuleType(1).ToString());
			Assert.AreEqual("12h - SNAP-AIV", iou.GetModuleType(2).ToString());
			Assert.AreEqual("A7h - SNAP-AOV27", iou.GetModuleType(3).ToString());
			
			iou.SetDigitalPointState(1 * 4 + 2, false);
			iou.SetDigitalPointState(1 * 4 + 3, false);
			Thread.Sleep(50);
			iou.GetAndClearDigitalPointOffLatch(0 * 4 + 0);
			iou.GetAndClearDigitalPointOffLatch(0 * 4 + 1);
			iou.GetAndClearDigitalPointOffLatch(0 * 4 + 2);
			iou.GetAndClearDigitalPointOffLatch(0 * 4 + 3);
			iou.GetAndClearDigitalPointOnLatch(0 * 4 + 0);
			iou.GetAndClearDigitalPointOnLatch(0 * 4 + 1);
			iou.GetAndClearDigitalPointOnLatch(0 * 4 + 2);
			iou.GetAndClearDigitalPointOnLatch(0 * 4 + 3);
			
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 1));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 1));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 3));
			
			Assert.AreEqual(true, iou.GetDigitalPointState(0 * 4 + 0));
			Assert.AreEqual(false, iou.GetDigitalPointState(0 * 4 + 1));
			
			iou.SetDigitalPointState(1 * 4 + 2, true);
			iou.SetDigitalPointState(1 * 4 + 3, false);
			Assert.AreEqual(true, iou.GetDigitalPointState(1 * 4 + 2));
			Assert.AreEqual(false, iou.GetDigitalPointState(1 * 4 + 3));
			Thread.Sleep(50);
			Assert.AreEqual(true, iou.GetDigitalPointState(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetDigitalPointState(0 * 4 + 3));
			Assert.AreEqual(true, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 3));
			
			iou.SetDigitalPointState(1 * 4 + 2, false);
			iou.SetDigitalPointState(1 * 4 + 3, true);
			Assert.AreEqual(false, iou.GetDigitalPointState(1 * 4 + 2));
			Assert.AreEqual(true, iou.GetDigitalPointState(1 * 4 + 3));
			Thread.Sleep(50);
			Assert.AreEqual(false, iou.GetDigitalPointState(0 * 4 + 2));
			Assert.AreEqual(true, iou.GetDigitalPointState(0 * 4 + 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 2));
			Assert.AreEqual(true, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 3));
			Assert.AreEqual(true, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 3));			
			
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 1));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOffLatch(0 * 4 + 3));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 0));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 1));			
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 2));
			Assert.AreEqual(false, iou.GetAndClearDigitalPointOnLatch(0 * 4 + 3));			
			
			iou.SetAnalogPointValue(3 * 4 + 0, 0f);
			iou.SetAnalogPointValue(3 * 4 + 1, 5f);
			Assert.AreEqual(0f, iou.GetAnalogPointValue(3 * 4 + 0));
			Assert.AreEqual(5f, iou.GetAnalogPointValue(3 * 4 + 1));
			Thread.Sleep(50);
			Assert.That(0f, Is.EqualTo(iou.GetAnalogPointValue(2 * 4 + 0)).Within(0.1));
			Assert.That(5f, Is.EqualTo(iou.GetAnalogPointValue(2 * 4 + 1)).Within(0.1));

			iou.SetAnalogPointValue(3 * 4 + 0, 5f);
			iou.SetAnalogPointValue(3 * 4 + 1, 10f);
			Assert.AreEqual(5f, iou.GetAnalogPointValue(3 * 4 + 0));
			Assert.AreEqual(10f, iou.GetAnalogPointValue(3 * 4 + 1));
			Thread.Sleep(50);
			Assert.That(5f, Is.EqualTo(iou.GetAnalogPointValue(2 * 4 + 0)).Within(0.1));
			Assert.That(10f, Is.EqualTo(iou.GetAnalogPointValue(2 * 4 + 1)).Within(0.1));

			iou.SetAnalogPointValue(3 * 4 + 0, -10f);
			iou.SetAnalogPointValue(3 * 4 + 1, -5f);
			Assert.AreEqual(-10f, iou.GetAnalogPointValue(3 * 4 + 0));
			Assert.AreEqual(-5f, iou.GetAnalogPointValue(3 * 4 + 1));
			Thread.Sleep(50);
			Assert.That(-10f, Is.EqualTo(iou.GetAnalogPointValue(2 * 4 + 0)).Within(0.1));
			Assert.That(-5f, Is.EqualTo(iou.GetAnalogPointValue(2 * 4 + 1)).Within(0.1));
		}
	}
	
	
}

