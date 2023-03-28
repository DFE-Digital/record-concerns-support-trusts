terraform {
  required_version = ">= 1.4.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.41.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = ">= 1.1.0"
    }
  }
}
