//
// Module Name:
//
//      Adapter.h
//
// Abstract:
//
//      Adapter class declaration.
//
//      Adapter class implements the IAdapter interface.
//      When the DSB bridge component uses the adapter it instantiates an Adapter object 
//      and uses it as the IAdapter.
//
//
#pragma once

#include "AdapterDefinitions.h"
#include "Misc.h"

namespace AdapterLib
{
    //
    // Adapter class.
    // Description:
    // The class that implements the IAdapter.
    //
    public ref class Adapter sealed : public BridgeRT::IAdapter
    {
    public:
        Adapter();
        virtual ~Adapter();

        //
        // Adapter information
        //
        virtual property Platform::String^ Vendor
        {
            Platform::String^ get() { return this->vendor; }
        }
        virtual property Platform::String^ AdapterName
        {
            Platform::String^ get() { return this->adapterName; }
        }
        virtual property Platform::String^ Version
        {
            Platform::String^ get() { return this->version; }
        }
        virtual property Platform::String^ ExposedAdapterPrefix
        {
            Platform::String^ get() { return this->exposedAdapterPrefix; }
        }
        virtual property Platform::String^ ExposedApplicationName
        {
            Platform::String^ get() { return this->exposedApplicationName; }
        }
        virtual property Platform::Guid ExposedApplicationGuid
        {
            Platform::Guid get() { return this->exposedApplicationGuid; }
        }

        // Adapter signals
        virtual property BridgeRT::IAdapterSignalVector^ Signals
        {
            BridgeRT::IAdapterSignalVector^ get() { return ref new BridgeRT::AdapterSignalVector(this->signals); }
        }

        virtual uint32 SetConfiguration(_In_ const Platform::Array<byte>^ ConfigurationData);
        virtual uint32 GetConfiguration(_Out_ Platform::Array<byte>^* ConfigurationDataPtr);

        virtual uint32 Initialize();
        virtual uint32 Shutdown();

        virtual uint32 EnumDevices(
            _In_ BridgeRT::ENUM_DEVICES_OPTIONS Options,
            _Out_ BridgeRT::IAdapterDeviceVector^* DeviceListPtr,
            _Out_opt_ BridgeRT::IAdapterIoRequest^* RequestPtr
            );

        virtual uint32 GetProperty(
            _Inout_ BridgeRT::IAdapterProperty^ Property,
            _Out_opt_ BridgeRT::IAdapterIoRequest^* RequestPtr
            );
        virtual uint32 SetProperty(
            _In_ BridgeRT::IAdapterProperty^ Property,
            _Out_opt_ BridgeRT::IAdapterIoRequest^* RequestPtr
            );

        virtual uint32 GetPropertyValue(
            _In_ BridgeRT::IAdapterProperty^ Property,
            _In_ Platform::String^ AttributeName,
            _Out_ BridgeRT::IAdapterValue^* ValuePtr,
            _Out_opt_ BridgeRT::IAdapterIoRequest^* RequestPtr
            );
        virtual uint32 SetPropertyValue(
            _In_ BridgeRT::IAdapterProperty^ Property,
            _In_ BridgeRT::IAdapterValue^ Value,
            _Out_opt_ BridgeRT::IAdapterIoRequest^* RequestPtr
            );

        virtual uint32 CallMethod(
            _Inout_ BridgeRT::IAdapterMethod^ Method,
            _Out_opt_ BridgeRT::IAdapterIoRequest^* RequestPtr
            );

        virtual uint32 RegisterSignalListener(
            _In_ BridgeRT::IAdapterSignal^ Signal,
            _In_ BridgeRT::IAdapterSignalListener^ Listener,
            _In_opt_ Platform::Object^ ListenerContext
            );
        virtual uint32 UnregisterSignalListener(
            _In_ BridgeRT::IAdapterSignal^ Signal,
            _In_ BridgeRT::IAdapterSignalListener^ Listener
            );

        //
        //  Routine Description:
        //      NotifySignalListener is called by the Adapter to notify a registered
        //      signal listener of an intercepted signal.
        //
        //  Arguments:
        //
        //      Signal - The signal object to notify listeners.
        //
        //  Return Value:
        //      ERROR_SUCCESS,
        //      ERROR_INVALID_HANDLE: Invalid signal object.
        //
        uint32 NotifySignalListener(
            _In_ BridgeRT::IAdapterSignal^ Signal
            );

		/// <summary>
		/// Calculate current temperature
		/// </summary>
		/// <returns>
		/// The temperature in Celcius (C)
		/// </returns>
		property double TemperatureCelcius
		{
			double get() {
				uint32 raw_temperature_data = rawTemperature();
				double temperature_C = (((175.72 * raw_temperature_data) / 65536) - 46.85);
				
				return temperature_C;
				//return Convert::ToSingle(doubleVal);
			}
		}

		/// <summary>
		/// Calculate relative humidity
		/// </summary>
		/// <returns>
		/// The relative humidity
		/// </returns>
		property double Humidity
		{
			double get()
			{
				uint32 raw_humidity_data = rawHumidity();
				return (((125.0 * raw_humidity_data) / 65536) - 6.0);
			
			}
		}

		/// <summary>
		/// Read pressure data
		/// </summary>
		/// <returns>
		/// The pressure in Pascals (Pa)
		/// </returns>
		property double Pressure
		{
			double get()
		{
			uint32 raw_pressure_data = rawPressure();
			double pressure_Pa = ((raw_pressure_data >> 6) + (((raw_pressure_data >> 4) & 0x03) / 4.0));

			return pressure_Pa;//Convert.ToSingle(pressure_Pa);
		}
		}

    private:
        uint32 createSignals();
		uint32  rawTemperature();
		uint32  rawHumidity();
		uint32  rawPressure();
		double	Celcius2Fahrenheits(float Celcius);
		double	Pascal2InchesOfMercury(float Pascal);
		double	Pascal2MmOfMercury(float Pascal);
		double	Pascal2Altitude(float Pascal);
		bool	ValidHtdu21dCyclicRedundancyCheck(uint32 data_, byte crc_);
		Platform::String^ FormatInterfaceHint(Platform::String^ propertyName);

    private:
        Platform::String^ vendor;
        Platform::String^ adapterName;
        Platform::String^ version;
        // the prefix for AllJoyn service should be something like
        // com.mycompany (only alpha num and dots) and is used by the Device System Bridge
        // as root string for all services and interfaces it exposes
        Platform::String^ exposedAdapterPrefix;

        // name and GUID of the DSB/Adapter application that will be published on AllJoyn
        Platform::String^ exposedApplicationName;
        Platform::Guid exposedApplicationGuid;

        // Devices
        std::vector<BridgeRT::IAdapterDevice^> devices;

        // Signals 
        std::vector<BridgeRT::IAdapterSignal^> signals;

        // Sync object
        DsbCommon::CSLock lock;

        //
        // Signal listener entry
        //
        struct SIGNAL_LISTENER_ENTRY
        {
            SIGNAL_LISTENER_ENTRY(
                BridgeRT::IAdapterSignal^ SignalToRegisterTo,
                BridgeRT::IAdapterSignalListener^ ListenerObject,
                Platform::Object^ ListenerContext
                )
                : Signal(SignalToRegisterTo)
                , Listener(ListenerObject)
                , Context(ListenerContext)
            {
            }

            // The  signal object
            BridgeRT::IAdapterSignal^ Signal;

            // The listener object
            BridgeRT::IAdapterSignalListener^ Listener;

            //
            // The listener context that will be
            // passed to the signal handler
            //
            Platform::Object^ Context;
        };

        // A map of signal handle (object's hash code) and related listener entry
        std::multimap<int, SIGNAL_LISTENER_ENTRY> signalListeners;
    };
} // namespace AdapterLib