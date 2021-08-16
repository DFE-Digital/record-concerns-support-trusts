## ========================================================================== ##
#  Locals					                                                   #
## ========================================================================== ##
locals {
  app_name_suffix      		= var.app_environment
  web_app_name         		= var.app_environment != "production" ? "amsd-casework-${local.app_name_suffix}" : "amsd-casework"
  web_app_routes       		= cloudfoundry_route.web_app_cloudapp_digital_route
  redis_service_name   		= "amsd-casework-redis-${local.app_name_suffix}"
  docker_image         		= "ghcr.io/dfe-digital/amsd-casework:${var.cf_app_image_tag}"
  cloudfoundry_app_strategy	= "blue-green-v2"
  cloudfoundry_app_urls		= "http://+:8080"
  
  ## Created manually see README.md
  aws_service_key			= "amsd-casework-tf-state"
}