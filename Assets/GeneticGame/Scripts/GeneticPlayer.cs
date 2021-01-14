using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GeneticGame 
{
    public abstract class GeneticPlayer : MonoBehaviour
    {
        public bool alive = true;
        public float score = 0;

        [SerializeField]
        protected Sensors sensors = new Sensors();
        [SerializeField]
        protected Genom genom = new Genom();


        public void SetGenom(Genom Genom)
        {
            this.genom = Genom;
        }

        public Genom GetGenom()
        {
            return this.genom;
        }


        private void Start()
        {
            //sensors = InitSensors();
            //Debug.Log("ApplyGenom");
            ApplyGenom(genom);
        }

        private void Update()
        {
            if (!alive)
                return;

            //UpdateSensors(sensors);
            score += ProcessStep(sensors, genom);

            alive = IsAlive();

            if (!alive)
                Die();
        }

        public abstract Genom InitGenom();

        //public abstract Sensors InitSensors();

        protected abstract void ApplyGenom(Genom genom);

        //protected abstract void UpdateSensors(Sensors sensors);

        //врозващает очки, заработанные за шаг
        protected abstract float ProcessStep(Sensors sensors, Genom genom);

        protected abstract bool IsAlive();

        protected void Die() 
        {
            Debug.Log(gameObject.name + "died\nscore: " + score);
            GeneticGame.DecreaseAliveCount();
        }

    }
}