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

ref class PressureConsumer;

public ref class PressureJoinSessionResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property PressureConsumer^ Consumer
    {
        PressureConsumer^ get() { return m_consumer; }
    internal:
        void set(_In_ PressureConsumer^ value) { m_consumer = value; }
    };

private:
    int32 m_status;
    PressureConsumer^ m_consumer;
};

public ref class PressureGetAltitudeResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property double Altitude
    {
        double get() { return m_value; }
    internal:
        void set(_In_ double value) { m_value = value; }
    }

    static PressureGetAltitudeResult^ CreateSuccessResult(_In_ double value)
    {
        auto result = ref new PressureGetAltitudeResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->Altitude = value;
        return result;
    }

    static PressureGetAltitudeResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureGetAltitudeResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    double m_value;
};

public ref class PressureSetAltitudeResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    static PressureSetAltitudeResult^ CreateSuccessResult()
    {
        auto result = ref new PressureSetAltitudeResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        return result;
    }

    static PressureSetAltitudeResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureSetAltitudeResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
};

public ref class PressureGetInchesOfMercuryResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property double InchesOfMercury
    {
        double get() { return m_value; }
    internal:
        void set(_In_ double value) { m_value = value; }
    }

    static PressureGetInchesOfMercuryResult^ CreateSuccessResult(_In_ double value)
    {
        auto result = ref new PressureGetInchesOfMercuryResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->InchesOfMercury = value;
        return result;
    }

    static PressureGetInchesOfMercuryResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureGetInchesOfMercuryResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    double m_value;
};

public ref class PressureSetInchesOfMercuryResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    static PressureSetInchesOfMercuryResult^ CreateSuccessResult()
    {
        auto result = ref new PressureSetInchesOfMercuryResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        return result;
    }

    static PressureSetInchesOfMercuryResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureSetInchesOfMercuryResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
};

public ref class PressureGetMmOfMercuryResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property double MmOfMercury
    {
        double get() { return m_value; }
    internal:
        void set(_In_ double value) { m_value = value; }
    }

    static PressureGetMmOfMercuryResult^ CreateSuccessResult(_In_ double value)
    {
        auto result = ref new PressureGetMmOfMercuryResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->MmOfMercury = value;
        return result;
    }

    static PressureGetMmOfMercuryResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureGetMmOfMercuryResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    double m_value;
};

public ref class PressureSetMmOfMercuryResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    static PressureSetMmOfMercuryResult^ CreateSuccessResult()
    {
        auto result = ref new PressureSetMmOfMercuryResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        return result;
    }

    static PressureSetMmOfMercuryResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureSetMmOfMercuryResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
};

public ref class PressureGetPascalResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property double Pascal
    {
        double get() { return m_value; }
    internal:
        void set(_In_ double value) { m_value = value; }
    }

    static PressureGetPascalResult^ CreateSuccessResult(_In_ double value)
    {
        auto result = ref new PressureGetPascalResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->Pascal = value;
        return result;
    }

    static PressureGetPascalResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureGetPascalResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    double m_value;
};

public ref class PressureSetPascalResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    static PressureSetPascalResult^ CreateSuccessResult()
    {
        auto result = ref new PressureSetPascalResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        return result;
    }

    static PressureSetPascalResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new PressureSetPascalResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
};

} } } } 
