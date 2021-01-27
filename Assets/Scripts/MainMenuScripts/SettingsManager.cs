using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public enum VRSelectables
    {
        NoVR,
        OculusRift
    }

    public VRSelectables vrSetting;

    // Start is called before the first frame update
    void Start()
    {
        vrSetting = VRSelectables.NoVR;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVRSetting(GameObject dropdown)
    {
        switch (dropdown.GetComponent<Dropdown>().value)
        {
            case 0:
                vrSetting = VRSelectables.NoVR;
                Debug.Log("Successfully selected NoVR");
                break;
            case 1:
                vrSetting = VRSelectables.OculusRift;
                Debug.Log("Successfully selected OculusRift");
                break;
            default:
                Debug.Log("An error has occurred with selecting the VR setting");
                break;
        }
    }
}
