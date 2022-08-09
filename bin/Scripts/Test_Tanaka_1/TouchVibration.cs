using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TouchVibration : MonoBehaviour
{
    SteamVR_Action_Vibration haptics = SteamVR_Actions._default.Haptic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider hit)
    {
        Debug.Log("hit!");
        if (hit.CompareTag("TestUI"))
        {
            haptics.Execute(0, 0.1f, 100, 1, SteamVR_Input_Sources.RightHand);
        }
    }
}
