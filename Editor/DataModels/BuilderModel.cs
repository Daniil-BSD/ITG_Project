using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ITG_Core;
using ITG_Core.Bulders;

namespace Editor.DataModels {
	public partial class BuilderModel : INotifyPropertyChanged {
		private readonly IAlgorithmBuilder builder;
		private readonly List<IPropertyModel> properties;

		public event PropertyChangedEventHandler PropertyChanged;
		private string id;
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
		public IAlgorithmBuilder Builder => builder;

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
