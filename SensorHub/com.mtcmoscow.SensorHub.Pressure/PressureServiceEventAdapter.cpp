//-----------------------------------------------------------------------------
// <auto-generated> 
//   This code was generated by a tool. 
// 
//   Changes to this file may cause incorrect behavior and will be lost if  
//   the code is regenerated.
//
//   Tool: AllJoynCodeGenerator.exe
//
//   This tool is located in the Windows 10 SDK and the Windows 10 AllJoyn 
//   Visual Studio Extension in the Visual Studio Gallery.  
//
//   The generated code should be packaged in a Windows 10 C++/CX Runtime  
//   Component which can be consumed in any UWP-supported language using 
//   APIs that are available in Windows.Devices.AllJoyn.
//
//   Using AllJoynCodeGenerator - Invoke the following command with a valid 
//   Introspection XML file and a writable output directory:
//     AllJoynCodeGenerator -i <INPUT XML FILE> -o <OUTPUT DIRECTORY>
// </auto-generated>
//-----------------------------------------------------------------------------
#include "pch.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Devices::AllJoyn;
using namespace com::mtcmoscow::SensorHub::Pressure;

// Note: Unlike an Interface implementation, which provides a single handler for each member, the event
// model allows for 0 or more listeners to be registered. The EventAdapter implementation deals with this
// difference by implementing a last-writer-wins policy. The lack of any return value (i.e., 0 listeners)
// is handled by returning a null result.

// Methods
// Property Reads
IAsyncOperation<PressureGetAltitudeResult^>^ PressureServiceEventAdapter::GetAltitudeAsync(_In_ AllJoynMessageInfo^ info)
{
    auto args = ref new PressureGetAltitudeRequestedEventArgs(info);
    AllJoynHelpers::DispatchEvent([=]() {
        GetAltitudeRequested(this, args);
    });
    return PressureGetAltitudeRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<PressureGetInchesOfMercuryResult^>^ PressureServiceEventAdapter::GetInchesOfMercuryAsync(_In_ AllJoynMessageInfo^ info)
{
    auto args = ref new PressureGetInchesOfMercuryRequestedEventArgs(info);
    AllJoynHelpers::DispatchEvent([=]() {
        GetInchesOfMercuryRequested(this, args);
    });
    return PressureGetInchesOfMercuryRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<PressureGetMmOfMercuryResult^>^ PressureServiceEventAdapter::GetMmOfMercuryAsync(_In_ AllJoynMessageInfo^ info)
{
    auto args = ref new PressureGetMmOfMercuryRequestedEventArgs(info);
    AllJoynHelpers::DispatchEvent([=]() {
        GetMmOfMercuryRequested(this, args);
    });
    return PressureGetMmOfMercuryRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<PressureGetPascalResult^>^ PressureServiceEventAdapter::GetPascalAsync(_In_ AllJoynMessageInfo^ info)
{
    auto args = ref new PressureGetPascalRequestedEventArgs(info);
    AllJoynHelpers::DispatchEvent([=]() {
        GetPascalRequested(this, args);
    });
    return PressureGetPascalRequestedEventArgs::GetResultAsync(args);
}

// Property Writes
IAsyncOperation<PressureSetAltitudeResult^>^ PressureServiceEventAdapter::SetAltitudeAsync(_In_ AllJoynMessageInfo^ info, _In_ double value)
{
    auto args = ref new PressureSetAltitudeRequestedEventArgs(info, value);
    AllJoynHelpers::DispatchEvent([=]() {
        SetAltitudeRequested(this, args);
    });
    return PressureSetAltitudeRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<PressureSetInchesOfMercuryResult^>^ PressureServiceEventAdapter::SetInchesOfMercuryAsync(_In_ AllJoynMessageInfo^ info, _In_ double value)
{
    auto args = ref new PressureSetInchesOfMercuryRequestedEventArgs(info, value);
    AllJoynHelpers::DispatchEvent([=]() {
        SetInchesOfMercuryRequested(this, args);
    });
    return PressureSetInchesOfMercuryRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<PressureSetMmOfMercuryResult^>^ PressureServiceEventAdapter::SetMmOfMercuryAsync(_In_ AllJoynMessageInfo^ info, _In_ double value)
{
    auto args = ref new PressureSetMmOfMercuryRequestedEventArgs(info, value);
    AllJoynHelpers::DispatchEvent([=]() {
        SetMmOfMercuryRequested(this, args);
    });
    return PressureSetMmOfMercuryRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<PressureSetPascalResult^>^ PressureServiceEventAdapter::SetPascalAsync(_In_ AllJoynMessageInfo^ info, _In_ double value)
{
    auto args = ref new PressureSetPascalRequestedEventArgs(info, value);
    AllJoynHelpers::DispatchEvent([=]() {
        SetPascalRequested(this, args);
    });
    return PressureSetPascalRequestedEventArgs::GetResultAsync(args);
}

