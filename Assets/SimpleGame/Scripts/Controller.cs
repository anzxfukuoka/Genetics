using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float forcePower = 100f;
    public float randomForceDelay = 1f;

    public Transform forcePoint;
    public CapsuleCollider heartJar;

    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        StartCoroutine(AddRandomForce());
    }
    

    private IEnumerator AddRandomForce()
    {
        while (true)
        {
            float power = Random.Range(-forcePower, forcePower) * 0.1f;
            rig.AddForceAtPosition(Vector3.forward * power, forcePoint.position);

            yield return new WaitForSeconds(randomForceDelay);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
        {
            rig.AddForceAtPosition(Vector3.back * forcePower, forcePoint.position);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            rig.AddForceAtPosition(Vector3.forward * forcePower, forcePoint.position);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(Vector3.up * forcePower);
        }
    }
}
