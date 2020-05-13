using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ITG_Core;
using ITG_Core.Bulders;

namespace ITG_Editor.ViewModels {
	/// <summary>
	/// A ViewModel for all IAlgorithmBuilders.
	/// </summary>
	public partial class BuilderModel : INotifyPropertyChanged {

		private readonly IAlgorithmBuilder builder;

		//Displayable properties
		private readonly List<IPropertyModel> properties;

		/// <summary>
		/// String identifier used in the Dictionary
		/// </summary>
		private string id;

		public IAlgorithmBuilder Builder => builder;

		public string ID
		{
			get => id;
			set {
				id = value;
				if ( PropertyChanged != null )
					PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ID)));
			}
		}

		public string Name => builder.GetType().Name.Replace("`1", " ") + ( ( builder.GetType().IsGenericType ) ? "<" + builder.GetType().GetGenericArguments()[0].Name + ">" : "" );

		public List<IPropertyModel> Properties => properties;

		public event PropertyChangedEventHandler PropertyChanged;

		public BuilderModel(IAlgorithmBuilder builder)
		{
			this.builder = builder;
			PropertyInfo[] propertiesInfo = builder.GetType().GetProperties();
			properties = new List<IPropertyModel>();
			foreach ( PropertyInfo propertyInfo in propertiesInfo ) {
				properties.Add(PropertyModel<NULL_CLASS>.Instantiate(propertyInfo, builder));
			}
		}
	}
}