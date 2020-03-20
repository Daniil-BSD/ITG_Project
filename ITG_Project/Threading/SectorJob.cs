namespace ITG_Core {
	using ITG_Core.Base;




	public interface ISectorJob {

		bool InProcess { get; }

		bool Ready { get; }

		void ExecuteFromWorkerThread();

	}

	public class SectorJob<T> : ISectorJob where T : struct {

		private volatile bool inProcess = false;

		private object mutex = new object();

		private volatile bool ready = false;

		private Sector<T> sector = null;

		private readonly Algorithm<T>.SectorPopulationDelegate SectorPopulation;

		public readonly RequstSector requstSector;

		public bool InProcess => inProcess;

		public bool Ready => ready;

		public Sector<T> Sector => sector;

		public SectorJob(RequstSector requstSector, Algorithm<T>.SectorPopulationDelegate SectorPopulation)
		{
			this.requstSector = requstSector;
			this.SectorPopulation = SectorPopulation;
		}

		public Sector<T> ExecuteFromMainThread()
		{
			inProcess = true;
			lock ( mutex ) {
				if ( ready )
					return sector;
				sector = SectorPopulation(requstSector);
				ready = true;
			}
			inProcess = false;
			return sector;
		}

		public void ExecuteFromWorkerThread()
		{
			if ( inProcess || ready )
				return;
			inProcess = true;
			lock ( mutex ) {
				sector = SectorPopulation(requstSector);
				ready = true;
			}
			inProcess = false;
		}
	}
}