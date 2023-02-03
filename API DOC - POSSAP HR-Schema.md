# Central Billing System API README
***


## Get Officers Report [link](...)
***

## Method Type - POST
***


### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| StateCode                     | string      | Officer command State Code                                   |
| LGACode                       | string      | Officer command LGA Code                                     |
| CommandCode                   | string      | Command code is the key that can uniquely identify a command |
| RankCode                      | string      | Rank code is the key that can uniquely identify a Rank       |
| GenderCode                    | string      | Officer Gender Code                                          |
| ServiceNumber                 | string      | Officer service Number                                       |
| Name                          | string      | Officer name                                                 |
| IPPISNumber                   | string      | Officer IPPIS Number                                         |
| Page                          | int         | Pagination page number                                       |
| PageSize                      | int         | Pagination page size, number of records to be returned       |

### Sample request JSON
```JSON
{
    "StateCode": "STC999",
    "LGACode": "LGC093",
    "CommandCode": "Codexxxx",
    "RankCode": "RCxxxxxx",
    "GenderCode": "GC1",
    "ServiceNumber": "RCxxxxxx",
    "Name": "Ajala Akeem",
    "IPPISNumber": "IPxxxxxx",
    "Page": 1,
    "PageSize": 100
}
```

### Output Parameters [Content-type - JSON]
***
For every API call a standard response object is returned. If there was anything wrong with the request, the Error value would be true.

### Standard API output
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | Indicates if request was processed successfully. If this     |
|                               |             | request was processed successfully the ResponseObject would  |
|                               |             | contain an object detailing the payer info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Command",
            "ErrorMessage": "Unauthorized: Access denied"
        }
    ]
}
```


#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Name                          | string      | Officer name                                                 |
| IPPISNumber                   | string      | Officer IPPIS Number                                         |
| ServiceNumber                 | string      | Officer Service Number                                       |
| PhoneNumber                   | string      | Officer Phone Number                                         |
| GenderName                    | string      | Officer Gender Name                                          |
| GenderCode                    | string      | Officer Gender Code                                          |
| RankName                      | string      | Officer Rank Name                                            |
| RankCode                      | string      | Officer Rank Code                                            |
| CommandName                   | string      | Officer Command Name                                         |    
| CommandCode                   | string      | Officer Command Code                                         |    
| StateName                     | string      | Officer State Name                                           |    
| StateCode                     | string      | Officer State Code                                           |    
| LGAName                       | string      | Officer LGA Name                                             |    
| LGACode                       | string      | Officer LGA Code                                             |    
| DateOfBirth                   | string      | Officer Date Of Birth (dd/MM/yyyy)                           |    
| StateOfOrigin                 | string      | Officer State Of Origin                                      |    
| TotalNumberOfOfficers         | int         | Total number of officers matching the search parameters      |

### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "ReportRecords": [
            {
                "Name": "Testing Name",
                "IPPISNumber": "IP82225",
                "ServiceNumber": "NPF76363",
                "PhoneNumber": "07064955645",
                "Gender": "Male",
                "GenderCode": "GC022",
                "RankName": "Corporal",
                "RankCode": "RC883838",
                "StateName": "Lagos",
                "StateCode": "STC033",
                "CommandName": "Yaba Division",
                "CommandCode": "CC533",
                "LGAName": "Shomolu",
                "LGACode": "LC013",
                "DateOfBirth": "07/04/1982",
                "StateOfOrigin": "Osun State",
                "AccountNumber": "0034543337",
                "BankCode": "058"
            }
        ],
        "SearchFilter": {
            "StateCode": "STC999",
            "LGACode": "LGC093",
            "CommandCode": "Codexxxx",
            "RankCode": "RCxxxxxx",
            "GenderCode": "GC1",
            "ServiceNumber": "RCxxxxxx",
            "Name": "Ajala Akeem",
            "IPPISNumber": "IPxxxxxx",
            "Page": 1,
            "PageSize": 10,
            "TotalNumberOfOfficers": 1
        }
    }
}
```


## Get Ranks [link](...)
***

## Method Type - GET
***


### Output Parameters [Content-type - JSON]
***
For every API call a standard response object is returned. If there was anything wrong with the request, the Error value would be true.

