## =============================================================================
#  Configure the AWS Provider                                                  #
## =============================================================================

# Provides the AWS account ID to other resources
# Interpolate: data.aws_caller_identity.current.account_id
data "aws_caller_identity" "current" {}

terraform {
  required_version = "~> 0.14"
  required_providers {
	aws = {
	  source  = "hashicorp/aws"
	  version = "~> 3.0"
	}
	cloudfoundry = {
	  source  = "cloudfoundry-community/cloudfoundry"
	  version = ">= 0.12.6"
	}
  }
  ## ========================================================================== ##
  #  Terraform State S3 Bucket                                                   #
  ## ========================================================================== ##
  backend s3 {
	bucket 	= local.aws_bucket_name
	key    	= local.aws_bucket_key
	region 	= local.aws_region
	encrypt = true
  }
}

provider aws {
  region 	= var.aws_region
  shared_credentials_file = "~/.aws/credentials"
  profile = "dfe"
}

provider cloudfoundry {
  api_url	= var.cf_api_url
  user      = var.cf_user
  password  = var.cf_password
}
