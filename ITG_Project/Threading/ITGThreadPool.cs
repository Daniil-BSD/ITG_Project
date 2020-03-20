namespace ITG_Core {
	using System.Collections.Concurrent;
	using System.Threading;
	using ITG_Core.Base;

	public class ITGThreadPool {

		private ConcurrentStack<ISectorJob> jobs;
		private volatile bool terminating = false;
		private volatile int threadsRunning = 0;
		private Thread[] workers;
		public readonly int threadCapacity;
		public readonly int minimumForkingFactor = 2;

		private object mutex = new object();
		private ManualResetEvent hasJobs = new ManualResetEvent(false);
		private ManualResetEvent terminationResetEvent = new ManualResetEvent(false);
		public int ThreadsRunning => threadsRunning;


		public ITGThreadPool(int threadCapacity)
		{
			this.threadCapacity = threadCapacity;
			workers = new Thread[threadCapacity];
			jobs = new ConcurrentStack<ISectorJob>();
			terminating = false;
			for ( int i = 0 ; i < threadCapacity ; i++ ) {
				workers[i] = new Thread(WorkerThread) {
					IsBackground = true,
					Priority = ThreadPriority.Normal,
				};
				workers[i].Start();
			}
		}
		private void WorkerThread()
		{
			lock ( mutex ) {
				threadsRunning++;
			}
			while ( !terminating ) {
				ISectorJob job;
				if ( TryGet(out job) ) {
					job.ExecuteFromWorkerThread();
				}
			}
			lock ( mutex ) {
				threadsRunning--;
			}
		}

		private bool TryGet(out ISectorJob job)
		{
			job = null;
			bool ret = false;
			if ( jobs.IsEmpty )
				hasJobs.Reset();
			if ( WaitHandle.WaitAny(new WaitHandle[] { hasJobs, terminationResetEvent }) == 0 ) {
				ret = jobs.TryPop(out job);
			}
			return ret;
		}
		internal void Enqueue(in ISectorJob job)
		{
			jobs.Push(job);
			hasJobs.Set();
		}
		internal void Enqueue(in ISectorJob[] newJobs)
		{
			jobs.PushRange(newJobs);
			hasJobs.Set();
		}

		public Sector<T>[] Execute<T>(in RequstSector[] requstSectors, Algorithm<T>.SectorPopulationDelegate SectorPopulation) where T : struct
		{
			return new ThreadingFork<T>(this, requstSectors, SectorPopulation).Execute(requstSectors.Length >= minimumForkingFactor);
		}

		public Sector<T>[] Execute<T>(in SectorJob<T>[] sectorJobs) where T : struct
		{
			return new ThreadingFork<T>(this, sectorJobs).Execute(sectorJobs.Length >= minimumForkingFactor);
		}
	}
}