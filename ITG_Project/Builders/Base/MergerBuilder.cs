namespace ITG_Core {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="MergerBuilder{T, S1, S2}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S1"></typeparam>
	/// <typeparam name="S2"></typeparam>
	public abstract class MergerBuilder<T, S1, S2> : LayerBuilder<T, S1> where T : struct where S1 : struct where S2 : struct {
		public string Source2ID { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( !landscapeBuilder.CheckValidityOf(Source2ID) )
				return false;
			if ( !landscapeBuilder.TypeOf(Source2ID).IsSubclassOf(typeof(AlgorithmBuilder<S2>)) )
				return false;
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);

			if ( !landscapeBuilder.CheckValidityOf(Source2ID) )
				messages.Add("Source 2 Layer \"" + Source2ID + "\" is missing or invalid.");
			else if ( !landscapeBuilder.TypeOf(Source2ID).IsSubclassOf(typeof(AlgorithmBuilder<S2>)) )
				messages.Add("Source 2 is of uncompattible type.");

			return messages;
		}
	}
}
