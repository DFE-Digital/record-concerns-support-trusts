locals {
  environment                                  = var.environment
  project_name                                 = var.project_name
  azure_location                               = var.azure_location
  tags                                         = var.tags
  virtual_network_address_space                = var.virtual_network_address_space
  enable_container_registry                    = var.enable_container_registry
  image_name                                   = var.image_name
  container_command                            = var.container_command
  container_secret_environment_variables       = var.container_secret_environment_variables
  container_health_probe_path                  = var.container_health_probe_path
  container_cpu                                = var.container_cpu
  container_memory                             = var.container_memory
  container_min_replicas                       = var.container_min_replicas
  container_max_replicas                       = var.container_max_replicas
  enable_redis_cache                           = var.enable_redis_cache
  enable_mssql_database                        = var.enable_mssql_database
  mssql_sku_name                               = var.mssql_sku_name
  mssql_server_admin_password                  = var.mssql_server_admin_password
  mssql_database_name                          = var.mssql_database_name
  mssql_firewall_ipv4_allow_list               = var.mssql_firewall_ipv4_allow_list
  mssql_azuread_admin_username                 = var.mssql_azuread_admin_username
  mssql_azuread_admin_object_id                = var.mssql_azuread_admin_object_id
  mssql_server_public_access_enabled           = var.mssql_server_public_access_enabled
  redis_cache_sku                              = var.redis_cache_sku
  redis_cache_capacity                         = var.redis_cache_capacity
  enable_cdn_frontdoor                         = var.enable_cdn_frontdoor
  enable_dns_zone                              = var.enable_dns_zone
  dns_zone_domain_name                         = var.dns_zone_domain_name
  dns_ns_records                               = var.dns_ns_records
  dns_txt_records                              = var.dns_txt_records
  container_apps_allow_ips_inbound             = var.container_apps_allow_ips_inbound
  cdn_frontdoor_custom_domains                 = var.cdn_frontdoor_custom_domains
  cdn_frontdoor_host_redirects                 = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers      = var.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_health_probe_path              = var.cdn_frontdoor_health_probe_path
  cdn_frontdoor_enable_rate_limiting           = var.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_threshold        = var.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_origin_fqdn_override           = var.cdn_frontdoor_origin_fqdn_override
  cdn_frontdoor_origin_host_header_override    = var.cdn_frontdoor_origin_host_header_override
  cdn_frontdoor_forwarding_protocol            = var.cdn_frontdoor_forwarding_protocol
  key_vault_access_users                       = toset(var.key_vault_access_users)
  key_vault_access_ipv4                        = var.key_vault_access_ipv4
  tfvars_filename                              = var.tfvars_filename
  enable_event_hub                             = var.enable_event_hub
  enable_logstash_consumer                     = var.enable_logstash_consumer
  eventhub_export_log_analytics_table_names    = var.eventhub_export_log_analytics_table_names
  enable_monitoring                            = var.enable_monitoring
  monitor_email_receivers                      = var.monitor_email_receivers
  monitor_endpoint_healthcheck                 = var.monitor_endpoint_healthcheck
  existing_logic_app_workflow                  = var.existing_logic_app_workflow
  existing_network_watcher_name                = var.existing_network_watcher_name
  existing_network_watcher_resource_group_name = var.existing_network_watcher_resource_group_name
  statuscake_api_token                         = var.statuscake_api_token
  statuscake_monitored_resource_address        = var.statuscake_monitored_resource_address == "" ? "https://${local.dns_zone_domain_name}${local.monitor_endpoint_healthcheck}" : var.statuscake_monitored_resource_address
  statuscake_contact_group_name                = var.statuscake_contact_group_name
  statuscake_contact_group_integrations        = var.statuscake_contact_group_integrations
  statuscake_contact_group_email_addresses     = var.statuscake_contact_group_email_addresses
}
