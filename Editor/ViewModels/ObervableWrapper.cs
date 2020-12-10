using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ITG_Editor.ViewModels {

	/// <summary>
	/// A costume wrapper for making any object use INotifyPropertyChanged.
	/// Also provides the ability for value editing from outside of the UI thread.
	/// </summary>
	/// <typeparam name="T">The Class that is to be wrapped</typeparam>
	public class NotifyPropertyChangedWrapper<T> : INotifyPropertyChanged {

		private bool changed = false;

		private T value;

		public T Value => value;

		public event PropertyChangedEventHandler PropertyChanged;

		public async Task SetAsync(T newValue)
		{
			value = newValue;
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
				if ( PropertyChanged != null )
					PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}).AsTask();
		}

		public void SetFromUIThread(T newValue)
		{
			value = newValue;
			if ( PropertyChanged != null )
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
		}
	}
}