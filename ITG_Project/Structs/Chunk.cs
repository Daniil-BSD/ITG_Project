namespace ITG_Core {

	public class Chunk<T> where T : struct {

		public const int CHUNK_COMPARISON_STEP = Constants.CHUNK_SIZE + 7;

		public T[,] value;

		public Chunk()
		{
			value = new T[Constants.CHUNK_SIZE, Constants.CHUNK_SIZE];
		}

		public override bool Equals(object obj)
		{
			if ( obj == null || GetType() != obj.GetType() ) {
				return false;
			}
			Chunk<T> ch = (Chunk<T>)obj;
			for ( int i = 0 ; i < Constants.CHUNK_NUMBER_OF_VALUES ; i += CHUNK_COMPARISON_STEP )
				if ( !value[i % Constants.CHUNK_SIZE, i / Constants.CHUNK_SIZE].Equals(ch.value[i % Constants.CHUNK_SIZE, i / Constants.CHUNK_SIZE]) )
					return false;
			return true;
		}

		public Chunk<T> GetCopy()
		{
			Chunk<T> ret = new Chunk<T>();
			for ( int i = 0 ; i < Constants.CHUNK_SIZE ; i++ ) {
				for ( int j = 0 ; j < Constants.CHUNK_SIZE ; j++ ) {
					ret.value[i, j] = value[i, j];
				}
			}
			return ret;
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

		public T this[in int x, in int y]
		{
			get => value[x, y];
			set => this.value[x, y] = value;
		}
	}
}