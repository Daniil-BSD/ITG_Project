﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ITG_Core;
using ITG_Core.Bulders;

namespace Editor.DataModels {
	public partial class BuilderModel {
		private readonly IAlgorithmBuilder builder;
		private readonly List<IPropertyModel> properties;

		public string Name => builder.GetType().Name;
		public List<IPropertyModel> Properties => properties;

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
