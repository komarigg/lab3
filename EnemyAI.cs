using System;
using System.Collections.Generic;

namespace BattleCity
{
    public static class EnemyAI
    {
        private static Random random = new Random();

        public static void UpdateAI(Tank enemy, Map map)
        {
            if (enemy.IsDestroyed) return;
            
            enemy.AiActionTimer++;
            
            if (enemy.AiActionTimer >= 30 + random.Next(30))
            {
                double oldX = enemy.X;
                double oldY = enemy.Y;
                
                enemy.Move(enemy.CurrentAiDirection, map.Objects);
                
                if (Math.Abs(enemy.X - oldX) < 0.1 && Math.Abs(enemy.Y - oldY) < 0.1)
                {
                    enemy.AiDirectionChangeCounter++;
                }
                
                if (enemy.AiDirectionChangeCounter > 5 || random.Next(100) < 5)
                {
                    enemy.CurrentAiDirection = random.Next(4);
                    enemy.AiDirectionChangeCounter = 0;
                }

                enemy.AiActionTimer = 0;
            }
            else
            {
                enemy.Move(enemy.CurrentAiDirection, map.Objects);
            }
            
            if (random.Next(1000) < 15)
            {
                var bullet = enemy.Shoot();
                if (bullet != null)
                {
                    map.Objects.Add(bullet);
                }
            }
        }
    }
}