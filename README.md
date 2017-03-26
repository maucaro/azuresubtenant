# azuresubtenant
Azure Functions project invoked by a WebHook that receives a JSON array of Azure Subscription IDs in the form of 
[
{"SubscriptionGuid": "12345678-aadd-1234-123456789012"},
{"SubscriptionGuid": "12345678-aadd-1234-123456789013"},
....
]

and returns the subscriptions with their corresponding AAD Tenant in the form of:
[
{"SubscriptionGuid": "12345678-aadd-1234-123456789012", "AADTenant": "abcdefgh-1234-5678-234567890123"},
{"SubscriptionGuid": "12345678-aadd-1234-123456789013", "AADTenant": "abcdefgh-1234-5678-234567890124"},
....
]

Implements the technique described here:
http://stackoverflow.com/questions/26384034/how-to-get-the-azure-account-tenant-id
