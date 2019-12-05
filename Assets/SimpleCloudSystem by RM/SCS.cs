using UnityEngine;

public class SCS : MonoBehaviour {

	public Transform Player ;
	public float CloudsSpeed;
    public static float bassValue;

	void Update () {
		if (!Player)
			return;

		gameObject.transform.position = Player.transform.position;

		transform.Rotate(0, Time.deltaTime*CloudsSpeed, 0); 
	}
}
