﻿## ========================================================================== ##
#  Terraform State S3 Bucket                                                   #
## ========================================================================== ##
variable aws_region {
  type        = string
  description = "Default region for root module"
}

variable aws_bucket_name {
  type        = string
  description = "S3 Bucket name"
}

variable aws_bucket_key {
  type			= string
  description 	= "The Terraform state is written to the key"
}

variable aws_bucket_state_encrypt {
  type			= bool
  description 	= "Enable server side encryption of the state file"
}

## ========================================================================== ##
#  PaaS						                                                   #
## ========================================================================== ##
variable cf_api_url {
  type			= string
  description 	= "Cloud Foundry api url"
}

variable cf_user {
  type			= string
  description 	= "Cloud Foundry user"
}

variable cf_password {
  type			= string
  description 	= "Cloud Foundry password"
}

variable cf_space {
  type			= string
  description 	= "Cloud Foundry space"
}

variable cf_redis_service_plan {
  type			= string
  description 	= "Cloud Foundry redis service plan"
}

## ========================================================================== ##
#  Environment				                                                   #
## ========================================================================== ##
variable app_environment {
  type			= string
  description 	= "Application environment development, staging, production"
}