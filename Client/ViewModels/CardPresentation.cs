using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.ViewModels
{
    internal class CardPresentation : ViewModelBase
    {
        public delegate void CardSelectedEventHandler(object sender, EventArgs e);
        public static event CardSelectedEventHandler CardSelected;

        public int Id { get; set; }

        public string Name { get; set; }

        public BitmapImage Image { get; set; }

        private bool isSelected;
        public bool IsSelected
        { 
            get => isSelected;
            set
            {
                if(SetProperty(ref isSelected, value))
                {
                    CardSelected?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}
