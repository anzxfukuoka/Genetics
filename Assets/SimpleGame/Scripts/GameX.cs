using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace old 
{
    public class GameX : MonoBehaviour
    {
        private float score = 0;
        private bool alive = true;

        private static GameX instance;

        public void Awake()
        {
            instance = this;
        }

        public static GameX GetInstance()
        {
            return instance;
        }

        void Start()
        {
            score = 0;
            alive = true;

            Time.timeScale = 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (alive)
            {
                score += Time.deltaTime;
            }
        }

        public void GameOver()
        {
            Time.timeScale = 0.4f;
            alive = false;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(200, 40, 200, 20), "Score: " + score);

            if (!alive)
            {
                if (GUI.Button(new Rect(200, 200, 80, 20), "Restart"))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload 
                }
            }
        }

    }

}