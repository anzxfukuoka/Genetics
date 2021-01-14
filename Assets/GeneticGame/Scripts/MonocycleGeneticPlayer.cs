using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneticGame;

public class MonocycleGeneticPlayer : GeneticPlayer
{
    private static float randomForceDelay = 1f;

    private float forcePowerLeft;
    private float forcePowerRight;
    private float maxDegreeX;

    private bool onGround = false;

    private Transform forcePoint;
    private Rigidbody rig;

    private Material forcePointMat;

    public override GeneticGame.Genom InitGenom()
    {
        GeneticGame.Genom genom = new GeneticGame.Genom();

        genom.AddGen(new Gen("max_degree_x", 0, 180));
        //genom.AddGen(new Gen("force_power", 1000, 1));
        genom.AddGen(new Gen("force_power_left", 1000, 1));
        genom.AddGen(new Gen("force_power_right", 1000, 1));
        //genom.AddGen(new Gen("wheel_radius", 1, 10));

        return genom;
    }

    protected override void ApplyGenom(GeneticGame.Genom genom)
    {
        maxDegreeX = genom.GetGen("max_degree_x").value;
        forcePowerLeft = genom.GetGen("force_power_left").value;
        forcePowerRight = genom.GetGen("force_power_right").value;

        rig = GetComponent<Rigidbody>();

        forcePoint = transform.Find("ForcePoint");

        forcePointMat = forcePoint.gameObject.GetComponent<Renderer>().material;

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
        return !onGround;
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
        float scoreForStep = Time.deltaTime;

        forcePointMat.SetColor("_EmissionColor", Color.cyan);

        //Debug.LogError(genes.valuableDegree + " " + info.rotationX);

        //Debug.LogError(info.rotationX);

        float degreeX;

        if (transform.rotation.eulerAngles.x <= 180)
            degreeX = transform.rotation.eulerAngles.x;
        else
            degreeX = transform.rotation.eulerAngles.x - 360;


        if (-maxDegreeX > degreeX && degreeX < 0)
        {
            rig.AddForceAtPosition(Vector3.forward * forcePowerLeft, forcePoint.position);

            forcePointMat.SetColor("_EmissionColor", Color.magenta);

            //Debug.LogError("a");
        }
        if (maxDegreeX < degreeX && degreeX > 0)
        {
            rig.AddForceAtPosition(Vector3.back * forcePowerRight, forcePoint.position);

            forcePointMat.SetColor("_EmissionColor", Color.yellow);

            //Debug.LogError("b");
        }

        return scoreForStep;
    }

    
}
