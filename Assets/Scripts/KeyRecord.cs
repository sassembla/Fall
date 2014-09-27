using UnityEngine;
using System.Collections.Generic;

public class KeyRecord {
	public int initialDirection;
	public Vector3 initialPosition;
	public List<int> turnTimings;

	/**
		敵として再生される予定のキーログ
		初期の回転向き、回転のタイミングをもとに、ここから再生できるようにしておく。
	*/
	public KeyRecord (int initialDirection, Vector3 initialPosition, List<int> turnTimings) {
		this.initialDirection = initialDirection;
		this.initialPosition = initialPosition;
		this.turnTimings = turnTimings;
	}
}