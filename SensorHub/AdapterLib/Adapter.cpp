//
// Module Name:
//
//      Adapter.cpp
//
// Abstract:
//
//      Adapter class implementation.
//
//      Adapter class implements the IAdapter interface.
//      When the DSB bridge component uses the adapter it instantiates an Adapter object 
//      and uses it as the IAdapter.
//
//
#include "pch.h"
#include "Adapter.h"
#include "AdapterDevice.h"
#include <ppltasks.h>


using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace concurrency;

using namespace BridgeRT;
using namespace DsbCommon;
using namespace Windows::Devices::Gpio;
using namespace Windows::Devices::I2c;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation;

// GPIO Device
String^ deviceName = "Rpi2_GPIO_Device";
String^ vendorName = "MTC Lab";
String^ modelName = "SensorHub";
String^ version = "1.0.0.0";
String^ serialNumber = "1111111111111";
String^ description = "A Rpi2  GPIO Device created by Max Khlupnov in Microsoft Technology Center Moscow ";

String^ temperaturePropertyName = "Temperature";
String^ temperatureRawValueName = "RawTemperatureValue";
int temperatureValueData = -1;

/* GPIO Device Pin-5 Property
const int PIN_NUMBER = 5;
String^ pinName = "Pin-5";

// Pin-5 Property Attribute
String^ pinValueName = "PinValue";
int  pinValueData = -1;*/


// LED Control Pins
const  int STATUS_LED_BLUE_PIN = 6;
const int STATUS_LED_GREEN_PIN = 5;

// I2C Addresses
const int HTDU21D_I2C_ADDRESS = 0x0040;
const int MPL3115A2_I2C_ADDRESS = 0x0060;

// HTDU21D I2C Commands
const byte SAMPLE_TEMPERATURE_HOLD = 0xE3;
const byte SAMPLE_HUMIDITY_HOLD = 0xE5;

// MPL3115A2 Registers
const byte CTRL_REG1 = 0x26;
const byte OUT_P_MSB = 0x01;

GpioController^ controller;
GpioPin^ BlueLEDPin;
GpioPin^ GreenLEDPin;

// I2C Devices
I2cDevice^ htdu21d;  // Humidity and temperature sensor
I2cDevice^ mpl3115a2;  // Altitue, pressure and temperature sensor
I2cDevice^ alspt19;  // Ambient Light Sensor 


namespace AdapterLib
{

    // $appguidcomment$
    static const GUID APPLICATION_GUID = {0x755e83c7,0x52fe,0x4e25,{0xa9,0x13,0x4c,0x9c,0x81,0xf6,0x2f,0xf7}};

