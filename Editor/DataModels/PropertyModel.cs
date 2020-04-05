using System;
using System.Collections.Generic;
using System.Reflection;
using ITG_Core;
using ITG_Core.Bulders;

namespace Editor.DataModels {

	public interface IPropertyModel {

		string Name { get; }

		Type Type { get; }

		string Type_Name { get; }

		object Value { get; set; }
	}

	public partial class BuilderModel {

		abstract public class PropertyModel<T> : IPropertyModel {

			private static Dictionary<Type, Type> propertyModelTypes;

			private IAlgorithmBuilder Builder { get; }

			private PropertyInfo Propperty { get; set; }

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
									//&& assemblyType.BaseType.Name == typeof(PropertyModel<T>).Name
									) {
									Type modelsType = assemblyType.BaseType.GenericTypeArguments[0];
									//Type modelsType = assemblyType.BaseType.GetGenericTypeDefinition();
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
				set => Propperty.SetValue(Builder, value);
			}

			object IPropertyModel.Value { get => Value; set => Value = (T)value; }

			public PropertyModel(IAlgorithmBuilder builder, PropertyInfo propperty)
			{
				Propperty = propperty;
				Builder = builder;
			}

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

		public class PropertyModel_Coordinate : PropertyModel<Coordinate> {

			public int X
			{
				get => Value.x;
				set => Value = new Coordinate(value, Value.y);
			}

			public int Y
			{
				get => Value.x;
				set => Value = new Coordinate(Value.x, value);
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
}