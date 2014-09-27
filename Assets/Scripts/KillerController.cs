using UnityEngine;
using System.Collections.Generic;

public class KillerController : MonoBehaviour {

	public int enemyIndex;
	List<int> turnTimings;

	public int fallDirection;
	private int localDirection;
	float acceleration = 0;

	int killerState;

	/**
		初期向きとキー、ポジションを受け取る。
	*/
	public void ResetKiller (KeyRecord record) {
		this.turnTimings = record.turnTimings;

		fallDirection = record.initialDirection;
		transform.localPosition = record.initialPosition;

		killerState = FallerController.CHARACTER_FALLING;
	}


	public void TurnKiller (int frame) {
		if (turnTimings.Contains(frame)) {
			Rotate();
		}
	}

	void Update () {
		switch (killerState) {
			case FallerController.CHARACTER_FALLING:
				Fall();
				break;

			case FallerController.CHARACTER_STOP:

				break;

			case FallerController.CHARACTER_DEAD:

				break;

		}
	}


	public void Rotate () {
		
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

	void StopFall () {
		rigidbody.velocity = Vector3.zero;
		transform.localPosition = new Vector3(0, 0, 100);
	}

	/**
		Dead by hit
	*/
	void OnCollisionEnter (Collision c) {
		StopFall();

		killerState = FallerController.CHARACTER_DEAD;
	}

}
