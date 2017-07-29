using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// BASED OFF OF: https://gist.github.com/yamanyar/95a2f42b5c984aef9860#file-gistfile1-cs-L18
/// </summary>

namespace VRB.Utility.Threading {
	public class Threading {
		//readonly
		//Serialized

		/////Protected/////
		//References
		//Primitives
        
		//TO-DO: MAKE GENERIC THREAD ADAPTER WITHOUT IT BREAKING

		///////////////////////////////////////////////////////////////////////////
		//
		// Threading Functions
		//

		/// <summary>
		/// Executes the job on seperate thread.
		/// </summary>
		/// <param name="function">Function.</param>
		/// <param name="callback">Callback.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public static void ExecuteJobOnSeperateThread<TResult>(Func<TResult> function, Action<TResult> callback) {
			//Create Thread adapter
			ThreadAdapter adapter = (callback == null ? null : CreateThreadAdapter<TResult>());
			//Throw task into pool
			System.Threading.ThreadPool.QueueUserWorkItem(
				new System.Threading.WaitCallback(ExecuteJob<TResult>), (new object[] { adapter, function, callback })
			);
		}

		/// <summary>
		/// Executes the job on seperate thread.
		/// </summary>
		/// <param name="function">Function.</param>
		/// <param name="callback">Callback.</param>
		/// <param name="param">Parameter.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		public static void ExecuteJobOnSeperateThread<T,TResult>(Func<T, TResult> function, Action<TResult> callback, T param) {
			//Create Thread adapter
			ThreadAdapter adapter = (callback == null ? null : CreateThreadAdapter<TResult>());
			//Throw task into pool
			System.Threading.ThreadPool.QueueUserWorkItem(
				new System.Threading.WaitCallback(ExecuteJob<T,TResult>), (new object[] { adapter, function, callback, param })
			);
		}

		/// <summary>
		/// Executes the job.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		private static void ExecuteJob<TResult>(object obj) {
			object[] arr = obj as object[];

			ThreadAdapter adapter = arr[0] as ThreadAdapter;
			Func<TResult> function = arr[1] as Func<TResult>;
			Action<object> callback = arr[2] as Action<object>;

			TResult output = function();
			//callback isn't null if adapter isn't null
			if (adapter != null)
				adapter.Execute(callback, output);
		}

		/// <summary>
		/// Executes the job.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="TResult">The 2nd type parameter.</typeparam>
		private static void ExecuteJob<T,TResult>(object obj) {
			object[] arr = obj as object[];

			ThreadAdapter adapter = arr[0] as ThreadAdapter;
			Func<T,TResult> function = arr[1] as Func<T,TResult>;
			Action<object> callback = arr[2] as Action<object>;
			T param = (T) arr[3];

			TResult output = function(param);
			//callback isn't null if adapter isn't null
			if (adapter != null) {
				adapter.Execute(callback, output);
			}
		}

		/// <summary>
		/// Creates the thread adapter.
		/// </summary>
		/// <returns>The thread adapter.</returns>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		internal static ThreadAdapter CreateThreadAdapter<TResult>() {
			GameObject go = new GameObject();
			return go.AddComponent<ThreadAdapter>();
		}
	}

	internal class ThreadAdapter : MonoBehaviour {

		public float age = 0f;

		private volatile bool waiting = true;
		private volatile Action<object> action;
		private object result;

		protected void Awake() {
			DontDestroyOnLoad(this.gameObject);
			this.name = "ThreadAdapter-" + Time.time;
		}

		public IEnumerator Start() {
			while (waiting) {
				yield return new WaitForSeconds(0.1f);
				age += 0.1f;
			}
			this.action(result);				
			Destroy(this.gameObject);
		}

		public void Execute(Action<object> action, object result = null) {
			this.action = action;
			this.result = result;
			waiting = false;
		}
	}
}
