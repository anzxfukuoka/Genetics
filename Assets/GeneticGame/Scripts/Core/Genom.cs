using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace GeneticGame
{
    [System.Serializable]
    public class Genom
    {
        [SerializeField]
        private List<Gen> gens_list = new List<Gen>(); // нужно для отображения генов в инспекторе и сериализации
        [System.NonSerialized]
        private Dictionary<string, Gen> gens_dict = new Dictionary<string, Gen>(); //для поиска гена по имени

        public Genom() { }

        //public Genom(Dictionary<string, Gen> gens_dict)
        //{
        //    this.gens_dict = new Dictionary<string, Gen>(gens_dict);
        //}

        public Genom(Genom parent)
        {
            for (int j = 0; j < parent.Size(); j++)
            {
                Gen pGen = parent.GetGen(j);
                AddGen(new Gen(pGen)); //copy
            }

            //this.gens_dict = new Dictionary<string, Gen>(copy.gens_dict);
        }

        public int Size() 
        {
            return gens_dict.Count;
        }

        public void AddGen(Gen gen)
        {
            gens_list.Add(gen);
            gens_dict.Add(gen.name, gen);
        }

        public string GetGenNameFromIndex(int index)
        {
            return gens_dict.ElementAt(index).Key;
        }

        public Gen GetGen(string name)
        {
            return gens_dict[name];
        }

        public Gen GetGen(int index)
        {
            string name = GetGenNameFromIndex(index);
            return gens_dict[name];
        }

        public static Genom LoadFromFile(string path) 
        {
            string json = "";

            if (File.Exists(path))
            {
                json = File.ReadAllText(path);
            }
            else 
            {
                Debug.LogError("path: " + path + "\nfile not exists.");
            }

            Genom loaded = JsonUtility.FromJson<Genom>(json);

            foreach(Gen gen in loaded.gens_list) 
            {
                loaded.gens_dict.Add(gen.name, gen);
            }

            return loaded;
        }

        public static void SaveToFile(Genom genom, string path) 
        {
            string json = JsonUtility.ToJson(genom);

            string directoryName = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryName)) 
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(path, json);
        }

        public void UpdateGenValue(string name, float value)
        {
            gens_dict[name].value = value;

            Gen g = gens_dict[name];

            g.value = value;

            gens_dict[name] = g;
        }

        public void UpdateGenValue(int index, float value)
        {
            string name = GetGenNameFromIndex(index);

            UpdateGenValue(name, value);
        }

        public void RandomizeAll()
        {
            foreach (var item in gens_dict)
            {
                Gen g = item.Value;
                g.Randomize();
            }
        }

        //
        public static Genom MutateAll(Genom parent, float f)
        {
            Genom newGenes = new Genom(parent);

            foreach (var item in newGenes.gens_dict)
            {
                Gen g = item.Value;
                g.Mutate(f);
            }

            return new Genom(newGenes);
        }

        public static List<Genom> MutateAllPopulation(List<Genom> population, float f)
        {
            List<Genom> mutants = new List<Genom>(population);

            for (int i = 0; i < population.Count; i++) 
            {
                mutants[i] = MutateAll(mutants[i], f);
            }

            return mutants;
        }

        //возвращает случайную популящию;
        public static List<Genom> GetChilds(Genom parent, int childsCount)
        {
            List<Genom> childs = new List<Genom>();

            for (int i = 0; i < childsCount; i++)
            {
                Genom g = new Genom(parent); //copy

                g.RandomizeAll();

                childs.Add(g);
            }

            return childs;
        }

        //возвращает mutation-% мутации от parent;
        public static List<Genom> GetChilds(Genom parent, int childsCount, float mutation) 
        {
            List<Genom> childs = new List<Genom>();

            for (int i = 0; i < childsCount; i++)
            {
                Genom c = MutateAll(parent, mutation);
                childs.Add(c);
            }

            return childs;
        }

        public static List<Genom> GetChilds(List<Genom> parents, int childsCount, bool save_best = false /* только для parents.Count > 1 */)
        {
            List<Genom> childs = new List<Genom>();

            if (childsCount < 1)
                throw new System.Exception("childsCount < 1");

            if (parents.Count < 0)
                throw new System.Exception("parents.Count < 1");


            if (parents.Count < 1)
            {
                //возвращает случайную популящию;
                childs = GetChilds(parents[0], childsCount);
            }
            else if (parents.Count == 1)
            {
                //возвращает стандартные (4%) мутации от parents[0];
                childs = GetChilds(parents[0], childsCount, 0.04f);
            }
            else if (parents.Count > 1) 
            {
                /*
                 * i-тый родитель передаст 1/(2^(i + 1)) своих генов 1/(2^(i + 1)) детей, i э 1, 2 ... n.
                 * т.е. 1-й - половину генов половине детей, 2 - 1/4 генов 1/4 детей...
                 * (замечание: расчитывается, что массив родителей parents отcортирован по score по убыванию, 1-й родитель - самый успешный)
                 * чтобы на каждом поколении передавались разные гены, они передаются по случайной перестановке. 
                 * (замечание2: если save_best = true - первый ребенок будет точной копией лучшего, следующее - по схеме выше)
                 */

                childs = GetChilds(parents[0], childsCount);

                int parentsGenomSize = parents[0].Size();

                int[] permutation = Enumerable.Range(0, parentsGenomSize).ToArray(); //перестановка
                permutation = permutation.OrderBy(x => Random.value).ToArray();

                for (int i = 0; i < parents.Count; i++)
                {
                    //Debug.Log("P" + i);

                    for (int j = i; j < childsCount; j++) 
                    {
                        //Debug.Log("C" + j);

                        for (int k = 0; k < parentsGenomSize; k++)
                        {
                            //Debug.Log("K" + k);

                            int d = 0;

                            if (save_best)
                                d = 1;

                            if (k % Mathf.Pow(2, (j + d)) != 0)
                                continue;

                            int perIndex = (int)Mathf.Repeat(k + i + j, parentsGenomSize);
                            int genIndex = permutation[perIndex];

                            //Debug.Log("gen name: " + parents[i].GetGenNameFromIndex(genIndex));

                            float pValue = parents[i].GetGen(genIndex).value;

                            //float cValue = childs[j].GetGen(genIndex).value;
                            //Debug.Log("pVal: " + pValue + "  cVal: " + cValue);

                            childs[j].UpdateGenValue(genIndex, pValue);

                            //cValue = childs[j].GetGen(genIndex).value;
                            //Debug.Log("pVal: " + pValue + "  cVal: " + cValue);
                        }
                    }
                }
            }


            return childs;
        }

    }

    [System.Serializable]
    public class Gen
    {
        public string name;

        [HideInInspector]
        public float maxVal;
        [HideInInspector]
        public float minVal;

        public float value;

        public Gen(string name, float maxVal = 1, float minVal = 0)
        {
            this.name = name;

            this.maxVal = maxVal;
            this.minVal = minVal;

            Validate();
        }

        public Gen(Gen gen) 
        {
            this.name = gen.name;
            this.maxVal = gen.maxVal;
            this.minVal = gen.minVal;
            this.value = gen.value;
        }

        private void Validate() 
        {
            if (value > maxVal)
                value = maxVal;
            if (value <= minVal)
                value = minVal;
        }

        public void Randomize()
        {
            value = Random.Range(maxVal, minVal);
        }

        public void Mutate(float f) //f э [0, 1], процент мутации
        {
            value += f * (maxVal - minVal) * Mathf.Sign(Random.Range(-1.0f, 1.0f));
            
            Validate();
        }


    }
}
  