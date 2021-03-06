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
#pragma once

namespace com { namespace mtcmoscow { namespace SensorHub { namespace Pressure {

public interface class IPressureService
{
public:
    // Implement this function to handle requests for the value of the Altitude property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureGetAltitudeResult^>^ GetAltitudeAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    // Implement this function to handle requests to set the Altitude property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureSetAltitudeResult^>^ SetAltitudeAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, double value);

    // Implement this function to handle requests for the value of the InchesOfMercury property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureGetInchesOfMercuryResult^>^ GetInchesOfMercuryAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    // Implement this function to handle requests to set the InchesOfMercury property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureSetInchesOfMercuryResult^>^ SetInchesOfMercuryAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, double value);

    // Implement this function to handle requests for the value of the MmOfMercury property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureGetMmOfMercuryResult^>^ GetMmOfMercuryAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    // Implement this function to handle requests to set the MmOfMercury property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureSetMmOfMercuryResult^>^ SetMmOfMercuryAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, double value);

    // Implement this function to handle requests for the value of the Pascal property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureGetPascalResult^>^ GetPascalAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

    // Implement this function to handle requests to set the Pascal property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<PressureSetPascalResult^>^ SetPascalAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info, double value);

};

} } } } 
