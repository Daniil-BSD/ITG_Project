namespace ITG_Core {
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="AlgorithmGroupBuilder" />
	/// </summary>
	public interface AlgorithmGroupBuilder {
		Type GetGenericType(string key);
	}

	/// <summary>
	/// Defines the <see cref="AlgorithmGroupBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AlgorithmGroupBuilder<T> : AlgorithmBuilder, AlgorithmGroupBuilder where T : struct {
		public Algorithm<float> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			string key = itermidiate.GetKeyFor(this);
			return itermidiate.Get<float>(key);
		}

		public abstract Dictionary<string, Algorithm> BuildGeneric(LandscapeBuilder.LandscapeItermidiate landscapeItermidiate);

		public Type GetGenericType()
		{
			return typeof(T);
		}

		public virtual Type GetGenericType(string key)
		{
			return typeof(NULL_CLASS);
		}

		public virtual bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			return true;
		}

		public string ValidityMessage(LandscapeBuilder landscapeBuilder)
		{
			if ( IsValid(landscapeBuilder) )
				return "Valid.";
			else
				return string.Join("\n", ValidityMessages(landscapeBuilder));
		}

		public virtual List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			return new List<string>();
		}
	}
}
