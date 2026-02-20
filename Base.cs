using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace BattleCity
{
    // Класс базы (штаба), который нужно защищать
    public class Base : GameObject, IGameObject, IDamageable
    {
        // Свойство здоровья (Lives из интерфейса IDamageable)
        public int Lives { get; set; } = 5; 
        public int MaxHealth { get; set; } = 5;

        // Переопределяем метод получения урона из базового класса GameObject
        public override void TakeDamage(int damage = 1)
        {
            if (IsDestroyed) return;
            
            Lives -= damage;
            if (Lives < 0) Lives = 0;
            
            if (Lives <= 0)
            {
                IsDestroyed = true;
                IsActive = false; // База уничтожена — игра проиграна
            }
        }

        // Вспомогательный метод для восстановления
        public void Repair(int amount = 1)
        {
            Lives += amount;
            if (Lives > MaxHealth) Lives = MaxHealth;
            IsDestroyed = false;
            IsActive = true;
        }

        public override void Update() { }
        
        public override void Draw(Canvas canvas)
        {
            // Если объект не активен и не разрушен, не рисуем
            if (!IsActive && !IsDestroyed) return;
            
            // Рисуем основной квадрат базы
            var rect = new Avalonia.Controls.Shapes.Rectangle
            {
                Width = Width,
                Height = Height,
                Fill = IsDestroyed ? Brushes.DarkRed : Brushes.Gray,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };
            
            Canvas.SetLeft(rect, X);
            Canvas.SetTop(rect, Y);
            canvas.Children.Add(rect);
            
            // Рисуем букву "B" или "X" (если разрушена)
            var letter = new Avalonia.Controls.TextBlock
            {
                Text = IsDestroyed ? "X" : "B",
                FontSize = 20,
                Foreground = IsDestroyed ? Brushes.Black : Brushes.White,
                FontWeight = FontWeight.Bold
            };
            Canvas.SetLeft(letter, X + Width / 2 - 6);
            Canvas.SetTop(letter, Y + Height / 2 - 10);
            canvas.Children.Add(letter);
            
            // Если база жива, рисуем полоску здоровья (Health Bar)
            if (!IsDestroyed)
            {
                var healthBar = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Spacing = 1
                };
                
                for (int i = 0; i < MaxHealth; i++)
                {
                    var healthSegment = new Avalonia.Controls.Shapes.Rectangle
                    {
                        Width = 6,
                        Height = 8,
                        Fill = i < Lives ? Brushes.LimeGreen : Brushes.DarkRed,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5
                    };
                    healthBar.Children.Add(healthSegment);
                }
                
                // Центрируем полоску здоровья над базой
                Canvas.SetLeft(healthBar, X + Width / 2 - (MaxHealth * 3.5));
                Canvas.SetTop(healthBar, Y - 15);
                canvas.Children.Add(healthBar);
            }
        }
    }
}