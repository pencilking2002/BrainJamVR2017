using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Invoke("loadNext", 20f);
	}
	
	void loadNext()
    {

        SceneManager.LoadScene(0);
       

    }
}
