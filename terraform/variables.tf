variable "environment" {
  description = "Environment name. Will be used along with `project_name` as a prefix for all resources."
  type        = string
}

variable "key_vault_access_ipv4" {
  description = "List of IPv4 Addresses that are permitted to access the Key Vault"
  type        = list(string)
}

variable "container_port" {
  description = "Container port"
  type        = number
  default     = 8080
}

variable "tfvars_filename" {
  description = "tfvars filename. This file is uploaded and stored encrupted within Key Vault, to ensure that the latest tfvars are stored in a shared place."
  type        = string
}

variable "project_name" {
  description = "Project name. Will be used along with `environment` as a prefix for all resources."
  type        = string
}

variable "azure_location" {
  description = "Azure location in which to launch resources."
  type        = string
}

variable "tags" {
  description = "Tags to be applied to all resources"
  type        = map(string)
}

variable "virtual_network_address_space" {
  description = "Virtual network address space CIDR"
  type        = string
}

variable "enable_container_registry" {
  description = "Set to true to create a container registry"
  type        = bool
}

variable "registry_admin_enabled" {
  description = "Do you want to enable access key based authentication for your Container Registry?"
  type        = bool
  default     = false
}

variable "registry_use_managed_identity" {
  description = "Create a User-Assigned Managed Identity for the Container App. Note: If you do not have 'Microsoft.Authorization/roleAssignments/write' permission, you will need to manually assign the 'AcrPull' Role to the identity"
  type        = bool
  default     = true
}

variable "registry_managed_identity_assign_role" {
  description = "Assign the 'AcrPull' Role to the Container App User-Assigned Managed Identity. Note: If you do not have 'Microsoft.Authorization/roleAssignments/write' permission, you will need to manually assign the 'AcrPull' Role to the identity"
  type        = bool
  default     = false
}

variable "image_name" {
  description = "Image name"
  type        = string
}

variable "container_command" {
  description = "Container command"
  type        = list(any)
}

variable "container_secret_environment_variables" {
  description = "Container secret environment variables"
  type        = map(string)
  sensitive   = true
}

variable "container_scale_http_concurrency" {
  description = "When the number of concurrent HTTP requests exceeds this value, then another replica is added. Replicas continue to add to the pool up to the max-replicas amount."
  type        = number
  default     = 10
}

variable "enable_redis_cache" {
  description = "Set to true to create a Redis Cache"
  type        = bool
}

variable "enable_cdn_frontdoor" {
  description = "Enable Azure CDN FrontDoor. This will use the Container Apps endpoint as the origin."
  type        = bool
  default     = false
}

variable "container_apps_allow_ips_inbound" {
  description = "Restricts access to the Container Apps by creating a network security group rule that only allow inbound traffic from the provided list of IPs"
  type        = list(string)
  default     = []
}

variable "enable_dns_zone" {
  description = "Conditionally create a DNS zone"
  type        = bool
}

variable "dns_zone_domain_name" {
  description = "DNS zone domain name. If created, records will automatically be created to point to the CDN."
  type        = string
}

variable "dns_ns_records" {
  description = "DNS NS records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
}

variable "dns_txt_records" {
  description = "DNS TXT records to add to the DNS Zone"
  type = map(
    object({
      ttl : optional(number, 300),
      records : list(string)
    })
  )
}

variable "cdn_frontdoor_custom_domains" {
  description = "Azure CDN Front Door custom domains. If they are within the DNS zone (optionally created), the Validation TXT records and ALIAS/CNAME records will be created"
  type        = list(string)
}

variable "cdn_frontdoor_host_redirects" {
  description = "CDN Front Door host redirects `[{ \"from\" = \"example.com\", \"to\" = \"www.example.com\" }]`"
  type        = list(map(string))
  default     = []
}

variable "cdn_frontdoor_host_add_response_headers" {
  description = "List of response headers to add at the CDN Front Door `[{ \"name\" = \"Strict-Transport-Security\", \"value\" = \"max-age=31536000\" }]`"
  type        = list(map(string))
}

variable "cdn_frontdoor_forwarding_protocol" {
  description = "Azure CDN Front Door forwarding protocol"
  type        = string
  default     = "HttpsOnly"
}

variable "redis_cache_sku" {
  description = "Redis Cache SKU"
  type        = string
}

variable "redis_cache_capacity" {
  description = "Redis Cache Capacity"
  type        = number
}

variable "enable_monitoring" {
  description = "Create App Insights monitoring groups for the container app"
  type        = bool
}

variable "monitor_email_receivers" {
  description = "A list of email addresses that will receive alerts from App Insights"
  type        = list(string)
}

variable "enable_event_hub" {
  description = "Send Azure Container App logs to an Event Hub sink"
  type        = bool
  default     = false
}

variable "enable_logstash_consumer" {
  description = "Create an Event Hub consumer group for Logstash"
  type        = bool
  default     = false
}

