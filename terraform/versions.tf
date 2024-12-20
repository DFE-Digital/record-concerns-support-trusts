terraform {
  required_version = "~> 1.9"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }

    statuscake = {
      source  = "StatusCakeDev/statuscake"
      version = "~> 2.1"
    }

    azapi = {
      source  = "Azure/azapi"
      version = "~> 1.13"
    }
  }
}
