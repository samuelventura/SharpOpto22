using System;

namespace SharpOpto22
{
	public interface IIoUnit: IDisposable
	{
		MmNamedCode GetDeviceType();
	
		string GetDevicePartNumber();
	
		string GetFirmwareRevision();
	
		string GetFirmwareDate();
	
		string GetFirmwareTime();
	
		void SendPowerupClear();
	
		bool RequiresPowerupClear();
	
		MmNamedCode GetModuleType(int module);
		
		string GetPointName(int point);
	
		void SetDigitalPointState(int point, bool state);
	
		bool GetDigitalPointState(int point);
	
		bool GetAndClearDigitalPointOnLatch(int point);
		
		bool GetAndClearDigitalPointOffLatch(int point);
		
		void SetAnalogPointValue(int point, float value);
	
		float GetAnalogPointValue(int point);
		
		string GetPointName(int module, int point);
	
		void SetDigitalPointState(int module, int point, bool state);
	
		bool GetDigitalPointState(int module, int point);
	
		bool GetAndClearDigitalPointOnLatch(int module, int point);
		
		bool GetAndClearDigitalPointOffLatch(int module, int point);
		
		void SetAnalogPointValue(int module, int point, float value);
	
		float GetAnalogPointValue(int module, int point);
	}
}

