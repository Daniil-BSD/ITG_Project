namespace ITG_Core {

	public interface ITGJob {

		bool InProcess { get; }

		bool Ready { get; }

		void ExecuteFromWorkerThread();
	}

	public class Job<Ret, Req> : ITGJob {

		private volatile bool inProcess = false;

		private object mutex = new object();

		private volatile bool ready = false;

		private Ret result = default;

		public readonly Process process;

		public readonly Req request;

		public bool InProcess => inProcess;

		public bool Ready => ready;

		public Ret Result => result;

		public delegate Ret Process(in Req request);

		public Job(Req request, Process process)
		{
			this.request = request;
			this.process = process;
		}

		public Ret ExecuteFromMainThread()
		{
			inProcess = true;
			lock ( mutex ) {
				if ( ready )
					return result;
				result = process(request);
				ready = true;
			}
			inProcess = false;
			return result;
		}

		public void ExecuteFromWorkerThread()
		{
			if ( inProcess || ready )
				return;
			inProcess = true;
			lock ( mutex ) {
				result = process(request);
				ready = true;
			}
			inProcess = false;
		}
	}

	public class SectorJob<T> : Job<Sector<T>, RequstSector> where T : struct {

		public SectorJob(RequstSector request, Process process) : base(request, process)
		{
		}
	}
}