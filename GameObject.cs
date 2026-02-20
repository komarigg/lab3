using Avalonia;
using Avalonia.Controls;

namespace BattleCity
{
    public abstract class GameObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; } = 32;
        public double Height { get; set; } = 32;
        public bool IsActive { get; set; } = true;
        public bool IsDestroyed { get; set; }

        // ОБНОВЛЕННОЕ СВОЙСТВО Bounds
        // Теперь хитбокс на 2 пикселя меньше с каждой стороны (32 - 4 = 28)
        // Это позволит проезжать в проемы шириной ровно в 1 блок.
        public virtual Rect Bounds => new Rect(X + 2, Y + 2, Width - 4, Height - 4);

        public virtual void TakeDamage(int damage = 1)
        {
            IsDestroyed = true;
            IsActive = false;
        }

        public abstract void Update();
        public abstract void Draw(Canvas canvas);
    }
}