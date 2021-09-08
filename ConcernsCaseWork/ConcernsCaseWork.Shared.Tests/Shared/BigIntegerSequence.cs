using System.Numerics;

namespace ConcernsCaseWork.Shared.Tests.Shared
{
	public static class BigIntegerSequence
	{
		private static BigInteger _sequence = new BigInteger(0);

		public static BigInteger Generator()
		{
			return _sequence = BigInteger.Add(_sequence, 1);
		}
	}
}