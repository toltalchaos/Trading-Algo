-- create DB

IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = N'Algo-Test01')
BEGIN
    CREATE DATABASE [Algo-Test01]
END
GO

USE [Algo-Test01]
GO

-- drop existing tables 
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SYMBOL_HISTORY')
DROP TABLE SYMBOL_HISTORY
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WALLET_HISTORY')
DROP TABLE WALLET_HISTORY
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Wallet')
DROP TABLE Wallet
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Portfolio')
DROP TABLE Portfolio
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stock_Item')
DROP TABLE Stock_Item
GO

-- create tables 

CREATE TABLE Stock_Item
(
    Symbol VARCHAR(5)    CONSTRAINT PK_STKITEM       PRIMARY KEY,
    [Open] DECIMAL(10,4)     CONSTRAINT CK_OPN_PRICE CHECK([Open] > 0),
    High DECIMAL(10,4)     CONSTRAINT CK_HI_PRICE CHECK(High > 0),
    Low DECIMAL(10,4)     CONSTRAINT CK_LO_PRICE CHECK(Low > 0),
    [Close] DECIMAL(10,4)     CONSTRAINT CK_CLS_PRICE CHECK([Close] > 0),
    Volume INTEGER CONSTRAINT CK_VOL_AMNT CHECK(Volume >= 0),
    DataTime DATETIME 
)
GO 

Create TABLE Portfolio
(
    PortfolioNumber INTEGER CONSTRAINT PK_Portfolio PRIMARY KEY IDENTITY(1,1),
    Symbol VARCHAR(5) CONSTRAINT FK_port_Symbol FOREIGN KEY(Symbol) REFERENCES Stock_Item(Symbol),
    PurchasePrice DECIMAL(10,4) CONSTRAINT CK_PurPrice CHECK(PurchasePrice > 0),
    AmountOwned INTEGER CONSTRAINT CK_Portfolio_AmtOwned CHECK(AmountOwned >= 0),
    --create trigger to remove if amountOwned < 0 
)
GO 

CREATE TABLE Wallet
(
    WalletNumber INTEGER CONSTRAINT PK_Wallet PRIMARY KEY IDENTITY(1,1),
    PortfolioNumber INTEGER CONSTRAINT FK_Portfolio FOREIGN KEY(PortfolioNumber) REFERENCES Portfolio(PortfolioNumber),
    CurrentBalance Decimal(15,2) CONSTRAINT CK_Wallet_Balance CHECK(CurrentBalance > 0),

)
GO

--audit log table for each symbol
CREATE TABLE SYMBOL_HISTORY
(
     Symbol VARCHAR(5),
    [Open] DECIMAL(10,4)     CONSTRAINT CK_OPN_PRICE_Hist CHECK([Open] > 0),
    High DECIMAL(10,4)     CONSTRAINT CK_HI_PRICE_Hist CHECK(High > 0),
    Low DECIMAL(10,4)     CONSTRAINT CK_LO_PRICE_Hist CHECK(Low > 0),
    [Close] DECIMAL(10,4)     CONSTRAINT CK_CLS_PRICE_Hist CHECK([Close] > 0),
    Volume INTEGER CONSTRAINT CK_VOL_AMNT_Hist CHECK(Volume >= 0),
    DataTime DATETIME,

    PRIMARY KEY (Symbol, DataTime)

)
GO

CREATE TABLE WALLET_HISTORY
(
    transactionNumber INTEGER CONSTRAINT PK_Transaction PRIMARY KEY IDENTITY(1,1),
    PortfolioNumber INTEGER CONSTRAINT FK_Walhist_Portfolio FOREIGN KEY(PortfolioNumber) REFERENCES Portfolio(PortfolioNumber),
    Balance  Decimal(15,2), 
    Symbol VARCHAR(5) CONSTRAINT FK_transact_Symbol FOREIGN KEY(Symbol) REFERENCES Stock_Item(Symbol),
    Amount DECIMAL(10,4) CONSTRAINT CK_trans_zero CHECK(Amount > 0),
    Direction VARCHAR(4) CONSTRAINT CK_transact_BuySell CHECK(Direction IN ('BUY', 'SELL'))
)
GO

--load nill data
INSERT INTO Stock_Item(Symbol, [Open], High, Low, [Close], Volume, DataTime)
VALUES ('XXX', 420.69, 555.00, 69.00, 410.69, 1234, GETDATE()),
        ('YYY', 420.69, 555.00, 69.00, 410.69, 1234, GETDATE())

INSERT INTO Portfolio(Symbol, PurchasePrice, AmountOwned)
VALUES ('XXX', 420.69, 15),
        ('YYY', 420.69, 0)

INSERT INTO Wallet(PortfolioNumber, CurrentBalance)
VALUES (1, 420420)

-- create triggers to maintain DB
    --REMOVE ROWS FROM PORTFOLIO WHERE AMOUNT OWNED = 0 
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Portfolio_Update_Removezerown]'))
DROP TRIGGER Portfolio_Update_Removezerown
GO

CREATE TRIGGER Portfolio_Update_Removezerown
ON Portfolio
FOR Insert, Update 
AS
    IF EXISTS(SELECT Symbol from Portfolio WHERE AmountOwned = 0)
        DELETE from Portfolio 
        where AmountOwned = 0
    
RETURN
GO

-- trigger to add to audit logs
--market data trigger

--wallet transaction trigger


-- create procs/trigs 
    --add purchase
    --get portfolio value
    --audit log