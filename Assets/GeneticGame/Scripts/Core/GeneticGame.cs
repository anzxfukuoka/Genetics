using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticGame 
{
    public class GeneticGame : MonoBehaviour
    {
        private static GeneticGame instance;

        [Space(10)]

        public int populationCount = 4;
        public int maxGenerations = 100;

        [Space(10)]

        public GeneticPlayer geneticPlayerPrefab;

        [Space(10)]

        public Transform startSpawnPoint;

        public float spawnDistance = 6f; // растояния между особями при спавне

        [Space(10)]

        private int aliveCount;

        private List<GeneticPlayer> population;
        private int generation = 0;

        public Genom bestGenom;
        public float bestScore;

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else 
            {
                throw new System.Exception("More than one GeneticGame on Scene.");
            }
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

        public Vector3 GetPos(int i)
        {
            Vector3 pos;

            if (startSpawnPoint == null)
            {
                pos = Vector3.right * i * spawnDistance + Vector3.up * 2;
            }
            else
            {
                pos = startSpawnPoint.position + Vector3.right * i * spawnDistance;
            }

            return pos;
        }

        public void GenerateInitalPopulation()
        {
            population = new List<GeneticPlayer>();
            aliveCount = populationCount;

            for (int i = 0; i < populationCount; i++)
            {
                Vector3 pos = GetPos(i);

                GeneticPlayer geneticPlayer = Instantiate(geneticPlayerPrefab, pos, new Quaternion());

                //Debug.Log("Instantiate geneticPlayerPrefab");

                Genom genom = geneticPlayer.InitGenom();
                genom.RandomizeAll(); //случайные гены
                
                geneticPlayer.SetGenom(genom);

                //Debug.Log("geneticPlayer.SetGenom(genom)");

                population.Add(geneticPlayer);
            }
        }

        //генерирует популяцию того же размера, что и предедущая
        public void GenerateNextPopulation(List<Genom> parents)
        {
            aliveCount = populationCount;

            for (int i = 0; i < populationCount; i++)
            {
                Destroy(population[i].gameObject);
            }

            List<Genom> childsGenes = Genom.GetChilds(parents, populationCount, true);

            for (int i = 0; i < populationCount; i++)
            {
                Vector3 pos = GetPos(i);

                GeneticPlayer geneticPlayer = Instantiate(geneticPlayerPrefab, pos, new Quaternion());

                geneticPlayer.SetGenom(childsGenes[i]);

                geneticPlayer.name += " #" + i;

                population[i] = geneticPlayer;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (aliveCount <= 0 && generation < maxGenerations)
            {
                //сортровка по score
                population.Sort((x, y) => (y.score.CompareTo(x.score)));

                GeneticPlayer best = population[0];

                Debug.Log("generation " + generation + " best \np1: " + best.score);

                if (bestScore < best.score)
                {
                    bestScore = best.score;
                    bestGenom = best.GetGenom();
                }

                Debug.Log("global best score: " + bestScore);

                /*
                 * передавая отсортированный список генов всех особей можно всегда надеятся, 
                 * что гены лучшей особи передадутся самому большому кол-ву детей. 
                 * но если разнится между 1 и 2 коосальная, есть ли смысл оставлять гены второго в популяции?
                 * (возможно, стоит переделать сам алгорим скрещевания)
                 * ((но делать я этого, конечно же, не буду))
                 */

                List<Genom> populationGenom = new List<Genom>();

                foreach (GeneticPlayer p in population) 
                {
                    // коосальная разница = 50%
                    if (p.score > best.score * 0.5f)
                        populationGenom.Add(p.GetGenom());
                }

                /*
                 * в список генов родителей 100% попадет хотя бы 1 ген - лучший.
                 * если попадет только он - поколение сгенерируется на основе полных случайных мутаций лучшего
                 */

                GenerateNextPopulation(populationGenom);

                generation++;
            }
        }

        void OnGUI()
        {
            GUI.Box(new Rect(20, 20, 200, 60), "");

            GUI.Label(new Rect(40, 30, 200, 20), "generation: " + generation);
            GUI.Label(new Rect(40, 50, 200, 20), "best score: " + bestScore);

        }
    }
}
