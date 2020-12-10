using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ITG_Core;
using ITG_Core.Builders;

namespace ITG_Editor.ViewModels {

	/// <summary>
	/// An interfacese used to contain different types of property Models.
	/// </summary>
	public interface IPropertyModel : INotifyPropertyChanged {

		string Name { get; }

		Type Type { get; }

		string Type_Name { get; }

		object Value { get; set; }
	}

	/// <summary>
	/// A wrapper for a property of an IAlgorithmBuilder
	/// </summary>
	/// <typeparam name="T">Type of the value</typeparam>
	abstract public class PropertyModel<T> : IPropertyModel {

		private static Dictionary<Type, Type> propertyModelTypes;

		private IAlgorithmBuilder Builder { get; }

		private PropertyInfo Propperty { get; set; }

		/// <summary>
		/// the dictionary of all PropertyModels.
		/// </summary>
		public static Dictionary<Type, Type> PropertyModelTypes
		{
			get {
				if ( propertyModelTypes == null ) {
					propertyModelTypes = new Dictionary<Type, Type>();
					foreach ( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() ) {
						foreach ( Type assemblyType in assembly.GetTypes() ) {
							if ( typeof(IPropertyModel).IsAssignableFrom(assemblyType)
								&& assemblyType.IsClass
								&& !assemblyType.IsAbstract
								) {
								Type modelsType = assemblyType.BaseType.GenericTypeArguments[0];
								propertyModelTypes.Add(modelsType, assemblyType);
							}
						}
					}
				}
				return propertyModelTypes;
			}
		}

		public string Name => Propperty.Name;

		public Type Type => Propperty.PropertyType;

		public string Type_Name => Type.Name;

		public T Value
		{
			get => (T)Propperty.GetValue(Builder);
			set {
				Propperty.SetValue(Builder, value);
				InvokePropertyChanged(nameof(Value));
			}
		}

		object IPropertyModel.Value { get => Value; set => Value = (T)value; }

		public event PropertyChangedEventHandler PropertyChanged;

		public PropertyModel(IAlgorithmBuilder builder, PropertyInfo propperty)
		{
			Propperty = propperty;
			Builder = builder;
		}

		protected void InvokePropertyChanged(string propertyName)
		{
			if ( PropertyChanged != null )
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Create a property model that allows simple dynamic UI binding.
		/// </summary>
		/// <param name="propertyInfo">The property.</param>
		/// <param name="builder">Instance to which the IPropertyModel will be bound to</param>
		/// <returns>the instance of an appropriate non-generic IPropertyModel based on the property type.</returns>
		public static IPropertyModel Instantiate(PropertyInfo propertyInfo, IAlgorithmBuilder builder)
		{
			if ( PropertyModelTypes.ContainsKey(propertyInfo.PropertyType) ) {
				Type type = PropertyModelTypes[propertyInfo.PropertyType];
				IPropertyModel obj = (IPropertyModel)Activator.CreateInstance(type, new object[] { builder, propertyInfo });
				return obj;
			}
			return null;
		}
	}

	//Concrete types for UI mapping

	public class PropertyModel_bool : PropertyModel<bool> {

		public PropertyModel_bool(IAlgorithmBuilder builder, PropertyInfo propperty) : base(builder, propperty)
		{
		}
	}

	public class PropertyModel_Coordinate : PropertyModel<CoordinateBasic> {

		public int X
		{
			get => Value.x;
			set {
				Value = new CoordinateBasic(value, Value.y);
				InvokePropertyChanged(nameof(X));
			}
		}

		public int Y
		{
			get => Value.y;
			set {
				Value = new CoordinateBasic(Value.x, value);
				InvokePropertyChanged(nameof(Y));
			}
		}

		public PropertyModel_Coordinate(IAlgorithmBuilder builder, PropertyInfo propperty) : base(builder, propperty)
		{
		}
	}

	public class PropertyModel_float : PropertyModel<float> {

		public PropertyModel_float(IAlgorithmBuilder builder, PropertyInfo propperty) : base(builder, propperty)
		{
		}
	}

	public class PropertyModel_int : PropertyModel<int> {

		public PropertyModel_int(IAlgorithmBuilder builder, PropertyInfo propperty) : base(builder, propperty)
		{
		}
	}

	public class PropertyModel_string : PropertyModel<string> {

		public PropertyModel_string(IAlgorithmBuilder builder, PropertyInfo propperty) : base(builder, propperty)
		{
		}
	}
}