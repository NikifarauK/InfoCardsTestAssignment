using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get => title = "Инфокарты";
            set => SetProperty(ref title, value);
        }

        private string enter;
        public string Enter
        {
            get => enter;
            set {
                if (SetProperty(ref enter, value)) 
                    Title = value; 
            }
        }

    }
}
