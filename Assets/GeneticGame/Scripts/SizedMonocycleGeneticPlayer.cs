using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneticGame;

public class SizedMonocycleGeneticPlayer : GeneticPlayer
{
    public float deathTimeOut = 60f; // 1 min
    private float _t = 0;

    private static float randomForceDelay = 1f;

    private float maxDegreeXLeft;
    private float maxDegreeXRight;

    private float forcePowerLeft;
    private float forcePowerRight;

    private float wheelRadius;

    private bool onGround = false;

    private Transform forcePoint;
    private Rigidbody rig;
    private GameObject wheel;

    private Material forcePointMat;

    private Vector3 lastPos;

    public override GeneticGame.Genom InitGenom()
    {
        GeneticGame.Genom genom = new GeneticGame.Genom();

        genom.AddGen(new Gen("max_degree_x_left", 180, 0));
        genom.AddGen(new Gen("max_degree_x_right", 180 ,0));
        //genom.AddGen(new Gen("force_power", 1000, 1));
        genom.AddGen(new Gen("force_power_left", 1000, 1));
        genom.AddGen(new Gen("force_power_right", 1000, 1));
        
        genom.AddGen(new Gen("wheel_radius", 2, 0.6f));

        return genom;
    }

    protected override void ApplyGenom(GeneticGame.Genom genom)
    {
        maxDegreeXLeft = genom.GetGen("max_degree_x_left").value;
        maxDegreeXRight = genom.GetGen("max_degree_x_right").value;

        forcePowerLeft = genom.GetGen("force_power_left").value;
        forcePowerRight = genom.GetGen("force_power_right").value;

        wheelRadius = genom.GetGen("wheel_radius").value;

        rig = GetComponent<Rigidbody>();

        forcePoint = transform.Find("ForcePoint");

        forcePointMat = forcePoint.gameObject.GetComponent<Renderer>().material;

        wheel = transform.Find("wheels/Wheel").gameObject;

        wheel.transform.localScale *= wheelRadius;
        wheel.transform.localPosition *= wheelRadius/2;

        StartCoroutine(AddRandomForce(genom.GetGen("force_power_left").maxVal));
    }

    private IEnumerator AddRandomForce(float max)
    {
        while (true)
        {
            float power = Random.Range(-max, max) * 0.1f;
            rig.AddForceAtPosition(Vector3.forward * power, forcePoint.position);

            yield return new WaitForSeconds(randomForceDelay);
        }
    }

    protected override bool IsAlive()
    {
        _t += Time.deltaTime;

        return !onGround && _t < deathTimeOut;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (onGround)
            return;

        if (collision.gameObject.tag == "GROUND")
        {
            //death
            onGround = true;
        }
        
    }


    protected override float ProcessStep(Sensors sensors, GeneticGame.Genom genom)
    {
        //
        forcePointMat.SetColor("_EmissionColor", Color.cyan);

        float degreeX;

        if (transform.rotation.eulerAngles.x <= 180)
            degreeX = transform.rotation.eulerAngles.x;
        else
            degreeX = transform.rotation.eulerAngles.x - 360;


        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        Vector3 backForward = transform.worldToLocalMatrix.MultiplyVector(-transform.forward);


        if (-maxDegreeXLeft > degreeX && degreeX < 0)
        {
            rig.AddForceAtPosition(localForward * forcePowerLeft, forcePoint.position);

            forcePointMat.SetColor("_EmissionColor", Color.magenta);

            //Debug.LogError("a");
        }
        if (maxDegreeXRight < degreeX && degreeX > 0)
        {
            rig.AddForceAtPosition(backForward * forcePowerRight, forcePoint.position);

            forcePointMat.SetColor("_EmissionColor", Color.yellow);

            //Debug.LogError("b");
        }

        // define score
        float meters =  lastPos.z - transform.position.z;

        float scoreForStep = meters / Time.deltaTime;

        lastPos = transform.position;


        return scoreForStep;
    }
}
