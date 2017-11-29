CREATE PROCEDURE GetGCFundsReportDataPro
	@Year INT,
	@FundId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT ParentFunds.FundNumber as PrimaryFundNumber, ParentFunds.Year, ParentFunds.DisplayName AS FundDisplayName, ParentFunds.MCAG, ParentFunds.DbSource
	, ReportView.PERIODID AS View_Period, ReportView.ACTNUMBR_1 AS View_FundNumber, ReportView.ACTNUMBR_5 AS View_BarNumber, ReportView.DEBITAMT AS Debit, ReportView.CRDTAMNT AS Credit
	, ReportView.ACTNUMBR_1, ReportView.ACTNUMBR_2, ReportView.ACTNUMBR_3, ReportView.ACTNUMBR_4, ReportView.ACTNUMBR_5, ReportView.ACTDESCR AS AccountDescription
	FROM Funds AS ParentFunds INNER JOIN GCTest.dbo.slbAccountSummary AS ReportView 
	ON ( (ReportView.ACTNUMBR_1 = ParentFunds.FundNumber) OR (ReportView.ACTNUMBR_1 IN (SELECT FundNumber FROM Funds AS ChildFunds WHERE MapToFundId IS NOT NULL AND IsActive = 1 AND ChildFunds.MapToFundId = ParentFunds.Id AND Year = @Year AND ChildFunds.DbSource = 1)) )
	WHERE MapToFundId IS NULL AND ParentFunds.Year = @Year AND ParentFunds.IsActive = 1 AND ParentFunds.DbSource = 1 -- 1 stands for GC
	AND ReportView.YEAR1 = @Year AND ReportView.ACTIVE = 1
	AND (@FundId IS NULL OR (ParentFunds.Id = @FundId))
END
GO
