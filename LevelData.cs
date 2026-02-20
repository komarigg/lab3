using System;

namespace BattleCity
{
    public class LevelData
    {
        public string Name { get; set; } = "Level 1";
        public int PlayerX { get; set; } = 5;
        public int PlayerY { get; set; } = 11;
        public int BaseX { get; set; } = 6;
        public int BaseY { get; set; } = 11;
        public EnemySpawn[]? Enemies { get; set; } = Array.Empty<EnemySpawn>();
        public WallData[]? Walls { get; set; } = Array.Empty<WallData>();
        public bool HasWalls { get; set; } = true;
    }

    public class EnemySpawn
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class WallData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsBreakable { get; set; } = true;
    }
}