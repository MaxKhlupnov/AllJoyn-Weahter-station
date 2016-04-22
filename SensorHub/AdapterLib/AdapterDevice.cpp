//
// Module Name:
//
//      AdapterDevice.cpp
//
// Abstract:
//
//      AdapterValue, AdapterProperty, AdapterSignal, and AdapterDevice classes implementation.
//
//      Together with Adapter class, the classes declared in this file implement the IAdapter interface.
//
//
#include "pch.h"
#include "Adapter.h"
#include "AdapterDevice.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::System::Profile;

using namespace BridgeRT;
using namespace DsbCommon;

namespace AdapterLib
{
    //
    // AdapterValue.
    // Description:
    // The class that implements BridgeRT::IAdapterValue.
    //
    AdapterValue::AdapterValue(
        String^ ObjectName,
        Object^ DefaultData // = nullptr
        )
        : name(ObjectName)
        , data(DefaultData)
    {
    }


    AdapterValue::AdapterValue(const AdapterValue^ Other)
        : name(Other->name)
        , data(Other->data)
    {
    }


    uint32 AdapterValue::Set(IAdapterValue^ Other)
    {
        this->name = Other->Name;
        this->data = Other->Data;

        return ERROR_SUCCESS;
    }


    //
    // AdapterProperty.
    // Description:
    // The class that implements BridgeRT::IAdapterProperty.
    //
    AdapterProperty::AdapterProperty(
        String^ ObjectName, 
        String^ IfHint
        )
        : name(ObjectName)
        , interfaceHint(IfHint)
    {
    }


    AdapterProperty::AdapterProperty(const AdapterProperty^ Other)
        : name(Other->name)
        , attributes(Other->attributes)
        , interfaceHint(Other->interfaceHint)
    {
    }


    uint32
    AdapterProperty::Set(IAdapterProperty^ Other)
    {
        this->name = Other->Name;

        IAdapterValueVector^ otherAttributes = Other->Attributes;

        if (this->attributes.size() != otherAttributes->Size)
        {
            throw ref new InvalidArgumentException(L"Incompatible Adapter Properties");
        }

        for (uint32 attrInx = 0; attrInx < otherAttributes->Size; ++attrInx)
        {
            AdapterValue^ attr = static_cast<AdapterValue^>(this->attributes[attrInx]);

            attr->Set(otherAttributes->GetAt(attrInx));
        }

        return ERROR_SUCCESS;
    }


    //
    // AdapterMethod.
    // Description:
    // The class that implements BridgeRT::IAdapterMethod.
    //
    AdapterMethod::AdapterMethod(
		String^ ObjectName
        )
        : name(ObjectName)
        , result(HRESULT_FROM_WIN32(ERROR_NOT_READY))
    {
    }


    AdapterMethod::AdapterMethod(const AdapterMethod^ Other)
        : name(Other->name)
        , description(Other->description)
        , inParams(Other->inParams)
        , outParams(Other->outParams)
        , result(Other->result)
    {
    }


    void
    AdapterMethod::InputParams::set(IAdapterValueVector^ Params)
    {
        if (Params->Size != this->inParams.size())
        {
            throw ref new InvalidArgumentException(L"Incompatible method input parameters");
        }

        for (uint32 paramInx = 0; paramInx < Params->Size; ++paramInx)
        {
            dynamic_cast<AdapterValue^>(this->inParams[paramInx])->Set(Params->GetAt(paramInx));
        }
    }


    void
    AdapterMethod::SetResult(HRESULT Hr)
    {
        this->result = Hr;
    }


    //
    // AdapterSignal.
    // Description:
    // The class that implements BridgeRT::IAdapterSignal.
    //
    AdapterSignal::AdapterSignal(
		String^ ObjectName
        )
        : name(ObjectName)
    {
    }


    AdapterSignal::AdapterSignal(const AdapterSignal^ Other)
        : name(Other->name)
        , params(Other->params)
    {
    }


    //
    // AdapterDevice.
    // Description:
    // The class that implements BridgeRT::IAdapterDevice.
    //
	AdapterDevice::AdapterDevice(
		String^ ObjectName 
		)
		: name(ObjectName)
	{
	}

    AdapterDevice::AdapterDevice(
		const DEVICE_DESCRIPTOR* DeviceDescPtr 
		)
        : name(DeviceDescPtr->FriendlyName)
		, id(DeviceDescPtr->Id)		
		, systemHardwareVersion(DeviceDescPtr->HardwareVersion)
		, systemSoftwareVersion(DeviceDescPtr->SoftwareVersion)
        , systemManufacturer(DeviceDescPtr->SystemManufacturer)
        , systemProductName(DeviceDescPtr->SystemProductName)        
        , systemSku(DeviceDescPtr->SystemSku)
    {
    }


    AdapterDevice::AdapterDevice(const AdapterDevice^ Other)
        : name(Other->name)
		, id(Other->id)		
		, systemHardwareVersion(Other->systemHardwareVersion)
		, systemSoftwareVersion(Other->systemSoftwareVersion)
		, systemManufacturer(Other->systemManufacturer)
		, systemProductName(Other->systemProductName)
		, systemSku(Other->systemSku)
    {
    }
	bool AdapterDevice::ValidHtdu21dCyclicRedundancyCheck(uint16 data_, byte crc_)
	{
		return false;
	}

	void AdapterDevice::Initialize() {
		
		
	}

} // namespace AdapterLib