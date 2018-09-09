using UnityEngine;
using System.Collections;

namespace TwoDLaserPack
{
    public class PlayerMovement : MonoBehaviour
    {

        public enum PlayerMovementType
        {
            Normal,
            FreeAim
        }

        /// <summary>
        /// Sets the player movement type - either normal top down horizontal or vertical (good for shmups), or to FreeAim, which allows for free aiming type top down shooter style controls
        /// </summary>
        public PlayerMovementType playerMovementType;

        public bool IsMoving;

        public float aimAngle;

        [Range(1f, 5f)] public float freeAimMovementSpeed = 2f;

        private float SmoothSpeedX = 0f;
        private float SmoothSpeedY = 0f; //Don't touch this
        private const float SmoothMaxSpeedX = 7f;
        private const float SmoothMaxSpeedY = 7f; //This is the maximum speed that the object will achieve
        private const float AccelerationX = 22f;
        private const float AccelerationY = 22f; // How fast will object reach a maximum speed
        private const float DecelerationX = 33f;
        private const float DecelerationY = 33f; // How fast will object reach a speed of 0

        private Animator playerAnimator;

        // Use this for initialization
        private void Start()
        {

            // If the player has an animator component then get a handle to it and cache it in the playerAnimator field.
            if (gameObject.GetComponent<Animator>() != null)
            {
                playerAnimator = gameObject.GetComponent<Animator>();
            }
        }

        private void moveForward(float amount)
        {
            var moveUp = new Vector3(transform.position.x, transform.position.y + amount*Time.deltaTime,
                transform.position.z);
            transform.position = moveUp;
        }

        private void moveBack(float amount)
        {
            var moveBack = new Vector3(transform.position.x, transform.position.y - amount*Time.deltaTime,
                transform.position.z);
            transform.position = moveBack;
        }

        private void moveRight(float amount)
        {
            var moveRight = new Vector3(transform.position.x + amount*Time.deltaTime, transform.position.y,
                transform.position.z);
            transform.position = moveRight;
        }

        private void moveLeft(float amount)
        {
            var moveLeft = new Vector3(transform.position.x - amount*Time.deltaTime, transform.position.y,
                transform.position.z);
            transform.position = moveLeft;
        }

        private void HandlePlayerToggles()
        {

        }

        /// <summary>
        /// Not the best player movement code, but it is not the focus of this asset, so for now this works for the demo and allowing the player to move around :)
        /// </summary>
        private void HandlePlayerMovement()
        {
            // Get axis information

            var inputX = Input.GetAxis("Horizontal");
            var inputY = Input.GetAxis("Vertical");

            // Once off check to determine if player is moving and set flag accordingly.
            if (Mathf.Abs(inputX) > 0f || Mathf.Abs(inputY) > 0f)
            {
                IsMoving = true;
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("IsMoving", true);
                }
            }
            else
            {
                IsMoving = false;
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("IsMoving", false);
                }
            }

            var facingDirection = Vector2.zero;

            switch (playerMovementType)
            {
                // Normal top down horizontal or vertical style player controls
                case PlayerMovementType.Normal:

                    // Horizontal movement
                    if ((inputX < 0f) && (SmoothSpeedX > -SmoothMaxSpeedX)) //left
                    {
                        SmoothSpeedX = SmoothSpeedX - AccelerationX*Time.deltaTime;
                    }
                    else if ((inputX > 0f) && (SmoothSpeedX < SmoothMaxSpeedX)) //right
                    {
                        SmoothSpeedX = SmoothSpeedX + AccelerationX*Time.deltaTime;
                    }
                    else
                    {
                        if (SmoothSpeedX > DecelerationX*Time.deltaTime)
                            SmoothSpeedX = SmoothSpeedX - DecelerationX*Time.deltaTime;
                        else if (SmoothSpeedX < -DecelerationX*Time.deltaTime)
                            SmoothSpeedX = SmoothSpeedX + DecelerationX*Time.deltaTime;
                        else
                            SmoothSpeedX = 0;
                    }

                    // Vertical movement
                    if ((inputY < 0f) && (SmoothSpeedY > -SmoothMaxSpeedY)) // down
                    {
                        SmoothSpeedY = SmoothSpeedY - AccelerationY*Time.deltaTime;
                    }
                    else if ((inputY > 0f) && (SmoothSpeedY < SmoothMaxSpeedY)) // up
                    {
                        SmoothSpeedY = SmoothSpeedY + AccelerationY*Time.deltaTime;
                    }
                    else
                    {
                        if (SmoothSpeedY > DecelerationY*Time.deltaTime)
                            SmoothSpeedY = SmoothSpeedY - DecelerationY*Time.deltaTime;
                        else if (SmoothSpeedY < -DecelerationY*Time.deltaTime)
                            SmoothSpeedY = SmoothSpeedY + DecelerationY*Time.deltaTime;
                        else
                            SmoothSpeedY = 0;
                    }

                    var newPosition = new Vector2(transform.position.x + SmoothSpeedX*Time.deltaTime,
                        transform.position.y + SmoothSpeedY*Time.deltaTime);
                    transform.position = newPosition;

                    break;

                    // top down free aim style player controls
                case PlayerMovementType.FreeAim:

                    if (inputY > 0)
                    {
                        moveForward(freeAimMovementSpeed);
                    }
                    else if (inputY < 0)
                    {
                        moveBack(freeAimMovementSpeed);
                    }

                    if (inputX > 0)
                    {
                        moveRight(freeAimMovementSpeed);
                    }
                    else if (inputX < 0)
                    {
                        moveLeft(freeAimMovementSpeed);
                    }

                    // Get the world position of the mouse cursor and set facing direction to that minus the player's current position.
                    var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));

                    // Calculate based on whether this weapon configuration is set relative to the WeaponSystem object, or the assigned gunpoint object.
                    //facingDirection = worldMousePosition - gunPoint.transform.position;
                    facingDirection = worldMousePosition - transform.position;

                    break;
            }

            CalculateAimAndFacingAngles(facingDirection);

            // Clamp the player to the screen
            var pos = Camera.main.WorldToViewportPoint(transform.position);
            pos.x = Mathf.Clamp(pos.x, 0.05f, 0.95f);
            pos.y = Mathf.Clamp(pos.y, 0.05f, 0.95f);
            transform.position = Camera.main.ViewportToWorldPoint(pos);

        }

        /// <summary>
        /// Calculate aim angle and other settings that apply to all ShooterType orientations
        /// </summary>
        /// <param name="facingDirection"></param>
        private void CalculateAimAndFacingAngles(Vector2 facingDirection)
        {
            aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
            if (aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }

            // Rotate the player to face the direction of the mouse cursor (the object with the weaponsystem component attached, or, if the weapon configuration specifies relative to the gunpoint, rotate the gunpoint instead.
            //gunPoint.transform.eulerAngles = new Vector3(0.0f, 0.0f, aimAngle * Mathf.Rad2Deg);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, aimAngle * Mathf.Rad2Deg);

        }

        // Update is called once per frame
        private void Update()
        {
            HandlePlayerMovement();
            HandlePlayerToggles();
        }
    }
}