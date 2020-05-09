using System;
using System.Collections.Generic;
using System.Text;
using ITG_Core.Base;
using ITG_Core.Bulders;

namespace ITG_Core.Builders.Base {
	public abstract class OutputterBuilder<O> : IAlgorithmBuilder {
		public CoordinateBasic Offset { get; set; } = new CoordinateBasic(0, 0);

		public abstract Outputter<O> Build(LandscapeBuilder.LandscapeIntermidiate intermidiate);

		public Dictionary<string, IAlgorithm> BuildGeneric(LandscapeBuilder.LandscapeIntermidiate intermidiate)
		{
			Dictionary<string, IAlgorithm> ret = new Dictionary<string, IAlgorithm> {
				{ LandscapeBuilder.MAIN_ALGORITHM_KEY, Build(intermidiate) }
			};
			return ret;
		}

		public Type GetGenericType()
		{
			return typeof(NULL_CLASS);
		}

		public virtual bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			return true;
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
	public abstract class OutputterBuilder<O, S> : OutputterBuilder<O> where S : struct {

		public string SourceID { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( !landscapeBuilder.CheckValidityOf(SourceID) )
				return false;
			if ( !( landscapeBuilder.TypeOf(SourceID).IsSubclassOf(typeof(AlgorithmBuilder<S>)) || landscapeBuilder.TypeOf(SourceID).IsSubclassOf(typeof(AlgorithmGroupBuilder<S>)) ) )
				return false;
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);

			if ( !landscapeBuilder.CheckValidityOf(SourceID) )
				messages.Add("Source Layer \"" + SourceID + "\" is missing or invalid.");
			else if ( !landscapeBuilder.TypeOf(SourceID).IsSubclassOf(typeof(AlgorithmBuilder<S>)) )
				messages.Add("Source \"" + SourceID + "\" is of uncompattible type.");
			return messages;
		}

	}
}
