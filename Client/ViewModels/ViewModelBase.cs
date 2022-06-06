using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(
			ref T field,
			T value,
			[CallerMemberName] string propertyName = "",
			Action onChanged = null,
			Func<T, T, bool> validateValue = null)
		{
			//if value didn't change
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;

			//if value changed but didn't validate
			if (validateValue != null && !validateValue(field, value))
				return false;

			field = value;
			onChanged?.Invoke();
			OnPropertyChanged(propertyName);
			return true;
		}

	}
}
