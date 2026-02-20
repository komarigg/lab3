using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace BattleCity
{
    public class LevelManager
    {
        private readonly string LevelsFolder;
        
        public LevelManager()
        {
            LevelsFolder = GetLevelsFolderPath();
            Console.WriteLine($"=== LevelManager Initialized ===");
            Console.WriteLine($"Levels folder: {LevelsFolder}");
            Console.WriteLine($"Folder exists: {Directory.Exists(LevelsFolder)}");
            
            EnsureLevelsFolderExists();
            CopyLevelsFromProjectToBin();
            CreateDefaultLevels();
            
            Console.WriteLine($"=== LevelManager Ready ===");
        }
        
        private string GetLevelsFolderPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var levelsPath = Path.Combine(baseDir, "Levels");
            
            Console.WriteLine($"Base directory: {baseDir}");
            Console.WriteLine($"Levels path: {levelsPath}");
            
            return levelsPath;
        }
        
        private void EnsureLevelsFolderExists()
        {
            if (!Directory.Exists(LevelsFolder))
            {
                Console.WriteLine($"Creating Levels folder: {LevelsFolder}");
                Directory.CreateDirectory(LevelsFolder);
            }
        }
        
        private void CopyLevelsFromProjectToBin()
        {
            try
            {
                var projectFolder = Directory.GetCurrentDirectory();
                var projectLevelsPath = Path.Combine(projectFolder, "Levels");
                
                Console.WriteLine($"Project folder: {projectFolder}");
                Console.WriteLine($"Project Levels path: {projectLevelsPath}");
                Console.WriteLine($"Project Levels exists: {Directory.Exists(projectLevelsPath)}");
                
                if (Directory.Exists(projectLevelsPath))
                {
                    var projectFiles = Directory.GetFiles(projectLevelsPath, "*.json");
                    Console.WriteLine($"Found {projectFiles.Length} JSON files in project Levels");
                    
                    foreach (var file in projectFiles)
                    {
                        var fileName = Path.GetFileName(file);
                        var destFile = Path.Combine(LevelsFolder, fileName);
                        
                        if (!File.Exists(destFile))
                        {
                            File.Copy(file, destFile, true);
                            Console.WriteLine($"Copied: {fileName}");
                        }
                        else
                        {
                            Console.WriteLine($"Already exists: {fileName}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Project Levels folder doesn't exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying levels: {ex.Message}");
            }
        }
        
        public void CreateDefaultLevels()
        {
            var level1 = new LevelData
            {
                Name = "Level 1",
                PlayerX = 5,
                PlayerY = 11,
                BaseX = 6,
                BaseY = 11,
                Enemies = new[]
                {
                    new EnemySpawn { X = 2, Y = 1 },
                    new EnemySpawn { X = 5, Y = 1 },
                    new EnemySpawn { X = 8, Y = 1 }
                },
                Walls = new[]
                {
                    new WallData { X = 2, Y = 4, IsBreakable = true },
                    new WallData { X = 3, Y = 4, IsBreakable = true },
                    new WallData { X = 4, Y = 4, IsBreakable = true },
                    new WallData { X = 5, Y = 4, IsBreakable = true },
                    new WallData { X = 2, Y = 8, IsBreakable = true },
                    new WallData { X = 3, Y = 8, IsBreakable = true },
                    new WallData { X = 4, Y = 8, IsBreakable = true },
                    new WallData { X = 5, Y = 8, IsBreakable = true }
                }
            };
            SaveLevel(1, level1);

            var level2 = new LevelData
            {
                Name = "Level 2",
                PlayerX = 4,
                PlayerY = 11,
                BaseX = 7,
                BaseY = 11,
                Enemies = new[]
                {
                    new EnemySpawn { X = 1, Y = 1 },
                    new EnemySpawn { X = 4, Y = 1 },
                    new EnemySpawn { X = 7, Y = 1 },
                    new EnemySpawn { X = 10, Y = 1 }
                },
                Walls = new[]
                {
                    new WallData { X = 1, Y = 3, IsBreakable = true },
                    new WallData { X = 3, Y = 3, IsBreakable = true },
                    new WallData { X = 5, Y = 3, IsBreakable = true },
                    new WallData { X = 7, Y = 3, IsBreakable = true },
                    new WallData { X = 9, Y = 3, IsBreakable = true },
                    new WallData { X = 11, Y = 3, IsBreakable = true },
                    new WallData { X = 1, Y = 9, IsBreakable = true },
                    new WallData { X = 3, Y = 9, IsBreakable = true },
                    new WallData { X = 5, Y = 9, IsBreakable = true },
                    new WallData { X = 7, Y = 9, IsBreakable = true },
                    new WallData { X = 9, Y = 9, IsBreakable = true },
                    new WallData { X = 11, Y = 9, IsBreakable = true }
                }
            };
            SaveLevel(2, level2);
            
            Console.WriteLine("Default levels created");
        }
        
        public void SaveLevel(int levelNumber, LevelData levelData)
        {
            try
            {
                string filePath = Path.Combine(LevelsFolder, $"level{levelNumber}.json");
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                string json = JsonSerializer.Serialize(levelData, options);
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Saved level: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving level {levelNumber}: {ex.Message}");
            }
        }
        
        public LevelData? LoadLevel(int levelNumber)
        {
            try
            {
                string filePath = Path.Combine(LevelsFolder, $"level{levelNumber}.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    };
                    return JsonSerializer.Deserialize<LevelData>(json, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading level {levelNumber}: {ex.Message}");
            }
            return null;
        }
        
        public LevelData? LoadCustomLevel(string fileName)
        {
            try
            {
                string filePath = Path.Combine(LevelsFolder, fileName);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    };
                    return JsonSerializer.Deserialize<LevelData>(json, options);
                }
                else
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading custom level {fileName}: {ex.Message}");
                return null;
            }
        }
        
        public List<string> GetCustomLevels()
        {
            var customLevels = new List<string>();
            
            try
            {
                Console.WriteLine($"=== GetCustomLevels() START ===");
                
                if (!Directory.Exists(LevelsFolder))
                {
                    Console.WriteLine($"ERROR: Levels folder doesn't exist!");
                    return customLevels;
                }
                
                var allFiles = Directory.GetFiles(LevelsFolder, "*.*");
                Console.WriteLine($"All files in Levels folder: {allFiles.Length}");
                foreach (var file in allFiles)
                {
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine($"  File: {fileName}");
                }
                
                var jsonFiles = Directory.GetFiles(LevelsFolder, "*.json");
                Console.WriteLine($"JSON files: {jsonFiles.Length}");
                
                foreach (var file in jsonFiles)
                {
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine($"  JSON: {fileName}");
                    
                    bool isStandardLevel = fileName.StartsWith("level", StringComparison.OrdinalIgnoreCase);
                    
                    if (!isStandardLevel)
                    {
                        customLevels.Add(fileName);
                        Console.WriteLine($"    -> ADDED to custom levels");
                    }
                    else
                    {
                        Console.WriteLine($"    -> SKIPPED (standard level)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetCustomLevels: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            
            Console.WriteLine($"=== GetCustomLevels() END ===");
            Console.WriteLine($"Returning {customLevels.Count} custom levels");
            foreach (var level in customLevels)
            {
                Console.WriteLine($"  - {level}");
            }
            
            return customLevels;
        }
        
        public List<LevelData> GetAllLevels()
        {
            var levels = new List<LevelData>();
            
            try
            {
                if (!Directory.Exists(LevelsFolder))
                {
                    Directory.CreateDirectory(LevelsFolder);
                    CreateDefaultLevels();
                }
                
                var files = Directory.GetFiles(LevelsFolder, "*.json");
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.StartsWith("level") && int.TryParse(
                        Path.GetFileNameWithoutExtension(file).Replace("level", ""), 
                        out int levelNumber))
                    {
                        var level = LoadLevel(levelNumber);
                        if (level != null)
                        {
                            level.Name = $"Уровень {levelNumber}";
                            levels.Add(level);
                        }
                    }
                    else
                    {
                        var level = LoadCustomLevel(fileName);
                        if (level != null)
                        {
                            if (string.IsNullOrEmpty(level.Name))
                                level.Name = Path.GetFileNameWithoutExtension(fileName);
                            levels.Add(level);
                        }
                    }
                }
                
                levels = levels.OrderBy(l => l.Name).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all levels: {ex.Message}");
            }
            
            return levels;
        }
        
        public void SaveCustomLevel(string fileName, LevelData levelData)
        {
            try
            {
                string filePath = Path.Combine(LevelsFolder, fileName);
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true 
                };
                string json = JsonSerializer.Serialize(levelData, options);
                File.WriteAllText(filePath, json);
                Console.WriteLine($"Custom level saved: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving custom level {fileName}: {ex.Message}");
            }
        }
    }
}