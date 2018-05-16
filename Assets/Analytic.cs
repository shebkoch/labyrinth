using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
public class Analytic : MonoBehaviour {
	public int levelTotal;
	public int death;
	public static Analytic Instance { get; private set; }
	private void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		} else if (Instance != null) {
			Destroy(gameObject);
		}
	}
	public void SendData() {
		death++;
		Dictionary<string, object> data = new Dictionary<string, object>();
		data.Add("time", System.DateTime.Now);
		data.Add("total", levelTotal);
		data.Add("death", death);
		AnalyticsEvent.Custom("score",data);
	}
	
}
