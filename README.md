# tp24TT

To run the project, create a sql server db called 'tp24TT', open the package manger console and run 'update database' to run the latest migration to build the relevant tables. Then its just a case of hitting F5 and running via IIS Express for the sake of simplicity. 

there are some unit tests attached to test for output of the endpoints. 

I made a few assumptions on the business logic based on what I thought would make sense in real life, naturally i could be wrong but :D 

the assumptions were:  

reject the call to store payload if the reference ID already existed 

reject the call if the closed date is before the issue date (assuming issue date is the date the loan was created, therefore it cannot close before it was opened) 

reject the call if the closed date is after the due date (assuming it had to be closed before it is due)

the record cannot be 'cancelled' without a 'closedDate' (assuming that the date of cancellation must be recorded somehow, and the closedDate seemed like the logical way to do that) 

also assumed that  the account cannot be closed with a paid value less than the opening value, unless it has been cancelled


in terms of the get endpoint, intially i thought the brief was asking to bring back specific information about a specific record e.g GetReceivablesSummaryByReference(guid reference), however it made more sense that the ask was to bring back statistical information about all the stored receivables in the table. therefore i decided to calculate TotalEntries, PercentageOpen, PercentageClosed, PercentageCancelled, TotalPaidValueOpen, TotalPaidValueClosed. Hoping i was correct in making this assumption. 

This was good fun, I enjoyed working through the little gotchas e.g making sure the issueDate, dueDate and closedDates were fully parseable from strings into date formats. I also opted to use decimals for the monetary datatypes for the same reason. 

This took me roughly 2.5 hours from reading the brief into then completing the project. really enjoyed it :) thanks for reading. 
