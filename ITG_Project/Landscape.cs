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
	}


	/// <summary>
	/// Defines the <see cref="LandscapeBuilder" />
	/// </summary>
	public class LandscapeBuilder {

		private Dictionary<string, AlgorithmBuilder> builders;

		public AlgorithmBuilder this[string key] {
			get {
				return builders[key];
			}
			set {
				if ( !builders.ContainsKey(key) )
					builders.Add(key, value);
			}
		}

		public LandscapeBuilder()
		{
			builders = new Dictionary<string, AlgorithmBuilder>();
		}
		public bool IsValid()
		{
			foreach ( var builder in builders.Values ) {
				if ( !builder.IsValid(this) )
					return false;
			}
			return true;
		}

		public Type TypeOf(string sourceID)
		{
			if ( builders.ContainsKey(sourceID) )
				return builders[sourceID].GetType();
			return null;
		}

		public bool CheckValidityOf(string sourceID)
		{
			if ( builders.ContainsKey(sourceID) )
				return builders[sourceID].IsValid(this);
			return false;
		}

		public Landscape Build()
		{
			if ( !IsValid() )
				throw new InvalidOperationException("LandscapeBuilder is in invalid state bor building Landscape.");
			var algorithms = new LandscapeItermidiate(this).GetAlgorithms();
			return new Landscape(algorithms);
		}

		public class LandscapeItermidiate {
			private Queue<string> buildQueue;
			private Dictionary<string, Algorithm> algorithms;
			private LandscapeBuilder builder;

			public LandscapeItermidiate(LandscapeBuilder builder) : this(builder, builder.builders.Keys) { }

			public LandscapeItermidiate(LandscapeBuilder builder, IEnumerable<string> roots)
			{
				this.builder = builder;
				algorithms = new Dictionary<string, Algorithm>();
				buildQueue = new Queue<string>(roots);
				Build();
			}

			public static implicit operator LandscapeBuilder(LandscapeItermidiate li) => li.builder;

			private void Build()
			{
				while ( buildQueue.Count > 0 ) {
					string key = buildQueue.Dequeue();
					var type = builder[key].GetGenericType();
					var getMethod = typeof(LandscapeItermidiate).GetMethod("Get");
					var getMethodTyped = getMethod.MakeGenericMethod(new[] { type });
					getMethodTyped.Invoke(this, new object[] { key });
				}
			}

			public Algorithm<T> Get<T>(string key) where T : struct
			{
				if ( algorithms.ContainsKey(key) )
					return (Algorithm<T>) algorithms[key];
				if ( !builder.builders.ContainsKey(key) )
					throw new KeyNotFoundException();
				if ( !builder.TypeOf(key).IsSubclassOf(typeof(AlgorithmBuilder<T>)) )
					throw new InvalidOperationException("Icompatible Types");
				algorithms.Add(key, builder[key].BuildGeneric(this));
				return (Algorithm<T>) algorithms[key];
			}

			public Dictionary<string, Algorithm> GetAlgorithms()
			{
				return algorithms;
			}
		}
	}
}
