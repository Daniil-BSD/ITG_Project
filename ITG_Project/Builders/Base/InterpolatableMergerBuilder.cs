namespace ITG_Core {
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="InterpolatableMergerBuilder" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S1"></typeparam>
	/// <typeparam name="S2"></typeparam>
	public class InterpolatableMergerBuilder<T, S1, S2> : MergerBuilder<T, S1, S2> where T : struct where S1 : struct where S2 : struct {
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
