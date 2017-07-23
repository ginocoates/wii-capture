using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace wiicapture.gui.Views
{
    public abstract class ModalDialogWindow : Window
    {
        private FrameworkElement titleBar;
        private FrameworkElement okButton;
        private FrameworkElement cancelButton;

        public ModalDialogWindow()
        {
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Loaded += ModalDialogWindow_Loaded;
        }

        void ModalDialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.titleBar = (FrameworkElement)this.Template.FindName("windowchrome", this);

            if (null != this.titleBar)
            {
                this.titleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
            }

            this.okButton = (FrameworkElement)this.Template.FindName("okbutton", this);

            if (null != this.okButton)
            {
                this.okButton.PreviewMouseDown += okButton_PreviewMouseDown;
            }

            this.cancelButton = (FrameworkElement)this.Template.FindName("cancelbutton", this);

            if (null != this.cancelButton)
            {
                this.cancelButton.PreviewMouseDown += cancelButton_MouseDown;
            }
        }

        void okButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is IModalCommands)
            {
                var okayable = (IModalCommands)this.DataContext;

                if (okayable != null)
                {
                    if (okayable.OKCommand.CanExecute(null))
                        okayable.OKCommand.Execute(null);
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        void cancelButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext is IModalCommands)
            {
                var cancellable = (IModalCommands)this.DataContext;

                if (cancellable != null)
                {
                    if (cancellable.CancelCommand.CanExecute(null))
                        cancellable.CancelCommand.Execute(null);
                }
            }

            this.DialogResult = false;
            this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
