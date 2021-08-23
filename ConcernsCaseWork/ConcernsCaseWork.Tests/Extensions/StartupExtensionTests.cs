using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Tests.Factory;
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
		public void WhenAddRedis_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configuration = ConfigurationFactory.ConfigurationBuilder(new Dictionary<string, string>());
			
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
				{ "VCAP_SERVICES:redis:0:credentials:host", "1234" }, 
				{ "VCAP_SERVICES:redis:0:credentials:password", "password" }
			};
			var configuration = ConfigurationFactory.ConfigurationBuilder(initialData);
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddRedis(configuration));
		}
		
		[Test]
		public void WhenAddRedis_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var initialData = new Dictionary<string, string>
			{
				{ "VCAP_SERVICES:redis:0:credentials:host", "127.0.0.1" }, 
				{ "VCAP_SERVICES:redis:0:credentials:password", "password" }, 
				{ "VCAP_SERVICES:redis:0:credentials:port", "1234" }
			};
			var configuration = ConfigurationFactory.ConfigurationBuilder(initialData);

			var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
			var mockMultiplexer = new Mock<IRedisMultiplexer>();
			mockMultiplexer.Setup(m => m.Connect(It.IsAny<ConfigurationOptions>())).Returns(mockConnectionMultiplexer.Object);
			
			// Inject the mock so that it is used by the extension methods
			StartupExtension.Implementation = mockMultiplexer.Object;
			
			// act
			serviceCollection.AddRedis(configuration);
			
			// assert
			Assert.That(serviceCollection, Is.Not.Null);
			Assert.That(serviceCollection.Count, Is.GreaterThan(10));
		}
		
		[Test]
		public void WhenAddTramsApi_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configuration = ConfigurationFactory.ConfigurationBuilder(new Dictionary<string, string>());
			
			// act
			Assert.Throws<ConfigurationErrorsException>(() => serviceCollection.AddTramsApi(configuration));
		}

		[Test]
		public void WhenAddTramsApi_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var initialData = new Dictionary<string, string> { { "trams:api_endpoint", "localhost" }, { "trams:api_key", "123" } };
			var configuration = ConfigurationFactory.ConfigurationBuilder(initialData);
			
			// act
			serviceCollection.AddTramsApi(configuration);
			
			// assert
			Assert.That(serviceCollection, Is.Not.Null);
			Assert.That(serviceCollection.Count, Is.GreaterThan(10));
		}
	}
}