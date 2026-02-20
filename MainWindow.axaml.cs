using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;

namespace BattleCity
{
    public partial class MainWindow : Window
    {
        private Game? game;
        private Timer? gameTimer;
        private HashSet<Key> pressedKeys = new HashSet<Key>();
        private Canvas? canvas;
        private int level;
        private string? customLevelFileName;
        private bool isPaused = false;
        private Grid? pauseMenu;
        
        public MainWindow(int level = 1)
        {
            this.level = level;
            customLevelFileName = null;
            InitializeWindow();
        }
        
        public MainWindow(string fileName)
        {
            this.level = 0;
            customLevelFileName = fileName;
            InitializeWindow();
        }
        
        private void InitializeWindow()
        {
            InitializeComponent();
            
            Width = 13 * 32 + 16;
            Height = 13 * 32 + 39;
            
            Title = customLevelFileName != null ? $"Battle City - {customLevelFileName}" : $"Battle City - Уровень {level}";
            
            var mainGrid = new Grid();
            canvas = new Canvas { Width = 13 * 32, Height = 13 * 32, Background = Brushes.Black };
            
            pauseMenu = CreatePauseMenu();
            pauseMenu.IsVisible = false;
            
            mainGrid.Children.Add(canvas);
            mainGrid.Children.Add(pauseMenu);
            Content = mainGrid;
            
            game = new Game();
            if (customLevelFileName != null) game.StartCustomLevel(customLevelFileName);
            else game.Start(level);
            
            gameTimer = new Timer(16); 
            gameTimer.Elapsed += (s, e) => UpdateGame();
            gameTimer.Start();
            
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            
            RenderGame();
        }

        // Вспомогательный метод для создания красивых кнопок
        private Button CreateStyledButton(string text, IBrush background, IBrush border)
        {
            return new Button
            {
                Content = text,
                Width = 220,
                Height = 45,
                FontSize = 18,
                FontWeight = FontWeight.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = background,
                Foreground = Brushes.White,
                BorderBrush = border,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                Cursor = new Cursor(StandardCursorType.Hand)
            };
        }

        private Grid CreatePauseMenu()
        {
            var grid = new Grid { Width = 13 * 32, Height = 13 * 32, Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)) };
            var panel = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Spacing = 20 };
            
            var title = new TextBlock { Text = "ПАУЗА", FontSize = 32, FontWeight = FontWeight.ExtraBold, Foreground = Brushes.Yellow, HorizontalAlignment = HorizontalAlignment.Center };
            
            var resumeButton = CreateStyledButton("ПРОДОЛЖИТЬ", Brushes.DarkGreen, Brushes.Lime);
            var restartButton = CreateStyledButton("ЗАНОВО", Brushes.DarkBlue, Brushes.LightBlue);
            var menuButton = CreateStyledButton("ГЛАВНОЕ МЕНЮ", Brushes.DarkRed, Brushes.Red);
            
            resumeButton.Click += (s, e) => TogglePause();
            restartButton.Click += (s, e) => RestartGame();
            menuButton.Click += (s, e) => ReturnToMenu();
            
            panel.Children.Add(title);
            panel.Children.Add(resumeButton);
            panel.Children.Add(restartButton);
            panel.Children.Add(menuButton);
            
            grid.Children.Add(panel);
            return grid;
        }

        private Grid CreateGameOverMenu(bool isWin)
        {
            var grid = new Grid { Width = 13 * 32, Height = 13 * 32, Background = new SolidColorBrush(Color.FromArgb(210, 0, 0, 0)) };
            var panel = new StackPanel { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Spacing = 20 };
            
            var title = new TextBlock 
            { 
                Text = isWin ? "ПОБЕДА!" : "ВЫ ПРОИГРАЛИ", 
                FontSize = 38, 
                FontWeight = FontWeight.ExtraBold, 
                Foreground = isWin ? Brushes.Lime : Brushes.Red, 
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            };
            
            var restartButton = CreateStyledButton("ЗАНОВО", Brushes.DarkBlue, Brushes.LightBlue);
            var menuButton = CreateStyledButton("ГЛАВНОЕ МЕНЮ", Brushes.DarkRed, Brushes.Red);
            
            restartButton.Click += (s, e) => RestartGame();
            menuButton.Click += (s, e) => ReturnToMenu();
            
            panel.Children.Add(title);
            panel.Children.Add(restartButton);
            panel.Children.Add(menuButton);
            
            grid.Children.Add(panel);
            return grid;
        }

        private void TogglePause()
        {
            isPaused = !isPaused;
            if (pauseMenu != null) pauseMenu.IsVisible = isPaused;
            if (isPaused) { gameTimer?.Stop(); pressedKeys.Clear(); }
            else gameTimer?.Start();
        }

        private void RestartGame()
        {
            var nextWindow = customLevelFileName != null ? new MainWindow(customLevelFileName) : new MainWindow(level);
            nextWindow.Show();
            this.Close();
        }

        private void ReturnToMenu() { new MainMenu().Show(); this.Close(); }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { TogglePause(); return; }
            if (!isPaused) pressedKeys.Add(e.Key);
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Key);
        }

        private void UpdateGame()
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (game == null || isPaused) return;

                if (game.IsGameOver || game.IsLevelComplete)
                {
                    gameTimer?.Stop();
                    ShowGameOverMenu(game.IsLevelComplete);
                    return;
                }

                // КЛАССИЧЕСКОЕ ДВИЖЕНИЕ (else if исключает диагонали)
                if (pressedKeys.Contains(Key.Up)) game.HandlePlayerInput(0);
                else if (pressedKeys.Contains(Key.Down)) game.HandlePlayerInput(1);
                else if (pressedKeys.Contains(Key.Left)) game.HandlePlayerInput(2);
                else if (pressedKeys.Contains(Key.Right)) game.HandlePlayerInput(3);
                
                // Стрельба независима от движения
                if (pressedKeys.Contains(Key.Space)) game.HandlePlayerShoot();

                game.Update();
                RenderGame();
            });
        }

        private void RenderGame()
        {
            canvas?.Children.Clear();
            game?.Draw(canvas!);
        }

        private void ShowGameOverMenu(bool isWin)
        {
            var gameOverMenu = CreateGameOverMenu(isWin);
            var mainGrid = Content as Grid;
            if (mainGrid != null)
            {
                if (pauseMenu != null) mainGrid.Children.Remove(pauseMenu);
                mainGrid.Children.Add(gameOverMenu);
            }
        }
    }
}