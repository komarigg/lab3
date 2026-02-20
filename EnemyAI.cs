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

            // 1. ЛОГИКА ДВИЖЕНИЯ
            enemy.AiActionTimer++;

            // Если пришло время сменить направление или если танк застрял
            if (enemy.AiActionTimer >= 30 + random.Next(30))
            {
                // Запоминаем текущую позицию перед попыткой хода
                double oldX = enemy.X;
                double oldY = enemy.Y;

                // Пробуем двигаться в текущем направлении
                enemy.Move(enemy.CurrentAiDirection, map.Objects);

                // Если позиция не изменилась (значит, уперлись в препятствие)
                if (Math.Abs(enemy.X - oldX) < 0.1 && Math.Abs(enemy.Y - oldY) < 0.1)
                {
                    enemy.AiDirectionChangeCounter++;
                }

                // Смена направления по таймеру ИЛИ если застряли (после нескольких неудачных попыток)
                if (enemy.AiDirectionChangeCounter > 5 || random.Next(100) < 5)
                {
                    enemy.CurrentAiDirection = random.Next(4);
                    enemy.AiDirectionChangeCounter = 0;
                }

                enemy.AiActionTimer = 0;
            }
            else
            {
                // Обычный ход в текущем направлении
                enemy.Move(enemy.CurrentAiDirection, map.Objects);
            }

            // 2. ЛОГИКА СТРЕЛЬБЫ
            // Враги стреляют случайно, но не слишком часто
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