using UnityEngine;
using System.Collections;

public class DuplicationUtility : MonoBehaviour {

	//structs
	[System.Serializable]
	public struct DuplicationElement {

		public int index;
		public GameObject prefab;
	}

	//Serialized
    public Vector3 oddLocalScale = new Vector3(-1f, 1f, -1f);
    public Vector3 offset = Vector3.zero;
	public int count = 1;
	public DuplicationElement[] overrideDuplicationElements;




	void Start () {
	
        for(int i = 0; i < count; ++i)
        {
			GameObject overrideDuplicationElement = GetOverrideDuplicationElement(i);
			GameObject prefab = (overrideDuplicationElement!=null) ? overrideDuplicationElement : this.gameObject;
			prefab.SetActive(true);
			GameObject clone = GameObject.Instantiate(prefab) as GameObject;
			
			clone.name = gameObject.name+"_Clone_"+i;
            clone.transform.SetParent(this.transform.parent);
            clone.transform.localPosition = this.transform.localPosition + offset * (i + 1);
            clone.transform.localScale = (i % 2 == 1) ? Vector3.one : oddLocalScale;
            clone.transform.localRotation = Quaternion.identity;

			DuplicationUtility clonedDuplicationUtility = clone.GetComponent<DuplicationUtility>();
			if(clonedDuplicationUtility!=null)
				Destroy(clonedDuplicationUtility);
        }

		HidePrefabs();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void HidePrefabs () {

		for (int i = 0; i < overrideDuplicationElements.Length; ++i) {

			overrideDuplicationElements[i].prefab.SetActive(false);
		}

//		this.gameObject.SetActive(false);
	}

	protected GameObject GetOverrideDuplicationElement(int index) {

		for (int i = 0; i < overrideDuplicationElements.Length; ++i) {

			if(overrideDuplicationElements[i].index == index)
				return overrideDuplicationElements[i].prefab;
		}
		return null;
	}
}
