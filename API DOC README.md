# Central Billing System API README
***

Change History

| Revision   | Developer           | Description                               | Date        |
|----------- |:-------------------:|------------------------------------------:|------------:|
| 0.00       | Ifetayo             | DOC Initiation                            | 5/4/2018    |
| 1.00       | Opeyemi             | Added PAYE schema to API DOC Readme       | 30/06/2021  |


## Invoice Validation [link](http://uat.nasarawaigr.com/api/v1/invoice/validate)
***

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the concatenation of InvoiceNumber and    |
|                               |             | ClientId                                                     |
| CLIENTID                      | string      | client Id                                                    |

Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| InvoiceNumber                 | string      | Invoice number you want to base the search on                |


### Sample request JSON
```JSON
{
  "InvoiceNumber": "add invoice number",
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
|                               |             | contain an object detailing the invoice info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the invoice details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |

#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| ResponseCode                  | string      | Response code. This will be 0000 for ok requests             |
| Amount                        | decimal     | Amount due on the invoice in two decimal places              |
| Email                         | string      | Email of the customer                                        |
| InvoiceNumber                 | string      | Invoice number                                               |
| PhoneNumber                   | string      | Phone number of the customer                                 |
| Recipient                     | string      | Name of the customer                                         |
| ResponseDescription           | string      | Invoice description                                          |
| SettlementCode                | string      | Code for settlement                                          |
| SettlementType                | string      | Type of settlement (1 for Flat, 2 for Percentage)            |

### Sample request JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Signature",
            "ErrorMessage": "We could not compute signature"
        }
    ]
}
```

```JSON
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "Recipient": "Tax Payer",
        "Amount": 11492.00,
        "InvoiceNumber": "1000425618",
        "Email": "taxpayer@example.com",
        "PhoneNumber": null,
        "PayerId": null,
        "ResponseCode": "0000",
        "ResponseDescription": "Invoice for Invoice Description",
        "SettlementCode": "001",
        "SettlementType": 2
    }
}
```


## Payment Notification [link](http://uat.nasarawaigr.com/api/v1/payment/notification)
***

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the concatenation of InvoiceNumber,       |
|                               |             | PaymentRef, AmoutPaid (decimal places), PaymentDate and      |
|                               |             | Channel                                                      |
| CLIENTID                      | string      | client Id                                                    |

Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| InvoiceNumber                 | string      | Invoice number you want to base the search on                |
| AmountPaid                    | decimal     | AmountPaid in two decimal places                             |
| BankBranch                    | string      | Bank branch name (optinal)                                   |
| BankCode                      | string      | CBN defined Bank Code                                        |
| BankName                      | string      | CBN defined Bank Name                                        |
| PaymentDate                   | string      | Payment date (dd/MM/yyyy HH:mm:ss)                           |
| PaymentRef                    | string      | Payment reference. This must be unique per transaction       |
| TransactionDate               | string      | Date of transaction (dd/MM/yyyy HH:mm:ss)                    |
| Channel                       | string      | Channel payment was made through                             |
| PaymentMethod                 | string      | Payment method                                               |


### Sample request JSON
```JSON
{
    "InvoiceNumber": "1000425618",
    "PaymentRef": "637211260852308293",
    "PaymentDate": "30/03/2018 17:45:04",
    "BankCode": "050",
    "BankName": "Eco Bank",
    "BankBranch": "Yaba, Alagomeji",
    "AmountPaid": 10.00,
    "TransactionDate": "29/03/2018 17:45:04",
    "Channel": "Bank Branch",
    "PaymentMethod": "Inter-transfer"
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
|                               |             | contain an object detailing the invoice info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the invoice details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |

#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| ResponseCode                  | string      | Response code. This will be 0000 for ok requests             |
| ResponseDescription           | string      | Response description                                          |

### Sample request JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Signature",
            "ErrorMessage": "We could not compute signature"
        }
    ]
}
```

```JSON
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "ResponseCode": "0000",
        "ResponseDescription": "Payment Notification Successful",
    }
}
```

