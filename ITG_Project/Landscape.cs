namespace ITG_Core {
	using System;
	using System.Collections.Generic;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="Landscape" />
	/// </summary>
	public class Landscape {

		private Dictionary<string, IAlgorithm> algorithms;

		public Landscape(Dictionary<string, IAlgorithm> algorithms)
		{
			this.algorithms = algorithms;
		}

		public Algorithm<T> GetAlgorithm<T>(string key) where T : struct
		{
			if ( algorithms.ContainsKey(key) ) {
				if ( Object.ReferenceEquals(algorithms[key].GetGenericType(), typeof(T)) ) {
					return (Algorithm<T>)algorithms[key];
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

		public IAlgorithm this[string key] => algorithms[key];
	}
}