using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSwitcher : MonoBehaviour
{
    [SerializeField] GameObject UI_touch;
    private bool flag = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!flag)
        {
            return;
        }

        int count = GameObject.FindGameObjectsWithTag("TouchUI").Length;
        if (count == 0)
        {
            Invoke("ChangeCube", 1.0f);
            flag = false;
        }
    }

    void ChangeCube()
    {
        UI_touch.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
