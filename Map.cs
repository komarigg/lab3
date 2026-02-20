using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace BattleCity
{
    public class Map
    {
        public List<GameObject> Objects { get; set; } = new List<GameObject>();
        public Base? PlayerBase { get; set; } 
        public int Level { get; private set; } = 1;
        public string CurrentLevelName { get; private set; } = "Level 1";
        
        public int RemainingEnemiesToSpawn { get; set; } = 0; 

        public void LoadLevel(int level)
        {
            Level = level;
            Objects.Clear();
            
            var levelManager = new LevelManager();
            var levelData = levelManager.LoadLevel(level);
            
            if (levelData == null)
            {
                LoadDefaultLevel();
                CurrentLevelName = $"Уровень {level}";
                return;
            }
            
            LoadFromLevelData(levelData);
            CurrentLevelName = levelData.Name;
        }

        public void LoadCustomLevel(string fileName)
        {
            Objects.Clear();
            
            var levelManager = new LevelManager();
            var levelData = levelManager.LoadCustomLevel(fileName);
            
            if (levelData == null)
            {
                LoadDefaultLevel();
                CurrentLevelName = "Default Level";
                return;
            }
            
            LoadFromLevelData(levelData);
            CurrentLevelName = levelData.Name;
            Level = 0; 
        }

        private void LoadDefaultLevel()
        {
            for (int i = 0; i < 13; i++)
            {
                Objects.Add(new Wall { X = i * 32, Y = 0, IsBreakable = false });
                Objects.Add(new Wall { X = i * 32, Y = 12 * 32, IsBreakable = false });
            }
            
            for (int i = 1; i < 12; i++)
            {
                Objects.Add(new Wall { X = 0, Y = i * 32, IsBreakable = false });
                Objects.Add(new Wall { X = 12 * 32, Y = i * 32, IsBreakable = false });
            }
            
            PlayerBase = new Base { X = 6 * 32, Y = 12 * 32 - 32, Width = 32, Height = 32 };
            Objects.Add(PlayerBase);
            
            var playerTank = new Tank(5 * 32, 12 * 32 - 64, true) 
            { 
                Direction = 0
            };
            Objects.Add(playerTank);

            RemainingEnemiesToSpawn = 0; 
        }

        private void LoadFromLevelData(LevelData levelData)
        {
            for (int i = 0; i < 13; i++)
            {
                Objects.Add(new Wall { X = i * 32, Y = 0, IsBreakable = false });
                Objects.Add(new Wall { X = i * 32, Y = 12 * 32, IsBreakable = false });
            }

            for (int i = 1; i < 12; i++)
            {
                Objects.Add(new Wall { X = 0, Y = i * 32, IsBreakable = false });
                Objects.Add(new Wall { X = 12 * 32, Y = i * 32, IsBreakable = false });
            }
            
            if (levelData != null)
            {
                PlayerBase = new Base { X = levelData.BaseX * 32, Y = levelData.BaseY * 32, Width = 32, Height = 32 };
                Objects.Add(PlayerBase);

                var playerTank = new Tank(levelData.PlayerX * 32, levelData.PlayerY * 32, true)
                {
                    Direction = 0
                };
                Objects.Add(playerTank);
                
                if (levelData.Enemies != null)
                {
                    foreach (var enemy in levelData.Enemies)
                    {
                        var enemyTank = new Tank(enemy.X * 32, enemy.Y * 32, false)
                        {
                            Direction = 2
                        };
                        Objects.Add(enemyTank);
                    }
                    RemainingEnemiesToSpawn = 0; 
                }
                
                if (levelData.Walls != null)
                {
                    foreach (var wall in levelData.Walls)
                    {
                        var wallObj = new Wall
                        {
                            X = wall.X * 32,
                            Y = wall.Y * 32,
                            IsBreakable = wall.IsBreakable
                        };
                        Objects.Add(wallObj);
                    }
                }
            }
        }

        public void Update()
        {
            foreach (var obj in Objects.ToList())
            {
                if (obj != null && obj.IsActive)
                    obj.Update();
            }
        }

        public void Draw(Canvas canvas)
        {
            if (canvas == null) return;

            foreach (var obj in Objects)
            {
                if (obj != null && obj.IsActive)
                    obj.Draw(canvas);
            }
        }
    }
}