## Search for Tax Profile [link](http://uat.nasarawaigr.com/api/v1/user/search-by-filter)
***

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the serialized request model              |
| CLIENTID                      | string      | client Id                                                    |

Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Name                          | string      | Name on the tax profile                                      |
| PhoneNumber                   | string      | Phonenumber on the tax profile                               |
| TIN                           | string      | TIN on the tax profile                                       |
| CategoryId                    | int         | Category code you want to base the search on                 |
| PayerId                       | string      | PayerId on the tax profile                                   |
| Page                          | int         | Pagination page number                                       |
| PageSize                      | int         | Pagination page size, number of records to be returned       |
|                               |             | (100 max)                                                    |


### Sample request JSON
```JSON
{
  "Name": "Tax Payer Name",
  "PhoneNumber": "08907777178",
  "TIN": "Federal issued TIN",
  "PayerId": "State TIN",
  "CategoryId": 1,
  "Page": 1,
  "PageSize": 10
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
|                               |             | contain an object detailing the invoice info. If the Error   |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the invoice details if false  |


#### ResponseObject ErrorModel
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |

#### ResponseObject (Error is false)
***
if Error is false the ResponseObject model will be populated with a array of records and the search model from the 
input request

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Name                          | string      | Name on the tax profile                                      |
| PhoneNumber                   | string      | Phonenumber on the tax profile                               |
| TaxPayerIdentificationNumber  | string      | TIN on the tax profile                                       |
| Category                      | string      | Category on the tax profile                                  |
| RegNumber                     | string      | Busniess Reg Number on the tax profile                       |
| Email                         | string      | Email on the tax profile                                     |
| PayerId                       | string      | PayerId on the tax profile                                   |
| StateName                     | string      | State on the tax profile                                     |
| LGA                           | string      | LGA on the tax profile                                       |
| TotalNumberOfTaxPayers        | int         | Total number of tax profiles matching the search parameters  |

### Sample request JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Signature",
            "ErrorMessage": "We could not compute signature"
        }
    ]
}
```

```JSON
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "ReportRecords": [
            {
                "Name": "Tax Payer",
                "PhoneNumber": "08907777178",
                "Address": "Address field",
                "Category": "Individual",
                "TaxPayerIdentificationNumber": "7777711",
                "RegNumber": null,
                "Email": "taxpayer@example.com",
                "PayerId": "IJ-0008",
                "StateName": "Abia",
                "LGA": "Umahia"
            },
            {
                "Name": "Corporate",
                "PhoneNumber": "08907777178",
                "Address": "Corporate Hq",
                "Category": "Corporate",
                "TaxPayerIdentificationNumber": "1234567890",
                "RegNumber": null,
                "Email": "corp@example.com",
                "PayerId": "DE-0003",
                "StateName": "Abia",
                "LGA": "Umahia"
            }
        ],
        "SearchFilter": {
            "Name": null,
            "PhoneNumber": "08907777178",
            "TIN": null,
            "PayerId": null,
            "CategoryId": 0,
            "Page": 1,
            "PageSize": 10
        },
        "TotalNumberOfTaxPayers": 2
    }
}
```


### Available Payment providers

| Payment Provider            | Value          |
| ----------------------------|:--------------:|
| Bank Branch                 | Bank Branch    |
| ATM                         | ATM            |
| POS                         | POS            |
| Web                         | Web            |
| Kiosk                       | Kiosk          |
| Voice                       | Voice          |

### Available Payment Methods

| Payment Methods             | Value          |
| ----------------------------|:--------------:|
| Cash                        | Cash           |
| Cheque                      | Cheque         |
| Card                        | Card           |
| Transfer                    | Transfer       |
| Inter-transfer              | Inter-transfer |



# PAYE API
***

## Steps for PAYE processing
1. To start processing, call an initialize batch endpoint.
2. Then, call the add batch items endpoint to start sending all the PAYE items for the batch chunked by the specified batch item limit.
3. Once all the items for the batch has been sent, call validate batch endpoint to trigger the validation of the batch request.
4. After validation, the validation response will be posted to the callback URL with a token to be sent when calling the confirmation endpoint.
4. Call the confirmation endpoint to confirm the result of the request, the calling party must send the token gotten from the validation request callback or call the batch status to get the token.
5. In case of invalid items in a batch after processing, get invalid items endpoint can be used to get the list.

