﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ITG_Core.Structs {

	/// <summary>
	/// taken from https://weblogs.asp.net/pwelter34/444961 and improved
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable {

		private const string item_tag = "item";

		private const string key_tag = "key";

		private const string value_tag = "value";

		private const string valueType_tag = "valueType";

		private XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

		private XmlSerializer stringSerializer = new XmlSerializer(typeof(string));

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();
			if ( wasEmpty )
				return;
			while ( reader.NodeType != System.Xml.XmlNodeType.EndElement ) {
				reader.ReadStartElement(item_tag);
				reader.ReadStartElement(key_tag);
				TKey key = (TKey)keySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement(valueType_tag);
				string typeName = (string)stringSerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement(value_tag);
				try {
					Type type = Type.GetType(typeName);
					XmlSerializer valueSerializer = new XmlSerializer(type);
					TValue value = (TValue)valueSerializer.Deserialize(reader);
					Add(key, value);
				} catch ( Exception e ) {
					throw e;
				} finally {
					reader.ReadEndElement();
					reader.ReadEndElement();
					reader.MoveToContent();
				}
			}
			reader.ReadEndElement();
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			foreach ( TKey key in Keys ) {
				writer.WriteStartElement(item_tag);
				writer.WriteStartElement(key_tag);
				keySerializer.Serialize(writer, key);
				writer.WriteEndElement();
				writer.WriteStartElement(valueType_tag);
				TValue value = this[key];
				string type = value.GetType().AssemblyQualifiedName;
				stringSerializer.Serialize(writer, type);
				writer.WriteEndElement();
				writer.WriteStartElement(value_tag);
				XmlSerializer valueSerializer = new XmlSerializer(value.GetType());
				valueSerializer.Serialize(writer, value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}
	}
}