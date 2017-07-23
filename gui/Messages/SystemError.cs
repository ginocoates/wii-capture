using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wiicapture.gui.Messages
{
    public class SystemError : SystemMessage
    {
        Exception Error { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}", this.Message, this.Error.Message, this.Error.StackTrace);
        }

        public static void Send(string message, Exception error)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new SystemError { Message = message, Error = error });
            });
        }
    }
}
