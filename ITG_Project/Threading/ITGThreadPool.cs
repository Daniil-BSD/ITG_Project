﻿namespace ITG_Core {
	using System.Collections.Concurrent;
	using System.Threading;

	public class ITGThreadPool {

		private ManualResetEvent hasJobs = new ManualResetEvent(false);

		private ConcurrentStack<ITGJob> jobs;

		private object mutex = new object();

		private volatile bool terminating = false;

		private ManualResetEvent terminationResetEvent = new ManualResetEvent(false);

		private volatile int threadsRunning = 0;

		private Thread[] workers;

		public readonly int minimumForkingFactor = 2;

		public readonly int threadCapacity;

		public int QueueLength => jobs.Count;

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
					Priority = ThreadPriority.Normal,
				};
				workers[i].Start();
			}
		}

		~ITGThreadPool()
		{
			terminating = true;
			terminationResetEvent.Set();
			foreach ( Thread thread in workers )
				thread.Join();
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

		public bool Acisst()
		{
			ITGJob job;
			bool ret = jobs.TryPop(out job);
			if ( ret )
				job.ExecuteFromWorkerThread();
			return ret;
		}

		public void Enqueue(in ITGJob job)
		{
			jobs.Push(job);
			hasJobs.Set();
		}

		public void Enqueue(in ITGJob[] newJobs, int timeout = -1)
		{
			if ( timeout < 0 ) {
				jobs.PushRange(newJobs);
				hasJobs.Set();
			} else {
				foreach ( ITGJob job in jobs ) {
					Enqueue(job);
					if ( timeout > 0 ) {
						Thread.Sleep(timeout);
					}
				}
			}
		}

		public void Enqueue(in ITGJob[,] newJobs, int timeout = -1)
		{
			ITGJob[] temp = new ITGJob[newJobs.Length];
			int index = 0;
			for ( int i = 0 ; i < newJobs.GetLength(0) ; i++ )
				for ( int j = 0 ; j < newJobs.GetLength(1) ; j++ )
					temp[index++] = newJobs[i, j];
			Enqueue(temp, timeout);
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