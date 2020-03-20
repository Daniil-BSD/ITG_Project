namespace ITG_Core {
	using ITG_Core.Base;

	public class ThreadingFork<T> where T : struct {
		private readonly ITGThreadPool pool;
		private readonly SectorJob<T>[] jobs;
		private bool areEnqueued = false;

		public bool AreEnqueued => areEnqueued;
		public ThreadingFork(ITGThreadPool pool, in RequstSector[] requstSectors, Algorithm<T>.SectorPopulationDelegate SectorPopulation)
		{
			this.pool = pool;
			jobs = new SectorJob<T>[requstSectors.Length];
			for ( int i = 0 ; i < requstSectors.Length ; i++ ) {
				jobs[i] = new SectorJob<T>(requstSectors[i], SectorPopulation);
			}
		}

		public ThreadingFork(ITGThreadPool pool, in SectorJob<T>[] jobs)
		{
			this.pool = pool;
			this.jobs = jobs;
		}

		public void EnqueueJobs()
		{
			if ( areEnqueued )
				return;
			pool.Enqueue(jobs);
			areEnqueued = true;
		}

		public Sector<T>[] Execute(bool enqueue = true)
		{
			if ( enqueue )
				EnqueueJobs();
			Sector<T>[] ret = new Sector<T>[jobs.Length];
			for ( int i = 0 ; i < jobs.Length ; i++ ) {
				Sector<T> temp = jobs[i].ExecuteFromMainThread();
				ret[i] = temp;
			}
			return ret;
		}
	}
}