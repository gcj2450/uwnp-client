using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerProject.Source
{
    class EnemyManager
    {
        public List<Enemy> Enemies { get { return _enemies; } }

        private List<Enemy> _enemies;

        public EnemyManager()
        {
            // Initialize the enemies list
            _enemies = new List<Enemy>();
        }


        public void Update(GameTime gameTime)
        {
            // Update the Enemies
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                if (_enemies[i].Active == false)
                {
                    _enemies.RemoveAt(i);
                }
            }
        }

        public Enemy AddEnemy()
        {
            // Create the animation object
            GameObject enemyAnimation = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemyAnimation.transform.position = Vector2.zero;

            // Randomly generate the position of the enemy
            Vector2 position = UnityEngine.Random.insideUnitCircle;

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            _enemies.Add(enemy);

            return enemy;
        }

        public void AddEnemy(Vector2 position, string enemyID)
        {
            // Create the animation object
            GameObject enemyAnimation = GameObject.CreatePrimitive(PrimitiveType.Cube);
            enemyAnimation.transform.position = Vector2.zero;

            // Create an enemy
            Enemy enemy = new Enemy(enemyID);

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            _enemies.Add(enemy);
        }

        public void DeactivateEnemy(string enemyID)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].EnemyID == enemyID)
                {
                    _enemies[i].Active = false;
                    return;
                }
            }
        }

        public Enemy DeactivateAndReturnEnemy(string enemyID)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].EnemyID == enemyID)
                {
                    _enemies[i].Active = false;
                    return _enemies[i];
                }
            }

            return null;
        }
    }
}
