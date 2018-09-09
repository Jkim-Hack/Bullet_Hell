using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingRocket : MonoBehaviour {

    private float currDuration = 0f;
    private float duration = 6f;
    public GameObject startPos;
    public GameObject controlPos;
    public GameObject endPos;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = CalcBezier(currDuration/duration, startPos.transform.position, controlPos.transform.position, endPos.transform.position);
        currDuration += Time.deltaTime;
	}
    private Vector2 CalcBezier(float t, Vector2 startPosition, Vector2 endPosition, Vector2 controlPoint)
    {
        float u = (1 - t);
        float uSqr = u * u;
        var point = (uSqr * startPosition) + (2 * (u) * t * controlPoint) + ((Mathf.Pow(t, 2)) * endPosition);
        return point;
    }
}
