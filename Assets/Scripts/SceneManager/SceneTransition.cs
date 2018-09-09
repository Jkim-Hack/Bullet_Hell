using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    public Animator transitionAnim;
    public string sceneName;
    private bool isDone = true;
    Scene scene;
    private string nameScene;
	// Use this for initialization

	void Start()
    {
        
    }
	// Update is called once per frame
	void Update () {

        scene = SceneManager.GetActiveScene();
        nameScene = scene.name;
        
        if (GameObject.Find("Alien1GameObject") == null && nameScene == "FirstScene")
        {
            Debug.Log("t");
            StartCoroutine(LoadScene());
            isDone = false;
        }
	}


    IEnumerator LoadScene()
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
       
    }

}
