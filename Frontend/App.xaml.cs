using System.Windows;
using Frontend.Script;

namespace Frontend;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    void App_Startup(object sender, StartupEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        
        ProgramInitializer.DirectoryCreator();
    }
}