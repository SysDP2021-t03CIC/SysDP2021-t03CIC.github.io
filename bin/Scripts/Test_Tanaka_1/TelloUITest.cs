using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelloUITest : MonoBehaviour
{
    [SerializeField] GameObject UI1;
    [SerializeField] GameObject UI2;
    [SerializeField] GameObject UI3;
    [SerializeField] GameObject UI4;
    [SerializeField] GameObject UI5;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Appear1", 0.2f);
        Invoke("Appear2", 0.4f);
        Invoke("Appear3", 0.6f);
        Invoke("Appear4", 0.8f);
        Invoke("Appear5", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Appear1()
    {
        UI1.SetActive(true);
    }

    void Appear2()
    {
        UI2.SetActive(true);
    }

    void Appear3()
    {
        UI3.SetActive(true);
    }

    void Appear4()
    {
        UI4.SetActive(true);
    }

    void Appear5()
    {
        UI5.SetActive(true);
    }
}
