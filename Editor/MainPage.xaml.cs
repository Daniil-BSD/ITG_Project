using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Editor.DataModels;
using ITG_Core;
using ITG_Core.Base;
using ITG_Core.Bulders;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Editor {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	///

	public sealed partial class MainPage : Page {

		public ObservableCollection<BuilderModel> BuilderModels { get; private set; }

		public List<Type> PossibleBuilders { get; private set; }

		public List<string> PossibleBuildersNames
		{
			get {
				List<string> ret = new List<string>(PossibleBuilders.Count);
				ret.AddRange(from Type builder in PossibleBuilders select builder.Name.Replace("Builder", "").Replace("`1", " (Generic)"));
				return ret;
			}
		}

		public List<Type> PossibleTypeParameters { get; private set; }

		private SoftwareBitmap LatestBitmap { get; set; }

		public List<string> PossibleTypeParametersNames
		{
			get {
				List<string> ret = new List<string>(PossibleBuilders.Count);
				ret.AddRange(from Type builder in PossibleTypeParameters select builder.Name);
				return ret;
			}
		}

		public MainPage()
		{
			IEnumerable<Type> Alltypes = (
				from assembly in AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in assembly.GetTypes()
				select assemblyType
				).Union(
				from assemblyType in Assembly.LoadFrom("ITG_Core.dll").GetTypes()
				select assemblyType);
			IEnumerable<Type> possibleBuilders = (
				from assemblyType in Alltypes
				where typeof(IAlgorithmBuilder).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
				orderby assemblyType.Name
				select assemblyType );
			IEnumerable<Type> possibleTypeParameters = (
				from assemblyType in Alltypes
				where assemblyType.IsValueType && !assemblyType.IsGenericType && assemblyType.IsPrimitive
				orderby assemblyType.Name
				select assemblyType ).Union(
				from assemblyType in Assembly.LoadFrom("ITG_Core.dll").GetTypes()
				where !assemblyType.IsEnum && assemblyType.IsValueType && !assemblyType.IsGenericType
				select assemblyType);

			PossibleBuilders = new List<Type>(possibleBuilders);
			PossibleTypeParameters = new List<Type>(possibleTypeParameters);
			BuilderModels = new ObservableCollection<BuilderModel>();
			InitializeComponent();
		}

		private async void CaptureButton_Click(object sender, RoutedEventArgs e)
		{
			if ( LatestBitmap != null ) {
				FileSavePicker fileSavePicker = new FileSavePicker {
					SuggestedStartLocation = PickerLocationId.PicturesLibrary
				};
				fileSavePicker.FileTypeChoices.Add("PNG files", new List<string>() { ".png" });
				fileSavePicker.SuggestedFileName = "snapshot";

				Windows.Storage.StorageFile outputFile = await fileSavePicker.PickSaveFileAsync();

				if ( outputFile != null ) {
					using ( IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite) ) {
						// Create an encoder with the desired format
						BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
						encoder.SetSoftwareBitmap(LatestBitmap);
						try {
							await encoder.FlushAsync();
						} catch ( Exception err ) {
							output.Text = "Failed to save the output." + "\n" + err.Message;
						}
					}
				} else {
					//cancelled
				}
			}
		}
		private async void RunButton_Click(object sender, RoutedEventArgs e)
		{
			const string nln = "\n";
			LandscapeBuilder landscapeBuilder = BuildLandscapeBuilder();
			output.Text = string.Join(nln, landscapeBuilder.GetFullReport());
			if ( landscapeBuilder.IsValid() ) {
				output.Text += nln + "===  VALID  ===";
				Landscape landscape = landscapeBuilder.Build();
				try {
					Outputter<SoftwareBitmap> outputter = landscape.GetOutputter<SoftwareBitmap>("img");
					output.Text += nln + "Generating the image";
					SoftwareBitmap softwareBitmap = outputter.GetObject(new Coordinate());
					if ( softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight ) {
						softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
					}
					LatestBitmap = softwareBitmap;
					SoftwareBitmapSource source = new SoftwareBitmapSource();
					await source.SetBitmapAsync(softwareBitmap);

					preview.Source = source;
					output.Text += nln + "Preview is ready";
				} catch ( Exception ) {
					output.Text += nln + "Failed to retrieve the Bitmap Outputter."
						+ nln + "To enable the preview add a HeightMapImageOutputterBuilder with id of \"img\"";
				}
			} else {
				output.Text += nln + "=== INVALID ===";
			}
		}

		private async void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if ( BuilderModelsList.SelectedItem != null ) {
				BuilderModels.Remove(BuilderModels[BuilderModelsList.SelectedIndex]);
			}
		}
		private async void AddBuilder_Click(object sender, RoutedEventArgs e)
		{
			ComboBox buildersComboBox = (ComboBox)FindName("BuildersComboBox");
			Type builderType = PossibleBuilders[buildersComboBox.SelectedIndex];
			Type specificType = builderType;
			if ( builderType.IsGenericType ) {
				ComboBox paramComboBox = (ComboBox)FindName("TypeParameterComboBox");
				specificType = builderType.MakeGenericType(new Type[] { PossibleTypeParameters[paramComboBox.SelectedIndex] });
			}
			BuilderModels.Add(new BuilderModel((IAlgorithmBuilder)Activator.CreateInstance(specificType)));
		}

		private LandscapeBuilder BuildLandscapeBuilder()
		{
			LandscapeBuilder landscapeBuilder = new LandscapeBuilder();
			foreach ( BuilderModel builderModel in BuilderModels ) {
				if ( builderModel.ID == null )
					continue;
				landscapeBuilder[builderModel.ID] = builderModel.Builder;
			}
			return landscapeBuilder;
		}

		private async void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			FileOpenPicker picker = new FileOpenPicker {
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			picker.FileTypeFilter.Add(".xml");

			StorageFile file = await picker.PickSingleFileAsync();
			if ( file != null ) {
				string xml = await FileIO.ReadTextAsync(file);
				//loading
				try {
					LandscapeBuilder lb = new LandscapeBuilder {
						XML = xml
					};
					BuilderModels.Clear();
					foreach ( string key in lb.Builders.Keys ) {
						BuilderModel model = new BuilderModel(lb.Builders[key]) {
							ID = key
						};
						BuilderModels.Add(model);
					}
					output.Text = "File was loaded sucessfully.";
				} catch ( Exception ) {
					output.Text = "Unable to deserialize the file.";
				} finally { }
			} else {
				//cancelled
			}
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			LandscapeBuilder landscapeBuilder = BuildLandscapeBuilder();
			if ( landscapeBuilder.IsValid() ) {
				FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker {
					SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
				};
				savePicker.FileTypeChoices.Add("Landscape XML", new List<string>() { ".xml" });
				savePicker.SuggestedFileName = "New Landscape";
				Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
				if ( file != null ) {
					Windows.Storage.CachedFileManager.DeferUpdates(file);
					await Windows.Storage.FileIO.WriteTextAsync(file, landscapeBuilder.XML);
					Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
					if ( status == Windows.Storage.Provider.FileUpdateStatus.Complete ) {
						output.Text = "File was saved sucessfully.";
					} else {
					}
				} else {
				}
			} else {
				RunButton_Click(sender, e);
			}
		}
	}
}