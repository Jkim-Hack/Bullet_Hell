using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TwoDLaserPack
{
    public class DemoSceneNavigation : MonoBehaviour
    {

        public Button buttonNextDemo;

        // Use this for initialization
        void Start()
        {
            buttonNextDemo.onClick.AddListener(OnButtonNextDemoClick);
        }

        private void OnButtonNextDemoClick()
        {
            var currentLevel = SceneManager.GetActiveScene().buildIndex;
            if (currentLevel < SceneManager.sceneCount - 1) SceneManager.LoadScene(currentLevel + 1);
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

