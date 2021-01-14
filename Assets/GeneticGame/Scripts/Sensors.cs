using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticGame
{
    [System.Serializable]
    public class Sensors
    {
        [SerializeField]
        private List<Sensor> sensors_list = new List<Sensor>(); // нужно для отображения в инспекторе
        [SerializeField]
        private Dictionary<string, float> sensors_dict = new Dictionary<string, float>();

        public void Add(string name, ref float reference)
        {
            sensors_dict.Add(name, reference);
            sensors_list.Add(new Sensor(name, ref reference));
        }

        public float Get(string name)
        {
            return sensors_dict[name];
        }
    }

    [System.Serializable]
    public class Sensor
    {
        public string name;
        public float value;

        public Sensor(string name, ref float value) 
        {
            this.name = name;
            this.value = value;
        }
    }
}