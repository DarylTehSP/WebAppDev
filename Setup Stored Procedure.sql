CREATE PROCEDURE uspSelectCustomerAccounts
(

  @pageNo INT = 1,   
  @pageSize INT = 10  
)
AS
BEGIN
    /*–Declaring local Variables corresponding to parameters for modification */
    DECLARE 
    @_pageNbr INT,
    @_pageSize INT,
    @_firstRec INT,
    @_lastRec INT,
    @_totalRows INT

    /*Setting local Variables*/
	/*Copying all parameter input values into the respective local variables */
	SET @_pageNbr = @pageNo
    SET @_pageSize = @pageSize
    SET @_firstRec = ( @_pageNbr - 1 ) * @_pageSize
    SET @_lastRec = ( @_pageNbr * @_pageSize + 1 )
    SET @_totalRows = @_firstRec - @_lastRec + 1

    ;WITH CTE_Results AS (
   
	  SELECT  ROW_NUMBER() OVER (ORDER BY AccountName ASC) As ROWNUM,
			  Count(*) over () AS TotalCount, CustomerAccountId,
			  AccountName, IsVisible, CreatedById, UpdatedById, UpdatedAt
	  FROM CustomerAccount
)
SELECT
  TotalCount,
  ROWNUM,
  CustomerAccountId,
  AccountName,
  IsVisible,
  CreatedById,
  UpdatedById,
  UpdatedAt
FROM CTE_Results AS CPC
WHERE ROWNUM > @_firstRec
  AND ROWNUM < @_lastRec
ORDER BY ROWNUM ASC

END
GO

CREATE PROCEDURE uspSelectCustomerAccountRates
(

  @pageNo INT = 1,   
  @pageSize INT = 10  
)
AS
BEGIN
    /*–Declaring local Variables corresponding to parameters for modification */
    DECLARE 
    @_pageNbr INT,
    @_pageSize INT,
    @_firstRec INT,
    @_lastRec INT,
    @_totalRows INT

    /*Setting local Variables*/
	/*Copying all parameter input values into the respective local variables */
	SET @_pageNbr = @pageNo
    SET @_pageSize = @pageSize
    SET @_firstRec = ( @_pageNbr - 1 ) * @_pageSize
    SET @_lastRec = ( @_pageNbr * @_pageSize + 1 )
    SET @_totalRows = @_firstRec - @_lastRec + 1

    ;WITH CTE_Results AS (
   
	  SELECT  ROW_NUMBER() OVER (ORDER BY RatePerHour ASC) As ROWNUM,
			  Count(*) over () AS TotalCount, AccountRateId, CustomerAccountId,
			  RatePerHour, EffectiveStartDate, EffectiveEndDate
	  FROM AccountRate
)
SELECT
  TotalCount,
  ROWNUM,
  AccountRateId,
  CustomerAccountId,
  RatePerHour,
  EffectiveStartDate,
  EffectiveEndDate

FROM CTE_Results AS CPC
WHERE ROWNUM > @_firstRec
  AND ROWNUM < @_lastRec
ORDER BY ROWNUM ASC

END
GO

CREATE PROCEDURE uspSelectCustomerComments
(

  @pageNo INT = 1,   
  @pageSize INT = 10  
)
AS
BEGIN
    /*–Declaring local Variables corresponding to parameters for modification */
    DECLARE 
    @_pageNbr INT,
    @_pageSize INT,
    @_firstRec INT,
    @_lastRec INT,
    @_totalRows INT

    /*Setting local Variables*/
	/*Copying all parameter input values into the respective local variables */
	SET @_pageNbr = @pageNo
    SET @_pageSize = @pageSize
    SET @_firstRec = ( @_pageNbr - 1 ) * @_pageSize
    SET @_lastRec = ( @_pageNbr * @_pageSize + 1 )
    SET @_totalRows = @_firstRec - @_lastRec + 1

    ;WITH CTE_Results AS (
   
	  SELECT  ROW_NUMBER() OVER (ORDER BY CreatedAt DESC) As ROWNUM,
			  Count(*) over () AS TotalCount, CustomerAccountId,
			  CustomerAccountCommentId, Comment, CreatedAt, CreatedById, ParentId
	  FROM CustomerAccountComment
)
SELECT
  TotalCount,
  ROWNUM,
  CustomerAccountId,
  CustomerAccountCommentId,
  CreatedAt,
  CreatedById,
  Comment,
  ParentId
