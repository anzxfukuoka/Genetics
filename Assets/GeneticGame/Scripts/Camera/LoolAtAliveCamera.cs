using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneticGame;

public class LoolAtAliveCamera : MonoBehaviour
{

    public float cameraDistance = 10f;
    
    public float cameraRotateSpeed = 100f;

    public float cameraFollowSpeed = 10f;

    public Camera camera;

    private GeneticPlayer[] targets;
    private GeneticPlayer currTarget;

    private int currtargetIndex = 0;

    private Vector3 targetLastPos;

    private float x;
    private float y;
    private Vector3 rotateValue;

    // Start is called before the first frame update
    void Start()
    {
        camera.transform.localPosition = Vector3.one * cameraDistance;
    }

    private void LateUpdate()
    {
        if (currTarget == null || !(currTarget.alive && currTarget.enabled))
        {
            currTarget = findTarget(currtargetIndex);
        }

        transform.position = currTarget.transform.position;

        if (Input.GetMouseButton(0)) 
        {
            y = Input.GetAxis("Mouse X");
            x = Input.GetAxis("Mouse Y");
            //Debug.Log(x + ":" + y);
            rotateValue = new Vector3(x * -1, y, 0);
            //transform.eulerAngles = transform.eulerAngles - rotateValue;

            camera.transform.RotateAround(transform.position, rotateValue, cameraRotateSpeed * Time.deltaTime);
        }

        camera.transform.LookAt(transform.position);

        targetLastPos = currTarget.transform.position;
    }

    GeneticPlayer findTarget(int startIndex = 0) 
    {
        targets = FindObjectsOfType<GeneticPlayer>();

        int i = startIndex;

        while (!(targets[i].alive && targets[i].enabled)) 
        {
            i++;
            i = (int)Mathf.Repeat(i, targets.Length);

            if (i == startIndex) 
            {
                Debug.LogWarning("No alive GP");
                break;
            }
        }

        currtargetIndex = i;

        //Debug.Log("current target index " + currtargetIndex);

        return targets[i];
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, Screen.height - 40, 100, 20), "Next Alive")) 
        {
            currTarget = findTarget(currtargetIndex + 1);
        }
    }
}
