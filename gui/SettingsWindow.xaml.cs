using System.Windows;
using System.Windows.Input;
using wiicapture.gui.Views;

namespace wiicapture.gui
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : ModalDialogWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close,
               new ExecutedRoutedEventHandler(delegate (object sender, ExecutedRoutedEventArgs args) { this.Close(); })));
        }

        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }
    }
}
