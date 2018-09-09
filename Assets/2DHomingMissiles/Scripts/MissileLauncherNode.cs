using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class MissileLauncherNode : MonoBehaviour
    {
        [HideInInspector]
        public GameObject nodeMissileSwarmTarget;

        /// <summary>
        /// Distance the swarm target should be placed above or below this launcher node
        /// </summary>
        public float nodeMissileSwarmTargetOffsetDistance;

        /// <summary>
        /// Set by the MissileLaunchScript editor when you create a missile launcher node. Affects the X Offset positioning for the initial target that missiles track before locking onto their main target.
        /// </summary>
        public float launcherNodeOffsetRandomXAmount;

        /// <summary>
        /// Set by the MissileLaunchScript editor when you create a missile launcher node. Affects the Y Offset positioning for the initial target that missiles track before locking onto their main target.
        /// </summary>
        public float launcherNodeOffsetRandomYAmount;

        /// <summary>
        /// Applies randomness using offsets defined for X and Y when launching a missile initially (the offset of the initial target) when enabled.
        /// </summary>
        public bool applyLauncherNodeRandomness;

        [HideInInspector] public Vector2 nodeOffsetPositionOriginalPos;

        public enum MissileNodeDirection
        {
            Up,
            Down,
            Left,
            Right
        };

        /// <summary>
        /// Specify whether the missile launcher node is located on the Up, Down, left, or right of the entity that is firing missiles (used to help with missile swarming/arcing/swinging effect on initial launch).
        /// </summary>
        public MissileNodeDirection MissileNodeDirectionSetting;

        private void OnEnable()
        {
            // Dynamically create swarm targets for missiles that need to arc/swarm out of missile launcher nodes when the game starts based on whether the MissileLauncherNode is set in "Up", "Down", "Left", or "Right" mode.
            switch (MissileNodeDirectionSetting)
            {
                case MissileNodeDirection.Up:
                    nodeMissileSwarmTarget = new GameObject(transform.name + "_NodeMissileSwarmTarget");
                    nodeOffsetPositionOriginalPos = new Vector2(transform.position.x,
                        transform.position.y + nodeMissileSwarmTargetOffsetDistance);
                    nodeMissileSwarmTarget.transform.position = nodeOffsetPositionOriginalPos;
                    break;
                case MissileNodeDirection.Down:
                    nodeMissileSwarmTarget = new GameObject(transform.name + "_NodeMissileSwarmTarget");
                    nodeOffsetPositionOriginalPos = new Vector2(transform.position.x,
                        transform.position.y - nodeMissileSwarmTargetOffsetDistance);
                    nodeMissileSwarmTarget.transform.position = nodeOffsetPositionOriginalPos;
                    break;
                case MissileNodeDirection.Left:
                    nodeMissileSwarmTarget = new GameObject(transform.name + "_NodeMissileSwarmTarget");
                    nodeOffsetPositionOriginalPos =
                        new Vector2(transform.position.x - nodeMissileSwarmTargetOffsetDistance,
                            transform.position.y);
                    nodeMissileSwarmTarget.transform.position = nodeOffsetPositionOriginalPos;
                    break;
                case MissileNodeDirection.Right:
                    nodeMissileSwarmTarget = new GameObject(transform.name + "_NodeMissileSwarmTarget");
                    nodeOffsetPositionOriginalPos =
                        new Vector2(transform.position.x + nodeMissileSwarmTargetOffsetDistance,
                            transform.position.y);
                    nodeMissileSwarmTarget.transform.position = nodeOffsetPositionOriginalPos;
                    break;
            }

            nodeMissileSwarmTarget.transform.parent = transform;
        }

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}