# Configuring Clockify-it

You can configure Clockify-it using different methods. I recommend using a json file since it's the most reliable and readable way. 

Here's a annotated example configuration:

```json
{
  "Redis": {
    "IsEnabled": false
  },
  "Workspaces": {
    "Default": {
      "ApiUrl": "https://api.clockify.me/api/v1",
      "ExperimentalApiUrl": "https://api.clockify.me/api/",
      "ReportsApiUrl": "https://reports.api.clockify.me/v1", 
      "Delay": "00.00:00:00", // Configure a treshold not to process entries; can be used to make entries editable for a given amount of time afterwards
      "FetchRange": "1.00:00:00", // Configure a interval to fetch entries in the past, minus the configured `Delay`
      "Integrations": [ // Define your integrations; the name can be freely chosen. See the `Integrations` section
        { "Name": "Redmine" },
        { "Name": "DevOpsWork" },
        { "Name": "DevOpsPrivate" }
      ]
    }
  },
  "Integrations": {
    "Redmine": {
      "Name": "Redmine@0", // Specify the integration to use
      "Args": {
        "Host": "http://localhost:8080", // Your redmine host
        "ApiKey": "01234567890123456789012345678901234567890", // Your redmine API key
        "VerifyServerCert": true, // Disable if you have self signed certs
        "Activities": { // Map Clockify tags to redmine activity IDs; you can use `0` to use the default activity as configured in your redmine instance
          "Default": 9,
          "design": 8 // The tag `design` will lead to the activity `8` in redmine
        },
        "Tags": [   // You can specify tag groups to filter the time entries which this integration should process
                    // Each group consists of an array of tags. All tags in a array must match. Groups are `OR` combined.
          [ "to-redmine" ],
          [ "redmine" ]
        ],
        "ProcessedTags": [ // Specify an array of tags to add after successful processing by this integration
          "in-redmine"
        ],
        "ErrorTags": [ // Specify an array of tags to add in case an error occured
          "failed-verify-manually"
        ]
      }
    },
    "DevOpsWork": {
      "Name": "RedmineOverDevOps@0", // Specify the integration to use
      "Args": {
        "Redmine": { // Take a look at the redmine integration options
          "Host": "http://localhost:8080",
          "ApiKey": "01234567890123456789012345678901234567890",
          "VerifyServerCert": true,
          "Activities": {
            "Default": 9,
            "design": 8
          }
        },
        "Host": "https://dev.azure.com/WORK-COLLECTION", // The url to your azure devops organization / collection
        "PAT": "01234567890123456789012345678901234567890123456789", // your PAT
        "KeyComment": "USE-THIS-LINK",
        "Tags": [
          [ "to-devops", "spcurrent" ]
        ],
        "ProcessedTags": [
          "in-devops"
        ],
        "ErrorTags": [
          "failed-verify-manually"
        ]
      }
    },
    "DevOpsPrivate": {
      "Name": "RedmineOverDevOps@0",
      "Args": {
        "Redmine": {
          "Host": "http://localhost:8080",
          "ApiKey": "01234567890123456789012345678901234567890",
          "VerifyServerCert": true,
          "Activities": {
            "Default": 9,
            "design": 8
          }
        },
        "Host": "https://lisdevops.lisdom.lan/LIS",
        "PAT": "01234567890123456789012345678901234567890123456789",
        "Tags": [
          [ "to-devops", "sp12" ]
        ],
        "ProcessedTags": [
          "in-devops"
        ],
        "ErrorTags": [
          "failed-verify-manually"
        ]
      }
    }
  }
}
```

## Docker

When using Docker, make sure to mount your configuration file to `/app/appsettings.Production.json`.