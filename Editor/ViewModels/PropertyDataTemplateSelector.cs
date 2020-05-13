using ITG_Editor.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ITG_Editor {


	/// <summary>
	/// A vary basic DataTemplateSelector for all defined Datatemplatess.
	/// </summary>
	public class PropertyDataTemplateSelector : DataTemplateSelector {

		public DataTemplate PropertyModel_bool { get; set; }

		public DataTemplate PropertyModel_Coordinate { get; set; }

		public DataTemplate PropertyModel_float { get; set; }

		public DataTemplate PropertyModel_int { get; set; }

		public DataTemplate PropertyModel_string { get; set; }

		protected override DataTemplate SelectTemplateCore(object item)
		{
			if ( item != null ) {
				if ( item.GetType().Equals(typeof(PropertyModel_int)) ) {
					return PropertyModel_int;
				}
				if ( item.GetType().Equals(typeof(PropertyModel_float)) ) {
					return PropertyModel_float;
				}
				if ( item.GetType().Equals(typeof(PropertyModel_string)) ) {
					return PropertyModel_string;
				}
				if ( item.GetType().Equals(typeof(PropertyModel_bool)) ) {
					return PropertyModel_bool;
				}
				if ( item.GetType().Equals(typeof(PropertyModel_Coordinate)) ) {
					return PropertyModel_Coordinate;
				}
				return null;
			}
			return null;
		}
	}
}