FROM CTE_Results AS CPC
WHERE ROWNUM > @_firstRec
  AND ROWNUM < @_lastRec
ORDER BY ROWNUM ASC

END
GO

CREATE PROCEDURE uspCheckOverlapBeforeCreate @CustomerAccountId int,@EffectiveStartDate date, @EffectiveEndDate date
AS(
  SELECT * FROM AccountRate WHERE 
    ((@EffectiveStartDate >= EffectiveStartDate and @EffectiveStartDate <= EffectiveEndDate)
    OR (@EffectiveEndDate >= EffectiveStartDate and @EffectiveEndDate <= EffectiveEndDate)
    OR (@EffectiveStartDate <= EffectiveStartDate and @EffectiveEndDate >= EffectiveEndDate)) AND
	AccountRate.CustomerAccountId = @CustomerAccountId 
)

GO

CREATE PROCEDURE uspCheckOverlapBeforeUpdate @RecordId int, @CustomerAccountId int,@EffectiveStartDate date, @EffectiveEndDate date
AS(
  SELECT * FROM AccountRate WHERE 
    ((@EffectiveStartDate >= EffectiveStartDate and @EffectiveStartDate <= EffectiveEndDate)
    OR (@EffectiveEndDate >= EffectiveStartDate and @EffectiveEndDate <= EffectiveEndDate)
    OR (@EffectiveStartDate <= EffectiveStartDate and @EffectiveEndDate >= EffectiveEndDate)) AND
	AccountRate.CustomerAccountId = @CustomerAccountId AND AccountRate.AccountRateId != @RecordId 
)
GO

Create PROCEDURE uspCheckAccountRateBeforeCreate @AccountRate int, @CustomerAccountId int, @EffectiveStartDate date
AS(
	SELECT * FROM AccountRate WHERE
		(@AccountRate = RatePerHour AND @EffectiveStartDate = DATEADD(d, 1, EffectiveEndDate)) AND
		AccountRate.CustomerAccountId = @CustomerAccountId
)
GO

CREATE PROCEDURE uspCheckAccountRateBeforeUpdate  @AccountRateId int,@CustomerAccountId int,@EffectiveStartDate date, @EffectiveEndDate date, @RatePerHour decimal(6,2) 
AS(   SELECT * FROM AccountRate WHERE      
((@EffectiveStartDate = DATEADD(DAY, 1, EffectiveEndDate))     
OR (@EffectiveEndDate = DATEADD(DAY, -1, EffectiveStartDate))) AND  
AccountRate.CustomerAccountId = @CustomerAccountId  AND AccountRate.RatePerHour = @RatePerHour AND AccountRate.AccountRateId != @AccountRateId)
Go

CREATE PROCEDURE uspSelectCustomerAccountTimeTable
(

  @pageNo INT = 1,   
  @pageSize INT = 10  
)
AS
BEGIN
    /*–Declaring local Variables corresponding to parameters for modification */
    DECLARE 
    @_pageNbr INT,
    @_pageSize INT,
    @_firstRec INT,
    @_lastRec INT,
    @_totalRows INT

    /*Setting local Variables*/
	/*Copying all parameter input values into the respective local variables */
	SET @_pageNbr = @pageNo
    SET @_pageSize = @pageSize
    SET @_firstRec = ( @_pageNbr - 1 ) * @_pageSize
    SET @_lastRec = ( @_pageNbr * @_pageSize + 1 )
    SET @_totalRows = @_firstRec - @_lastRec + 1

    ;WITH CTE_Results AS (
   
	  SELECT  ROW_NUMBER() OVER (ORDER BY DayOfWeekNumber ASC) As ROWNUM,
			  Count(*) over () AS TotalCount, AccountTimeTableId, AccountRateId, EffectiveStartDateTime, DayOfWeekNumber,
			  EffectiveEndDateTime, IsVisible
	  FROM AccountTimeTable
)
SELECT
  TotalCount,
  ROWNUM,
  AccountTimeTableId,
  AccountRateId,
  EffectiveStartDateTime,
  EffectiveEndDateTime,
  DayOfWeekNumber,
  IsVisible
