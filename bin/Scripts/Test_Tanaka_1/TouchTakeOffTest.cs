using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class TouchTakeOffTest : MonoBehaviour
{
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
        if (hit.CompareTag("Hand"))
        {
            Invoke("ChangeScene", 1.5f);
        }
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Flight_Test1");
    }
}
