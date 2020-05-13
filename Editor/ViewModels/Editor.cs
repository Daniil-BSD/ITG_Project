using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Extention;
using ITG_Core;
using ITG_Core.Bulders;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ITG_Editor.ViewModels {

	public class Editor {

		private SoftwareBitmap LatestBitmap { get; set; }

		public ObservableCollection<BuilderModel> BuilderModels { get; private set; }

		public NotifyPropertyChangedWrapper<string> Output { get; private set; }

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

		public List<string> PossibleTypeParametersNames
		{
			get {
				List<string> ret = new List<string>(PossibleBuilders.Count);
				ret.AddRange(from Type builder in PossibleTypeParameters select builder.Name);
				return ret;
			}
		}

		public NotifyPropertyChangedWrapper<SoftwareBitmapSource> SoftwareBitmapSource { get; private set; }

		public Editor()
		{
			//loading the types
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
			Output = new NotifyPropertyChangedWrapper<string>();
			SoftwareBitmapSource = new NotifyPropertyChangedWrapper<SoftwareBitmapSource>();
		}

		/// <summary>
		/// Adds a BuilderModel based on parameters.
		/// </summary>
		/// <param name="builderIndex"> Indesx of the selected type in PossibleBuilders.</param>
		/// <param name="typeParameterIndex">Indesx of the selected type in PossibleTypeParameters.</param>
		public void Add(int builderIndex, int typeParameterIndex)
		{
			Type builderType = PossibleBuilders[builderIndex];
			Type specificType = builderType;
			if ( builderType.IsGenericType ) {
				specificType = builderType.MakeGenericType(new Type[] { PossibleTypeParameters[typeParameterIndex] });
			}
			BuilderModels.Add(new BuilderModel((IAlgorithmBuilder)Activator.CreateInstance(specificType)));
		}

		/// <summary>
		/// An anti-code-dublication methood
		/// </summary>
		/// <returns>builder based on the Builder collection</returns>
		public LandscapeBuilder BuildLandscapeBuilder()
		{
			LandscapeBuilder landscapeBuilder = new LandscapeBuilder();
			foreach ( BuilderModel builderModel in BuilderModels ) {
				if ( builderModel.ID == null )
					continue;
				landscapeBuilder[builderModel.ID] = builderModel.Builder;
			}
			return landscapeBuilder;
		}

		/// <summary>
		/// Saves the latest output as an image.
		/// </summary>
		/// <param name="outputFile">the file where the data will be written.</param>
		public async Task Capture(StorageFile outputFile)
		{
			if ( outputFile != null ) {
				using ( IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite) ) {
					BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
					encoder.SetSoftwareBitmap(LatestBitmap);
					try {
						await encoder.FlushAsync();
					} catch ( Exception err ) {
						await Output.SetAsync("Failed to save the output." + "\n" + err.Message);
					}
				}
			} else {
				//cancelled
			}
		}

		/// <summary>
		/// Creates the Dialoog for saving the latest output as an image. (Must run on a UI thread)
		/// </summary>
		public async Task CaptureDialog()
		{
			if ( LatestBitmap != null ) {
				FileSavePicker fileSavePicker = new FileSavePicker {
					SuggestedStartLocation = PickerLocationId.PicturesLibrary
				};
				fileSavePicker.FileTypeChoices.Add("PNG files", new List<string>() { ".png" });
				fileSavePicker.SuggestedFileName = "snapshot";

				StorageFile outputFile = await fileSavePicker.PickSaveFileAsync();
				await Capture(outputFile);
			}
		}

		public void Delete(int index)
		{
			BuilderModels.Remove(BuilderModels[index]);
		}

		/// <summary>
		/// Loads landscape from a file.
		/// </summary>
		/// <param name="file"> the file to be read.</param>
		/// <returns></returns>
		public async Task Load(StorageFile file)
		{
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
					await Output.SetAsync("File was loaded sucessfully.");
				} catch ( Exception ) {
					await Output.SetAsync("Unable to deserialize the file.");
				} finally { }
			} else {
				//cancelled
			}
		}

		/// <summary>
		/// Creates the Dialoog for Loading a landscape from a file. (Must run on a UI thread)
		/// </summary>
		public async Task LoadDialog()
		{
			FileOpenPicker picker = new FileOpenPicker {
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			picker.FileTypeFilter.Add(".xml");

			StorageFile file = await picker.PickSingleFileAsync();

			await Load(file);
		}

		/// <summary>
		/// Evalutaes the builder.
		/// If invalid, lissts the problems.
		/// If valid, builds a landscape and generates the output (if the layer is present)
		/// </summary>
		public async Task Run()
		{
			const string nln = "\n";
			LandscapeBuilder landscapeBuilder = BuildLandscapeBuilder();
			await Output.SetAsync(string.Join(nln, landscapeBuilder.GetFullReport()));
			if ( landscapeBuilder.IsValid() ) {
				await Output.SetAsync(Output.Value + nln + "===  VALID  ===");
				Landscape landscape = landscapeBuilder.Build();
				try {
					HeightMapImageOutputter outputter = (HeightMapImageOutputter)landscape.GetOutputter<byte[]>("img");

					await Output.SetAsync(Output.Value + nln + "Generating the landscape.");
					byte[] bytes = outputter.GetObject(new Coordinate());
					await Output.SetAsync(Output.Value + nln + "Generating the image.");
					await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => {
						LatestBitmap = outputter.BytesToBitmap(bytes);
						if ( LatestBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || LatestBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight ) {
							LatestBitmap = SoftwareBitmap.Convert(LatestBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
						}
						SoftwareBitmapSource source = new SoftwareBitmapSource();
						await source.SetBitmapAsync(LatestBitmap);
						SoftwareBitmapSource.SetFromUIThread(source);
					}).AsTask();
					await Output.SetAsync(Output.Value + nln + "Preview is ready,");
				} catch ( Exception err ) {
					await Output.SetAsync(Output.Value + nln + "Failed to retrieve the Bitmap Outputter."
						+ nln + "To enable the preview add a HeightMapImageOutputterBuilder with id of \"img\"" + nln + err.Message);
				}
			} else {
				await Output.SetAsync(Output.Value + nln + "=== INVALID ===");
			}
		}

		/// <summary>
		/// Saves the ladscape into the file.
		/// </summary>
		/// <param name="file"> the file in which it will be saved.</param>
		/// <param name="landscapeBuilder">the landscapeBuilder that is to be saved.</param>
		public async Task Save(StorageFile file, LandscapeBuilder landscapeBuilder)
		{
			if ( file != null ) {
				CachedFileManager.DeferUpdates(file);
				await FileIO.WriteTextAsync(file, landscapeBuilder.XML);
				Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
				if ( status == Windows.Storage.Provider.FileUpdateStatus.Complete ) {
					await Output.SetAsync("File was saved sucessfully.");
				} else {
				}
			} else {
			}
		}

		/// <summary>
		/// Creates a save dialog and saves the landscape into the file
		/// </summary>
		public async Task SaveDialog()
		{
			LandscapeBuilder landscapeBuilder = BuildLandscapeBuilder();
			if ( landscapeBuilder.IsValid() ) {
				FileSavePicker savePicker = new FileSavePicker {
					SuggestedStartLocation = PickerLocationId.DocumentsLibrary
				};
				savePicker.FileTypeChoices.Add("Landscape XML", new List<string>() { ".xml" });
				savePicker.SuggestedFileName = "New Landscape";
				StorageFile file = await savePicker.PickSaveFileAsync();
				await Save(file, landscapeBuilder);
			} else {
				await Task.Run(Run);
			}
		}
	}
}