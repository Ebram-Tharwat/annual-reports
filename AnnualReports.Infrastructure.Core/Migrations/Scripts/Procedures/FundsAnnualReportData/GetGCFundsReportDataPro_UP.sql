﻿CREATE PROCEDURE GetGCFundsReportDataPro
	@Year INT,
	@FundNumber NVARCHAR(9)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT ParentFunds.FundNumber as PrimaryFundNumber, ParentFunds.Year, ParentFunds.DisplayName AS FundDisplayName
	, ReportView.PERIODID AS View_Period, ReportView.ACTNUMBR_1 AS View_FundNumber, ReportView.ACTNUMBR_5 AS View_BarNumber, ReportView.DEBITAMT AS Debit, ReportView.CRDTAMNT AS Credit
	FROM Funds AS ParentFunds INNER JOIN GCTest.dbo.slbAccountSummary AS ReportView 
	ON ( (ReportView.ACTNUMBR_1 = ParentFunds.FundNumber) OR (ReportView.ACTNUMBR_1 IN (SELECT FundNumber FROM Funds AS ChildFunds WHERE MapToFundId IS NOT NULL AND IsActive = 1 AND ChildFunds.MapToFundId = ParentFunds.Id AND Year = @Year AND ChildFunds.DbSource = 1)) )
	WHERE MapToFundId IS NULL AND ParentFunds.Year = @Year AND ParentFunds.IsActive = 1 AND ParentFunds.DbSource = 1 -- 1 stands for GC
	AND ReportView.YEAR1 = @Year AND ReportView.ACTIVE = 1
	AND (@FundNumber IS NULL OR (ParentFunds.FundNumber = @FundNumber AND ReportView.ACTNUMBR_1 = @FundNumber))
END
GO
