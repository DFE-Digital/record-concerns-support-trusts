## ========================================================================== ##
#  Locals					                                                   #
## ========================================================================== ##
locals {
  aws_region 				= "eu-west-2"
  aws_bucket_name 			= "paas-s3-broker-prod-lon-8f2a7b28-c6e6-439d-b1ca-732a888cbdbb"
  aws_bucket_key 			= "amsd-casework/terraform.tfstate"
  
  app_name_suffix      		= var.app_environment
  web_app_name         		= var.app_environment != "production" ? "amsd-casework-${local.app_name_suffix}" : "amsd-casework"
  web_app_routes       		= cloudfoundry_route.web_app_cloudapp_digital_route
  redis_service_name   		= "amsd-casework-redis-${local.app_name_suffix}"
}