﻿using Microsoft.AspNetCore.Builder;

namespace ConcernsCaseWork.Security
{
	public static class SecurityHeadersDefinitions
	{
		public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev)
		{
			var policy = new HeaderPolicyCollection()
				.AddFrameOptionsDeny()
				.AddXssProtectionDisabled()
				.AddContentTypeOptionsNoSniff()
				.AddReferrerPolicyStrictOriginWhenCrossOrigin()
				.RemoveServerHeader()
				.AddCrossOriginOpenerPolicy(builder =>
				{
					builder.SameOrigin();
				})
				.AddCrossOriginEmbedderPolicy(builder =>
				{
					builder.RequireCorp();
				})
				.AddCrossOriginResourcePolicy(builder =>
				{
					builder.SameOrigin();
				})
				.AddContentSecurityPolicy(builder =>
				{
					builder.AddObjectSrc().None();
					builder.AddBlockAllMixedContent();
					builder.AddImgSrc().Self().From("data:");
					builder.AddFormAction().Self();
					builder.AddStyleSrc().Self().UnsafeInline();
					builder.AddBaseUri().Self();
					builder.AddScriptSrc().Self()
						.From("https://www.googletagmanager.com")
						.From("https://js.monitor.azure.com")
						.WithNonce();

					builder.AddFrameAncestors().None();
				})
				.AddPermissionsPolicy(builder =>
				{
					builder.AddAccelerometer().None();
					builder.AddAutoplay().None();
					builder.AddCamera().None();
					builder.AddEncryptedMedia().None();
					builder.AddFullscreen().All();
					builder.AddGeolocation().None();
					builder.AddGyroscope().None();
					builder.AddMagnetometer().None();
					builder.AddMicrophone().None();
					builder.AddMidi().None();
					builder.AddPayment().None();
					builder.AddPictureInPicture().None();
					builder.AddSyncXHR().None();
					builder.AddUsb().None();
				});

			return policy;
		}
	}
}
