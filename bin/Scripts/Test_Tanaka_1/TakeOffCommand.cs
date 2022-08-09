using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeOffCommand : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] GameObject UI_parent;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider hit)
    {
        Debug.Log("hit!");
        if (hit.CompareTag("Hand"))
        {
            screen.SetActive(true);
            Invoke("TakeOff", 1.0f);
        }
    }

    void TakeOff()
    {
        screen.GetComponent<TelloController1122>().Tello_TakeOff();
        UI_parent.SetActive(false);
    }
}
