##  Terraform State S3 Bucket
aws_region 					= "eu-west-2"
aws_bucket_name 			= "paas-s3-broker-prod-lon-8f2a7b28-c6e6-439d-b1ca-732a888cbdbb"
aws_bucket_key 				= "amsd-casework/terraform.tfstate"
aws_bucket_state_encrypt 	= true

## PaaS
cf_api_url 					= "https://api.london.cloud.service.gov.uk"
cf_user 					= "paulo.lancao@education.gov.uk"
cf_password 				= "eqF5RnnpM6Nh"
cf_space 					= "amsd-casework-dev"
cf_redis_service_plan		= "micro-5_x"