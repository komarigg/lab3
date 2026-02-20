using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace BattleCity
{
    public partial class GameOverWindow : Window
    {
        private int currentLevel;
        private string customLevelFileName;
        private bool isWin;
        private string levelName;
        
        public GameOverWindow(int level, string customLevelFileName, bool win, string levelName)
        {
            InitializeComponent();
            currentLevel = level;
            this.customLevelFileName = customLevelFileName;
            isWin = win;
            this.levelName = levelName;
            
            UpdateUI();
            AttachEventHandlers();
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void UpdateUI()
        {
            var resultText = this.FindControl<TextBlock>("ResultText");
            if (resultText != null)
            {
                if (isWin)
                {
                    resultText.Text = $"{levelName} пройден!";
                    resultText.Foreground = Avalonia.Media.Brushes.Green;
                    this.Title = "Победа!";
                }
                else
                {
                    resultText.Text = $"Вы проиграли на {levelName}";
                    resultText.Foreground = Avalonia.Media.Brushes.Red;
                    this.Title = "Game Over";
                }
            }
        }
        
        private void AttachEventHandlers()
        {
            var restartButton = this.FindControl<Button>("RestartButton");
            var menuButton = this.FindControl<Button>("MenuButton");
            var exitButton = this.FindControl<Button>("ExitButton");
            
            if (restartButton != null) restartButton.Click += RestartButton_Click;
            if (menuButton != null) menuButton.Click += MenuButton_Click;
            if (exitButton != null) exitButton.Click += ExitButton_Click;
        }
        
        private void RestartButton_Click(object? sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(customLevelFileName))
            {
                var gameWindow = new MainWindow(customLevelFileName);
                gameWindow.Show();
            }
            else
            {
                var gameWindow = new MainWindow(currentLevel);
                gameWindow.Show();
            }
            this.Close();
        }
        
        private void MenuButton_Click(object? sender, RoutedEventArgs e)
        {
            var mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }
        
        private void ExitButton_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
            
            if (Application.Current.ApplicationLifetime is 
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
}