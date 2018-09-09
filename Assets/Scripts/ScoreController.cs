using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public Text numCoinText;

    private int totalCoins;

	void Start ()
    {
        GUIDelegate.OnCoinPickUp += this.updateCoinGUI;
	}
	
	private void updateCoinGUI(int numCoins)
    {
        totalCoins += numCoins;

        numCoinText.text = totalCoins.ToString();
    }
	
}
