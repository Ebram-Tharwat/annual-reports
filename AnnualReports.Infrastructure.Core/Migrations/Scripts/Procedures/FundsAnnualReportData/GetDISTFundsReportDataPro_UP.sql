CREATE PROCEDURE GetDISTFundsReportDataPro
	@Year INT,
	@FundId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT ParentFunds.FundNumber as PrimaryFundNumber, ParentFunds.Year, ParentFunds.DisplayName AS FundDisplayName, ParentFunds.MCAG
	, ReportView.PERIODID AS View_Period, ReportView.ACTNUMBR_1 AS View_FundNumber, ReportView.ACTNUMBR_3 AS View_BarNumber, ReportView.DEBITAMT AS Debit, ReportView.CRDTAMNT AS Credit
	FROM Funds AS ParentFunds INNER JOIN DISTTest.dbo.slbAccountSummary AS ReportView 
	ON ( (SUBSTRING(ReportView.Actnumbr_1, 1, 3) = ParentFunds.FundNumber) OR (SUBSTRING(ReportView.Actnumbr_1, 1, 3) IN (SELECT FundNumber FROM Funds AS ChildFunds WHERE MapToFundId IS NOT NULL AND IsActive = 1 AND ChildFunds.MapToFundId = ParentFunds.Id AND Year = @Year AND ChildFunds.DbSource = 2)) )
	WHERE MapToFundId IS NULL AND ParentFunds.Year = @Year AND ParentFunds.IsActive = 1 AND ParentFunds.DbSource = 2 -- 2 stands for DIST
	AND ReportView.YEAR1 = @Year AND ReportView.ACTIVE = 1
	AND (@FundId IS NULL OR (ParentFunds.Id = @FundId))
END
GO