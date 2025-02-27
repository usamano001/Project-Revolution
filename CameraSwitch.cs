using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public Camera playerCamera;
    public Camera vehicleCamera;

    public void SwitchToVehicleCamera()
    {
        playerCamera.enabled = false;
        vehicleCamera.enabled = true;
    }

    public void SwitchToPlayerCamera()
    {
        playerCamera.enabled = true;
        vehicleCamera.enabled = false;
    }
}
