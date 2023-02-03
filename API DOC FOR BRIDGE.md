# Central Billing System API README
***

Change History

| Revision   | Developer           | Description                               | Date        |
|----------- |:-------------------:|------------------------------------------:|------------:|
| 0.00       | Ifetayo             | DOC Initiation                            | 5/4/2018    |
|            |                     |                                           |             |
| 1.00       | Opeyemi             | Added documentation for informal sector   |             |
|            |                     | payer enumeration (i.e statatin creation) |             |
|            |                     | and invoice generation                    | 6/7/2021    |


## Invoice Validation [link](http://cbs.parkwayprojects.xyz/api/v1/bridge/invoice/validate-invoice)
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


## Payment Notification [link](http://cbs.parkwayprojects.xyz/api/v1/bridge/payment/payment-notification)
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
| ResponseDescription           | string      | Response description                                         |

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


## Search for Tax Profile [link](http://cbs.parkwayprojects.xyz/api/v1/bridge/user/search-by-filter)
***

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the serialized request model              |
| CLIENTID                      | string      | client Id                                                    |
| BILLERCODE                    | string      | Biller code assigned to the client                           |

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

## State TIN creation [link](https://cbs.parkwayprojects.xyz/api/v1/bridge/statetin/create)
***

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the concatenation of PhoneNumber,         |
|                               |             | PayerCategory, StateCode, LGACode and ClientId               |
| CLIENTID                      | string      | client Id                                                    |
| BILLERCODE                    | string      | Biller code signifies the tenant that owns the request       |

Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| Name                          | string      | Payer name  (required)                                       |
| PhoneNumber                   | string      | Payer phone number (required)                                |
| Email                         | string      | Payer email (optional)                                       |
| Address                       | string      | Payer address  (required)                                    |
| StateCode                     | string      | Payer state code (required)                                  |
| LGACode                       | string      | Payer lga code (required)                                    |
| PayerCategory                 | int         | Payer category (required)                                    |

