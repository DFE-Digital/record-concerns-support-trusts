namespace ConcernsCaseWork.API.UseCases
{
    public class ApiKeyValidationService : IUseCase<string, bool>
    {
        private readonly IConfiguration _configuration;

        public ApiKeyValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Execute(string keyToValidate)
        {
	        var keyExists = _configuration
		        .GetSection("ConcernsCaseworkApi:ApiKeys")
		        .AsEnumerable()
		        .Any(k => k.Value == keyToValidate);
	        
	        return keyExists;
        }
    }
}