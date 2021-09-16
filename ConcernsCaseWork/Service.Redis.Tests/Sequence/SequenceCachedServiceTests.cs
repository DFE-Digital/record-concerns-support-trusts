using Moq;
using NUnit.Framework;
using Service.Redis.Base;
using Service.Redis.Sequence;
using System.Threading.Tasks;

namespace Service.Redis.Tests.Sequence
{
	[Parallelizable(ParallelScope.All)]
	public class SequenceCachedServiceTests
	{
		[Test]
		public async Task WhenGenerator_ReturnsNextValue()
		{
			// arrange
			var mockCacheProvider = new Mock<ICacheProvider>();
			
			mockCacheProvider.SetupSequence(c => c.GetFromCache<string>(It.IsAny<string>()))
				.ReturnsAsync("0")
				.ReturnsAsync("1");

			var sequenceCachedService = new SequenceCachedService(mockCacheProvider.Object);

			// act
			var initialValue = await sequenceCachedService.Generator();
			var nextValue = await sequenceCachedService.Generator();

			// assert
			Assert.That(initialValue, Is.EqualTo(1));
			Assert.That(nextValue, Is.EqualTo(2));
		}
	}
}