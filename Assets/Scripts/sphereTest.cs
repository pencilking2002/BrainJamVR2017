using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sphereTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        var button = GetComponent < Neuromancers.Button>();
        button.SelectedActionSimple += OnSelected;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnSelected()
    {
        var colorVec = Neuromancers.Utility.Utility.GetRandomVector3(.5f);
        GetComponent<Renderer>().material.color = new Color(colorVec.x + .5f, colorVec.y + .5f, colorVec.z+ .5f);
    }
}
