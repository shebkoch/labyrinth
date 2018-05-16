using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SimpleStep : MonoBehaviour {
	[System.Serializable]
	public enum Dir
	{
		left,right,down,up
	};
	[System.Serializable]
	public class Tips
	{
		[SerializeField]
		public List<GameObject> tips = new List<GameObject>();
		public void Add(GameObject gameObject) {
			tips.Add(gameObject);
		}
		public GameObject this[int key]
		{
			get {
				return tips[key];
			}
			set {
				tips[key] = value;
			}
		}
		public int Count
		{
			get {
				return tips.Count;
			}
		}
	}
	[System.Serializable]
	public class TipsType {
		[SerializeField]
		public List<Tips> tipsType = new List<Tips>();
		public Tips this[int key]
		{
			get {
				return tipsType[key];
			}
			set {
				tipsType[key] = value;
			}
		}
		public int Count {
			get {
				return tipsType.Count;
			}
		}
		public void DeActivateAll() {
			foreach (var tipType in tipsType) {
				foreach (var tip in tipType.tips) {
					tip.SetActive(false);
				}
			}
		}
	}
	public List<GameObject> arrows;
	public GameObject map;
	public float speed;
	public List<GameObject> borders;
	public List<Vector3> bordersPos;
	public Text scoreText;
	public int score;
	public Dir trapFree;
	public Vector3 direction;
	public Dir curDir = Dir.down;
	public GameObject gg;
	public GameObject trap;
	public GameObject endPanel;
	public List<GameObject> tipsPos;
	public TipsType tips;
	public void Start () {
		speed /= 100;
		foreach (var border in borders) {
			bordersPos.Add(border.GetComponent<RectTransform>().position);
		}
		direction = Vector3.down;
		StopMove();
	}
	public void Left(){
		if (direction == Vector3.zero) {
			direction = Vector3.left;
			curDir = Dir.left;
		}
	}
	public void Right(){
		if(direction == Vector3.zero) {
			direction = Vector3.right;
			curDir = Dir.right;
		}
	}
	float xSpeedRatio = 1.2f;
	public void Down(){
		if(direction == Vector3.zero) {
			direction = Vector3.down * xSpeedRatio;
			curDir = Dir.down;
		}
	}
	public void Up() {
		if (direction == Vector3.zero) {
			direction = Vector3.up * xSpeedRatio;
			curDir = Dir.up;
		}
	}
	public void Restart() {
		Analytic.Instance.SendData();
		SceneManager.LoadScene(0);
	}
	public void EndGame() {
		gg.SetActive(false);
		trap.SetActive(true);
		endPanel.SetActive(true);
		direction = Vector3.zero;
	}
	public bool IsMove = false;
	Vector3 MapMove(Vector3 pos) {
		if (curDir == Dir.right && pos.x < bordersPos[0].x) {				
			IsMove = true;
			if (trapFree != Dir.right) EndGame();
			else score++;
			return bordersPos[1];
		}
		if (curDir == Dir.left && pos.x > bordersPos[1].x) {
			IsMove = true;
			if (trapFree != Dir.left) EndGame();
			else score++;
			return bordersPos[0];
		}
		if (curDir == Dir.down && pos.y > bordersPos[3].y) {
			IsMove = true;
			if (trapFree != Dir.down) EndGame();
			else score++;
			return bordersPos[2];
		}
		if (curDir == Dir.up && pos.y < bordersPos[2].y) {
			IsMove = true;
			if (trapFree != Dir.up) EndGame();
			else score++;
			return bordersPos[3];
		}
		return pos;
	}
	void StopMove() {
		Analytic.Instance.levelTotal++;
		foreach (var arrow in arrows) {
			arrow.SetActive(true);
		}
		int type = Random.Range(0, tips.Count);
		List<GameObject> buff = new List<GameObject>(tips[type].tips); 
		for (int i = 0; i < tipsPos.Count; i++) {
			int elem = Random.Range(0, buff.Count);
			buff[elem].SetActive(true);
			buff[elem].transform.position = tipsPos[i].transform.position;
			buff.RemoveAt(elem);
		}
		trapFree = (Dir)Random.Range(0,3);
		switch (curDir) {
			case Dir.left:
				arrows[1].SetActive(false);
				if (trapFree == Dir.left) trapFree++;
				break;
			case Dir.right:
				arrows[0].SetActive(false);
				if (trapFree == Dir.right) trapFree++;
				break;
			case Dir.down:
				arrows[3].SetActive(false);
				if (trapFree == Dir.down) trapFree++;
				break;
			case Dir.up:
				arrows[2].SetActive(false);
				if (trapFree == Dir.up) trapFree = 0;
				break;
			default:
				break;
		}
		direction = Vector3.zero;
		IsMove = false;	
	}
	void Update () {
		scoreText.text = "Score:" + score*100;
		if (direction != Vector3.zero) {
			var pos = map.GetComponent<RectTransform>().position;			
			pos += direction * speed * -1;
			pos = MapMove(pos);
			map.GetComponent<RectTransform>().position = pos;
			foreach (var arrow in arrows) {
				arrow.SetActive(false);
			}
			foreach (var item in tips.tipsType) {
				foreach (var ite1m in item.tips) {
					ite1m.transform.position = Vector3.zero;
				}
			} 
			tips.DeActivateAll();
			if (IsMove && Vector2.Distance(pos, Vector2.zero) <= speed) {
				StopMove();
			}
		}
	}
}