variable "eventhub_export_log_analytics_table_names" {
  description = "List of Log Analytics table names that you want to export to Event Hub. See https://learn.microsoft.com/en-gb/azure/azure-monitor/logs/logs-data-export?tabs=portal#supported-tables for a list of supported tables"
  type        = list(string)
  default     = []
}

variable "monitor_endpoint_healthcheck" {
  description = "Specify a route that should be monitored for a 200 OK status"
  type        = string
}

variable "container_health_probe_path" {
  description = "Specifies the path that is used to determine the liveness of the Container"
  type        = string
}

variable "cdn_frontdoor_health_probe_path" {
  description = "Specifies the path relative to the origin that is used to determine the health of the origin."
  type        = string
}

variable "container_cpu" {
  description = "Number of container CPU cores"
  type        = number
}

variable "container_memory" {
  description = "Container memory in GB"
  type        = number
}

variable "container_min_replicas" {
  description = "Container min replicas"
  type        = number
}

variable "container_max_replicas" {
  description = "Container max replicas"
  type        = number
}

variable "cdn_frontdoor_enable_rate_limiting" {
  description = "Enable CDN Front Door Rate Limiting. This will create a WAF policy, and CDN security policy. For pricing reasons, there will only be one WAF policy created."
  type        = bool
}

variable "cdn_frontdoor_rate_limiting_threshold" {
  description = "Number of connection requests per minute threshold for the CDN Rate Limiting policy"
  type        = number
}

variable "existing_logic_app_workflow" {
  description = "Name, and Resource Group of an existing Logic App Workflow. Leave empty to create a new Resource"
  type = object({
    name : string
    resource_group_name : string
  })
  default = {
    name                = ""
    resource_group_name = ""
  }
}

variable "existing_network_watcher_name" {
  description = "Use an existing network watcher to add flow logs."
  type        = string
}

variable "existing_network_watcher_resource_group_name" {
  description = "Existing network watcher resource group."
  type        = string
}

variable "enable_mssql_database" {
  description = "Set to true to create an Azure SQL server/database, with a private endpoint within the virtual network"
  type        = bool
}

variable "mssql_sku_name" {
  description = "Specifies the name of the SKU used by the database"
  type        = string
  default     = "Basic"
}

variable "mssql_server_admin_password" {
  description = "The administrator password for the MSSQL server. Must be set if `enable_mssql_database` is true"
  type        = string
  sensitive   = true
}

variable "mssql_database_name" {
  description = "The name of the MSSQL database to create. Must be set if `enable_mssql_database` is true"
  type        = string
}

variable "mssql_server_public_access_enabled" {
  description = "Enable public internet access to your MSSQL instance. Be sure to specify 'mssql_firewall_ipv4_allow_list' to restrict inbound connections"
  type        = bool
  default     = false
}

variable "mssql_firewall_ipv4_allow_list" {
  description = "A list of IPv4 Addresses that require remote access to the MSSQL Server"
  type        = list(string)
  default     = []
}

variable "mssql_azuread_admin_username" {
  description = "Username of a User within Azure AD that you want to assign as the SQL Server Administrator"
  type        = string
  default     = ""
}

variable "mssql_azuread_admin_object_id" {
  description = "Object ID of a User within Azure AD that you want to assign as the SQL Server Administrator"
  type        = string
  default     = ""
}

variable "cdn_frontdoor_origin_host_header_override" {
  description = "Manually specify the host header that the CDN sends to the target. Defaults to the recieved host header. Set to null to set it to the host_name (`cdn_frontdoor_origin_fqdn_override`)"
  type        = string
  default     = null
}

variable "cdn_frontdoor_origin_fqdn_override" {
  description = "Manually specify the hostname that the CDN Front Door should target. Defaults to the Container App FQDN"
  type        = string
  default     = ""
}

variable "statuscake_api_token" {
  description = "API token for StatusCake"
  type        = string
  sensitive   = true
  default     = "00000000000000000000000000000"
}

variable "statuscake_contact_group_name" {
  description = "Name of the contact group in StatusCake"
  type        = string
  default     = ""
}

variable "statuscake_contact_group_integrations" {
  description = "List of Integration IDs to connect to your Contact Group"
  type        = list(string)
  default     = []
}

variable "statuscake_monitored_resource_addresses" {
  description = "The URLs to perform TLS checks on"
  type        = list(string)
  default     = []
}

variable "statuscake_contact_group_email_addresses" {
  description = "List of email address that should receive notifications from StatusCake"
  type        = list(string)
  default     = []
}

variable "mssql_managed_identity_assign_role" {
  description = "Assign the 'Storage Blob Data Contributor' Role to the SQL Server User-Assigned Managed Identity. Note: If you do not have 'Microsoft.Authorization/roleAssignments/write' permission, you will need to manually assign the 'Storage Blob Data Contributor' Role to the identity"
  type        = bool
  default     = false
}
