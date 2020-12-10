namespace ITG_Core.Builders {
	using System;
	using System.Collections.Generic;
	using ITG_Core;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="IAlgorithmGroupBuilder" />
	/// </summary>
	public interface IAlgorithmGroupBuilder {

		Type GetGenericType(string key);
	}

	/// <summary>
	/// Defines the <see cref="AlgorithmGroupBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AlgorithmGroupBuilder<T> : IAlgorithmBuilder, IAlgorithmGroupBuilder where T : struct {

		public Algorithm<float> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			string key = intermidiate.GetKeyFor(this);
			return intermidiate.Get<float>(key);
		}

		public abstract Dictionary<string, IAlgorithm> BuildGeneric(LandscapeBuilder.LandscapeIntermidiate landscapeIntermidiate);

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

		public void VerifyVallidity(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			if ( !IsValid(intermidiate) )
				throw new InvalidOperationException("Builder is in an invalid satate and thus cannot build an instance.");
		}
	}
}