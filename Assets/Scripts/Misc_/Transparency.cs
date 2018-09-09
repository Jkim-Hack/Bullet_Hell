using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transparency : MonoBehaviour
{

    // Use this for initialization
    private Image image;
    public float transparencyConstant;

    public void Start()
    {
        
        image = GetComponent<Image>();

        

        Color temp = image.color;

        temp.a = transparencyConstant;

        image.color = temp;

    


    }
}
