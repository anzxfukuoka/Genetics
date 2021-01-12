using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartAnim : MonoBehaviour
{
    public float speed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.up * Mathf.Sin(Time.time * speed) * 0.1f;
        transform.Rotate(Vector3.up * speed/4);
    }
}
