resource cloudfoundry_app worker_app {
  name               = local.web_app_name
  space              = data.cloudfoundry_space.space.id
  docker_image		 = local.docker_image
  strategy           = local.cloudfoundry_app_strategy
  
  service_binding {
		service_instance = cloudfoundry_service_instance.redis.id
	}

  service_binding {
		service_instance = cloudfoundry_user_provided_service.logit.id
	}
  

  routes {
	route = cloudfoundry_route.web_app_cloudapp_digital_route.id
  }

  environment = {
	"ASPNETCORE_ENVIRONMENT" 	= local.app_name_suffix
	"ASPNETCORE_URLS"        	= local.cloudfoundry_app_urls
	"TRAMS:API_ENDPOINT"     	= var.app_trams_api_endpoint
	"TRAMS:API_KEY"          	= var.app_trams_api_key
	"APP:USERNAME"				= var.app_username
	"APP:PASSWORD"				= var.app_password
  }
}

resource cloudfoundry_route web_app_cloudapp_digital_route {
  domain   = data.cloudfoundry_domain.london_cloud_apps_digital.id
  space    = data.cloudfoundry_space.space.id
  hostname = local.web_app_name
}

resource cloudfoundry_service_instance redis {
  name         = local.redis_service_name
  space        = data.cloudfoundry_space.space.id
  service_plan = data.cloudfoundry_service.redis.service_plans[var.cf_redis_service_plan]
}

resource cloudfoundry_user_provided_service logit {
	name             = local.logit_service_name
	space            = data.cloudfoundry_space.space.id
	syslog_drain_url = var.logit_sink_url
}