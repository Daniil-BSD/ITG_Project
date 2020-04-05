namespace ITG_Core {
	using System.Collections.Concurrent;
	using System.Threading;

	public class ITGThreadPool {

		private ConcurrentStack<ITGJob> jobs;
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
			jobs = new ConcurrentStack<ITGJob>();
			terminating = false;
			for ( int i = 0 ; i < threadCapacity ; i++ ) {
				workers[i] = new Thread(WorkerThread) {
					IsBackground = true,
					Priority = ThreadPriority.Lowest,
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
				ITGJob job;
				if ( TryGet(out job) ) {
					job.ExecuteFromWorkerThread();
				}
			}
			lock ( mutex ) {
				threadsRunning--;
			}
		}

		private bool TryGet(out ITGJob job)
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
		public void Enqueue(in ITGJob job)
		{
			jobs.Push(job);
			hasJobs.Set();
		}
		public void Enqueue(in ITGJob[] newJobs)
		{
			jobs.PushRange(newJobs);
			hasJobs.Set();
		}

		public void Enqueue(in ITGJob[,] newJobs)
		{
			ITGJob[] temp = new ITGJob[newJobs.Length];
			int index = 0;
			for ( int i = 0 ; i < newJobs.GetLength(0) ; i++ )
				for ( int j = 0 ; j < newJobs.GetLength(1) ; j++ )
					temp[index++] = newJobs[i, j];
			Enqueue(temp);
		}

		public Sector<T>[] Execute<T>(in RequstSector[] requstSectors, SectorJob<T>.Process SectorPopulation) where T : struct
		{
			return new ThreadingForkSector<T>(this, requstSectors, SectorPopulation).Execute(requstSectors.Length >= minimumForkingFactor);
		}

		public Sector<T>[] Execute<T>(in SectorJob<T>[] sectorJobs) where T : struct
		{
			return new ThreadingForkSector<T>(this, sectorJobs).Execute(sectorJobs.Length >= minimumForkingFactor);
		}
	}
}