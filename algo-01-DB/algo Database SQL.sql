-- create DB

IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'Algo-Test01')
BEGIN
    CREATE DATABASE [Algo-Test01]
END
GO

USE [Algo-Test01]
GO

-- drop existing tables 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stock_Item')
DROP TABLE Stock_Item
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Portfolio')
DROP TABLE Portfolio
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Wallet')
DROP TABLE Wallet
GO

-- create tables 

CREATE TABLE Stock_Item
(
    Symbol VARCHAR(5)    CONSTRAINT PK_STKITEM       PRIMARY KEY,
    [Open] DECIMAL(6,4)     CONSTRAINT CK_OPN_PRICE CHECK([Open] > 0),
    High DECIMAL(6,4)     CONSTRAINT CK_HI_PRICE CHECK(High > 0),
    Low DECIMAL(6,4)     CONSTRAINT CK_LO_PRICE CHECK(Low > 0),
    [Close] DECIMAL(6,4)     CONSTRAINT CK_CLS_PRICE CHECK([Close] > 0),
    Volume INTEGER CONSTRAINT CK_VOL_AMNT CHECK(Volume >= 0),
    DataTime DATETIME CONSTRAINT UK_Date_DUPLICATE UNIQUE (DataTime)
)
GO 

Create TABLE Portfolio
(
    PortfolioNumber INTEGER CONSTRAINT PK_Portfolio PRIMARY KEY,
    Symbol VARCHAR(5) CONSTRAINT FK_port_Symbol FOREIGN KEY(Symbol) REFERENCES Stock_Item(Symbol),
    PurchasePrice DECIMAL(6,4) CONSTRAINT CK_PurPrice CHECK(PurchasePrice > 0),
    AmountOwned INTEGER CONSTRAINT CK_Portfolio_AmtOwned CHECK(AmountOwned > 0),
    --create trigger to remove if amountOwned < 0 
)
GO 

CREATE TABLE Wallet
(
    WalletNumber INTEGER CONSTRAINT PK_Wallet PRIMARY KEY,
    PortfolioNumber INTEGER CONSTRAINT FK_Portfolio FOREIGN KEY(PortfolioNumber) REFERENCES Portfolio(PortfolioNumber),
    CurrentBalance Decimal(15,2) CONSTRAINT CK_Wallet_Balance CHECK(CurrentBalance > 0),

)
GO

--audit log table for each symbol


--load nill data

-- create triggers to maintain DB
-- trigger to audit log

-- create procs 
    --add purchase
    --get portfolio value
    --audit log