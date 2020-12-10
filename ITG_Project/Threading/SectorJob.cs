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

		public Job(in Req request, Process process)
		{
			this.request = request;
			this.process = process;
		}

		public Ret ExecuteFromMainThread(ITGThreadPool activeWaitingThreadpool = null)
		{
			if ( ready )
				return result;
			if ( activeWaitingThreadpool != null && inProcess ) {
				while ( inProcess && activeWaitingThreadpool.Acisst() ) { }
				if ( ready )
					return result;
			}
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
				if ( ready )
					return;
				result = process(request);
				ready = true;
			}
			inProcess = false;
		}

		public Ret ExecuteProcess()
		{
			if ( ready )
				return result;
			result = process(request);
			ready = true;
			return result;
		}
	}

	public class SectorJob<T> : Job<Sector<T>, RequstSector> where T : struct {

		public SectorJob(in RequstSector request, Process process) : base(request, process)
		{
		}
	}
}