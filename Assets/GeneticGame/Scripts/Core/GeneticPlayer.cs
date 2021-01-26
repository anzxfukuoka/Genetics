using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticGame 
{
    public enum Mode 
    {
        Train,
        LoadAndTest,
        LoadAndTrain,
    }

    public abstract class GeneticPlayer : MonoBehaviour
    {
        public Mode mode;

        public bool alive = true;
        public float score = 0;

        //[SerializeField]
        //protected Sensors sensors = new Sensors();
        [SerializeField]
        private Genom genom = null;
        private bool genomInistalized = false;

        public string GetSavePath() 
        {
            return Application.dataPath + "/GenomData/" + gameObject.name + ".json";
        }

        public void SetGenom(Genom Genom)
        {
            this.genom = Genom;
            genomInistalized = true;
        }

        public Genom GetGenom()
        {
            return this.genom;
        }


        private void Start()
        {
            //sensors = InitSensors();
            //Debug.Log("ApplyGenom");

            if (mode == Mode.Train || mode == Mode.LoadAndTrain)
            {
                if (!genomInistalized)
                {
                    Debug.LogWarning("Genom is not inistalized.\n Maybe you forgot add GeneticGame.cs on scene?");
                    Genom newgenom = InitGenom();
                    newgenom.RandomizeAll();
                    SetGenom(newgenom);
                }
            }
            else if(mode == Mode.LoadAndTest)
            {
                SetGenom(Genom.LoadFromFile(GetSavePath()));
            }

            //Debug.Log(genom);

            ApplyGenom(genom);
        }

        private void Update()
        {
            if (!alive)
                return;

            //UpdateSensors(sensors);
            score += ProcessStep(genom);

            alive = IsAlive();

            if (!alive)
                Die();
        }

        public abstract Genom InitGenom();

        //public abstract Sensors InitSensors();

        protected abstract void ApplyGenom(Genom genom);

        //protected abstract void UpdateSensors(Sensors sensors);

        //врозващает очки, заработанные за шаг
        protected abstract float ProcessStep(Genom genom);

        protected abstract bool IsAlive();

        protected void Die() 
        {
            Debug.Log(gameObject.name + " died\nscore: " + score);
            GeneticGame.DecreaseAliveCount();
        }

    }
}