### Sample request JSON
```JSON
{
    "Name": "Testing Name",
    "PhoneNumber": "07064955644",
    "Email": "ibuoye2021@yahoo.com",
    "Address": "Testing address, Lagos",
    "StateCode": "NAS",
    "LGACode": "KARU",
    "PayerCategory": 1
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
| Name                          | string      | Payer name                                                   |
| StateTIN                      | string      | State TIN for the payer                                      |
| NormalizedStateTIN            | string      | State TIN with only numeric part                             |
| PhoneNumber                   | string      | Phone number of the payer                                    |
| PhoneNumberAlreadyExist       | bool        | Shows if phone number already exist and that the old record  |
|                               |             | for the payer is what was returned                           |    


### Sample successful response JSON
```JSON 
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "Name": "Testing Name",
        "StateTIN": "NO-82225",
        "NormalizedStateTIN" : "82225",
        "PhoneNumber": "07064955645",
        "PhoneNumberAlreadyExist": false
    }
}
```


## Informal sector invoice creation [link](https://cbs.parkwayprojects.xyz/api/v1/bridge/invoice/create)
***

### Header Parameters

| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| SIGNATURE                     | string      | HMACSHA256 hash of the concatenation of RevenueHeadId,       |
|                               |             | Amount (2 dp), CallBackURL and ClientId                      |
| CLIENTID                      | string      | client Id                                                    |
| BILLERCODE                    | string      | Biller code signifies the tenant that owns the request       |

Note: The signature concatenantion is hashed with the client secret

### Input Parameters [Content-type - JSON]
***
| Property                      | Data Format | Description                                                                                          
| ------------------------------|:-----------:| -------------------------------------------------------------|
| RevenueHeadId                 | int         | Revenue Head Id  (required)                                  |
| PayerId                       | string      | Payer Id is the State TIN for a particular payer (required)  |
| Amount                        | decimal     | Amount (optional)                                            |
| CallBackURL                   | string      | CallBackURL  (optional)                                      |

Note: If you are not required to send an amount, kindly send 0.00 as the amount and use same in your signature concatenantion

### Sample request JSON
```JSON
{
    "RevenueHeadId": 1009,
    "TaxEntityInvoice": {
        "TaxEntity": {
            "PayerId": "NO-00013"
        },
        "Amount": 0.00
    },
    "CallBackURL": ""
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


### Sample error response JSON
```JSON
{
    "Error": true,
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
| Recipient                     | string      | Payer name                                                   |
| PayerId                       | string      | State TIN for the payer                                      |
| Email                         | string      | Payer email address                                          |
| PhoneNumber                   | string      | Payer phone number                                           |
| TIN                           | string      | Payer Tax Identification number                              |
| MDAName                       | string      | MDA name                                                     |
| RevenueHeadName               | string      | Revenue Head name                                            |
| ExternalRefNumber             | string      | External reference number                                    |
| PaymentURL                    | string      | Invoice payment URL                                          |
| Description                   | string      | Invoice description                                          |
| RequestReference              | string      | Invoice request reference                                    |
| IsDuplicateRequestReference   | bool        | Marked if an invoice request is a duplicate                  |
| InvoiceNumber                 | string      | Invoice number                                               |
| InvoicePreviewUrl             | string      | Invoice preview URL                                          |
| AmountDue                     | decimal     | Invoice amount due                                           |


### Sample successful response JSON
```JSON
{
    "Error": false,
    "ErrorCode": null,
    "ResponseObject": {
        "Recipient": "Ajitech",
        "PayerId": "NO-00013",
        "Email": "ibu@gmail.com",
        "PhoneNumber": "07064955741",
        "TIN": null,
        "MDAName": "CUSTOM",
        "RevenueHeadName": "IMPORT DUTY",
        "ExternalRefNumber": null,
        "PaymentURL": "http://dev.cbs/c/make-payment/1000467394",
        "Description": "IMPORT DUTY | CUS/0012/001 ",
        "RequestReference": null,
        "IsDuplicateRequestReference": false,
        "InvoiceNumber": "1000467394",
        "InvoicePreviewUrl": "http://127.0.0.1/CashFlow.API/v2/ViewInvoice/1000467394/Html",
        "AmountDue": 20000.00
    }
}
```


### Available Payment Channels

| Payment Channels            | Value                     |
| ----------------------------|:-------------------------:|
| Bank Branch                 | Bank Branch               |
| ATM                         | ATM                       |
| POS                         | POS                       |
| Web                         | Web                       |
| Kiosk                       | Kiosk                     |
| Voice                       | Voice                     |
| BankBranch                  | BankBranch                |
| VendorMerchantWebPortal     | VendorMerchantWebPortal   |
| ThirdPartyPaymentPlatform   | ThirdPartyPaymentPlatform |
| OtherChannels               | OtherChannels             |
| AgencyBanking               | AgencyBanking             |

### Available Payment Methods

| Payment Methods             | Value               |
| ----------------------------|:-------------------:|
| Cash                        | Cash                |
| Cheque                      | Cheque              |
| DebitCard                   | DebitCard           |
| InternalTransfer            | InternalTransfer    |
| OtherBankCheque             | OtherBankCheque     |
| OwnBankCheque               | OwnBankCheque       |
| BankTransfer                | BankTransfer        |
| OtherPaymentMethods         | OtherPaymentMethods |


### Payer Category

| Name                        | Value          |
| ----------------------------|:--------------:|
| Individual                  | 1              |
| Corporate                   | 2              |

### Biller Code

| Name                        | Value          |
| ----------------------------|:--------------:|
| Nasarawa                    | nasarawa       |
| Niger                       | niger          |

### States

Name	    ShortName
Lagos	    LAG
Abia	    ABI
Adamawa	    ADA
Akwa Ibom	AKW
Anambra	    ANA
Bauchi	    BAU
Bayelsa	    BAY
Benue	    BEN
Borno	    BOR
Cross River	CRO
Delta	    DEL
Ebonyi	    EBO
Edo	        EDO
Ekiti	    EKI
Enugu	    ENU
Gombe	    GOM
Imo	        IMO
Jigawa	    JIG
Kaduna	    KAD
Kano	    KAN
Katsina	    KAT
Kebbi	    KEB
Kogi	    KOG
Kwara	    KWA
Nasarawa	NAS
Niger	    NIG
Ogun	    OGU
Ondo	    OND
Osun	    OSU
Oyo	        OYO
Plateau	    PLA
Rivers	    RIV
Sokoto	    SOK
Taraba	    TAR
Yobe	    YOB
Zamfara	    ZAM
Abuja (FCT)	ABU

### LGAs
Name	        CodeName
Akwanga	        AKWANGA
Awe	            AWE
Doma	        DOMA
Karu	        KARU
Keana	        KEANA
Keffi	        KEFFI
Kokona	        KOKONA
Lafia	        LAFIA
Nasarawa        NASARAWA
Nasarawa-Eggon	NASARAWA-EGGON
Obi	            OBI
Toto	        TOTO
Wamba	        WAMBA


Any issues >>>>> **please contact Opeyemi oibuoye[at]parkwayprojects.com**