using System.Numerics;

namespace Service.Redis.Shared
{
	/// <summary>
	/// Remove this class when TRAMS API is live with
	/// all endpoint.
	/// </summary>
	public static class BigIntegerSequence
	{
		private static BigInteger _sequence = new BigInteger(0);

		public static BigInteger Generator()
		{
			return _sequence = BigInteger.Add(_sequence, 1);
		}
	}
}