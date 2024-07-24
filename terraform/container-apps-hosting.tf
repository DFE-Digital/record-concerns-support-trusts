module "azure_container_apps_hosting" {
  source = "github.com/DFE-Digital/terraform-azurerm-container-apps-hosting?ref=v1.8.3"

  environment    = local.environment
  project_name   = local.project_name
  azure_location = local.azure_location
  tags           = local.tags

  virtual_network_address_space = local.virtual_network_address_space

  enable_container_registry             = local.enable_container_registry
  registry_admin_enabled                = local.registry_admin_enabled
  registry_use_managed_identity         = local.registry_use_managed_identity
  registry_managed_identity_assign_role = local.registry_managed_identity_assign_role
  registry_server                       = local.registry_server

  enable_mssql_database              = local.enable_mssql_database
  mssql_sku_name                     = local.mssql_sku_name
  mssql_server_admin_password        = local.mssql_server_admin_password
  mssql_database_name                = local.mssql_database_name
  mssql_firewall_ipv4_allow_list     = local.mssql_firewall_ipv4_allow_list
  mssql_azuread_admin_username       = local.mssql_azuread_admin_username
  mssql_azuread_admin_object_id      = local.mssql_azuread_admin_object_id
  mssql_server_public_access_enabled = local.mssql_server_public_access_enabled
  mssql_managed_identity_assign_role = local.mssql_managed_identity_assign_role

  image_name = local.image_name

  container_command                      = local.container_command
  container_secret_environment_variables = local.container_secret_environment_variables
  container_health_probe_path            = local.container_health_probe_path
  container_cpu                          = local.container_cpu
  container_memory                       = local.container_memory
  container_min_replicas                 = local.container_min_replicas
  container_max_replicas                 = local.container_max_replicas
  container_port                         = local.container_port
  container_scale_http_concurrency       = local.container_scale_http_concurrency

  enable_redis_cache   = local.enable_redis_cache
  redis_cache_sku      = local.redis_cache_sku
  redis_cache_capacity = local.redis_cache_capacity

  enable_cdn_frontdoor                            = local.enable_cdn_frontdoor
  cdn_frontdoor_forwarding_protocol               = local.cdn_frontdoor_forwarding_protocol
  cdn_frontdoor_custom_domains                    = local.cdn_frontdoor_custom_domains
  cdn_frontdoor_host_redirects                    = local.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers         = local.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_health_probe_path                 = local.cdn_frontdoor_health_probe_path
  cdn_frontdoor_enable_rate_limiting              = local.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_duration_in_minutes = local.cdn_frontdoor_rate_limiting_duration_in_minutes
  cdn_frontdoor_rate_limiting_threshold           = local.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_origin_fqdn_override              = local.cdn_frontdoor_origin_fqdn_override
  cdn_frontdoor_origin_host_header_override       = local.cdn_frontdoor_origin_host_header_override
  enable_cdn_frontdoor_health_probe               = local.enable_cdn_frontdoor_health_probe

  container_apps_allow_ips_inbound = local.container_apps_allow_ips_inbound

  enable_dns_zone      = local.enable_dns_zone
  dns_zone_domain_name = local.dns_zone_domain_name
  dns_ns_records       = local.dns_ns_records
  dns_txt_records      = local.dns_txt_records
  dns_mx_records       = local.dns_mx_records

  enable_event_hub                          = local.enable_event_hub
  enable_logstash_consumer                  = local.enable_logstash_consumer
  eventhub_export_log_analytics_table_names = local.eventhub_export_log_analytics_table_names

  enable_monitoring            = local.enable_monitoring
  monitor_email_receivers      = local.monitor_email_receivers
  monitor_endpoint_healthcheck = local.monitor_endpoint_healthcheck

  existing_logic_app_workflow                  = local.existing_logic_app_workflow
  existing_network_watcher_name                = local.existing_network_watcher_name
  existing_network_watcher_resource_group_name = local.existing_network_watcher_resource_group_name
}
