using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ITHB
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        /*
            MonoBehaviour singleton
        */
		private static T instance = null;
        public static T Instance {
            get {
                if (instance == null) {
                    instance = (T) FindObjectOfType(typeof(T));
//                    if (instance == null) {
//                        GameObject go = new GameObject();
//                        instance = go.AddComponent<T>();
//                        go.name = typeof(T).ToString();
//                    }
                }
 
                return instance;
            }
        }

		//Inherited from MonoBehaviour

		protected void SaveInstance(T objectReference)
		{
			instance = objectReference;
		}
    }
}
