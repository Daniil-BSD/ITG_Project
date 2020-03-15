namespace ITG_Core.Bulders {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="InterpolatableAlgorithmBuilder{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class InterpolatableAlgorithmBuilder<T, S> : LayerBuilder<T, S> where T : struct where S : struct {
		public int Scale { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if ( !base.IsValid(landscapeBuilder) )
				return false;
			if ( Scale < 1 )
				return false;
			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);
			if ( Scale < 1 )
				messages.Add("Scale (valee : " + Scale + ") has to have the value of at least 1.");
			return messages;
		}
	}
}
