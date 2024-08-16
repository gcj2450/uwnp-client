using System;
using UnityEngine;

namespace MultiplayerProject.Source
{
    class Explosion
    {
        public bool Active;

        private GameObject  _explosionAnimation;
        private Vector2 _position;      
        private int _timeToLive;

        public void Initialize(GameObject animation, Vector2 position)
        {
            _explosionAnimation = animation;
            _position = position;
            _timeToLive = 30;

            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            _timeToLive -= 1;

            if (_timeToLive <= 0)
            {
                Active = false;
            }
        }

    }
}
