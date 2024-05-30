terraform {
  required_version = ">= 1.6.1"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 3.52.0"
    }
    statuscake = {
      source  = "StatusCakeDev/statuscake"
      version = ">= 2.1.0"
    }
    azapi = {
      source  = "Azure/azapi"
      version = ">= 1.13.0"
    }
  }
}
