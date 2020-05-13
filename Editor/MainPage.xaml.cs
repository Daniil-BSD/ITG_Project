using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ITG_Editor.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ITG_Editor {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	///

	public sealed partial class MainPage : Page {

		private NotifyPropertyChangedWrapper<SoftwareBitmapSource> ImgSource => Editor.SoftwareBitmapSource;

		public ObservableCollection<BuilderModel> BuilderModels => Editor.BuilderModels;

		public ITG_Editor.ViewModels.Editor Editor { get; private set; }

		public NotifyPropertyChangedWrapper<string> Output => Editor.Output;

		public List<string> PossibleBuildersNames => Editor.PossibleBuildersNames;

		public List<string> PossibleTypeParametersNames => Editor.PossibleTypeParametersNames;

		public MainPage()
		{
			Editor = new ITG_Editor.ViewModels.Editor();
			InitializeComponent();
		}

		private void AddBuilder_Click(object sender, RoutedEventArgs e)
		{
			Editor.Add(BuildersComboBox.SelectedIndex, TypeParameterComboBox.SelectedIndex);
		}

		private async void CaptureButton_Click(object sender, RoutedEventArgs e)
		{
			await Editor.CaptureDialog();
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if ( BuilderModelsList.SelectedItem != null ) {
				Editor.Delete(BuilderModelsList.SelectedIndex);
			}
		}

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			await Editor.LoadDialog();
		}

		private async void RunButton_Click(object sender, RoutedEventArgs e)
		{
			await Task.Run(Editor.Run);
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			await Editor.SaveDialog();
		}
	}
}