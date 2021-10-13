# Trading-Algo
first attempt at creating some kind of console application to automate stock trading.

## to use
  1. run the provided SQL file in local database ( connected to ".") - file found inside algo-01-DB
  2. ensure stable internet connection 
  3. assuming the user has VS installed open the soloution under algo-02 
  4. run and follow instructions and prompts
  5. enter kill and follow termination process
  6. a CSV file should open to provide an audit report of the trades made within that session
 
### note
program will nuke all data within the database on startup. save audit report elsewhere to save historical audit reports

for now:
-code memory and storage is based on a windows server database (see SQL file under algo-01-DB) 
-goal is to create a paper trading console application that uses Alpha Vantage API to gather data in Json format and evaluate and log the data to the DB to make decisions on wether to buy or sell stocks at the current price (closing). 
-ideally once this is all working i can take it to a windowed application and have live metrics and other useful tools. but lets see if this gets completed first.

**tech used**
- C# .NET framework (console application)
- windows SQL server
- json data from Alpha Vantage 
