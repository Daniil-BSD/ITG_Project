namespace ITG_Core {
	using System;
	using System.Collections.Generic;
	using ITG_Core.Base;

	/// <summary>
	/// Defines the <see cref="Landscape" />
	/// </summary>
	public class Landscape {

		private Dictionary<string, IAlgorithm> algorithms;
		public ITGThreadPool threadPool;

		public int RunnningThreads => threadPool.ThreadsRunning;

		public Landscape(Dictionary<string, IAlgorithm> algorithms, ITGThreadPool threadPool)
		{
			this.algorithms = algorithms;
			this.threadPool = threadPool;
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

		public Outputter<O> GetOutputter<O>(string key)
		{
			if ( algorithms.ContainsKey(key) ) {
				if ( algorithms[key].GetType().IsSubclassOf(typeof(Outputter<O>)) ) {
					return (Outputter<O>)algorithms[key];
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