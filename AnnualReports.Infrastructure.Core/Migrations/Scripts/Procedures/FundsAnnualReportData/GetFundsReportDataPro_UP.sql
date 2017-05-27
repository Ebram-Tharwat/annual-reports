CREATE PROCEDURE GetFundsReportDataPro
	@Year INT,
	@FundNumber NVARCHAR(9)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    CREATE TABLE #ReportDate (PrimaryFundNumber NVARCHAR(9), Year SMALLINT, FundDisplayName NVARCHAR(100), MCAG NVARCHAR(10), View_Period SMALLINT, View_FundNumber VARCHAR(9), View_BarNumber VARCHAR(9), Debit NUMERIC(19,5), Credit NUMERIC(19,5))
	
	INSERT #ReportDate
	EXEC GetGCFundsReportDataPro @Year, @FundId

	INSERT #ReportDate
	EXEC GetDISTFundsReportDataPro @Year, @FundId

	SELECT * FROM #ReportDate
END
GO