    Adapter::Adapter()
    {
        Windows::ApplicationModel::Package^ package = Windows::ApplicationModel::Package::Current;
        Windows::ApplicationModel::PackageId^ packageId = package->Id;
        Windows::ApplicationModel::PackageVersion versionFromPkg = packageId->Version;

        this->vendor = L"MaximKhlupnov";
        this->adapterName = L"SensorHub";
        // the adapter prefix must be something like "com.mycompany" (only alpha num and dots)
        // it is used by the Device System Bridge as root string for all services and interfaces it exposes
        this->exposedAdapterPrefix = L"com." + DsbCommon::ToLower(this->vendor->Data());
        this->exposedApplicationGuid = Platform::Guid(APPLICATION_GUID);

        if (nullptr != package &&
            nullptr != packageId )
        {
            this->exposedApplicationName = packageId->Name;
            this->version = versionFromPkg.Major.ToString() + L"." + versionFromPkg.Minor.ToString() + L"." + versionFromPkg.Revision.ToString() + L"." + versionFromPkg.Build.ToString();
        }
        else
        {
            this->exposedApplicationName = L"DeviceSystemBridge";
            this->version = L"0.0.0.0";
        }
		

		controller = GpioController::GetDefault();
		/*
		* Initialize the blue LED and set to "off"
		*
		* Instantiate the blue LED pin object
		* Write the GPIO pin value of low on the pin
		* Set the GPIO pin drive mode to output
		*/
		BlueLEDPin = controller->OpenPin(STATUS_LED_BLUE_PIN, GpioSharingMode::Exclusive);
		BlueLEDPin->Write(GpioPinValue::Low);
		BlueLEDPin->SetDriveMode(GpioPinDriveMode::Output);

		/*
		* Initialize the green LED and set to "off"
		*
		* Instantiate the green LED pin object
		* Write the GPIO pin value of low on the pin
		* Set the GPIO pin drive mode to output
		*/
		GreenLEDPin = controller->OpenPin(STATUS_LED_GREEN_PIN, GpioSharingMode::Exclusive);
		GreenLEDPin->Write(GpioPinValue::Low);
		GreenLEDPin->SetDriveMode(GpioPinDriveMode::Output);

		/*
		* Acquire the I2C device
		* MSDN I2C Reference: https://msdn.microsoft.com/en-us/library/windows/apps/windows.devices.i2c.aspx
		*
		* Use the I2cDevice device selector to create an advanced query syntax string
		* Use the Windows.Devices.Enumeration.DeviceInformation class to create a collection using the advanced query syntax string
		* Take the device id of the first device in the collection
		*/
		Platform::String^ advanced_query_syntax = I2cDevice::GetDeviceSelector("I2C1");
		
		IAsyncOperation<DeviceInformationCollection^>^  device_information_collection = DeviceInformation::FindAllAsync(advanced_query_syntax);
		auto deviceEnumTask = create_task(device_information_collection);
		
		deviceEnumTask.then([this](DeviceInformationCollection^ devices)
		{
			DeviceInformation^ di = devices->GetAt(0);
			Platform::String^ deviceId = di->Id;
		
			/*
			* Establish an I2C connection to the HTDU21D
			*
			* Instantiate the I2cConnectionSettings using the device address of the HTDU21D
			* - Set the I2C bus speed of connection to fast mode
			* - Set the I2C sharing mode of the connection to shared
			*
			* Instantiate the the HTDU21D I2C device using the device id and the I2cConnectionSettings
			*/
			I2cConnectionSettings^ htdu21d_connection = ref new I2cConnectionSettings(HTDU21D_I2C_ADDRESS);
			htdu21d_connection->BusSpeed = I2cBusSpeed::FastMode;
			htdu21d_connection->SharingMode = I2cSharingMode::Shared;

			//htdu21d = await I2cDevice.FromIdAsync(deviceId, htdu21d_connection);
			IAsyncOperation<I2cDevice^>^  htdu21d_information_collection = I2cDevice::FromIdAsync(deviceId, htdu21d_connection);
			auto htdu21dEnumTask = create_task(htdu21d_information_collection);
			htdu21dEnumTask.then([this](I2cDevice^ htdu21dDevice) {
				htdu21d = htdu21dDevice;
			});

		});
		/*
		* Establish an I2C connection to the MPL3115A2
		*
		* Instantiate the I2cConnectionSettings using the device address of the MPL3115A2
		* - Set the I2C bus speed of connection to fast mode
		* - Set the I2C sharing mode of the connection to shared
		*
		* Instantiate the the MPL3115A2 I2C device using the device id and the I2cConnectionSettings
		
		I2cConnectionSettings mpl3115a2_connection = new I2cConnectionSettings(MPL3115A2_I2C_ADDRESS);
		mpl3115a2_connection.BusSpeed = I2cBusSpeed.FastMode;
		mpl3115a2_connection.SharingMode = I2cSharingMode.Shared;

		mpl3115a2 = await I2cDevice.FromIdAsync(deviceId, mpl3115a2_connection);*/


    }


    Adapter::~Adapter()
    {
    }


