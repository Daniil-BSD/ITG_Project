namespace ITG_Core {
	using System;
	using System.Collections.Generic;
	using ITG_Core.Base;
	using ITG_Core.Bulders;

	/// <summary>
	/// Defines the <see cref="LandscapeBuilder" />
	/// </summary>
	public class LandscapeBuilder {

		private Dictionary<string, IAlgorithmBuilder> builders;

		private ITGThreadpoolBuilder threadpoolBuilder;

		public static readonly string MAIN_ALGORITHM_KEY = "MAIN";

		public static readonly string SUBKEY_SEPARATOR = "->";

		public ITGThreadpoolBuilder ThreadpoolBuilder { get => threadpoolBuilder; set => threadpoolBuilder = value; }

		public LandscapeBuilder()
		{
			builders = new Dictionary<string, IAlgorithmBuilder>();
			threadpoolBuilder = new ITGThreadpoolBuilder();
		}

		public Landscape Build()
		{
			if ( !IsValid() )
				throw new InvalidOperationException("LandscapeBuilder is in invalid state for building Landscape.");
			LandscapeIntermidiate intermediate = new LandscapeIntermidiate(this);
			Dictionary<string, IAlgorithm> algorithms = intermediate.GetAlgorithms();
			return new Landscape(algorithms, intermediate.ThreadPool);
		}

		public bool CheckValidityOf(string sourceID)
		{
			if ( builders.ContainsKey(sourceID) )
				return builders[sourceID].IsValid(this);
			return false;
		}

		public string GetKeyFor(IAlgorithmBuilder builder)
		{
			foreach ( string key in builders.Keys )
				if ( object.ReferenceEquals(builders[key], builder) )
					return key;
			throw new InvalidProgramException();
		}

		public bool IsValid()
		{
			foreach ( IAlgorithmBuilder builder in builders.Values ) {
				if ( !builder.IsValid(this) )
					return false;
			}
			return true;
		}

		public Type TypeOf(string sourceID)
		{
			if ( builders.ContainsKey(sourceID) )
				return builders[sourceID].GetType();
			string[] keys = sourceID.Split(SUBKEY_SEPARATOR.ToCharArray());
			if (
				keys.Length == 2 && //keys are in a vlid form
				builders.ContainsKey(keys[0]) && // first key is valid
				builders[keys[0]].GetType().IsSubclassOf(typeof(IAlgorithmGroupBuilder))// second key can be used
				) {
				IAlgorithmGroupBuilder groupBuilder = (IAlgorithmGroupBuilder)builders[keys[0]];
				return groupBuilder.GetGenericType(keys[1]);
			}
			return typeof(NULL_CLASS);
		}

		public IAlgorithmBuilder this[string key]
		{
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

		public class LandscapeIntermidiate {

			private Dictionary<string, IAlgorithm> algorithms;

			private LandscapeBuilder builder;

			private Queue<string> buildQueue;

			private ITGThreadPool threadPool;

			public ITGThreadPool ThreadPool => threadPool;

			public LandscapeIntermidiate(LandscapeBuilder builder) : this(builder, builder.builders.Keys)
			{
			}

			public LandscapeIntermidiate(LandscapeBuilder builder, IEnumerable<string> roots)
			{
				this.builder = builder;
				algorithms = new Dictionary<string, IAlgorithm>();
				buildQueue = new Queue<string>(roots);
				threadPool = builder.threadpoolBuilder.Build();
				Build();
			}

			private void Build()
			{
				while ( buildQueue.Count > 0 ) {
					string key = buildQueue.Dequeue();
					IAlgorithmBuilder algorithmBuilder = builder[key];
					AddAlgorithm(key, algorithmBuilder);
				}
			}

			public static implicit operator LandscapeBuilder(LandscapeIntermidiate li)
			{
				return li.builder;
			}

			public void AddAlgorithm(string key, IAlgorithmBuilder builder)
			{
				if ( algorithms.ContainsKey(key) )
					return;
				Dictionary<string, IAlgorithm> newAlgoruthms = builder.BuildGeneric(this);
				if ( newAlgoruthms.Count == 1 ) {
					//convert dictionary to a single object
					IAlgorithm[] temp = new IAlgorithm[1];
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

			public Algorithm<T> Get<T>(string key) where T : struct
			{
				if ( algorithms.ContainsKey(key) )
					return (Algorithm<T>)algorithms[key];
				if ( !builder.builders.ContainsKey(key) ) {
					string[] keys = key.Split(SUBKEY_SEPARATOR.ToCharArray());
					if (
						keys.Length != 2 || //keys are in a vlid form
						!builder.builders.ContainsKey(keys[0]) || // first key is valid
						!builder.builders[keys[0]].GetType().IsSubclassOf(typeof(IAlgorithmGroupBuilder))// second key can be used
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
				return (Algorithm<T>)algorithms[key];
			}

			public Dictionary<string, IAlgorithm> GetAlgorithms()
			{
				return algorithms;
			}

			public string GetKeyFor(IAlgorithmBuilder builder)
			{
				return this.builder.GetKeyFor(builder);
			}

			public void UsedOnlyForAOTCodeGeneration()
			{
				Get<float>("");
				Get<uint>("");
				Get<Vec2>("");
				Get<Vec3>("");
				Get<Angle>("");
				Get<int>("");
			}
		}
	}
}