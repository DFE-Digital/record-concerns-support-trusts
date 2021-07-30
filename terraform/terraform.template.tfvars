##  Terraform State S3 Bucket
aws_region 					= "eu-west-2"
aws_bucket_name 			= ""
aws_bucket_key 				= "amsd-casework/terraform.tfstate"
aws_bucket_state_encrypt 	= true

## Environment
app_environment				= "development"

## PaaS
cf_api_url 					= "https://api.london.cloud.service.gov.uk"
cf_user 					= ""
cf_password 				= ""
cf_space 					= "amsd-casework-dev"
cf_redis_service_plan		= "micro-5_x"