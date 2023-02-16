# Performance Testing with K6

## Installing k6
Go to the k6 site and download the installer

https://github.com/grafana/k6/releases

## Create a config.json, with the following structure

```
{
    "url": "<insert url>",
    "cookie": "<insert cookie>",
    "username": "<insert username>"
}
```

url - the url of the application you are performance testing

cookie - the cookie used to login to the record concerns and support for trusts service

username - the user to run requests as

## Executing the scripts
1. Navigate to the scripts folder
2. Run the command `k6 run <scriptName.js>`
3. Inspect the output
4. Change any parameters within the script to nail down any performance issues
