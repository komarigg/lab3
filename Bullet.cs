using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BattleCity
{
    public class Bullet : GameObject
    {
        public GameObject Owner { get; set; } // Добавь это
        public double Speed { get; set; } = 5;
        public int Direction { get; set; } // 0: Up, 1: Down, 2: Left, 3: Right

        public Bullet(GameObject owner, double x, double y, int direction)
        {
            Owner = owner;
            X = x;
            Y = y;
            Direction = direction;
            Width = 8;
            Height = 8;
        }

        public override void Update()
        {
            if (Direction == 0) Y -= Speed;
            if (Direction == 1) Y += Speed;
            if (Direction == 2) X -= Speed;
            if (Direction == 3) X += Speed;

            // Если пуля улетела далеко, помечаем как уничтоженную
            if (X < -100 || X > 2000 || Y < -100 || Y > 2000) Destroy();
        }

        public override void Draw(Canvas canvas)
        {
            var ellipse = new Avalonia.Controls.Shapes.Ellipse
            {
                Width = Width,
                Height = Height,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(ellipse, X);
            Canvas.SetTop(ellipse, Y);
            canvas.Children.Add(ellipse);
        }

        // Добавь этот метод
        public void Destroy()
        {
            IsDestroyed = true;
            IsActive = false;
        }
    }
}