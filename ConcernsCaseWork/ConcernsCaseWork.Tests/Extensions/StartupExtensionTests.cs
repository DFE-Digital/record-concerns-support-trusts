using ConcernsCaseWork.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Configuration;

namespace ConcernsCaseWork.Tests.Extensions
{
	[Parallelizable(ParallelScope.All)]
	public class StartupExtensionTests
	{
		[Test]
		public void WhenAddRedisLocal_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configDic = new Dictionary<string, string> { { "redis:local", "true" } };
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(configDic).Build();
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddRedis(configuration));
		}
		
		[Test]
		public void WhenAddRedisLocal_MissingPartialConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configDic = new Dictionary<string, string> { { "redis:local", "true" }, { "redis:host", "1234" }, { "redis:password", "password" } };
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(configDic).Build();
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddRedis(configuration));
		}
		
		[Test]
		public void WhenAddRedisLocal_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configDic = new Dictionary<string, string> { { "redis:local", "true" }, { "redis:host", "127.0.0.1" }, { "redis:password", "password" }, { "redis:port", "1234" } };
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(configDic).Build();

			var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
			var mockMultiplexer = new Mock<IRedisMultiplexer>();
			mockMultiplexer.Setup(m => m.Connect(It.IsAny<ConfigurationOptions>())).Returns(mockConnectionMultiplexer.Object);
			
			// Inject the mock so that it is used by the extension methods
			StartupExtension.Implementation = mockMultiplexer.Object;
			
			// act
			serviceCollection.AddRedis(configuration);
			
			// assert
			Assert.That(serviceCollection, Is.Not.Null);
			Assert.That(serviceCollection.Count, Is.EqualTo(19));
		}
		
		[Test]
		public void WhenAddTramsApi_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build();
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddTramsApi(configuration));
		}

		[Test]
		public void WhenAddTramsApi_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configDic = new Dictionary<string, string> { { "trams_api_endpoint", "localhost" }, { "trams_api_key", "123" } };
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(configDic).Build();
			
			// act
			serviceCollection.AddTramsApi(configuration);
			
			// assert
			Assert.That(serviceCollection, Is.Not.Null);
			Assert.That(serviceCollection.Count, Is.EqualTo(18));
		}
	}
}