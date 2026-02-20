using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace BattleCity
{
    public static class CollisionManager
    {
        private const double MapSize = 13 * 32;

        public static void HandleBulletCollisions(List<GameObject> objects)
        {
            var bullets = objects.OfType<Bullet>().Where(b => !b.IsDestroyed).ToList();
            var targets = objects.Where(obj => !(obj is Bullet) && !obj.IsDestroyed).ToList();

            foreach (var bullet in bullets)
            {
                foreach (var target in targets)
                {
                    if (target == bullet.Owner) continue;
                    
                    if (bullet.Owner is Tank bOwner && target is Tank bTarget)
                        if (!bOwner.IsPlayer && !bTarget.IsPlayer) continue;

                    if (bullet.Bounds.Intersects(target.Bounds))
                    {
                        bullet.Destroy();
                        target.TakeDamage(1);
                        break;
                    }
                }
            }
        }

        public static bool CanMoveTo(Tank tank, double newX, double newY, List<GameObject> allObjects)
        {
            // 1. Проверка границ карты (используем полные визуальные границы для безопасности)
            if (newX < 0 || newX + tank.Width > MapSize || newY < 0 || newY + tank.Height > MapSize)
                return false;

            // 2. СОЗДАЕМ УМЕНЬШЕННЫЙ ХИТБОКС ДЛЯ ПРЕДСКАЗАНИЯ
            // Мы берем X+2 и Y+2, а размер уменьшаем на 4 (по 2 пикселя с каждой стороны)
            // Это в точности повторяет логику Bounds из GameObject.cs
            Rect futureBounds = new Rect(newX + 2, newY + 2, tank.Width - 4, tank.Height - 4);

            // 3. Проверка столкновений с объектами
            foreach (var obj in allObjects)
            {
                // Пропускаем самого себя, пули и разрушенные объекты
                if (obj == tank || obj is Bullet || !obj.IsActive || obj.IsDestroyed) continue;

                // Важно: obj.Bounds уже возвращает уменьшенный прямоугольник, 
                // если это танк, или обычный, если это стена.
                if (futureBounds.Intersects(obj.Bounds))
                    return false; 
            }

            return true;
        }
    }
}