using System;
using UnityEngine;

namespace MultiplayerProject.Source
{
    public class Laser
    {
        // animation the represents the laser animation.
        public GameObject LaserAnimation;

        // position of the laser
        public Vector2 Position;

        public float Rotation;

        // set the laser to active
        public bool Active;

        public string PlayerFiredID { get; set; }
        public string LaserID { get; set; }

        // the speed the laser travels
        private const float _laserMoveSpeed = 30f;
        private const float _laserMaxTimeActive = 5f;

        private float _timeActive;

        public Laser()
        {
            LaserID = Guid.NewGuid().ToString();
            PlayerFiredID = "";
        }

        public Laser(string ID, string playerFiredID)
        {
            LaserID = ID;
            PlayerFiredID = playerFiredID;
        }

        public void Initialize(GameObject animation, Vector2 position, float rotation)
        {
            _timeActive = 0;
            LaserAnimation = animation;
            Position = position;
            Rotation = rotation;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            LaserAnimation.transform.position= Position;
            Vector3 angle = LaserAnimation.transform.localEulerAngles;
            angle.y = Rotation;
            LaserAnimation.transform.localEulerAngles = angle;
            //LaserAnimation.Update(gameTime);
        }

        public void Update(float deltaTime)
        {
            Vector2 direction = new Vector2((float)Math.Cos(Rotation),
                                     (float)Math.Sin(Rotation));
            direction.Normalize();
            Position += direction * _laserMoveSpeed;

            _timeActive += deltaTime;

            if (_timeActive > _laserMaxTimeActive)
            {
                Active = false;
            }
        }

    }
}
