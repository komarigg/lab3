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

            // 1. Обновляем все объекты (движение пуль и т.д.)
            Map.Update();

            // 2. Обновляем список врагов
            Enemies = Map.Objects.OfType<Tank>().Where(t => !t.IsPlayer && t.IsActive).ToList();

            // 3. Логика врагов
            foreach (var enemy in Enemies)
            {
                if (!enemy.IsDestroyed)
                {
                    EnemyAI.UpdateAI(enemy, Map);
                }
            }

            // 4. Обработка столкновений пуль (здесь база может получить IsDestroyed = true)
            CollisionManager.HandleBulletCollisions(Map.Objects);

            // 5. ВАЖНО: Сначала проверяем состояние игры (жива ли база и игрок)...
            CheckGameState();

            // 6. ...и только ПОТОМ удаляем "мертвые" объекты из списка
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
            // Проверка базы: ищем её среди всех объектов до очистки
            var baseObj = Map.Objects.OfType<Base>().FirstOrDefault();
            
            // Если база была уничтожена в этом кадре или вообще исчезла - проигрыш
            if (baseObj == null || baseObj.IsDestroyed)
            {
                IsGameOver = true;
                return;
            }

            // Проверка игрока
            if (Player == null || Player.IsDestroyed)
            {
                IsGameOver = true;
                return;
            }

            // Проверка победы (если база жива, проверяем врагов)
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