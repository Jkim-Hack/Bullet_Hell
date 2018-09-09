using System.Timers;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeText : MonoBehaviour
{

    private Text textRef;
    public float alpha;

	// Use this for initialization
	void Start ()
	{
	    textRef = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    alpha = textRef.color.a;
	    if (alpha > 0f)
	    {
	        alpha -= 0.01f;
	        var color = new Color(1f, 1f, 1f, alpha);
            textRef.color = color;
	    }

        
	}
}
