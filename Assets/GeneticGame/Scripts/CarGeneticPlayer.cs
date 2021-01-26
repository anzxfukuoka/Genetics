using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneticGame;

public class CarGeneticPlayer : GeneticPlayer
{

    public float maxStayTime = 20; //20 sec

    [Header("ColoredParts")]
    public ParticleSystem bubles;
    public CarsNeonLights neonLights;

    [Header("Wheels")]

    public WheelCollider steerWheelRight;
    public WheelCollider steerWheelLeft;
    
    [Space(10)]
    
    public WheelCollider motorWheelRight;
    public WheelCollider motorWheelLeft;

    private float motorTorque;
    
    private float maxSpeed;

    private float maxSteerAngle;
    private float currSteerAngle = 0;

    private float minDistance;
    
    private float viewRadius;

    private float wheelRotatingSpeed;

    private bool crushed = false;

    private Vector3 dirLeft;
    private Vector3 dirRight;

    private Color bubleColor;
    private Color ghostColor = new Color(1, 1, 1, 0.6f);

    private Vector3 lastpos;

    [SerializeField]
    private float stayTime = 0;

    public override Genom InitGenom()
    {
        Genom genom = new Genom();

        genom.AddGen(new Gen("motor_torque", 1000, 1));

        genom.AddGen(new Gen("max_speed", 6, 1));
        
        genom.AddGen(new Gen("max_steer_angle", 60, 1));

        genom.AddGen(new Gen("min_distance", 10, 1));

        genom.AddGen(new Gen("view_radius", 2, 0.6f));

        genom.AddGen(new Gen("wheel_rotating_speed", 2, 0.6f));

        genom.AddGen(new Gen("bubles_color", 1, 0)); //Hue value

        return genom;
    }

    protected override void ApplyGenom(Genom genom)
    {
        motorTorque = genom.GetGen("motor_torque").value;

        maxSpeed = genom.GetGen("max_speed").value;

        motorWheelRight.motorTorque = -motorTorque;
        motorWheelLeft.motorTorque = -motorTorque;

        maxSteerAngle = genom.GetGen("max_steer_angle").value;

        minDistance = genom.GetGen("min_distance").value;

        viewRadius = genom.GetGen("view_radius").value;

        wheelRotatingSpeed = genom.GetGen("wheel_rotating_speed").value;  

        float bublesHue = genom.GetGen("bubles_color").value;

        bubleColor = Color.HSVToRGB(bublesHue, 1, 1);

        //set color to particle system
        var main = bubles.main;
        main.startColor = bubleColor;

        //set color to lights
        neonLights.SetLightsColor(bubleColor);

        //
        lastpos = transform.position;
    }

    protected override bool IsAlive()
    {
        if (crushed) 
        {
            motorWheelRight.motorTorque = 0;
            motorWheelLeft.motorTorque = 0;
        }

        return !crushed && stayTime < maxStayTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (crushed)
            return;

        if (collision.gameObject.tag == "WALL" || collision.gameObject.tag == "GROUND")
        {
            //death
            crushed = true;
        }
    }

    protected override float ProcessStep(Genom genom)
    {
        Vector3 posDelta = lastpos - transform.position;

        //Debug.LogError(posDelta.magnitude);

        if (posDelta.magnitude > maxSpeed)
        {
            motorWheelRight.motorTorque = 0;
            motorWheelLeft.motorTorque = 0;
        }
        else 
        {
            motorWheelRight.motorTorque = -motorTorque;
            motorWheelLeft.motorTorque = -motorTorque;
        }

        RaycastHit hit;

        dirLeft = -transform.TransformDirection(Vector3.forward) * minDistance + transform.TransformDirection(Vector3.left) * minDistance * viewRadius;
        dirRight = -transform.TransformDirection(Vector3.forward) * minDistance + transform.TransformDirection(Vector3.right) * minDistance * viewRadius;

        // Does the ray intersect any objects
        // left
        if (Physics.Raycast(transform.position, dirLeft.normalized, out hit, dirLeft.magnitude))
        {
            Debug.DrawRay(transform.position, dirLeft.normalized * hit.distance, bubleColor);

            if (-maxSteerAngle < currSteerAngle)
            {
                currSteerAngle -= wheelRotatingSpeed; //* (1 / (dirLeft.magnitude / hit.distance));
            }
            else
            {
                currSteerAngle = -maxSteerAngle;
            }

            //motorWheelRight.motorTorque = 0;
            //motorWheelLeft.motorTorque = 0;

        }
        // right
        if (Physics.Raycast(transform.position, dirRight.normalized, out hit, dirRight.magnitude))
        {
            Debug.DrawRay(transform.position, dirRight.normalized * hit.distance, bubleColor);

            if (maxSteerAngle > currSteerAngle)
            {
                currSteerAngle += wheelRotatingSpeed; //* (1 / (dirRight.magnitude / hit.distance));
            }
            else
            {
                currSteerAngle = maxSteerAngle;
            }

            //motorWheelRight.motorTorque = 0;
            //motorWheelLeft.motorTorque = 0;

        }
        // no hit
        if(hit.distance <= 0)
        {
            float value = currSteerAngle - wheelRotatingSpeed;
            float normalazed = Mathf.Abs(value) / value; // [-1, 1]

            if (Mathf.Abs(value) > wheelRotatingSpeed)
            {
                currSteerAngle -= normalazed;
            }
            else 
            {
                currSteerAngle = 0;
            }
        }

        steerWheelRight.steerAngle = currSteerAngle;
        steerWheelLeft.steerAngle = currSteerAngle;

        // count scores
        if (posDelta.magnitude < 0.08f)
        {
            stayTime += Time.deltaTime;
        }
        else 
        {
            stayTime = 0;
        }

        float score = posDelta.magnitude;

        lastpos = transform.position;

        return score;
    }

    private void OnDrawGizmos()
    {
        if (!alive)
            return;

        Gizmos.color = bubleColor;

        //draw rays
        Gizmos.color = ghostColor;
        Gizmos.DrawRay(transform.position, dirLeft);
        Gizmos.DrawRay(transform.position, dirRight);
        //draw dots
        Gizmos.color = bubleColor;
        Gizmos.DrawSphere(transform.position + dirLeft, 0.0666f);
        Gizmos.DrawSphere(transform.position + dirRight, 0.0666f);
    }

}
