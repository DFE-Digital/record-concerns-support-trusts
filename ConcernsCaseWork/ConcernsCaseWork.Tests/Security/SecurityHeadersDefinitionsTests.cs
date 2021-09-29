using ConcernsCaseWork.Security;
using NetEscapades.AspNetCore.SecurityHeaders.Headers;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Security
{
	[Parallelizable(ParallelScope.All)]
	public class SecurityHeadersDefinitionsTests
	{
		[TestCase(true)]
		[TestCase(false)]
		public void WhenGetHeaderPolicyCollection_Returns_HeaderPolicyCollection(bool isDev)
		{
			// act
			var headerPolicyCollection = SecurityHeadersDefinitions.GetHeaderPolicyCollection(isDev);

			// assert
			Assert.That(headerPolicyCollection, Is.Not.Null);
			Assert.That(headerPolicyCollection.Count, Is.EqualTo(isDev ? 10 : 11));

			Assert.That(headerPolicyCollection.ContainsKey("X-Frame-Options"), Is.True);
			headerPolicyCollection.TryGetValue("X-Frame-Options", out var xFrameOptions);
			Assert.That(xFrameOptions, Is.AssignableFrom<XFrameOptionsHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("X-XSS-Protection"), Is.True);
			headerPolicyCollection.TryGetValue("X-XSS-Protection", out var xssProtection);
			Assert.That(xssProtection, Is.AssignableFrom<XssProtectionHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("X-Content-Type-Options"), Is.True);
			headerPolicyCollection.TryGetValue("X-Content-Type-Options", out var xContentTypeOptions);
			Assert.That(xContentTypeOptions, Is.AssignableFrom<XContentTypeOptionsHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Referrer-Policy"), Is.True);
			headerPolicyCollection.TryGetValue("Referrer-Policy", out var referrerPolicy);
			Assert.That(referrerPolicy, Is.AssignableFrom<ReferrerPolicyHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Server"), Is.True);
			headerPolicyCollection.TryGetValue("Server", out var server);
			Assert.That(server, Is.AssignableFrom<ServerHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Cross-Origin-Opener-Policy"), Is.True);
			headerPolicyCollection.TryGetValue("Cross-Origin-Opener-Policy", out var crossOriginOpenerPolicy);
			Assert.That(crossOriginOpenerPolicy, Is.InstanceOf<CrossOriginOpenerPolicyHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Cross-Origin-Embedder-Policy"), Is.True);
			headerPolicyCollection.TryGetValue("Cross-Origin-Embedder-Policy", out var crossOriginEmbedderPolicy);
			Assert.That(crossOriginEmbedderPolicy, Is.InstanceOf<CrossOriginEmbedderPolicyHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Cross-Origin-Resource-Policy"), Is.True);
			headerPolicyCollection.TryGetValue("Cross-Origin-Resource-Policy", out var crossOriginResourcePolicy);
			Assert.That(crossOriginResourcePolicy, Is.InstanceOf<CrossOriginResourcePolicyHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Content-Security-Policy"), Is.True);
			headerPolicyCollection.TryGetValue("Content-Security-Policy", out var contentSecurityPolicy);
			Assert.That(contentSecurityPolicy, Is.InstanceOf<ContentSecurityPolicyHeader>());
			
			Assert.That(headerPolicyCollection.ContainsKey("Permissions-Policy"), Is.True);
			headerPolicyCollection.TryGetValue("Permissions-Policy", out var permissionsPolicy);
			Assert.That(permissionsPolicy, Is.InstanceOf<PermissionsPolicyHeader>());

			if (!isDev)
			{
				Assert.That(headerPolicyCollection.ContainsKey("Strict-Transport-Security"), Is.True);
				headerPolicyCollection.TryGetValue("Strict-Transport-Security", out var strictTransportSecurity);
				Assert.That(strictTransportSecurity, Is.InstanceOf<StrictTransportSecurityHeader>());
			}
		}
	}
}