using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleCity
{
    public class Game
    {
        public Map Map { get; private set; }
        public Tank? Player { get; private set; }
        public List<Tank> Enemies { get; private set; } = new List<Tank>();
        
        public bool IsGameOver { get; private set; } = false;
        public bool IsLevelComplete { get; private set; } = false;
        
        private int playerShootCooldown = 0;
        private const int ShootCooldownMax = 30; 

        public Game()
        {
            Map = new Map();
        }

        public void Start(int level)
        {
            Map.LoadLevel(level);
            InitializeGame();
        }

        public void StartCustomLevel(string fileName)
        {
            Map.LoadCustomLevel(fileName);
            InitializeGame();
        }

        private void InitializeGame()
        {
            Player = Map.Objects.OfType<Tank>().FirstOrDefault(t => t.IsPlayer);
            Enemies = Map.Objects.OfType<Tank>().Where(t => !t.IsPlayer).ToList();
            IsGameOver = false;
            IsLevelComplete = false;
        }

        public void Update()
        {
            if (IsGameOver || IsLevelComplete) return;

            if (playerShootCooldown > 0) playerShootCooldown--;
            
            Map.Update();
            
            Enemies = Map.Objects.OfType<Tank>().Where(t => !t.IsPlayer && t.IsActive).ToList();
            
            foreach (var enemy in Enemies)
            {
                if (!enemy.IsDestroyed)
                {
                    EnemyAI.UpdateAI(enemy, Map);
                }
            }
            
            CollisionManager.HandleBulletCollisions(Map.Objects);
            
            CheckGameState();
            
            Map.Objects.RemoveAll(obj => !obj.IsActive || obj.IsDestroyed);
        }

        public void HandlePlayerInput(int direction)
        {
            if (Player != null && !Player.IsDestroyed && Player.IsActive)
            {
                Player.Move(direction, Map.Objects);
            }
        }

        public void HandlePlayerShoot()
        {
            if (Player != null && !Player.IsDestroyed && playerShootCooldown <= 0)
            {
                var bullet = Player.Shoot();
                Map.Objects.Add(bullet);
                playerShootCooldown = ShootCooldownMax;
            }
        }

        private void CheckGameState()
        {
            var baseObj = Map.Objects.OfType<Base>().FirstOrDefault();
            
            if (baseObj == null || baseObj.IsDestroyed)
            {
                IsGameOver = true;
                return;
            }
            
            if (Player == null || Player.IsDestroyed)
            {
                IsGameOver = true;
                return;
            }
            
            if (Enemies.Count == 0 && Map.RemainingEnemiesToSpawn <= 0)
            {
                IsLevelComplete = true;
            }
        }

        public void Draw(Avalonia.Controls.Canvas canvas)
        {
            Map.Draw(canvas);
        }
    }
}