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
          //  neuronManagerPrefab = Resources.Load("Prefabs/NeuronManager") as GameObject;

            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(instance);
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
            if (Input.GetKeyDown("f5"))
                SceneManager.LoadScene("Ending");
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
                 * StartCoroutine("FadeToEnd");
                 * 
                 * }
                 */
            }
            yield return null;
        }

        IEnumerator Ending()
        {
            Instantiate(neuronManagerPrefab);
          
            
            while (gameStatus == GameState.Ending)
            {
                //TODO  CHECK THAT player has picked up controllers
                /*
                 * if(ControllerLEFT == TRUE && ControllerRIGHT == TRUE){
                 * 
                 * }
                 */

                
            }
            yield return null;
        }

        IEnumerator FadeToEnd()
        {
           while (audio.volume >= 0.1f)
            {
                audio.volume -= 0.05f;
                yield return new WaitForSeconds(0.2f);
            }
            audio.Pause();
            StartCoroutine("Ending");
            yield return null;
        }
    }
}
