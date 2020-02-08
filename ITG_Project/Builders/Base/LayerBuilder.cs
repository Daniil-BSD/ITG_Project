using System;
using System.Collections.Generic;
using System.Text;

namespace ITG_Core {

	public abstract class LayerBuilder<T, S> : AlgorithmBuilder<T> where T : struct where S : struct {

		public string SourceID { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( !landscapeBuilder.CheckValidityOf(SourceID) )
				return false;
			if ( !landscapeBuilder.TypeOf(SourceID).IsSubclassOf(typeof(AlgorithmBuilder<S>)) )
				return false;
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);

			if ( !landscapeBuilder.CheckValidityOf(SourceID) )
				messages.Add("Source Layer \"" + SourceID + "\" is missing or invalid.");
			else if ( !landscapeBuilder.TypeOf(SourceID).IsSubclassOf(typeof(AlgorithmBuilder<S>)) )
				messages.Add("Source is of uncompattible type.");

			return messages;
		}

	}
}
