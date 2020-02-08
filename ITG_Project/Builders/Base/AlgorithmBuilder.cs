using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {

	public abstract class AlgorithmBuilder {
		public abstract bool IsValid(LandscapeBuilder landscapeBuilder);
		public abstract string ValidityMessage(LandscapeBuilder landscapeBuilder);
		public abstract Algorithm BuildGeneric(LandscapeBuilder.LandscapeItermidiate landscapeItermidiate);
		public abstract Type GetGenericType();
	}
	/// <summary>
	/// Defines the <see cref="AlgorithmBuilder{T}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class AlgorithmBuilder<T> : AlgorithmBuilder where T : struct {
		public virtual Algorithm<T> Build(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			if ( !IsValid(itermidiate) )//casting is implicit and defined
				throw new InvalidOperationException("Builder is in an invalid satate and thus cannot build the ");
			return null;
		}
		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			return true;
		}

		public override string ValidityMessage(LandscapeBuilder landscapeBuilder)
		{
			if ( IsValid(landscapeBuilder) )
				return "Valid.";
			else
				return String.Join("\n", ValidityMessages(landscapeBuilder));
		}

		public virtual List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			return new List<string>();
		}

		public override Algorithm BuildGeneric(LandscapeBuilder.LandscapeItermidiate itermidiate)
		{
			return Build(itermidiate);
		}

		public sealed override Type GetGenericType()
		{
			return typeof(T);
		}
	}
}
