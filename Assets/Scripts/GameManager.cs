using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neuromancers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        public int neuronHitCounter = 0;
        public GameObject introLight;
        public GameObject nueronsTutorial;
        public AudioClip mainTheme;


        enum GameState { Intro, Tutorial, Main, Ending };
        private GameState gameStatus;

        AudioSource audio;


        /////Protected/////
        protected GameObject neuronManagerPrefab;



        protected void Awake()
        {
            neuronManagerPrefab = Resources.Load("Prefabs/NeuronManager") as GameObject;

            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }

        protected void Start()
        {
            audio = GetComponent<AudioSource>();
            InitGame();
        }

        protected void Update()
        {
            DemoEventControls();
        }

        private void InitGame()
        {
            gameStatus = GameState.Intro;
        }

        /// <summary>
        /// EVENT DEMO CONTROLS TO CONTROL DEMOS AT EVENTS F2 to reset game
        /// </summary>
        private void DemoEventControls()
        {
            if (Input.GetKeyDown("f2"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        IEnumerator Intro()
        {
            while(gameStatus == GameState.Intro)
            {
                //TODO  CHECK THAT player has picked up controllers
                /*
                 * if(ControllerLEFT == TRUE && ControllerRIGHT == TRUE){
                 * 
                 * gameStatus = GameState.Tutorial;
                 * StartCoroutine("Tutorial");
                 * 
                 * Destroy(IntroLight);
                 * }
                 */


            }
            yield return null;
        }

        IEnumerator Tutorial()
        {
            audio.pitch = 1f;
            audio.volume = 1f;
            nueronsTutorial.SetActive(true);
            while (gameStatus == GameState.Tutorial)
            {

                if (neuronHitCounter >= 5)
                {
                    gameStatus = GameState.Main;
                    StartCoroutine("Main");
                }
            }
            yield return null;
        }

        IEnumerator Main()
        {
            nueronsTutorial.SetActive(false);
            Instantiate(neuronManagerPrefab);
            audio.clip = mainTheme;
            audio.Play();
            while (gameStatus == GameState.Main)
            {
                //TODO  WIN STATE
                /*
                 * if(){
                 * 
                 * gameStatus = GameState.Ending;
                 * StartCoroutine("Ending");
                 * 
                 * }
                 */
            }
            yield return null;
        }

        IEnumerator Ending()
        {
            Instantiate(neuronManagerPrefab);
            audio.clip = mainTheme;
            audio.Play();
            while (gameStatus == GameState.Main)
            {
                //TODO  CHECK THAT player has picked up controllers
                /*
                 * if(ControllerLEFT == TRUE && ControllerRIGHT == TRUE){
                 * 
                 * }
                 */

                StartCoroutine("Tutorial");
            }
            yield return null;
        }
    }
}
