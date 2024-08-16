﻿using System;
using UnityEngine;

namespace MultiplayerProject.Source
{
    class Enemy
    {
        public GameObject EnemyAnimation;
        public Vector2 Position;
        public bool Active;

        public int Health;
        public int Damage; // The amount of damage the enemy inflicts on the player ship
        public int Value;  // The amount of score the enemy will give to the player

        public string EnemyID { get; set; }

        const float ENEMY_MOVE_SPEED = 6f;

        const int ENEMY_STARTING_HEALTH = 10;
        const int ENEMY_DAMAGE = 10;
        const int ENEMY_DEATH_SCORE_INCREASE = 100;

        public Enemy()
        {
            EnemyID = Guid.NewGuid().ToString();
        }

        public Enemy(string ID)
        {
            EnemyID = ID;
        }

        public void Initialize(GameObject animation, Vector2 position)
        {    
            EnemyAnimation = animation;

            Position = position;
            animation.transform.position = position;
            Active = true;

            Health = ENEMY_STARTING_HEALTH;

            Damage = ENEMY_DAMAGE;

            Value = ENEMY_DEATH_SCORE_INCREASE;
        }

        public void Update(GameTime gameTime)
        {
            Update();

            // Update the position of the Animation
            EnemyAnimation.transform.position= Position;

            // Update Animation
            //EnemyAnimation.Update(gameTime);
        }

        public void Update()
        {
            // The enemy always moves to the left so decrement its x position
            Position.x -= ENEMY_MOVE_SPEED;
            EnemyAnimation.transform.position = Position;
            // If the enemy is past the screen or its health reaches 0 then deactivate it
            if (Health <= 0)
            {
                // By setting the Active flag to false, the game will remove this objet from the
                // active game list
                EnemyAnimation.SetActive(false);
            }
        }

    }
}
