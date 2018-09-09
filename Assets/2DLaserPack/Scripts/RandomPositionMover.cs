using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class RandomPositionMover : MonoBehaviour
{

    public float pickerInterval, radius;

    public GameObject player;

    public Vector2 randomPointInCircle;

    // Use this for initialization
    void Start()
    {

        if (pickerInterval == 0f)
        {
            pickerInterval = 3f;
        }

        randomPointInCircle = Vector2.zero;
        InvokeRepeating("PickRandomPointInCircle", Random.Range(0f, pickerInterval), pickerInterval);

    }

    private void PickRandomPointInCircle()
    {
        transform.position = player.transform.position;
        randomPointInCircle = (Vector2)transform.localPosition + Random.insideUnitCircle * radius;
        transform.localPosition = randomPointInCircle;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
