using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.gui.Messages
{
    public class SystemMessage
    {
        public string Message { get; set; }

        public override string ToString()
        {
            return this.Message;
        }

        public static void Send(string message, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   Messenger.Default.Send(new SystemMessage { Message = string.Format(message, args) });
               });
        }
    }
}