## PAYE Batch Initialization [link](.../api/v1/PAYE/initialize-batch)
***
This is used to initialize PAYE batch invoice processing. This endpoint opens a batch so items can be added to batch. 

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the BatchIdentifier and EmployerPayerId   |
| CLIENTID                      | string      | client Id                                                    |


Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| BatchIdentifier               | string      | An identifier for the PAYE items to be sent. Should be unique|
|                               |             | per request  (required)                                      |
| EmployerPayerId               | string      | The employer State TIN Id (required)                         |
| CallbackURL                   | string      | The POST endpoint URL where the response will be posted      |
|                               |             | after validation (required)                                  |


### Sample request JSON
```JSON
{
    "BatchIdentifier": "xxxddexxx",
    "EmployerPayerId": "nd-363673",
    "CallbackURL": "http:xxxxxxxx"
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
|                               |             | contain an object detailing the payer info. If the Error     |
|                               |             | value is true, the ResponseObject would contain a list of    |
|                               |             | ErrorModel                                                   |
| ResponseObject                | object      | Response object. This value would either contain the         |
|                               |             | ErrorModel if Error is true or the payer details if false    |


#### ResponseObject ErrorModel (Error is true)
***
if Error is true is a list of objects containing the FieldName and ErrorMessage

| Property                      | Data Format | Description                                                  |
| ------------------------------|:-----------:| -------------------------------------------------------------|
| FieldName                     | string      | Field name                                                   |
| ErrorMessage                  | string      | Error message                                                |


#### Sample error response JSON
```JSON
{
    "Error": true,
    "ErrorCode": "0001",
    "ResponseObject": [
        {
            "FieldName": "Signature",
            "ErrorMessage": "We could not compute signature"
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
| Message                       | string      | Response message                                             |
| BatchIdentifier               | string      | Batch Identifier                                             |
| BatchItemLimit                | int         | This is the number of items that can be sent at a go when    |
|                               |             | calling add batch items endpoint.                            |
    


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "Message": "Initialized successfully",
        "BatchIdentifier": "xxxxxxx",
        "BatchItemLimit": 20
    }
}
```

###### PAYE Batch items [link](.../api/v1/PAYE/add-batch-items)
***
After batch initialization, call this endpoint to add PAYE items into a batch that has been initialized.

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the BatchIdentifier and EmployerPayerId   |
| CLIENTID                      | string      | client Id                                                    |


Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| BatchIdentifier               | string      | Batch identifier for the PAYE items (required)               |
| PageNumber                    | int         | Current page of the batch items request (required)           |
| ItemNumber                    | int         | A unique identifier for each of the item to be               |
|                               |             | processed (required)               |
| PayerId                       | string      | The employee State TIN Id (required)                         |
| GrossAnnual                   | decimal     | The Gross Annual earning for the Payee (required)            |
| Exemptions                    | decimal     | The tax exemptions for the Payee (required)                  |
| IncomeTaxPerMonth             | decimal     | Income Tax Per Month (required)                              |
| Month                         | string      | The month to be paid for (required)                          |
| Year                          | int         | Year to be paid for (required)                               |
| Mac                           | string      | HMACSHA256 hash of the PayerId, IncomeTaxPerMonth (2 dp),    |
|                               |             | Month and Year (required)                                    |   


### Sample request JSON
```JSON
{
    "BatchIdentifier": "xxxddexxx",
    "PageNumber": 1,
    "PayeItems": [
        {
            "ItemNumber": 10000,
            "PayerId": "77hhhee",
            "GrossAnnual": 0.00,
            "Exemptions": 0.00,
            "IncomeTaxPerMonth": 0.00,
            "Month": "Jan",
            "Year": 2020,
            "Mac": "gsgsshsh6733"
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
| Message                       | string      | Response message                                             |
| BatchIdentifier               | string      | Batch Identifier                                             |
| BatchItemLimit                | int         | This is the number of items that can be sent at a go when    |
|                               |             | calling create batch items endpoint.                         |
| PageNumber                    | int         | Current page of the batch items request                      |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "Message": "Successful",
        "BatchIdentifier": "xxxxxxx",
        "BatchItemLimit": 20,
        "PageNumber": 1
    }
}
```

###### PAYE Batch Processing [link](.../api/v1/PAYE/validate-batch)
***
This endpoint triggers the system to start the processing of the batch items for a specified batch identifier

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the BatchIdentifier and EmployerPayerId   |
| CLIENTID                      | string      | client Id                                                    |


### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| BatchIdentifier               | string      | An identifier for the PAYE Batch  (required)                 |


### Sample request JSON
```JSON
{
    "BatchIdentifier": "xxxddexxx"
}
```

#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| Message                       | string      | Response message                                             |
| BatchIdentifier               | string      | Batch Identifier                                             |

### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "Message": "Successful",
        "BatchIdentifier": "xxxxxxx"
    }
}
```

###### PAYE Batch Processing [link](.../api/v1/PAYE/confirm-batch)
***
This endpoint is called when the result of the validation process has been confirmed by the calling party.
A token is expected to be passed when making a request to this endpoint. This token is gotten when validation is completed and a notification has been sent to the callback URL supplied at the initialization stage.
This token can also be gotten from the batch status endpoint.


### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the BatchIdentifier and EmployerPayerId   |
| CLIENTID                      | string      | client Id                                                    |


### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| BatchIdentifier               | string      | An identifier for the PAYE Batch  (required)                 |
| Token                         | string      | Token received from validation notification                  |


### Sample request JSON
```JSON
{
    "BatchIdentifier": "xxxddexxx",
    "Token": "rtreeeeeee"
}
```

#### ResponseObject (Error is false)
***

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| BatchIdentifier               | string      | Batch Identifier                                             |
| InvoiceNumber                 | string      | Invoice Number for the batch                                 |
| InvoiceAmount                 | decimal     | Invoice amount, this includes the TotalValidItemAmount       |
|                               |             | plus other additional surcharges                             |
| TotalValidItemAmount          | decimal     | Total amount for the valid items in the batch                |
| TotalItems                    | int         | Total items in the batch                                     |
| TotalValidItems               | int         | Total valid items in the batch                               |
| TotalInvalidItems             | int         | Total invalid items in the batch                             |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ResponseObject": {
        "BatchIdentifier": "xxxxxxx",
        "InvoiceNumber": "edrffgeeeee",
        "InvoiceAmount": 50001.00,
        "TotalValidItemAmount": 50000.00,
        "TotalItems": 10,
        "TotalValidItems": 5,
        "TotalInvalidItems": 5        
    }
}
```


###### Get Batch Invalid Items [link](.../api/v1/PAYE/get-invalid-items)
***
This endpoints get the list of invalid items in a particular batch after validation processing has been completed

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the BatchIdentifier and EmployerPayerId   |
| CLIENTID                      | string      | client Id                                                    |


### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| BatchIdentifier               | string      | An identifier for the PAYE Batch  (required)                 |
| Page                          | int         | Page number to be retrieved  (required)                      |
| PageSize                      | int         | Page size to be retrieved  (required)                        |


### Sample request JSON
```JSON
{
    "BatchIdentifier": "xxxddexxx",
    "PageParameters": {
        "Page": 1,
        "PageSize": 100
    }
}
```

#### ResponseObject (Error is false)
***
if Error is false this model will be populated

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| ErrorCode                     | string      | It will be null for successful request                       |
| BatchIdentifier               | string      | Batch Identifier                                             |
| TotalCount                    | int         | Total invalid items in the batch                             |
| Page                          | int         | Page number of the chunked invalid items                     |
| PageSize                      | int         | Max chunk size of number of the invalid items                |
| ItemNumber                    | string      | A unique identifier for each of the item be processed        |
| PayerId                       | string      | The employee State TIN Id                                    |
| GrossAnnual                   | decimal     | The Gross Annual earning for the employee                    |
| Exemptions                    | decimal     | The tax exemptions for the employee                          |
| IncomeTaxPerMonth             | decimal     | Income Tax Per Month                                         |
| Month                         | string      | The month to be paid for                                     |
| Year                          | int         | Year to be paid for                                          |
| Message                       | string      | Message containing the reason the item failed                |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "BatchIdentifier": "xxxddexxx",
        "TotalCount": 2,
        "PageParameters": {
            "Page": 0,
            "PageSize": 0
        },
        "Items": [
            {
                "ItemNumber": 10000,
                "PayerId": "77hhhee",
                "GrossAnnual": 0.00,
                "Exemptions": 0.00,
                "IncomeTaxPerMonth": 0.00,
                "Month": "Jan",
                "Year": 2020,
                "Message": "Payer Id doesn't exist"
            },
            {
                "ItemNumber": 10001,
                "PayerId": "77hhhe2",
                "GrossAnnual": 0.00,
                "Exemptions": 0.00,
                "IncomeTaxPerMonth": 0.00,
                "Month": "Jan",
                "Year": 2020,
                "Message": "Payer Id doesn't exist"
            }
        ]
    }
}
```

