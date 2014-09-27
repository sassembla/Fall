using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FallerController : MonoBehaviour {

	public int fallDirection;
	private int localDirection;
	float acceleration = 0;

	int fallerState;
	public const int CHARACTER_FALLING = 0;
	public const int CHARACTER_STOP = 1;
	public const int CHARACTER_DEAD = 2;
	public const int CHARACTER_OVER = 3;

	public Vector3 startPosition;
	private List<int> timingRecords;


	int rotationPower = 0;
	int scalePower = 0;

	public AudioClip SE_up;
	public AudioClip SE_dead;
	AudioSource audioSource;

	public List<int> GetTurnRecords () {
		return timingRecords;
	}

	void Start () {
		StartFall();
		audioSource = gameObject.GetComponent<AudioSource>();
	}



	// Update is called once per frame
	void Update () {
		
		switch (fallerState) {
			case CHARACTER_FALLING:
				Fall();
				break;

			case CHARACTER_STOP:

				break;

			case CHARACTER_DEAD:
				rotationPower++;
				scalePower++;
				transform.Rotate(new Vector3(0, 0, rotationPower));
				transform.localScale = new Vector3(transform.localScale.x + scalePower, transform.localScale.y + scalePower, transform.lossyScale.z);

				if (100 < rotationPower) {
					fallerState = CHARACTER_OVER;
				}
				break;

		}
	}

	float turnPower = 3f;
	float upPower = 8f;

	public void Rotate (int frame) {
		timingRecords.Add(frame);
		acceleration += turnPower;

		/**
			次の方向のセットを行い、画像の回転も行う。
		*/
		switch (fallDirection) {
			case GameController.DIRECTION_DOWN:
				fallDirection = GameController.DIRECTION_LEFT;
				break;

			case GameController.DIRECTION_LEFT:
				fallDirection = GameController.DIRECTION_UP;
				break;

			case GameController.DIRECTION_UP:
				fallDirection = GameController.DIRECTION_RIGHT;
				break;

			case GameController.DIRECTION_RIGHT:
				fallDirection = GameController.DIRECTION_DOWN;
				break;
		}

		transform.Rotate(new Vector3(0, 0, 90));
		audioSource.clip = SE_up;
		audioSource.Play();
	}
	
	int limit = 50;

	/**
		場外に出た = ゲーム上の範囲より、どこかの値(絶対値)が大きくなる
	*/
	public bool IsOut () {
		var x = Math.Abs(transform.position.x);
		var y = Math.Abs(transform.position.y);
		
		if (limit < x) {

			if (x < 0) {
				Debug.Log("L:右");
			} else {
				Debug.Log("L:左");
			}

			// 落下中止
			StopFall();
			acceleration += upPower;
			return true;
		}

		if (limit < y) {
			if (y < 0) {
				// 上

			} else {
				// 下

			}

			// 落下中止
			StopFall();
			acceleration += upPower;
			return true;
		}

		return false;
	}

	public bool IsDead () {
		if (fallerState == CHARACTER_DEAD) return true;
		return false;
	}

	private void Fall () {
		/**
			特定の方向に吹っ飛んでいく(面と時間でだんだんと加速)
		*/
		var currentAccel = 10f + acceleration;

		switch (fallDirection) {
			case GameController.DIRECTION_LEFT:
				rigidbody.velocity = new Vector3(1 * currentAccel, 0, 0);				
				break;
			case GameController.DIRECTION_UP:
				rigidbody.velocity = new Vector3(0, 1 * currentAccel, 0);

				break;
			case GameController.DIRECTION_RIGHT:
				rigidbody.velocity = new Vector3(-1 * currentAccel, 0, 0);
				break;
			case GameController.DIRECTION_DOWN:
				rigidbody.velocity = new Vector3(0, -1 * currentAccel, 0);
				break;
		}
	}

	/**
		向きとかは変わってないと思うので、
		画面端の位置を調整して戻る。
		開始位置を保存する。
	*/
	public void RestartAtEdge () {

		switch (fallDirection) {
			case GameController.DIRECTION_UP:
				transform.localPosition = new Vector3(transform.position.x, -50, 0);
				break;
			case GameController.DIRECTION_RIGHT:
				transform.localPosition = new Vector3(50, transform.position.y, 0);
				break;
			case GameController.DIRECTION_DOWN:
				transform.localPosition = new Vector3(transform.position.x, 50, 0);
				break;
			case GameController.DIRECTION_LEFT:
				transform.localPosition = new Vector3(-50, transform.position.y, 0);
				break;
		}

		StartFall();
	}


	/**
		停止する
	*/
	private void StopFall () {
		fallerState = CHARACTER_STOP;
		rigidbody.velocity = new Vector3(0, 0);
	}

	/**
		落下を開始する
	*/
	public void StartFall () {
		startPosition = new Vector3(transform.position.x, transform.position.y, 0);
		fallerState = CHARACTER_FALLING;
		
		// キーレコードを空にする
		timingRecords = new List<int>();
	}

	/**
		Dead by hit
	*/
	void OnCollisionEnter (Collision c) {
		StopFall();
		audioSource.clip = SE_dead;
		audioSource.Play();
		fallerState = CHARACTER_DEAD;

		rigidbody.Sleep();
	}

}
