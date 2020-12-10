namespace ITG_Core.Builders {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="MultiInputAlgorithmBuilder{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class MultiInputAlgorithmBuilder<T, S> : AlgorithmBuilder<T> where T : struct where S : struct {

		public string[] Sources { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			foreach ( string source in Sources ) {
				if ( !landscapeBuilder.CheckValidityOf(source) )
					return false;
				if ( !landscapeBuilder.TypeOf(source).IsSubclassOf(typeof(AlgorithmBuilder<S>)) )
					return false;
			}
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);

			foreach ( string source in Sources ) {
				if ( !landscapeBuilder.CheckValidityOf(source) )
					messages.Add("Source Layer \"" + source + "\" is missing or invalid.");
				else if ( !landscapeBuilder.TypeOf(source).IsSubclassOf(typeof(AlgorithmBuilder<S>)) )
					messages.Add("Source \"" + source + "\" is of uncompattible type.");
			}
			return messages;
		}
	}
}