using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace old 
{
    public class GameG : MonoBehaviour
    {
        public int populationCount = 4;
        public int maxGenerations = 100;

        public Unit unitPrefab;

        private float distance = 6f; // растояния между особями при спавне

        private static GameG instance;

        private int aliveCount;

        private List<Unit> population;
        private int generation = 0;

        public Genom bestGenom;
        public float bestScore;

        public void Awake()
        {
            instance = this;
        }

        public static void DecreaseAliveCount()
        {
            if (instance.aliveCount > 0)
                instance.aliveCount--;

            //Debug.Log("alive count: " + instance.aliveCount);
        }

        // Start is called before the first frame update
        void Start()
        {
            GenerateInitalPopulation();
        }

        public void GenerateInitalPopulation()
        {
            population = new List<Unit>();
            aliveCount = populationCount;

            for (int i = 0; i < populationCount; i++)
            {
                Unit unit = Instantiate(unitPrefab, Vector3.right * i * distance + Vector3.up * 2, new Quaternion());
                unit.InitGenes();
                population.Add(unit);
            }
        }

        public void GenerateNextPopulation(List<Genom> genes)
        {
            aliveCount = populationCount;

            for (int i = 0; i < populationCount; i++)
            {
                Destroy(population[i].gameObject);
            }

            for (int i = 0; i < populationCount; i++)
            {
                Unit unit = Instantiate(unitPrefab, Vector3.right * i * distance + Vector3.up * 2, new Quaternion());
                unit.InitGenes(genes[i]);

                population[i] = unit;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (aliveCount <= 0 && generation < maxGenerations)
            {
                population.Sort((x, y) => (y.unitInfo.finalScore.CompareTo(x.unitInfo.finalScore)));

                Unit parent1 = population[0];
                Unit parent2 = population[1];

                Debug.Log("generation " + generation + " best \np1: " + parent1.unitInfo.finalScore + "\np2: " + parent2.unitInfo.finalScore);

                if (bestScore < parent1.unitInfo.finalScore)
                {
                    bestScore = parent1.unitInfo.finalScore;
                    bestGenom = parent1.genes;
                }

                Debug.Log("generation " + generation + " best \np1: " + parent1.unitInfo.finalScore + "\np2: " + parent2.unitInfo.finalScore + "\nglobal best score: " + bestScore);

                List<Genom> childGenoms = Unit.GetChildsGenoms(parent1.genes, parent2.genes, populationCount);

                GenerateNextPopulation(childGenoms);

                generation++;
            }
        }

        void OnGUI()
        {
            GUI.Box(new Rect(20, 20, 200, 60), "");

            GUI.Label(new Rect(40, 30, 200, 20), "generation: " + generation);
            GUI.Label(new Rect(40, 50, 200, 20), "best time: " + bestScore);

        }

        public static GameG GetInstance()
        {
            return instance;
        }
    }
}
