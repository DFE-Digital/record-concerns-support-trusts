using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class StartupExtensionTests
	{
		[Test]
		public void WhenAddRedis_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var initialData = new Dictionary<string, string>
			{
				{ "VCAP_SERVICES", "{}" }
			};
			var configuration = new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build();
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddRedis(configuration));
		}
		
		[Test]
		public void WhenAddRedis_MissingPartialConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var initialData = new Dictionary<string, string>
			{
				{ "VCAP_SERVICES", "{'redis': [{'credentials': {'host': '127.0.0.1', 'port': '6379', 'tls_enabled': 'false'}}]}" }
			};
			var configuration = new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build();
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddRedis(configuration));
		}
		
		[Test]
		public void WhenAddRedis_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
			var mockMultiplexer = new Mock<IRedisMultiplexer>();
			mockMultiplexer.Setup(m => m.Connect(It.IsAny<ConfigurationOptions>())).Returns(mockConnectionMultiplexer.Object);
			
			// Inject the mock so that it is used by the extension methods
			StartupExtension.Implementation = mockMultiplexer.Object;
			
			// act
			serviceCollection.AddRedis(new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build());
			
			// assert
			Assert.That(serviceCollection, Is.Not.Null);
			Assert.That(serviceCollection.Where(t => t.ServiceType == typeof(RedisCache)), Is.Not.Null);
		}
		
		[Test]
		public void WhenAddTramsApi_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configuration = new ConfigurationBuilder().ConfigurationInMemoryBuilder(new Dictionary<string, string>()).Build();
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddTramsApi(configuration));
		}

		[Test]
		public void WhenAddTramsApi_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			
			// act
			serviceCollection.AddTramsApi(new ConfigurationBuilder().ConfigurationUserSecretsBuilder().Build());
			
			// assert
			Assert.That(serviceCollection, Is.Not.Null);
			Assert.That(serviceCollection.Where(t => t.ServiceType == typeof(RedisCache)), Is.Not.Null);
		}
	}
}