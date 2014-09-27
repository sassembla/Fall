using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using SublimeSocketAsset;

/**
	1ボタンでのゲーム

	・落下方向を変える
		左回転

	・画面外に出たら別の画面へのスクロールが発生する
		そっちの方向にスクロールしたいな！！

	・スクロールが完了したら、以前の自分の挙動をした鉄球が画面のどっかから落ちてくる
		それにぶつかると死亡

*/
public class GameController : MonoBehaviour {
	int state = 0;
	int gameFrame = 0;

	const int STATE_NONE = 1;
	const int STATE_FALL = 2;
	const int STATE_SCROLL = 3;
	const int STATE_DEAD = 4;

	public GameObject playerObj;
	private FallerController faller;

	public GameObject worldBoxObj;
	private GameObject worldBox;

	int scrollDirection;

	public const int DIRECTION_LEFT = 0;
	public const int DIRECTION_UP = 1;
	public const int DIRECTION_RIGHT = 2;
	public const int DIRECTION_DOWN = 3;


	int scrollState;

	const int SCROL_WAIT = 0;
	const int SCROLL_SCROLLING = 1;
	const int SCROLL_OVER = 2;

	int scrollCount;
	Dictionary<int, KeyRecord> scrollRecord;

	public GameObject killerPrefab;
	List<KillerController> killers;

	int point = 0;

	
	public AudioClip audioClip;
	AudioSource audioSource;

	public void Start () {
		scrollRecord = new Dictionary<int, KeyRecord>();

		state = STATE_NONE;
		ReadyFall();

		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.clip = audioClip;
	}

	/**
		キャラクターを画面の適当なところから
		落下開始させる
	*/
	public void ReadyFall () {
		Debug.Log("start fall!");

		/**
			キャラクターの初期化を行う
			初期落下パラメータは下方向
		*/
		InitCharacter(DIRECTION_DOWN);
		InitWorld();
		InitKillers();


		state = STATE_FALL;
	}


	public void Update () {
		
		if (Input.GetMouseButtonDown(0)) {
			Turn();
		}

		switch (state) {
			case STATE_NONE:
				break;

			case STATE_DEAD:
				
				break;
			case STATE_SCROLL:
				Scrolling();
				break;

			case STATE_FALL:
				Falling();
				break;

			
			default:
				break;
		}
	}

	/**
		キャラクター初期化
	*/
	void InitCharacter (int direction) {
		var charaObj = Instantiate(playerObj, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		faller = charaObj.GetComponent<FallerController>();
		faller.fallDirection = direction;
	}

	/**
		世界の初期化
	*/
	void InitWorld () {
		worldBox = Instantiate(worldBoxObj) as GameObject;
	}

	/**
		敵の初期化
	*/
	void InitKillers () {
		killers = new List<KillerController>();
	}

	/**
		落下中の動作
	*/
	void Falling () {
		gameFrame++;

		/**
			画面外に出たらスクロールに切り替える
		*/
		if (faller.IsOut()) {
			state = STATE_SCROLL;
			StartScroll(faller.fallDirection);
		}

		/**
			存在するすべてのオブジェクトに対して、ターンの再生をさせる。
		*/
		foreach (var killer in killers) {
			killer.TurnKiller(gameFrame);
		}

		if (faller.IsDead()) {
			state = STATE_DEAD;
		}
	}


	/**
		スクロール開始
	*/
	void StartScroll (int nextScollDirection) {
		scrollState = SCROL_WAIT;
		scrollDirection = nextScollDirection;


		audioSource.Play();
		PointUp(1000);
	}


	/**
		スクロール中の処理
		移動開始、移動完了、のステートを持つ
	*/
	void Scrolling () {
		switch (scrollState) {
			case SCROL_WAIT:
				scrollState = SCROLL_SCROLLING;


				
				break;

			case SCROLL_SCROLLING:
				var speed = 400;

				/**
					落下方向に応じて画面がスクロールする
				*/
				switch (scrollDirection) {
					case DIRECTION_LEFT:
						worldBox.rigidbody.velocity = new Vector3(-speed, 0);
						break;
					case DIRECTION_UP:
						worldBox.rigidbody.velocity = new Vector3(0, -speed);
						break;
					case DIRECTION_RIGHT:
						worldBox.rigidbody.velocity = new Vector3(speed, 0);
						break;
					case DIRECTION_DOWN:
						worldBox.rigidbody.velocity = new Vector3(0, speed);
						break;
				}

				if (IsScrolled()) {
					worldBox.rigidbody.velocity = Vector3.zero;
					scrollState = SCROLL_OVER;
				}

				break;
			case SCROLL_OVER:
				scrollState = SCROL_WAIT;

				/**
					画面端からスタート
				*/
				Restart();
				break;
		}
	}

	/**
		スクロール完了を取って返す
	*/
	private bool IsScrolled () {
		int limit = 150;

		var x = Math.Abs(worldBox.transform.position.x);
		var y = Math.Abs(worldBox.transform.position.y);
		
		if (limit < x) {
			return true;
		}

		if (limit < y) {
			return true;
		}

		return false;
	}

	/**
		一定時間に一回だけ
		かつ、
		動けるstateの時だけ

		ターンできる
	*/
	public void Turn () {
		switch (state) {
			case STATE_FALL:
				if (faller != null) {
					faller.Rotate(gameFrame);
					PointUp(50);
				}
				break;
			default:
				break;
		}
	}

	/**
		ポイント表示。
	*/
	void PointUp (int upValue) {
		point += upValue;
	}


	/**
		再開する
	*/
	void Restart () {
		/**
			原点に戻す
		*/
		worldBox.transform.localPosition = new Vector3(50, 50, 0);

		
		gameFrame = 0;
		
		/**
			新しい敵をInstantiate
		*/
		var newEnemy = Instantiate(killerPrefab) as GameObject;
		var killerCont = newEnemy.GetComponent<KillerController>();

		// new KillerController(new MoveRecord(faller.fallDirection, faller.startPosition, faller.GetTurnRecords());
		killerCont.enemyIndex = scrollCount;

		/**
			レコードの追加
		*/
		scrollRecord[scrollCount] = new KeyRecord(faller.fallDirection, faller.startPosition, faller.GetTurnRecords());


		scrollCount++;


		killers.Add(killerCont);


		/**
			今までの敵のリセットを行う
		*/
		foreach (var currentKillerCont in killers) {
			var index = currentKillerCont.enemyIndex;
			currentKillerCont.ResetKiller(scrollRecord[index]);
		}

		faller.RestartAtEdge();
		state = STATE_FALL;
	}

	void OnGUI () {
		/**
			画面枚数の表示
		*/

		/**
			レベルアップ
		*/

	}
}