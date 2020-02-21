namespace ITG_Core {
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="AlgorithmBuilder" />
	/// </summary>
	public interface AlgorithmBuilder {
		Dictionary<string, Algorithm> BuildGeneric(LandscapeBuilder.LandscapeItermidiate landscapeItermidiate);

		Type GetGenericType();

		bool IsValid(LandscapeBuilder landscapeBuilder);

		string ValidityMessage(LandscapeBuilder landscapeBuilder);
	}

	/// <summary>
	/// Defines the <see cref="AlgorithmBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AlgorithmBuilder<T> : AlgorithmBuilder where T : struct {
		public abstract Algorithm<T> Build(LandscapeBuilder.LandscapeItermidiate itermidiate);

		public Dictionary<string, Algorithm> BuildGeneric(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			var ret = new Dictionary<string, Algorithm> {
				{ LandscapeBuilder.MAIN_ALGORITHM_KEY, Build(itermidiate) }
			};
			return ret;
		}

		public virtual Type GetGenericType()
		{
			return typeof(T);
		}

		public virtual bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			return true;
		}

		public virtual string ValidityMessage(LandscapeBuilder landscapeBuilder)
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

		public void VerifyVallidity(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			if ( !IsValid(itermidiate) )
				throw new InvalidOperationException("Builder is in an invalid satate and thus cannot build an instance.");
		}
	}
}
