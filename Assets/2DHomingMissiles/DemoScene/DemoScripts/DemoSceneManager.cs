using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class DemoSceneManager : MonoBehaviour
    {
        public int maxMissiles = 10;
        public List<GameObject> missileColletion = new List<GameObject>();
        public GameObject missilePrefab, asteroidPrefab;
        public bool spawnAsteroids;
        public float missileSpeedSliderValue = 0.35f;
        public float missileProportionalConstSliderValue = 0.55f;
        public GUIText propConstLabel, maxSpeedLabel, demo2Text;

        // Use this for initialization
        private void Start()
        {
            // Spawn some asteroids just for a bit of visual variety. They don't provide any functional aspect to the demo scene, although if you set their tag to "Targets" the missiles will start tracking them too!
            if (spawnAsteroids) InvokeRepeating("SpawnAsteroid", 1.0f, 1.0f);
        }

        private void SpawnAsteroid()
        {
            if (Application.loadedLevelName != "DemoScene02")
            {
                var pos = (Vector2)Camera.main.ViewportToWorldPoint(new Vector3(1.0f, Random.Range(0.05f, 0.95f)));
                var asteroid = (GameObject)Instantiate(asteroidPrefab, new Vector3(pos.x, pos.y, -1f), Quaternion.identity);
                asteroid.name = "Asteroid";
            }
        }

        private void OnGUI()
        {
            if (Application.loadedLevelName != "DemoScene02")
            {
                missileSpeedSliderValue = GUI.HorizontalSlider(new Rect(Screen.width / 4, 35, 200, 20),
                missileSpeedSliderValue, 0.05F, 1.5F);
                missileProportionalConstSliderValue = GUI.HorizontalSlider(new Rect(Screen.width / 2, 35, 200, 20),
                    missileProportionalConstSliderValue, 0.05F, 1.0F);

                maxSpeedLabel.text = missileSpeedSliderValue.ToString();
                propConstLabel.text = missileProportionalConstSliderValue.ToString();
            }
            else
            {
                demo2Text.text =
                    "In this demo the launch control feature is being used. Launch control is set up in the editor script and allows you to fire at targets by tag name, \n closest distance, and set up auto-fire." +
                    " Here, the asteroids are tagged 'Asteroid' and the MissileLaunchScript component is set in the editor \n for missiles to target the closest 6 objects with this tag name.";
            }

            if (GUI.Button(new Rect(Screen.width - 420, Screen.height - 90, 140, 30), "Next demo scene >>"))
            {
                var currentLevelIndex = Application.loadedLevel;
                if (currentLevelIndex < Application.levelCount - 1)
                {
                    Application.LoadLevel(currentLevelIndex + 1);
                }
                else
                {
                    Application.LoadLevel(0);
                }
            }
            if (GUI.Button(new Rect(280, Screen.height - 90, 140, 30), "<< Prev demo scene"))
            {
                var currentLevelIndex = Application.loadedLevel;
                if (currentLevelIndex > 0)
                {
                    Application.LoadLevel(currentLevelIndex - 1);
                }
                else
                {
                    Application.LoadLevel(Application.levelCount - 1);
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (Application.loadedLevelName != "DemoScene02")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var targets = GameObject.FindGameObjectsWithTag("Targets").ToList();
                    Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // Note, you should ideally use Object Pooling rather than instantiation for mass missile use!
                    if (missileColletion.Count < maxMissiles)
                    {
                        // We modify the position of instantiation here to ensure the Z-ordering is correct in the demo scene. Normally we would use the 2D layer sorting order for this, but there is an issue with Unity 4.3 where
                        // exported assets do not retain layer sort ordering, and therefore we have to use Z depth for ordering here instead.
                        var newMissile =
                            (GameObject)
                                Instantiate(missilePrefab, new Vector3(clickPosition.x, clickPosition.y, -3f),
                                    Quaternion.identity);

                        // Adjust missile main performance properties based on scene slider values
                        if (Application.loadedLevelName == "DemoScene01")
                        {
                            var missileScriptReference = newMissile.GetComponent<MissileController>();
                            missileScriptReference.kProportionalConst = missileProportionalConstSliderValue;
                            missileScriptReference.maxSpeed = missileSpeedSliderValue;

                            // Target a random gameobject tagged with "Targets" found in the scene.
                            missileScriptReference.target = targets[Random.Range(0, targets.Count - 1)];
                            missileColletion.Add(newMissile);
                        }
                    }
                }

                // Clean up old missiles so we can fire more (depends on maxMissiles)
                for (var index = 0; index < missileColletion.Count; index++)
                {
                    var missile = missileColletion[index];
                    if (missile == null)
                    {
                        missileColletion.RemoveAt(index);
                    }
                }
            }
        }
    }
}