###### PAYE Batch Status [link](.../api/v1/PAYE/batch-status)
***
This endpoint gets the status of a batch for a specified batch identifier

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the BatchIdentifier and EmployerPayerId   |
| CLIENTID                      | string      | client Id                                                    |


### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| BatchIdentifier               | string      | An identifier for the PAYE Batch  (required)                 |


### Sample request JSON
```JSON
{
    "BatchIdentifier": "xxxddexxx"
}
```

#### ResponseObject (Error is false)
***

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| BatchIdentifier               | string      | Batch Identifier                                             |
| InvoiceNumber                 | string      | Invoice Number for the batch                                 |
| InvoiceAmount                 | decimal     | Invoice amount, this includes the TotalValidItemAmount       |
|                               |             | plus other additional surcharges                             |
| TotalValidItemAmount          | decimal     | Total amount for the valid items in the batch                |
| TotalItems                    | int         | Total items in the batch                                     |
| TotalValidItems               | int         | Total valid items in the batch                               |
| TotalInvalidItems             | int         | Total invalid items in the batch                             |
| Token                         | string      | Token confirming validation results                          |
| Status                        | string      | Description of the batch status                              |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ResponseObject": {
        "BatchIdentifier": "xxxxxxx",
        "InvoiceNumber": "edrffgeeeee",
        "InvoiceAmount": 50001.00,
        "TotalValidItemAmount": 50000.00,
        "TotalItems": 10,
        "TotalValidItems": 5,
        "TotalInvalidItems": 5,        
        "Token": "edrffgeeeee",        
        "Status": "Done",        
    }
}
```


## PAYE Batch Processing Validation Notification to CallbackURL
***
After batch validation is completed, the callbackurl is notified about the details of the processing.
Note: CallbackURL method type should be POST

#### Payload
***

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Error                         | bool        | False                                                        |
| BatchIdentifier               | string      | Batch Identifier                                             |
| TotalValidItemAmount          | decimal     | Total amount for the valid items in the batch                |
| TotalItems                    | int         | Total items in the batch                                     |
| TotalValidItems               | int         | Total valid items in the batch                               |
| TotalInvalidItems             | int         | Total invalid items in the batch                             |
| Token                         | string      | Token confirming validation results                          |


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ResponseObject": {
        "BatchIdentifier": "xxxxxxx",
        "TotalValidItemAmount": 50000.00,
        "TotalItems": 10,
        "TotalValidItems": 5,
        "TotalInvalidItems": 5,        
        "Token": "edrffgeeeee",        
    }
}
```

### Months

| Value      |
| -----------|
| Jan        | 
| Feb        |  
| Mar        |  
| Apr        |  
| May        | 
| Jun        | 
| Jul        | 
| Aug        |  
| Sep        |  
| Oct        | 
| Nov        |
| Dec        | 



Any issues >>>>> **please contact Opeyemi oibuoye[at]parkwayprojects.com**