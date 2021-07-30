## =============================================================================
#  Configure the AWS Provider                                                  #
## =============================================================================

# Provides the AWS account ID to other resources
# Interpolate: data.aws_caller_identity.current.account_id
data "aws_caller_identity" "current" {}

terraform {
  required_providers {
	aws = "~> 2.59"
	cloudfoundry = {
	  source  = "cloudfoundry-community/cloudfoundry"
	  version = ">= 0.12.6"
	}
  }
  ## ========================================================================== ##
  #  Terraform State S3 Bucket                                                   #
  ## ========================================================================== ##
  backend s3 {
	bucket 	= var.aws_bucket_name
	key    	= var.aws_bucket_key
	region 	= var.aws_region
	encrypt = var.aws_bucket_state_encrypt
  }
}

provider "aws" {
  region = var.aws_region
}

provider cloudfoundry {
  api_url           = var.cf_api_url
  user              = var.cf_user
  password          = var.cf_password
}

