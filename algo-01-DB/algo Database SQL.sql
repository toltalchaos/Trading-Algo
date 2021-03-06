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

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WatchList')
DROP TABLE WatchList
GO

-- create tables 

CREATE TABLE Stock_Item
(
    Symbol VARCHAR(5)    CONSTRAINT PK_STKITEM      PRIMARY KEY NOT NULL ,
    [Open] DECIMAL(10,4)     CONSTRAINT CK_OPN_PRICE CHECK([Open] > 0) NOT NULL ,
    High DECIMAL(10,4)     CONSTRAINT CK_HI_PRICE CHECK(High > 0) NOT NULL ,
    Low DECIMAL(10,4)     CONSTRAINT CK_LO_PRICE CHECK(Low > 0) NOT NULL ,
    [Close] DECIMAL(10,4)     CONSTRAINT CK_CLS_PRICE CHECK([Close] > 0) NOT NULL ,
    Volume INTEGER CONSTRAINT CK_VOL_AMNT CHECK(Volume >= 0) NOT NULL ,
    DataTime DATETIME 
)
GO 

Create TABLE Portfolio
( 
    PortfolioLineNumber INTEGER CONSTRAINT PK_PortfolioLineNumber PRIMARY KEY IDENTITY(1,1),
    Symbol VARCHAR(5) CONSTRAINT FK_port_Symbol FOREIGN KEY(Symbol) REFERENCES Stock_Item(Symbol) ,
    SalePrice DECIMAL(10,4),
    AmountOwned INTEGER CONSTRAINT CK_Portfolio_AmtOwned CHECK(AmountOwned >= 0),

)
GO 

CREATE TABLE Wallet
(
    WalletNumber INTEGER CONSTRAINT PK_Wallet PRIMARY KEY NOT NULL ,
    CurrentBalance Decimal(15,2) CONSTRAINT CK_Wallet_Balance CHECK(CurrentBalance > 0) NOT NULL ,
    LastTransactionDirection VARCHAR(4) CONSTRAINT CK_wallet_BuySell CHECK(LastTransactionDirection IN ('BUY', 'SELL'))

)
GO

--audit log table for each symbol
CREATE TABLE SYMBOL_HISTORY
(
     Symbol VARCHAR(5) NOT NULL ,
    [Open] DECIMAL(10,4)     CONSTRAINT CK_OPN_PRICE_Hist CHECK([Open] > 0) NOT NULL ,
    High DECIMAL(10,4)     CONSTRAINT CK_HI_PRICE_Hist CHECK(High > 0) NOT NULL ,
    Low DECIMAL(10,4)     CONSTRAINT CK_LO_PRICE_Hist CHECK(Low > 0) NOT NULL ,
    [Close] DECIMAL(10,4)     CONSTRAINT CK_CLS_PRICE_Hist CHECK([Close] > 0) NOT NULL ,
    Volume INTEGER CONSTRAINT CK_VOL_AMNT_Hist CHECK(Volume >= 0) NOT NULL ,
    DataTime DATETIME NOT NULL ,

    PRIMARY KEY (Symbol, DataTime)

)
GO

CREATE TABLE WALLET_HISTORY
(
    transactionNumber INTEGER CONSTRAINT PK_Transaction PRIMARY KEY IDENTITY(1,1) NOT NULL ,
    PortfolioLineNumber INTEGER CONSTRAINT FK_Walhist_Portfolio FOREIGN KEY(PortfolioLineNumber) REFERENCES Portfolio(PortfolioLineNumber) NOT NULL ,
    Balance  Decimal(15,2), 
    Symbol VARCHAR(5) CONSTRAINT FK_transact_Symbol FOREIGN KEY(Symbol) REFERENCES Stock_Item(Symbol),
    Shares INTEGER CONSTRAINT CK_transact_numShares check(Shares != 0),
    Amount DECIMAL(10,4) CONSTRAINT CK_trans_zero CHECK(Amount != 0),
    Direction VARCHAR(4) CONSTRAINT CK_transact_BuySell CHECK(Direction IN ('BUY', 'SELL')) 
)
GO
CREATE Table WatchList
(
    symbol VARCHAR(5) PRIMARY KEY NOT NULL 
)
GO

--load nill data
-- INSERT INTO Stock_Item(Symbol, [Open], High, Low, [Close], Volume, DataTime)
-- VALUES ('XXX', 420.69, 555.00, 69.00, 410.69, 1234, GETDATE()),
--         ('YYY', 420.69, 555.00, 69.00, 410.69, 1234, GETDATE())

