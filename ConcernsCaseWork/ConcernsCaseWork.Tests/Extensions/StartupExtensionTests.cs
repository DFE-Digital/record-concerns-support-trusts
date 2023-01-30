using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

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
			Assert.Throws<Exception>(() => serviceCollection.AddRedis(configuration));
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
			Assert.Throws<Exception>(() => serviceCollection.AddRedis(configuration));
		}

		[Test]
		public void WhenAddTramsApi_MissingConfiguration_ThrowException()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var configuration = new ConfigurationBuilder().ConfigurationInMemoryBuilder(new Dictionary<string, string>()).Build();

			// act
			Assert.Throws<Exception>(() => serviceCollection.AddTramsApi(configuration));
		}

		[Test]
		public void WhenAddTramsApi_Configuration_Success()
		{
			// arrange
			var serviceCollection = new ServiceCollection();
			var initialData = new Dictionary<string, string>
			{
				{ "trams:api_key", "a-key" },
				{ "trams:api_endpoint", "https:localhost" },
			};
			var configuration = new ConfigurationBuilder().ConfigurationInMemoryBuilder(initialData).Build();

			// act
			serviceCollection.AddTramsApi(configuration);

			// assert
			Assert.That(serviceCollection, Is.Not.Null);
		}
	}
}