﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AnnualReports.Infrastructure.Core.Migrations {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ScriptResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ScriptResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AnnualReports.Infrastructure.Core.Migrations.ScriptResources", typeof(ScriptResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE GetDISTFundsReportDataPro;.
        /// </summary>
        internal static string GetDISTFundsReportDataPro_Down {
            get {
                return ResourceManager.GetString("GetDISTFundsReportDataPro_Down", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE GetDISTFundsReportDataPro
        ///	@Year INT,
        ///	@FundNumber NVARCHAR(9)
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from
        ///	-- interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///    SELECT ParentFunds.FundNumber as PrimaryFundNumber, ParentFunds.Year, ParentFunds.DisplayName AS FundDisplayName
        ///	, ReportView.PERIODID AS View_Period, ReportView.ACTNUMBR_1 AS View_FundNumber, ReportView.ACTNUMBR_3 AS View_BarNumber, ReportView.DEBITAMT AS Debit, ReportView.CRDTAMNT AS Cred [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetDISTFundsReportDataPro_UP {
            get {
                return ResourceManager.GetString("GetDISTFundsReportDataPro_UP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE GetFundsReportDataPro;.
        /// </summary>
        internal static string GetFundsReportDataPro_Down {
            get {
                return ResourceManager.GetString("GetFundsReportDataPro_Down", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE GetFundsReportDataPro
        ///	@Year INT,
        ///	@FundNumber NVARCHAR(9)
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from
        ///	-- interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///    CREATE TABLE #ReportDate (PrimaryFundNumber NVARCHAR(9), Year SMALLINT, FundDisplayName NVARCHAR(100), View_Period SMALLINT, View_FundNumber VARCHAR(9), View_BarNumber VARCHAR(9), Debit NUMERIC(19,5), Credit NUMERIC(19,5))
        ///	
        ///	INSERT #ReportDate
        ///	EXEC GetGCFundsReportDataPro @Year, @FundNumber
        /// </summary>
        internal static string GetFundsReportDataPro_UP {
            get {
                return ResourceManager.GetString("GetFundsReportDataPro_UP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE GetGCFundsReportDataPro;.
        /// </summary>
        internal static string GetGCFundsReportDataPro_Down {
            get {
                return ResourceManager.GetString("GetGCFundsReportDataPro_Down", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE GetGCFundsReportDataPro
        ///	@Year INT,
        ///	@FundNumber NVARCHAR(9)
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from
        ///	-- interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///    SELECT ParentFunds.FundNumber as PrimaryFundNumber, ParentFunds.Year, ParentFunds.DisplayName AS FundDisplayName
        ///	, ReportView.PERIODID AS View_Period, ReportView.ACTNUMBR_1 AS View_FundNumber, ReportView.ACTNUMBR_5 AS View_BarNumber, ReportView.DEBITAMT AS Debit, ReportView.CRDTAMNT AS Credit [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetGCFundsReportDataPro_UP {
            get {
                return ResourceManager.GetString("GetGCFundsReportDataPro_UP", resourceCulture);
            }
        }
    }
}