locals {
  environment                             = var.environment
  project_name                            = var.project_name
  azure_location                          = var.azure_location
  tags                                    = var.tags
  virtual_network_address_space           = var.virtual_network_address_space
  enable_container_registry               = var.enable_container_registry
  image_name                              = var.image_name
  container_command                       = var.container_command
  container_secret_environment_variables  = var.container_secret_environment_variables
  enable_mssql_database                   = var.enable_mssql_database
  enable_redis_cache                      = var.enable_redis_cache
  redis_cache_sku                         = var.redis_cache_sku
  redis_cache_capacity                    = var.redis_cache_capacity
  enable_cdn_frontdoor                    = var.enable_cdn_frontdoor
  enable_dns_zone                         = var.enable_dns_zone
  dns_zone_domain_name                    = var.dns_zone_domain_name
  cdn_frontdoor_custom_domains            = var.cdn_frontdoor_custom_domains
  cdn_frontdoor_host_redirects            = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers = var.cdn_frontdoor_host_add_response_headers
  key_vault_access_users                  = toset(var.key_vault_access_users)
  tfvars_filename                         = var.tfvars_filename
}
