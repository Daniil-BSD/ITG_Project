namespace ITG_Core {

	public class ITGThreadpoolBuilder {

		public ITGThreadPool Build()
		{
			return new ITGThreadPool(8);
		}
	}
}