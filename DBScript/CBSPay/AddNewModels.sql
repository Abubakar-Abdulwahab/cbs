Use CBSPaymentEngine

create table EconomicActivities (
        Id INT IDENTITY NOT NULL,
       EconomicActivitiesID INT NOT null,
       EconomicActivitiesName NVARCHAR(255) null,
       TaxPayerTypeID INT null,
       TaxPayerTypeName NVARCHAR(255) null,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )
	
create table TaxPayerType (
        Id INT IDENTITY NOT NULL,
       TaxPayerTypeID INT null,
       TaxPayerTypeName NVARCHAR(255) null,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )

create table RevenueStream (
        Id INT IDENTITY NOT NULL,
       RevenueStreamID INT null,
       RevenueStreamName NVARCHAR(255) null,
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )
	
create table RevenueSubStream (
        Id INT IDENTITY NOT NULL,
       RevenueSubStreamID INT null,
       RevenueSubStreamName NVARCHAR(255) null,	   
       Active BIT null,
       ActiveText NVARCHAR(255) null,
       DateCreated DATETIME not null,
       DateModified DATETIME not null,
       IsDeleted BIT not null,
       primary key (Id)
    )