namespace ITG_Core {
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the <see cref="Landscape" />
	/// </summary>
	public class Landscape {

		public Landscape(Dictionary<string, Algorithm> algorithms)
		{
			this.algorithms = algorithms;
		}

		private Dictionary<string, Algorithm> algorithms;
		public Algorithm this[string key] {
			get {
				return algorithms[key];
			}
		}

		public Algorithm<T> GetAlgorithm<T>(string key) where T : struct
		{
			if ( algorithms.ContainsKey(key) ) {
				if ( Object.ReferenceEquals(algorithms[key].GetGenericType(), typeof(T)) ) {
					return (Algorithm<T>) algorithms[key];
				}
				throw new InvalidOperationException("Type mismatch");
			}
			throw new KeyNotFoundException();
		}

		public void UsedOnlyForAOTCodeGeneration()
		{
			GetAlgorithm<float>("");
			GetAlgorithm<uint>("");
			GetAlgorithm<Vec2>("");
			GetAlgorithm<Vec3>("");
			GetAlgorithm<Angle>("");
			GetAlgorithm<int>("");
		}
	}
}