### Standard API output
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | Indicates if request was processed successfully. If this     |
|                               |             | request was processed successfully the ResponseObject would  |
|                               |             | contain an object detailing the payer info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Rank",
            "ErrorMessage": "Unauthorized: Access denied"
        }
    ]
}
```


#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Name                          | string      | Rank Name                                                    |
| Code                          | string      | Rank unique code                                             |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "ReportRecords": [
            {
                "Name": "Lance Corporal",
                "Code": "LC001"
            },
            {
                "Name": "Corporal",
                "Code": "C002"
            }
        ]
    }
}
```


## Get Command Category [link](...)
***

## Method Type - GET
***


### Output Parameters [Content-type - JSON]
***
For every API call a standard response object is returned. If there was anything wrong with the request, the Error value would be true.

### Standard API output
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | Indicates if request was processed successfully. If this     |
|                               |             | request was processed successfully the ResponseObject would  |
|                               |             | contain an object detailing the payer info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "CommandCategory",
            "ErrorMessage": "Unauthorized: Access denied"
        }
    ]
}
```


#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Name                          | string      | Command Category Name                                        |
| Code                          | string      | Command Category Code                                        |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "ReportRecords": [
            {
                "Name": "Zonal Command",
                "Code": "CC001"
            },
            {
                "Name": "State Command",
                "Code": "CC002"
            }
        ]
    }
}
```



## Get Commands [link](...)
***

## Method Type - GET
***


### Output Parameters [Content-type - JSON]
***
For every API call a standard response object is returned. If there was anything wrong with the request, the Error value would be true.

### Standard API output
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | Indicates if request was processed successfully. If this     |
|                               |             | request was processed successfully the ResponseObject would  |
|                               |             | contain an object detailing the payer info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Command",
            "ErrorMessage": "Unauthorized: Access denied"
        }
    ]
}
```


#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Name                          | string      | Command Name                                                 |
| Code                          | string      | Command Code                                                 |
| Address                       | string      | Command Address                                              |
| CategoryCode                  | string      | Command Category Code                                        |
| StateCode                     | string      | State Code                                                   |
| LGACode                       | string      | LGA Code                                                     |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "ReportRecords": [
            {
                "Name": "Shomolu Division",
                "Code": "SD002",
                "Address": "Testing Address",
                "CategoryCode": "C012",
                "StateCode": "STC023",
                "LGACode": "LGC345"
            },
            {
                "Name": "Yaba Division",
                "Code": "YB934",
                "Address": "Testing Address",
                "CategoryCode": "C012",
                "StateCode": "STC023",
                "LGACode": "LGC345"
            }
        ]
    }
}
```

## Get States [link](...)
***

## Method Type - GET
***


### Output Parameters [Content-type - JSON]
***
For every API call a standard response object is returned. If there was anything wrong with the request, the Error value would be true.

### Standard API output
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | Indicates if request was processed successfully. If this     |
|                               |             | request was processed successfully the ResponseObject would  |
|                               |             | contain an object detailing the payer info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "State",
            "ErrorMessage": "Unauthorized: Access denied"
        }
    ]
}
```

#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Name                          | string      | State Name                                                   |
| Code                          | string      | State unique code                                            |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": [
        {
            "Name": "Lagos",
            "Code": "ST001"
        },
        {
            "Name": "Abuja",
            "Code": "ST002"
        },
    ]
}
```
## Get LGAs [link](...)
***

## Method Type - GET
***


### Output Parameters [Content-type - JSON]
***
For every API call a standard response object is returned. If there was anything wrong with the request, the Error value would be true.

### Standard API output
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | Indicates if request was processed successfully. If this     |
|                               |             | request was processed successfully the ResponseObject would  |
|                               |             | contain an object detailing the payer info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "LGA",
            "ErrorMessage": "Unauthorized: Access denied"
        }
    ]
}
```

#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Name                          | string      | LGA Name                                                     |
| Code                          | string      | LGA unique code                                              |
| StateCode                     | string      | State unique code                                            |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": [
        {
            "Name": "Lagos",
            "Code": "LG001",
            "StateCode": "ST001"
        },
        {
            "Name": "Abuja",
            "Code": "LG002",
            "StateCode": "ST002"
        },
    ]
}
```
