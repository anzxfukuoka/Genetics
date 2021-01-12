using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private static float maxForce = 10000f;
    private static float randomForceDelay = 1f;

    public UnitInfo unitInfo = new UnitInfo();
    public Genom genes;

    private Transform forcePoint;
    private CapsuleCollider heartJar;

    private Rigidbody rig;

    private float time; // время жизни юнита

    private Material forcePointMat;

    public static Genom GemerateRandomGenes() 
    {
        Genom genes = new Genom();
        genes.forcePower = Random.Range(0, maxForce);
        genes.valuableDegree = Random.rotation.eulerAngles.x/2;
        return genes;
    }

    public void InitGenes(Genom genom)
    {
        this.genes = genom;
    }

    public void InitGenes()
    {
        InitGenes(GemerateRandomGenes());
    }

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        
        forcePoint = transform.Find("ForcePoint");
        heartJar = GetComponent<CapsuleCollider>();

        forcePointMat = forcePoint.gameObject.GetComponent<Renderer>().material;

        StartCoroutine(AddRandomForce(genes.forcePower));
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

    public void UpdateUnitInfo()
    {
        if (unitInfo.alive)
        {
            if(transform.rotation.eulerAngles.x <= 180)
                unitInfo.rotationX = transform.rotation.eulerAngles.x;
            else
                unitInfo.rotationX = transform.rotation.eulerAngles.x - 360;
        }
    }

    public void GeneticInput(Genom genes, UnitInfo info) 
    {
        forcePointMat.SetColor("_EmissionColor", Color.cyan);

        if (!unitInfo.alive)
            return;

        //Debug.LogError(genes.valuableDegree + " " + info.rotationX);

        //Debug.LogError(info.rotationX);

        if (-genes.valuableDegree > info.rotationX && info.rotationX < 0) 
        {
            rig.AddForceAtPosition(Vector3.forward * genes.forcePower, forcePoint.position);
            forcePointMat.SetColor("_EmissionColor", Color.magenta);

            //Debug.LogError("a");
        }
        if (genes.valuableDegree < info.rotationX && info.rotationX > 0)
        {
            rig.AddForceAtPosition(Vector3.back * genes.forcePower, forcePoint.position);
            forcePointMat.SetColor("_EmissionColor", Color.yellow);

            //Debug.LogError("b");
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        UpdateUnitInfo();

        GeneticInput(genes, unitInfo);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!unitInfo.alive)
            return;

        if (collision.gameObject.tag == "GROUND") 
        {
            //death
            unitInfo.alive = false;
            unitInfo.finalScore = time;

            //Debug.Log("Unit " + gameObject.name + "died" + "\nscore: " + unitInfo.finalScore);

            GameG.DecreaseAliveCount();
        }
    }

    public static List<Genom> GetChildsGenoms(Genom parent1, Genom parent2, float populationCount)
    {
        List<Genom> childs = new List<Genom>();

        //Debug.LogError("p1 " + parent1.forcePower + " " + parent1.valuableDegree);
        //Debug.LogError("p2 " + parent2.forcePower + " " + parent2.valuableDegree);

        //Debug.LogError("c:");

        childs.Add(parent1);
        childs.Add(parent2);

        for (int i = 0; i < populationCount - 2; i++)
        {
            Genom childGenom = GemerateRandomGenes();

            if (i % 2 == 0) 
            {
                if (Random.value > 0.5)
                {
                    childGenom.forcePower = parent1.forcePower;
                }
                else 
                {
                    childGenom.valuableDegree = parent1.valuableDegree;
                }
            }

            if (i % 4 == 0)
            {
                if (Random.value < 0.5)
                {
                    childGenom.forcePower = parent2.forcePower;
                }
                else
                {
                    childGenom.valuableDegree = parent2.valuableDegree;
                }
            }

            //Debug.LogError("c" + i + " " + childGenom.forcePower + " " + childGenom.valuableDegree);

            childs.Add(childGenom);
        }

        return childs;
    }
}

public class UnitInfo 
{
    public float finalScore;
    public bool alive = true;

    public float rotationX;
}

[System.Serializable]
public class Genom 
{
    public float forcePower;
    public float valuableDegree;

}
