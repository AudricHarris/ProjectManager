using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace View
{
	public partial class Init : Application
	{
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

		public override void OnFrameworkInitializationCompleted()
    	{
        	if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        	{
            	desktop.MainWindow = new MainWindow();
        	}

        	base.OnFrameworkInitializationCompleted();
    	}
	}
}
