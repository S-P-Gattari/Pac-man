using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitch : MonoBehaviour
{
    public GameObject canvasB;  
    public GameObject canvasS;  
    public float switchInterval = 1f;  

    private void Start()
    {
        StartCoroutine(SwitchCanvases());
    }

    IEnumerator SwitchCanvases()
    {
        while (true)
        {
            canvasB.SetActive(true);
            canvasS.SetActive(false);
            yield return new WaitForSeconds(switchInterval);

            canvasB.SetActive(false);
            canvasS.SetActive(true);
            yield return new WaitForSeconds(switchInterval);
        }
    }
}
