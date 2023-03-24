using UnityEngine;

public static class DeviceInfo
{
    public static string GetDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}