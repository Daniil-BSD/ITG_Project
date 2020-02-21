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
		public static readonly string SUBKEY_SEPARATOR = "->";
		public static readonly string MAIN_ALGORITHM_KEY = "MAIN";

		private Dictionary<string, AlgorithmBuilder> builders;

		public AlgorithmBuilder this[string key] {
			get {
				if ( builders.ContainsKey(key) )
					return builders[key];
				key = key.Split(SUBKEY_SEPARATOR.ToCharArray())[0];
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
			var keys = sourceID.Split(SUBKEY_SEPARATOR.ToCharArray());
			if (
				keys.Length == 2 && //keys are in a vlid form
				builders.ContainsKey(keys[0]) && // first key is valid
				builders[keys[0]].GetType().IsSubclassOf(typeof(AlgorithmGroupBuilder))// second key can be used
				) {
				AlgorithmGroupBuilder groupBuilder = (AlgorithmGroupBuilder) builders[keys[0]];
				return groupBuilder.GetGenericType(keys[1]);
			}
			return typeof(NULL_CLASS);
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

		public string GetKeyFor(AlgorithmBuilder builder)
		{
			foreach ( var key in builders.Keys )
				if ( object.ReferenceEquals(builders[key], builder) )
					return key;
			throw new InvalidProgramException();
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
				if ( !builder.builders.ContainsKey(key) ) {
					var keys = key.Split(SUBKEY_SEPARATOR.ToCharArray());
					if (
						keys.Length != 2 || //keys are in a vlid form
						!builder.builders.ContainsKey(keys[0]) || // first key is valid
						!builder.builders[keys[0]].GetType().IsSubclassOf(typeof(AlgorithmGroupBuilder))// second key can be used
					) {
						throw new KeyNotFoundException();
					} else {
						if ( !object.ReferenceEquals(typeof(T), builder[keys[0]].GetGenericType()) )
							throw new InvalidOperationException($"Icompatible Types:\nType : {builder[keys[0]].GetGenericType()}\nExpected:{typeof(T)}\nKey: {key}");
					}
				} else {
					if ( !builder.TypeOf(key).IsSubclassOf(typeof(AlgorithmBuilder<T>)) && !builder.TypeOf(key).IsSubclassOf(typeof(AlgorithmGroupBuilder<T>)) )
						throw new InvalidOperationException($"Icompatible Types:\nType : {builder.TypeOf(key)}\nExpected: AlgorithmBuilder<{typeof(T)}>\nKey: {key}");
				}
				AddAlgorithm(key, builder[key]);
				return (Algorithm<T>) algorithms[key];
			}

			public void AddAlgorithm(string key, AlgorithmBuilder builder)
			{
				var newAlgoruthms = builder.BuildGeneric(this);
				if ( newAlgoruthms.Count == 1 ) {
					//convert dictionary to a single object
					Algorithm[] temp = new Algorithm[1];
					newAlgoruthms.Values.CopyTo(temp, 0);
					algorithms.Add(key, temp[0]);
				} else if ( newAlgoruthms.Count > 1 ) {
					foreach ( string subkey in newAlgoruthms.Keys ) {
						if ( subkey.Equals(MAIN_ALGORITHM_KEY) )
							algorithms.Add(key, newAlgoruthms[subkey]);
						else
							algorithms.Add(key + SUBKEY_SEPARATOR + subkey, newAlgoruthms[subkey]);
					}
				}
			}
			public string GetKeyFor(AlgorithmBuilder builder)
			{
				return this.builder.GetKeyFor(builder);
			}

			public Dictionary<string, Algorithm> GetAlgorithms()
			{
				return algorithms;
			}
		}
	}
}
