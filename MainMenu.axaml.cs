using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Layout;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BattleCity
{
    public partial class MainMenu : Window
    {
        private readonly LevelManager levelManager;
        
        public MainMenu()
        {
            InitializeComponent();
            levelManager = new LevelManager();
            AttachEventHandlers();
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void AttachEventHandlers()
        {
            var standardButton = this.FindControl<Button>("StandardLevelsButton");
            var customButton = this.FindControl<Button>("CustomLevelsButton");
            var exitButton = this.FindControl<Button>("ExitButton");
            
            if (standardButton != null) standardButton.Click += StandardLevelsButton_Click;
            if (customButton != null) customButton.Click += CustomLevelsButton_Click;
            if (exitButton != null) exitButton.Click += ExitButton_Click;
        }
        
        private async void StandardLevelsButton_Click(object? sender, RoutedEventArgs e)
        {
            var levels = levelManager.GetAllLevels();
            var standardLevels = levels.Where(l => l.Name != null && l.Name.StartsWith("Уровень ")).ToList();
            
            if (standardLevels.Count == 0)
            {
                await ShowMessageDialog("Нет стандартных уровней!", "Создайте стандартные уровни");
                return;
            }
            
            var selectedLevelName = await ShowLevelSelectionDialog(standardLevels);
            
            if (!string.IsNullOrEmpty(selectedLevelName))
            {
                if (selectedLevelName.StartsWith("Уровень "))
                {
                    if (int.TryParse(selectedLevelName.Replace("Уровень ", ""), out int levelNumber))
                    {
                        var gameWindow = new MainWindow(levelNumber);
                        gameWindow.Show();
                        this.Close();
                    }
                }
            }
        }
        
        private async void CustomLevelsButton_Click(object? sender, RoutedEventArgs e)
        {
            var customFiles = levelManager.GetCustomLevels();
            
            if (customFiles.Count == 0)
            {
                await ShowMessageDialog("Нет кастомных уровней!", 
                    "Добавьте JSON файлы в папку Levels/\nПроверьте консоль для отладки.");
                return;
            }
            
            var selectedFile = await ShowCustomLevelsDialog(customFiles);
            
            if (!string.IsNullOrEmpty(selectedFile))
            {
                var gameWindow = new MainWindow(selectedFile);
                gameWindow.Show();
                this.Close();
            }
        }

        private async Task<string?> ShowCustomLevelsDialog(List<string> files)
        {
            var dialog = new Window
            {
                Title = "Выберите уровень",
                Width = 500,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = Brushes.Black,
                CanResize = false
            };

            string? selectedFile = null;
            bool isLoaded = false;
            var mainPanel = new StackPanel { Margin = new Thickness(20), Spacing = 15 };
            
            mainPanel.Children.Add(new TextBlock { Text = "Кастомные уровни", FontSize = 20, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White });

            var itemsPanel = new StackPanel { Spacing = 5 };
            var itemGroup = new List<(Border border, TextBlock textBlock, string fileName)>();
            int selectedIndex = 0;

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var textBlock = new TextBlock { Text = file, Foreground = Brushes.White, FontSize = 14, TextWrapping = TextWrapping.Wrap };
                var border = new Border { Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Padding = new Thickness(12), Child = textBlock };
                if (i == 0) { border.Background = new SolidColorBrush(Color.FromRgb(80, 80, 120)); textBlock.Foreground = Brushes.Yellow; }
                
                int index = i;
                border.PointerPressed += (s, e) => {
                    selectedIndex = index;
                    foreach (var item in itemGroup) { item.border.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)); item.textBlock.Foreground = Brushes.White; }
                    border.Background = new SolidColorBrush(Color.FromRgb(80, 80, 120));
                    textBlock.Foreground = Brushes.Yellow;
                };
                itemsPanel.Children.Add(border);
                itemGroup.Add((border, textBlock, file));
            }

            mainPanel.Children.Add(new ScrollViewer { Content = itemsPanel, Height = 220 });

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Spacing = 20, Margin = new Thickness(0, 20, 0, 0) };
            
            var loadButton = new Button { 
                Content = "ИГРАТЬ", Width = 140, Height = 45, FontSize = 18, FontWeight = FontWeight.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.DarkGreen, Foreground = Brushes.White, BorderBrush = Brushes.Gold, BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(5), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            
            var cancelButton = new Button { 
                Content = "ОТМЕНА", Width = 140, Height = 45, FontSize = 18,
                HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.DarkRed, Foreground = Brushes.White, BorderBrush = Brushes.Red, BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(5), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            loadButton.Click += (s, args) => { if (selectedIndex >= 0) { selectedFile = itemGroup[selectedIndex].fileName; isLoaded = true; dialog.Close(); } };
            cancelButton.Click += (s, args) => { isLoaded = false; dialog.Close(); };

            buttonPanel.Children.Add(loadButton);
            buttonPanel.Children.Add(cancelButton);
            mainPanel.Children.Add(buttonPanel);
            dialog.Content = mainPanel;
            await dialog.ShowDialog(this);
            return isLoaded ? selectedFile : null;
        }

        private async Task<string?> ShowLevelSelectionDialog(List<LevelData> levels)
        {
            var dialog = new Window
            {
                Title = "Стандартные уровни",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = Brushes.Black,
                CanResize = false
            };

            string? selectedLevelName = null;
            bool isLoaded = false;
            var mainPanel = new StackPanel { Margin = new Thickness(20), Spacing = 15 };
            mainPanel.Children.Add(new TextBlock { Text = "Стандартные уровни", FontSize = 24, FontWeight = FontWeight.Bold, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White });

            var itemsPanel = new StackPanel { Spacing = 5 };
            var itemGroup = new List<(Border border, TextBlock textBlock, string levelName)>();
            int selectedIndex = 0;

            var levelNames = levels.Where(l => l.Name != null).Select(l => l.Name!).ToList();
            for (int i = 0; i < levelNames.Count; i++)
            {
                var name = levelNames[i];
                var textBlock = new TextBlock { Text = name, Foreground = Brushes.White, FontSize = 14 };
                var border = new Border { Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Padding = new Thickness(12), Child = textBlock };
                if (i == 0) { border.Background = new SolidColorBrush(Color.FromRgb(80, 80, 120)); textBlock.Foreground = Brushes.Yellow; }
                
                int index = i;
                border.PointerPressed += (s, e) => {
                    selectedIndex = index;
                    foreach (var item in itemGroup) { item.border.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)); item.textBlock.Foreground = Brushes.White; }
                    border.Background = new SolidColorBrush(Color.FromRgb(80, 80, 120));
                    textBlock.Foreground = Brushes.Yellow;
                };
                itemsPanel.Children.Add(border);
                itemGroup.Add((border, textBlock, name));
            }

            mainPanel.Children.Add(new ScrollViewer { Content = itemsPanel, Height = 180 });

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Spacing = 20, Margin = new Thickness(0, 20, 0, 0) };
            
            var loadButton = new Button { 
                Content = "ИГРАТЬ", Width = 140, Height = 45, FontSize = 18, FontWeight = FontWeight.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center, 
                VerticalContentAlignment = VerticalAlignment.Center,   
                Background = Brushes.DarkGreen, Foreground = Brushes.White, BorderBrush = Brushes.Gold, BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(5), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            var cancelButton = new Button { 
                Content = "ОТМЕНА", Width = 140, Height = 45, FontSize = 18,
                HorizontalContentAlignment = HorizontalAlignment.Center, 
                VerticalContentAlignment = VerticalAlignment.Center,   
                Background = Brushes.DarkRed, Foreground = Brushes.White, BorderBrush = Brushes.Red, BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(5), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            loadButton.Click += (s, args) => { if (selectedIndex >= 0) { selectedLevelName = itemGroup[selectedIndex].levelName; isLoaded = true; dialog.Close(); } };
            cancelButton.Click += (s, args) => { isLoaded = false; dialog.Close(); };

            buttonPanel.Children.Add(loadButton);
            buttonPanel.Children.Add(cancelButton);
            mainPanel.Children.Add(buttonPanel);
            dialog.Content = mainPanel;
            await dialog.ShowDialog(this);
            return isLoaded ? selectedLevelName : null;
        }
        
        private async Task ShowMessageDialog(string title, string message)
        {
            var dialog = new Window { Title = title, Width = 400, Height = 200, WindowStartupLocation = WindowStartupLocation.CenterOwner, Background = Brushes.Black };
            var panel = new StackPanel { Margin = new Thickness(20), Spacing = 10 };
            panel.Children.Add(new TextBlock { Text = message, HorizontalAlignment = HorizontalAlignment.Center, TextWrapping = TextWrapping.Wrap, FontSize = 14, Foreground = Brushes.White });
            
            var okButton = new Button { 
                Content = "OK", Width = 100, Height = 35, 
                HorizontalContentAlignment = HorizontalAlignment.Center, 
                VerticalContentAlignment = VerticalAlignment.Center,   
                HorizontalAlignment = HorizontalAlignment.Center, Background = Brushes.DarkGreen, Foreground = Brushes.White, BorderBrush = Brushes.Gold, BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(3), Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            
            okButton.Click += (s, args) => dialog.Close();
            panel.Children.Add(okButton);
            dialog.Content = panel;
            await dialog.ShowDialog(this);
        }
        
        private void ExitButton_Click(object? sender, RoutedEventArgs e) => this.Close();
    }
}