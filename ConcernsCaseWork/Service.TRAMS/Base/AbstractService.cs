using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Base
{
	/// <summary>
	/// Abstract base class for services which call the Trams/Academies API.
	/// Provides basic implementations of methods so they don't have to be implemented per specialized service.
	/// </summary>
	public abstract class AbstractService
	{
		internal readonly IHttpClientFactory ClientFactory;
		private readonly ILogger<AbstractService> _logger;
		internal const string HttpClientName = "TramsClient";
		internal const string EndpointsVersion = "v2";
		internal const string EndpointPrefix = "concerns-cases";

		protected AbstractService(IHttpClientFactory clientFactory, ILogger<AbstractService> logger)
		{
			ClientFactory = Guard.Against.Null(clientFactory);
			_logger = Guard.Against.Null(logger);
		}

		public Task<T> Get<T>(string endpoint, bool treatNoContentAsError = false) where T : class 
		{
			Guard.Against.NullOrWhiteSpace(endpoint);

			async Task<T> DoWork()
			{
				try
				{
					// Create a request
					var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

					// Create http client
					var client = ClientFactory.CreateClient(HttpClientName);

					// Execute request
					var response = await client.SendAsync(request);

					// Check status code
					response.EnsureSuccessStatusCode();

					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						if (treatNoContentAsError)
						{
							throw new ArgumentNullException(nameof(response.Content),
								$"A GET from endpoint '{endpoint}' resulted in a NoContent response. Exception thrown because {nameof(treatNoContentAsError)} is 'true'");
						}

						return default;
					}

					// Read response content
					var content = await response.Content.ReadAsStringAsync();

					// Deserialize content to POCO
					var apiWrapperRecordsDto = JsonConvert.DeserializeObject<ApiWrapper<T>>(content);

					// Unwrap response
					if (apiWrapperRecordsDto is { Data: { } })
					{
						return apiWrapperRecordsDto.Data;
					}

					throw new Exception("Academies API error unwrap response");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					throw;
				}
			}

			return DoWork();
		}

		public Task<ApiListWrapper<T>> GetByPagination<T>(string endpoint, bool treatNoContentAsError = false) where T : class 
		{
			Guard.Against.NullOrWhiteSpace(endpoint);

			async Task<ApiListWrapper<T>> DoWork()
			{
				try
				{
					// Create a request
					var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

					// Create http client
					var client = ClientFactory.CreateClient(HttpClientName);

					// Execute request
					var response = await client.SendAsync(request);

					// Check status code
					response.EnsureSuccessStatusCode();

					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						if (treatNoContentAsError)
						{
							throw new ArgumentNullException(nameof(response.Content),
								$"A GET from endpoint '{endpoint}' resulted in a NoContent response. Exception thrown because {nameof(treatNoContentAsError)} is 'true'");
						}

						return default;
					}

					// Read response content
					var content = await response.Content.ReadAsStringAsync();

					// Deserialize content to POCO
					return JsonConvert.DeserializeObject<ApiListWrapper<T>>(content);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					throw;
				}
			}

			return DoWork();
		}

		/// <summary>
		/// Sends a PUT message with T as the request content. Returns no response from message body but checks the response code is in the 200 range.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="endpoint"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public Task Put<T>(string endpoint, T dto)
		{
			Guard.Against.NullOrWhiteSpace(endpoint);
			Guard.Against.Null(dto);

			async Task DoWork()
			{
				try
				{
					Guard.Against.Null(dto);

					// Create a request
					var request = new StringContent(
						JsonConvert.SerializeObject(dto),
						Encoding.UTF8,
						MediaTypeNames.Application.Json);

					// Create http client
					var client = ClientFactory.CreateClient(HttpClientName);

					// Execute request
					var response = await client.PutAsync(endpoint, request);

					// Check status code
					response.EnsureSuccessStatusCode();

					// Read response content
					var content = await response.Content.ReadAsStringAsync();

					// Deserialize content to POCO
					var apiWrapperRecordsDto = JsonConvert.DeserializeObject<ApiWrapper<T>>(content);

					// Unwrap response
					if (apiWrapperRecordsDto is { Data: { } })
					{
						return;
					}

					throw new Exception($"Academies API error unwrap response. Endpoint '{endpoint}' returned a non-success code ({response.StatusCode})");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					throw;
				}
			}

			return DoWork();
		}

		/// <summary>
		/// Sends a PUT message with T as the request content. Returns a TResponse from message body and also checks (first) that the response code is in the 200 range.
		/// Note Prefer to PUT/POST without returning a body
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="endpoint"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		public Task<TResult> Put<T, TResult>(string endpoint, T dto, bool treatNoContentAsError = false)
		{
			Guard.Against.NullOrWhiteSpace(endpoint);
			Guard.Against.Null(dto);

			async Task<TResult> DoWork()
			{
				try
				{

					// Create a request
					var request = new StringContent(
						JsonConvert.SerializeObject(dto),
						Encoding.UTF8,
						MediaTypeNames.Application.Json);

					// Create http client
					var client = ClientFactory.CreateClient(HttpClientName);

					// Execute request
					var response = await client.PutAsync(endpoint, request);

					// Check status code
					response.EnsureSuccessStatusCode();

					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						if (treatNoContentAsError)
						{
							throw new ArgumentNullException(nameof(response.Content),
								$"A PUT to endpoint '{endpoint}' resulted in a NoContent response. Exception thrown because {nameof(treatNoContentAsError)} is 'true'");
						}

						return default;
					}

					// Read response content
					var content = await response.Content.ReadAsStringAsync();

					// Deserialize content to POCO
					var resultDto = JsonConvert.DeserializeObject<ApiWrapper<TResult>>(content);

					// Unwrap response
					if (resultDto is { Data: { } })
					{
						return resultDto.Data;
					}

					throw new Exception($"Academies API error unwrap response. Endpoint '{endpoint}' returned a non-success code ({response.StatusCode})");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					throw;
				}
			}

			return DoWork();
		}

		/// <summary>
		/// Sends a POST, with no result from response body. Ensures the response code is in the 200 range
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="endpoint"></param>
		/// <param name="dto"></param>
		/// <returns>Task</returns>
		public Task Post<T>(string endpoint, T dto)
		{
			Guard.Against.NullOrWhiteSpace(endpoint);
			Guard.Against.Null(dto);

			async Task DoWork()
			{
				try
				{
					// Create a request
					var request = new StringContent(
						JsonConvert.SerializeObject(dto),
						Encoding.UTF8,
						MediaTypeNames.Application.Json);

					// Create http client
					var client = ClientFactory.CreateClient(HttpClientName);

					// Execute request
					var response = await client.PostAsync(endpoint, request);

					// Check status code
					response.EnsureSuccessStatusCode();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					throw;
				}
			}

			return DoWork();
		}

		/// <summary>
		/// Sends a POST, with result deserialized from response body. Ensures the response code is in the 200 range before deserializing
		/// Note Prefer to PUT/POST without returning a body
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="endpoint"></param>
		/// <param name="dto"></param>
		/// <param name="treatNoContentAsError"></param>
		/// <returns>Task of TResult</returns>
		public Task<TResult> Post<T, TResult>(string endpoint, T dto, bool treatNoContentAsError = false)
		{
			Guard.Against.NullOrWhiteSpace(endpoint);
			Guard.Against.Null(dto);

			async Task<TResult> DoWork()
			{
				try
				{
					// Create a request
					var request = new StringContent(
						JsonConvert.SerializeObject(dto),
						Encoding.UTF8,
						MediaTypeNames.Application.Json);

					// Create http client
					var client = ClientFactory.CreateClient(HttpClientName);

					// Execute request
					var response = await client.PostAsync(endpoint, request);

					// Check status code
					response.EnsureSuccessStatusCode();

					if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
					{
						if (treatNoContentAsError)
						{
							throw new ArgumentNullException(nameof(response.Content),
								$"A POST to endpoint '{endpoint}' resulted in a NoContent response. Exception thrown because {nameof(treatNoContentAsError)} is 'true'");
						}

						return default;
					}

					// Read response content
					var content = await response.Content.ReadAsStringAsync();

					// Deserialize content to POJO
					var resultDto = JsonConvert.DeserializeObject<ApiWrapper<TResult>>(content);

					// Unwrap response
					if (resultDto is { Data: { } })
					{
						return resultDto.Data;
					}

					throw new Exception("Academies API error unwrap response");
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
					throw;
				}
			}

			return DoWork();
		}
	}
}