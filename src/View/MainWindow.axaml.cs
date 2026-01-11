using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace View;  // Keep your namespace

public class MainWindow : Window
{
    public MainWindow()
    {
        // Basic window setup (like JFrame)
        Title = "My Sticky Note Board";
        Width = 1000;
        Height = 700;
        Background = Brushes.DarkGray;  // Or any color

        // Create a main panel (like JPanel with BorderLayout)
        var mainPanel = new DockPanel();

        // Example: Add a top panel (like NORTH)
        var topPanel = new StackPanel
        {
            Background = Brushes.LightBlue,
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 60
        };
        topPanel.Children.Add(new TextBlock
        {
            Text = "My Board App",
            FontSize = 24,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(20)
        });
        topPanel.Children.Add(new Button
        {
            Content = "New Note",
            Margin = new Thickness(10),
            VerticalAlignment = VerticalAlignment.Center
        });

        DockPanel.SetDock(topPanel, Dock.Top);
        mainPanel.Children.Add(topPanel);

        // Center area – a canvas for absolute positioning (like null layout in Swing)
        var boardCanvas = new Canvas
        {
            Background = Brushes.White
        };

        // Example: Add a "sticky note" directly in code
        var stickyNote = new Border
        {
            Background = Brushes.Yellow,
            Width = 200,
            Height = 200,
            CornerRadius = new CornerRadius(10),
            Child = new TextBlock
            {
                Text = "Hello from code!\nThis is a sticky note.",
                Margin = new Thickness(10),
                FontSize = 16
            }
        };

        // Position it absolutely (like setBounds in Swing)
        Canvas.SetLeft(stickyNote, 100);
        Canvas.SetTop(stickyNote, 100);

        boardCanvas.Children.Add(stickyNote);

        mainPanel.Children.Add(boardCanvas);  // Fills the rest

        // Set as window content
        Content = mainPanel;
    }
}
