using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TwoDLaserPack
{
    public class DemoPlayerHealth : MonoBehaviour
    {
        public GameObject bloodSplatPrefab;
        public GameObject playerPrefab;
        public Button restartButton;
        public Text healthText;

        private LineBasedLaser[] allLasersInScene;

        public int HealthPoints
        {
            get { return _healthPoints; }
            set
            {
                _healthPoints = value;
                if (_healthPoints <= 0)
                {
                    // Player is dead
                    if (bloodSplatPrefab != null)
                    {
                        Instantiate(bloodSplatPrefab, transform.position, Quaternion.identity);
                    }

                    healthText.text = "Health: 0";
                    
                    var playerSprite = gameObject.GetComponent<Renderer>();
                    playerSprite.enabled = false;

                    var playerMovement = gameObject.GetComponent<PlayerMovement>();
                    playerMovement.enabled = false;

                    restartButton.gameObject.SetActive(true);

                    foreach (var lineBasedLaser in allLasersInScene)
                    {
                        lineBasedLaser.OnLaserHitTriggered -= LaserOnOnLaserHitTriggered;
                        lineBasedLaser.SetLaserState(false);
                    }
                }
                else
                {
                    healthText.text = "Health: " + _healthPoints;
                }
            }
        }

        public ParticleSystem bloodParticleSystem;
        
        [SerializeField]
        private int _healthPoints;

        // Use this for initialization
        void Start()
        {
            _healthPoints = 10;
            if (restartButton == null)
            {
                restartButton = GameObject.FindObjectsOfType<Button>().FirstOrDefault(b => b.name == "ButtonReplay");
            }

            healthText = GameObject.FindObjectsOfType<Text>().FirstOrDefault(t => t.name == "TextHealth");
            healthText.text = "Health: 10";

            allLasersInScene = GameObject.FindObjectsOfType<LineBasedLaser>();
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartButtonClick);

            if (allLasersInScene.Any())
            {
                foreach (var laser in allLasersInScene)
                {
                    laser.OnLaserHitTriggered += LaserOnOnLaserHitTriggered;
                    laser.SetLaserState(true);
                    laser.targetGo = gameObject;
                }
            }

            var playerMovement = gameObject.GetComponent<PlayerMovement>();
            playerMovement.enabled = true;

            var playerSprite = gameObject.GetComponent<Renderer>();
            playerSprite.enabled = true;

            restartButton.gameObject.SetActive(false);
        }

        private void OnRestartButtonClick()
        {
            CreateNewPlayer();
            Destroy(gameObject);
        }

        private void CreateNewPlayer()
        {
            var newPlayer = (GameObject)Instantiate(playerPrefab, new Vector2(6.26f, - 2.8f), Quaternion.identity);

            // Set all lasers to target the newly created player.
            foreach (var laser in allLasersInScene)
            {
                laser.targetGo = newPlayer;
            }
        }

        private void LaserOnOnLaserHitTriggered(RaycastHit2D hitInfo)
        {
            if (hitInfo.collider.gameObject == gameObject)
            {
                if (bloodParticleSystem != null)
                {
                    bloodParticleSystem.Play();
                    HealthPoints --;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

