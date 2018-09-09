using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    private LineRenderer lineRenderer;
    public GameObject LaserHit;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
        AddColliderToLine(lineRenderer, transform.position, LaserHit.transform.position);
    }
	
	// Update is called once per frame
	void Update () {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);
        Debug.DrawLine(transform.position, LaserHit.transform.position);
        //LaserHit.transform.position = hit.point;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, LaserHit.transform.position);
        

    }

    public LineRenderer getRenderer()
    {
        return lineRenderer;
    }

    private void AddColliderToLine(LineRenderer line, Vector2 startPoint, Vector2 endPoint)
    {
        BoxCollider2D lineCollider = new GameObject("LineCollider").AddComponent<BoxCollider2D>();
        lineCollider.tag = "EnemyBulletTag";
        lineCollider.isTrigger = true;
        lineCollider.transform.parent = line.transform;     
        float lineWidth = line.endWidth;      
        float lineLength = Vector2.Distance(startPoint, endPoint);      
        lineCollider.size = new Vector2(lineLength, lineWidth);        
        Vector2 midPoint = (startPoint + endPoint) / 2;        
        lineCollider.transform.position = midPoint;      
        float angle = Mathf.Atan2((endPoint.y - startPoint.y), (endPoint.x - startPoint.x));       
        angle *= Mathf.Rad2Deg;
        angle *= -1;
        lineCollider.transform.Rotate(0, angle, 0);
    }
}