FROM CTE_Results AS CPC
WHERE ROWNUM > @_firstRec
  AND ROWNUM < @_lastRec
ORDER BY ROWNUM ASC

END
GO

CREATE PROCEDURE uspAccountTimeTableCheckIdenticalBeforeAdd @AccountRateId int, @DayNum int, @EffectiveStartDateTime datetime, @EffectiveEndDatetime datetime
AS(
  SELECT * FROM AccountTimeTable WHERE 
    ((@DayNum = DayOfWeekNumber and @EffectiveStartDateTime = EffectiveStartDateTime and @EffectiveEndDatetime <= EffectiveEndDateTime) AND
	AccountTimeTable.AccountRateId = @AccountRateId)
	)

GO

CREATE PROCEDURE uspAccountTimeTableOverlapBeforeAdd @EffectiveStartDateTime datetime, @EffectiveEndDatetime datetime, @AccountRateId int, @DayNum int
AS(
  SELECT * FROM AccountTimeTable WHERE 
     (AccountTimeTable.AccountRateId = @AccountRateId) AND (AccountTimeTable.DayOfWeekNumber = @DayNum) and
     ((cast(@EffectiveStartDateTime as time) >= cast(EffectiveStartDateTime as time) and cast(@EffectiveStartDateTime as time) <= cast(EffectiveEndDateTime as time))
	 OR (cast(@EffectiveEndDateTime as time) >= cast(EffectiveStartDateTime as time) and cast(@EffectiveEndDateTime as time) <= cast(EffectiveEndDateTime as time))
	 OR (cast(@EffectiveStartDateTime as time) <= cast(EffectiveStartDateTime as time) and cast(@EffectiveEndDateTime as time) >= cast(EffectiveEndDateTime as time))
	 ))
GO

CREATE PROCEDURE uspAccountTimeTableCheckIdenticalBeforeUpdate @AccountRateId int, @DayNum int, @EffectiveStartDateTime datetime, @EffectiveEndDatetime datetime
AS(
   SELECT * FROM AccountTimeTable WHERE 
    ((@DayNum = DayOfWeekNumber and @EffectiveStartDateTime = EffectiveStartDateTime and @EffectiveEndDatetime <= EffectiveEndDateTime) AND
	AccountTimeTable.AccountRateId = @AccountRateId)
	)
GO

CREATE PROCEDURE uspAccountTimeTableOverlapBeforeUpdate @EffectiveStartDateTime datetime, @EffectiveEndDatetime datetime, @AccountRateId int, @DayNum int
AS(
  SELECT * FROM AccountTimeTable WHERE 
     (AccountTimeTable.AccountRateId = @AccountRateId) AND (AccountTimeTable.DayOfWeekNumber = @DayNum) and
     ((cast(@EffectiveStartDateTime as time) >= cast(EffectiveStartDateTime as time) and cast(@EffectiveStartDateTime as time) <= cast(EffectiveEndDateTime as time))
	 OR (cast(@EffectiveEndDateTime as time) >= cast(EffectiveStartDateTime as time) and cast(@EffectiveEndDateTime as time) <= cast(EffectiveEndDateTime as time))
	 OR (cast(@EffectiveStartDateTime as time) <= cast(EffectiveStartDateTime as time) and cast(@EffectiveEndDateTime as time) >= cast(EffectiveEndDateTime as time))
	 ))
GO