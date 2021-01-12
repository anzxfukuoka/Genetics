using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPlayer : MonoBehaviour
{
    protected bool alive = true;

    protected float score = 0;

    public Dictionary<string, float> sensors = new Dictionary<string, float>();
    public Gens gens = new Gens();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    protected void Die()
    {
        alive = false;
    }

    protected virtual void UpdateSensors() 
    {

    }

    protected virtual void ProcessData() 
    {

    }
    
}

[System.Serializable]
public class Gens 
{
    public Dictionary<string, Gen> genom = new Dictionary<string, Gen>();

    public Gens() { }

    public Gens(Dictionary<string, Gen> genom) 
    {
        this.genom = genom;
    }

    public void AddGen(string name, Gen gen) 
    {
        genom.Add(name, gen);
    }

    public Gen GetGen(string name) 
    {
        return genom["name"];
    }

    public void RandomizeAll() 
    {
        foreach (var item in genom)
        {
            Gen g = item.Value;
            g.Randomize();
        }
    }

    //

    public static Gens MutateAll(Gens parent,  float f) 
    {
        Dictionary<string, Gen> newGenom = new Dictionary<string, Gen>(parent.genom);

        foreach (var item in newGenom)
        {
            Gen g = item.Value;
            g.Mutate(f);
        }

        return new Gens(newGenom);
    }

    public static 
}

[System.Serializable]
public class Gen 
{
    private float maxVal;
    private float minVal;

    public float value;

    public Gen(float maxVal = 1, float minVal = 0) 
    {
        this.maxVal = maxVal;
        this.minVal = minVal;
    }

    public void Randomize() 
    {
        value = Random.Range(maxVal, minVal);
    }

    public void Mutate(float f) 
    {
        value += f;
    }
}