    _Use_decl_annotations_
    uint32
    Adapter::SetConfiguration(const Platform::Array<byte>^ ConfigurationData)
    {
        UNREFERENCED_PARAMETER(ConfigurationData);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::GetConfiguration(Platform::Array<byte>^* ConfigurationDataPtr)
    {
        UNREFERENCED_PARAMETER(ConfigurationDataPtr);

        return ERROR_SUCCESS;
    }


    uint32
    Adapter::Initialize()
    {
		// GPIO Device Descriptor: Static data for our device
		DEVICE_DESCRIPTOR gpioDeviceDesc;
		gpioDeviceDesc.Name = deviceName;
		gpioDeviceDesc.VendorName = vendorName;
		gpioDeviceDesc.Model = modelName;
		gpioDeviceDesc.Version = version;
		gpioDeviceDesc.SerialNumer = serialNumber;
		gpioDeviceDesc.Description = description;

		// Define Temperature as device property. Device contains properties
		AdapterProperty^ temperature_Property = ref new AdapterProperty(temperaturePropertyName, "");
		temperatureValueData = static_cast<int>(this->rawTemperature());
		AdapterValue^ temperatureAttr_Value = ref new AdapterValue(temperatureRawValueName, temperatureValueData);
		temperature_Property += temperatureAttr_Value;


		// Finally, put it all into a new device
		AdapterDevice^ gpioDevice = ref new AdapterDevice(&gpioDeviceDesc);
		gpioDevice->AddProperty(temperature_Property);
		devices.push_back(std::move(gpioDevice));



        return ERROR_SUCCESS;
    }
	

    uint32
    Adapter::Shutdown()
    {
        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::EnumDevices(
        ENUM_DEVICES_OPTIONS Options,
        IAdapterDeviceVector^* DeviceListPtr,
        IAdapterIoRequest^* RequestPtr
        )
    {
        UNREFERENCED_PARAMETER(Options);
        UNREFERENCED_PARAMETER(RequestPtr);

        try
        {
            *DeviceListPtr = ref new BridgeRT::AdapterDeviceVector(this->devices);
        }
        catch (OutOfMemoryException^ ex)
        {
            throw ref new OutOfMemoryException(ex->Message);
        }

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::GetProperty(
        IAdapterProperty^ Property,
        IAdapterIoRequest^* RequestPtr
        )
    {
        UNREFERENCED_PARAMETER(Property);
        UNREFERENCED_PARAMETER(RequestPtr);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::SetProperty(
        IAdapterProperty^ Property,
        IAdapterIoRequest^* RequestPtr
        )
    {
        UNREFERENCED_PARAMETER(Property);
        UNREFERENCED_PARAMETER(RequestPtr);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::GetPropertyValue(
        IAdapterProperty^ Property,
        String^ AttributeName,
        IAdapterValue^* ValuePtr,
        IAdapterIoRequest^* RequestPtr
        )
    {
        //UNREFERENCED_PARAMETER(Property);
        
        // UNREFERENCED_PARAMETER(ValuePtr);
        UNREFERENCED_PARAMETER(RequestPtr);
		UNREFERENCED_PARAMETER(AttributeName);

		temperatureValueData = static_cast<int>(this->rawTemperature());

		AdapterProperty^ adapterProperty = dynamic_cast<AdapterProperty^>(Property);
		AdapterValue^ attribute = dynamic_cast<AdapterValue^>(adapterProperty->Attributes->GetAt(0));
		attribute->Data = temperatureValueData;

		*ValuePtr = attribute;

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::SetPropertyValue(
        IAdapterProperty^ Property,
        IAdapterValue^ Value,
        IAdapterIoRequest^* RequestPtr
        )
    {
        UNREFERENCED_PARAMETER(Property);
        UNREFERENCED_PARAMETER(Value);
        UNREFERENCED_PARAMETER(RequestPtr);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::CallMethod(
        IAdapterMethod^ Method,
        IAdapterIoRequest^* RequestPtr
        )
    {
        UNREFERENCED_PARAMETER(Method);
        UNREFERENCED_PARAMETER(RequestPtr);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::RegisterSignalListener(
        IAdapterSignal^ Signal,
        IAdapterSignalListener^ Listener,
        Object^ ListenerContext
        )
    {
        UNREFERENCED_PARAMETER(Signal);
        UNREFERENCED_PARAMETER(Listener);
        UNREFERENCED_PARAMETER(ListenerContext);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::UnregisterSignalListener(
        IAdapterSignal^ Signal,
        IAdapterSignalListener^ Listener
        )
    {
        UNREFERENCED_PARAMETER(Signal);
        UNREFERENCED_PARAMETER(Listener);

        return ERROR_SUCCESS;
    }


    _Use_decl_annotations_
    uint32
    Adapter::NotifySignalListener(
        IAdapterSignal^ Signal
        )
    {
        UNREFERENCED_PARAMETER(Signal);

        return ERROR_SUCCESS;
    }


    uint32
    Adapter::createSignals()
    {
        return ERROR_SUCCESS;
    }
	uint32 Adapter::rawTemperature()
	{

		uint32 temperature = 0;
		int byteLen = 3;
		Platform::Array<BYTE>^ i2c_temperature_data = ref new Platform::Array<BYTE>(byteLen);
		//byte[] i2c_temperature_data = new byte[3];

		/*
		* Request temperature data from the HTDU21D
		* HTDU21D datasheet: http://dlnmh9ip6v2uc.cloudfront.net/datasheets/BreakoutBoards/HTU21D.pdf
		*
		* Write the SAMPLE_TEMPERATURE_HOLD command (0xE3) to the HTDU21D
		* - HOLD means it will block the I2C line while the HTDU21D calculates the temperature value
		*
		* Read the three bytes returned by the HTDU21D
		* - byte 0 - MSB of the temperature
		* - byte 1 - LSB of the temperature
		* - byte 2 - CRC
		*
		* NOTE: Holding the line allows for a `WriteRead` style transaction
		*/
		htdu21d->WriteRead(ref new Platform::Array<BYTE>{ SAMPLE_TEMPERATURE_HOLD }, i2c_temperature_data);

		/*
		* Reconstruct the result using the first two bytes returned from the device
		*
		* NOTE: Zero out the status bits (bits 0 and 1 of the LSB), but keep them in place
		* - status bit 0 - not assigned
		* - status bit 1
		* -- off = temperature data
		* -- on = humdity data
		*/
		temperature = (uint32)(i2c_temperature_data[0] << 8);
		temperature |= (uint32)(i2c_temperature_data[1] & 0xFC);

		/*
		* Test the integrity of the data
		*
		* Ensure the data returned is temperature data (hint: byte 1, bit 1)
		* Test cyclic redundancy check (CRC) byte
		*/
		bool temperature_data = (0x00 == (0x02 & i2c_temperature_data[1]));
		if (!temperature_data) { return 0; }

		bool valid_data = ValidHtdu21dCyclicRedundancyCheck(temperature, i2c_temperature_data[2]);
		if (!valid_data) { return 0; }

		return temperature;

	//	return ERROR_SUCCESS;
	}

	bool Adapter::ValidHtdu21dCyclicRedundancyCheck(
		uint32 data_,
		byte crc_
		)
	{
		/*
		* Validate the 8-bit cyclic redundancy check (CRC) byte
		* CRC: http://en.wikipedia.org/wiki/Cyclic_redundancy_check
		* Generator polynomial x^8 + x^5 + x^4 + 1: 100110001(0x0131)
		*/

		const int CRC_BIT_LENGTH = 8;
		const int DATA_LENGTH = 16;
		const uint32 GENERATOR_POLYNOMIAL = 0x0131;

		int crc_data = data_ << CRC_BIT_LENGTH;

		for (int i = (DATA_LENGTH - 1); 0 <= i; --i)
		{
			if (0 == (0x01 & (crc_data >> (CRC_BIT_LENGTH + i)))) { continue; }
			crc_data ^= (GENERATOR_POLYNOMIAL << i);
		}

		return (crc_ == crc_data);
	}

	uint32 Adapter::rawHumidity()
	{
		return ERROR_SUCCESS;
	}
	uint32 Adapter::rawPressure()
	{
		return ERROR_SUCCESS;
	}

	
} // namespace AdapterLib