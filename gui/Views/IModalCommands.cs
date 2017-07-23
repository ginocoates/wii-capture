using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace wiicapture.gui.Views
{
    public interface IModalCommands
    {
        RelayCommand OKCommand { get; }
        RelayCommand CancelCommand { get; }
    }
}
