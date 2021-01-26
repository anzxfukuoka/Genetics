using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarsNeonLights : MonoBehaviour
{
    public Light[] lights;

    public void SetLightsColor(Color color) 
    {
        for (int i = 0; i < lights.Length; i++) 
        {
            lights[i].color = color;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
