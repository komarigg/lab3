using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BattleCity
{
    public class Wall : GameObject
    {
        public bool IsBreakable { get; set; } = true;

        public override void Update()
        {
            /* Стены не двигаются */
        }

        public override void TakeDamage(int damage)
        {
            // КЛЮЧЕВОЙ МОМЕНТ: Если IsBreakable = false, игнорируем урон
            if (IsBreakable)
            {
                IsDestroyed = true;
                IsActive = false;
            }
        }

        public override void Draw(Canvas canvas)
        {
            var rect = new Avalonia.Controls.Shapes.Rectangle
            {
                Width = 32,
                Height = 32,
                // Визуально отличаем бетон от кирпича
                Fill = IsBreakable ? Brushes.Brown : Brushes.DarkGray,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(rect, X);
            Canvas.SetTop(rect, Y);
            canvas.Children.Add(rect);
        }
    }
}