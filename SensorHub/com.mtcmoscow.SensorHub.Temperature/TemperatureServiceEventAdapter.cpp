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
using namespace com::mtcmoscow::SensorHub::Temperature;

// Note: Unlike an Interface implementation, which provides a single handler for each member, the event
// model allows for 0 or more listeners to be registered. The EventAdapter implementation deals with this
// difference by implementing a last-writer-wins policy. The lack of any return value (i.e., 0 listeners)
// is handled by returning a null result.

// Methods
// Property Reads
IAsyncOperation<TemperatureGetCelsiusResult^>^ TemperatureServiceEventAdapter::GetCelsiusAsync(_In_ AllJoynMessageInfo^ info)
{
    auto args = ref new TemperatureGetCelsiusRequestedEventArgs(info);
    AllJoynHelpers::DispatchEvent([=]() {
        GetCelsiusRequested(this, args);
    });
    return TemperatureGetCelsiusRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<TemperatureGetFahrenheitsResult^>^ TemperatureServiceEventAdapter::GetFahrenheitsAsync(_In_ AllJoynMessageInfo^ info)
{
    auto args = ref new TemperatureGetFahrenheitsRequestedEventArgs(info);
    AllJoynHelpers::DispatchEvent([=]() {
        GetFahrenheitsRequested(this, args);
    });
    return TemperatureGetFahrenheitsRequestedEventArgs::GetResultAsync(args);
}

// Property Writes
IAsyncOperation<TemperatureSetCelsiusResult^>^ TemperatureServiceEventAdapter::SetCelsiusAsync(_In_ AllJoynMessageInfo^ info, _In_ double value)
{
    auto args = ref new TemperatureSetCelsiusRequestedEventArgs(info, value);
    AllJoynHelpers::DispatchEvent([=]() {
        SetCelsiusRequested(this, args);
    });
    return TemperatureSetCelsiusRequestedEventArgs::GetResultAsync(args);
}

IAsyncOperation<TemperatureSetFahrenheitsResult^>^ TemperatureServiceEventAdapter::SetFahrenheitsAsync(_In_ AllJoynMessageInfo^ info, _In_ double value)
{
    auto args = ref new TemperatureSetFahrenheitsRequestedEventArgs(info, value);
    AllJoynHelpers::DispatchEvent([=]() {
        SetFahrenheitsRequested(this, args);
    });
    return TemperatureSetFahrenheitsRequestedEventArgs::GetResultAsync(args);
}

