namespace ITG_Core {
	public class Chunk<T> where T : struct {
		public T[,] value;

		public T this[in int x, in int y] {
			get {
				return value[x, y];
			}
			set {
				this.value[x, y] = value;
			}
		}

		public Chunk()
		{
			value = new T[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
		}

		public override string ToString()
		{
			string ret = "";
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					ret += value[i, j].ToString() + "\t";
				}
				ret += "\n";
			}
			return ret + "";
		}

	}
}
