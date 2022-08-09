using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EstimateTargetTransform_OCVfU;

public class TextureSender : MonoBehaviour
{
    // CameraParaCal cameraPara;
    public EstimateTra estimater;
    Texture2D texture2;
    bool estFlag = false;
    Texture2D result;
    RenderTexture rt;
    Rect source;

    // Start is called before the first frame update
    void Start()
    {
        // cameraPara = new CameraParaCal();
        estimater = new EstimateTra();
    }
    public Texture2D ToTexture2D(Texture self)
    {
        var sw = self.width;
        var sh = self.height;
        var format = TextureFormat.RGBA32;
        result = new Texture2D(sw, sh, format, false);
        var currentRT = RenderTexture.active;
        rt = new RenderTexture(sw, sh, 32);
        Graphics.Blit(self, rt);
        RenderTexture.active = rt;
        source = new Rect(0, 0, rt.width, rt.height);
        result.ReadPixels(source, 0, 0);
        result.Apply();
        RenderTexture.active = currentRT;
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        time += Time.deltaTime;
        if (time > 0.2f)
        {
            texture = ToTexture2D(GetComponent<Renderer>().material.mainTexture);
            cameraPara.DataSampling(texture);
            time = 0.0f;
        }
        */

        texture2 = ToTexture2D(GetComponent<Renderer>().material.mainTexture);
        estimater.Estimate(texture2);
        MonoBehaviour.Destroy(result);
        MonoBehaviour.Destroy(rt);
        //Resources.UnloadUnusedAssets();

        //if (estFlag)
        //{
        //    texture = ToTexture2D(GetComponent<Renderer>().material.mainTexture);
        //    estimater.Estimate(texture);
        //    estFlag = false;
        //}
        //else
        //{
        //    estFlag = true;
        //}
    }
}
