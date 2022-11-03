namespace ITG_Core.Builders
{
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="LayerBuilder{T, S}" />
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public abstract class LayerBuilder<T, S> : AlgorithmBuilder<T> where T : struct where S : struct
	{

		public string SourceID { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if (!base.IsValid(landscapeBuilder))
			{
				return false;
			}

			if (!landscapeBuilder.CheckValidityOf(SourceID))
			{
				return false;
			}

			if (!landscapeBuilder.TypeOf(SourceID).Equals(typeof(S)))
			{
				return false;
			}

			return true;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);

			if (!landscapeBuilder.CheckValidityOf(SourceID))
			{
				messages.Add("Source Layer \"" + SourceID + "\" is missing or invalid.");
			}
			else if (!landscapeBuilder.TypeOf(SourceID).Equals(typeof(S)))
			{
				messages.Add("Source \"" + SourceID + "\" is of uncompattible type.");
			}

			return messages;
		}
	}

	public abstract class CombinationLayerBuilder<T, S, M> : LayerBuilder<T, S> where T : struct where S : struct where M : struct
	{

		public string ModifierID { get; set; }

		public override bool IsValid(LandscapeBuilder landscapeBuilder)
		{
			if (!base.IsValid(landscapeBuilder))
			{
				return false;
			}

			if (!landscapeBuilder.CheckValidityOf(ModifierID))
			{
				return false;
			}

			if (!landscapeBuilder.TypeOf(ModifierID).Equals(typeof(M)))
			{
				return false;
			}

			return base.IsValid(landscapeBuilder);
			;
		}

		public override List<string> ValidityMessages(LandscapeBuilder landscapeBuilder)
		{
			List<string> messages = base.ValidityMessages(landscapeBuilder);

			if (!landscapeBuilder.CheckValidityOf(ModifierID))
			{
				messages.Add("Source Layer \"" + ModifierID + "\" is missing or invalid.");
			}
			else if (!landscapeBuilder.TypeOf(ModifierID).Equals(typeof(S)))
			{
				messages.Add("Source \"" + ModifierID + "\" is of uncompattible type.");
			}
			base.ValidityMessages(landscapeBuilder);
			return messages;
		}
	}
}