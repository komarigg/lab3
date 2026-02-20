using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;

namespace BattleCity
{
    public class Tank : GameObject
    {
        public double Speed { get; set; }
        public int Direction { get; set; } = 0; // 0: Up, 1: Down, 2: Left, 3: Right
        public int Lives { get; set; } = 3;
        public bool IsPlayer { get; set; }

        // Свойства для работы AI
        public int AiActionTimer { get; set; } = 0;
        public int AiDirectionChangeCounter { get; set; } = 0;
        public int CurrentAiDirection { get; set; } = 0;

        public Tank(double x, double y, bool isPlayer)
        {
            X = x;
            Y = y;
            IsPlayer = isPlayer;
            Width = 32;
            Height = 32;
            Lives = 3;
            
            // Баланс скорости: игрок чуть маневреннее
            Speed = isPlayer ? 3.5 : 2.5;
        }

        public override void Update() { }

        /// <summary>
        /// Передвигает танк, если путь свободен.
        /// </summary>
        /// <param name="direction">Направление движения</param>
        /// <param name="allObjects">Список всех объектов на карте для проверки коллизий</param>
        public void Move(int direction, List<GameObject> allObjects)
        {
            Direction = direction;
            double nextX = X;
            double nextY = Y;

            // Рассчитываем потенциальную новую позицию
            switch (direction)
            {
                case 0: nextY -= Speed; break; // Вверх
                case 1: nextY += Speed; break; // Вниз
                case 2: nextX -= Speed; break; // Влево
                case 3: nextX += Speed; break; // Вправо
            }

            // ПРОВЕРКА: Можно ли переместиться в эти координаты?
            // Используем метод из CollisionManager для "предсказания" столкновения
            if (CollisionManager.CanMoveTo(this, nextX, nextY, allObjects))
            {
                X = nextX;
                Y = nextY;
            }
            // Если CanMoveTo вернул false, координаты X и Y остаются прежними, 
            // и танк просто стоит на месте, упершись в препятствие.
        }

        public Bullet Shoot()
        {
            double bulletX = X + (Width / 2) - 4;
            double bulletY = Y + (Height / 2) - 4;
            return new Bullet(this, bulletX, bulletY, Direction);
        }

        public override void TakeDamage(int damage)
        {
            Lives -= damage;
            if (Lives <= 0)
            {
                IsDestroyed = true;
                IsActive = false;
            }
        }

        public override void Draw(Canvas canvas)
        {
            DrawTankBody(canvas);
            DrawHealthPoints(canvas);
        }

        private void DrawHealthPoints(Canvas canvas)
        {
            double circleSize = 6;
            double spacing = 3;
            double startX = X + 18; 
            double posY = Y - 12;

            for (int i = 0; i < 3; i++)
            {
                var dot = new Ellipse
                {
                    Width = circleSize,
                    Height = circleSize,
                    Fill = (i < Lives) ? (IsPlayer ? Brushes.Lime : Brushes.Red) : Brushes.Gray,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5
                };

                Canvas.SetLeft(dot, startX + i * (circleSize + spacing));
                Canvas.SetTop(dot, posY);
                canvas.Children.Add(dot);
            }
        }

        private void DrawTankBody(Canvas canvas)
        {
            // 1. Корпус танка
            var rect = new Rectangle
            {
                Width = Width,
                Height = Height,
                Fill = IsPlayer ? Brushes.Green : Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(rect, X);
            Canvas.SetTop(rect, Y);
            canvas.Children.Add(rect);

            // 2. Башенка (Круглая часть в центре)
            var turret = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = IsPlayer ? Brushes.DarkGreen : Brushes.DarkRed,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(turret, X + 6);
            Canvas.SetTop(turret, Y + 6);
            canvas.Children.Add(turret);

            // 3. Дуло танка
            var gun = new Rectangle { Width = 4, Height = 12, Fill = Brushes.Gray };
            double gx = X + 14, gy = Y + 14;

            switch (Direction)
            {
                case 0: gy = Y - 4; break;
                case 1: gy = Y + 24; break;
                case 2: gx = X - 4; gun.Width = 12; gun.Height = 4; gy = Y + 14; break;
                case 3: gx = X + 24; gun.Width = 12; gun.Height = 4; gy = Y + 14; break;
            }

            Canvas.SetLeft(gun, gx);
            Canvas.SetTop(gun, gy);
            canvas.Children.Add(gun);
        }
    }
}