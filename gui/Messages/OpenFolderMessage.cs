using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.gui.Messages
{
    public class OpenFolderMessage
    {
        public static void Send(string description, Action<string> callback) {
            Send(string.Empty, Environment.SpecialFolder.MyDocuments, description, false, callback);
        }

        public static void Send(
            string selectedPath, 
            Environment.SpecialFolder rootFolder,
            string description, 
            bool showNextFolder,
            Action<string> callback)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new OpenFolderMessage { 
                    SelectedPath = selectedPath,
                    RootFolder = rootFolder,
                    Description = description, 
                    ShowNextFolder = showNextFolder,
                    Callback = callback });
            });
        }

        public Action<string> Callback { get; set; }

        public string Description { get; set; }

        public Environment.SpecialFolder RootFolder { get; set; }

        public string SelectedPath { get; set; }

        public bool ShowNextFolder { get; set; }
    }
}
