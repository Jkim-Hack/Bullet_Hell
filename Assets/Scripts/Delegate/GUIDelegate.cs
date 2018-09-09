using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDelegate : MonoBehaviour
{
    public delegate void HealthEventHandler(Health health);
    public static event HealthEventHandler OnHit;


    public delegate void CoinEventHandler(int numCoins);
    public static event CoinEventHandler OnCoinPickUp;


    public static void updateHealthUI(Health health)
    {
        if(OnHit != null)
        {
            OnHit(health);
        }
    }

    public static void updateCoinUI(int numCoins)
    {
        if (OnCoinPickUp != null)
        {
            OnCoinPickUp(numCoins);
        }
    }
}
