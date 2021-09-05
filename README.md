# Trading-Algo
first attempt at creating some kind of console application to automate stock trading
readme will be updated once i get around to it.

for now:
-code memory and storage is based on a windows server database (see SQL file under algo-01-DB) 
-goal is to create a paper trading console application that uses Alpha Vantage API to gather data in Json format and evaluate and log the data to the DB to make decisions on wether to buy or sell stocks at the current price (closing). 
-ideally once this is all working i can take it to a windowed application and have live metrics and other useful tools. but lets see if this gets completed first.

**tech used**
- C# .NET framework (console application)
- windows SQL server
- json data from Alpha Vantage 
