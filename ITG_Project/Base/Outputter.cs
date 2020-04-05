using System;

namespace ITG_Core.Base {
	public abstract class Outputter<O> : IAlgorithm {

		public readonly ITGThreadPool threadPool;

		public readonly Coordinate offset;

		public Outputter(Coordinate offset, ITGThreadPool threadPool)
		{
			this.threadPool = threadPool;
			this.offset = offset;
		}

		public O[,] GetObjects(in RequstSector requstSector)
		{
			O[,] ret = new O[requstSector.width, requstSector.height];
			Job<O, Coordinate>[,] jobs = new Job<O, Coordinate>[requstSector.width, requstSector.height];
			for ( int x = 0 ; x < requstSector.width ; x++ ) {
				for ( int y = 0 ; y < requstSector.height ; y++ ) {
					jobs[x, y] = new Job<O, Coordinate>(new Coordinate(x, y) + requstSector.coordinate, GetObject);
					threadPool.Enqueue(jobs[x, y]);
				}
			}
			for ( int x = 0 ; x < requstSector.width ; x++ ) {
				for ( int y = 0 ; y < requstSector.height ; y++ ) {
					ret[x, y] = jobs[x, y].ExecuteFromMainThread();
				}
			}
			return ret;
		}

		public O GetObject(in Coordinate coordinate)
		{
			return GenerarteObject(coordinate + offset);
		}
		protected abstract O GenerarteObject(in Coordinate coordinate);
		public Type GetGenericType()
		{
			return typeof(NULL_CLASS);
		}
	}


	public abstract class Outputter<O, S> : Outputter<O> where S : struct {

		protected readonly Algorithm<S> source;

		public Outputter(Coordinate offset, ITGThreadPool threadPool, Algorithm<S> source) : base(offset, threadPool)
		{
			this.source = source;
		}
	}
}