-- INSERT INTO Portfolio(Symbol, SalePrice, AmountOwned)
-- VALUES ('XXX', 420.69, 15),
--         ('YYY', 420.69, 0)

-- INSERT INTO Wallet(PortfolioLineNumber, CurrentBalance)
-- VALUES (1, 420420)

-- create triggers to maintain DB
    --REMOVE ROWS FROM PORTFOLIO WHERE AMOUNT OWNED = 0 
-- IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Portfolio_Update_Removezerown]'))
-- DROP TRIGGER Portfolio_Update_Removezerown
-- GO

-- CREATE TRIGGER Portfolio_Update_Removezerown
-- ON Portfolio
-- FOR Update 
-- AS
--     IF EXISTS(SELECT Symbol from Portfolio WHERE AmountOwned = 0)
--         DELETE from Portfolio 
--         where AmountOwned = 0
    
-- RETURN
-- GO

-- trigger to add to audit logs
--market data trigger

--TRIGGER MAY NOT HANDLE MULTIPLE INSERTS AT ONCE?? - REVIEW
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[StockItem_Update_LogAuditTrail]'))
DROP TRIGGER StockItem_Update_LogAuditTrail
GO

CREATE TRIGGER StockItem_Update_LogAuditTrail
ON Stock_Item
FOR Update 
AS
    IF @@ROWCOUNT > 0 
    BEGIN
        INSERT INTO SYMBOL_HISTORY(Symbol, [Open], High, Low, [Close], Volume, DataTime)
        SELECT I.Symbol, I.[Open], I.High, I.Low, I.[Close], I.Volume, I.DataTime
        FROM inserted I

        IF @@ERROR <> 0
        BEGIN
            RAISERROR('COULD NOT LOG SYMBOL HISTORY', 16,1)
            ROLLBACK TRANSACTION
        END 
    END 
    
RETURN
GO

--wallet transaction trigger
--updates current wallet balance as well as logs wallet history
IF EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Wallet_Update_LogAuditTrail]'))
DROP TRIGGER Wallet_Update_LogAuditTrail
GO

CREATE TRIGGER Wallet_Update_LogAuditTrail
ON Portfolio
FOR Update 
AS
DECLARE @upd_Direction VARCHAR(5)
DECLARE @CurrentBalance_var  Decimal(15,2)
BEGIN
        SET @CurrentBalance_var = (select CurrentBalance from Wallet where WalletNumber = 10)
    IF (select SalePrice from inserted) > 0
        SET @upd_Direction = 'SELL'
    ELSE
    BEGIN
        SET @upd_Direction = 'BUY'
    END
END
    IF @@ROWCOUNT > 0 
    BEGIN
        INSERT INTO WALLET_HISTORY(PortfolioLineNumber, Balance, Symbol, Amount, Direction, Shares)
        SELECT I.PortfolioLineNumber, (select CurrentBalance + I.SalePrice from Wallet),
                I.Symbol, I.SalePrice, @upd_Direction, ((SalePrice /(select [Close] from Stock_Item where Symbol = I.Symbol)) * -1)
        FROM inserted I

        
        UPDATE Wallet
        SET CurrentBalance = (@CurrentBalance_var + (select SalePrice from inserted)), LastTransactionDirection = @upd_Direction
        where WalletNumber = 10

        --check the balance of the wallet is > 0 
        IF (select CurrentBalance from Wallet) < 0
        BEGIN
            RAISERROR('insufficent funds', 16,1)
            ROLLBACK TRANSACTION
        END

        IF @@ERROR <> 0
        BEGIN
            RAISERROR('COULD NOT LOG SYMBOL HISTORY', 16,1)
            ROLLBACK TRANSACTION
        END 
    END 
    
RETURN
GO

-- insert into Wallet(WalletNumber, CurrentBalance)
-- VALUES(10, 500)

-- insert into Stock_Item(Symbol, [Open], High, Low, [Close], Volume, DataTime)
-- VALUES ('XXX', 420.69, 555.00, 69.00, 410.69, 1234, GETDATE())

-- UPDATE Wallet
-- set CurrentBalance = 500
-- where WalletNumber = 10

-- insert into Portfolio(Symbol, SalePrice,AmountOwned)
-- VALUES('XXX', 123, 2)

-- create procs/trigs 
    --add purchase
    --get portfolio value
    --audit log INIT ON STARTUP