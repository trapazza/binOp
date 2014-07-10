'--------------------------------------------------------------------------------------------------------
'--------------------------------------------------------------------------------------------------------
' CLASS
'--------------------------------------------------------------------------------------------------------
'--------------------------------------------------------------------------------------------------------
Public Class ReportsTables

    Private _dtReportDate As System.DateTime
    Private strConnectionString As String = ConfigurationSettings.AppSettings("EPPRConnectionString")
    '-------------------------------------------------------------------------------------------------------
    'Report Date
    '-------------------------------------------------------------------------------------------------------
    Public Property ReportDate() As System.DateTime
        Get

            Return _dtReportDate

        End Get
        Set(ByVal Value As System.DateTime)

            _dtReportDate = Value
            SetReportDate()

        End Set
    End Property
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportDate with _dtReportDate
    '-------------------------------------------------------------------------------------------------------
    Private Sub SetReportDate()

        Dim csExecute As DataAccessLayer.SqlHelper

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportDate", _dtReportDate)

        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportOilProduction
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportOilProduction()

        Dim csOilProd As New BusinessLogicLayer.OilProductionData(_dtReportDate, Globals.gintFieldSk)
        Dim arParameters(6)

        arParameters(0) = CInt(csOilProd.Total)
        arParameters(1) = CInt(csOilProd.Target)
        arParameters(2) = CInt(csOilProd.MRA)
        arParameters(3) = CInt(csOilProd.MonthTotal)
        arParameters(4) = CLng(csOilProd.YearTotal)
        arParameters(5) = CLng(csOilProd.TotalCumm)

        csOilProd = Nothing

        Dim csExecute As DataAccessLayer.SqlHelper

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportOilProduction")
        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportOilProduction", _dtReportDate, arParameters(0), arParameters(1), arParameters(2), arParameters(3), arParameters(4), arParameters(5))

        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportExport
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportExport()

        Dim csOilExport As New BusinessLogicLayer.OilExportData(_dtReportDate, Globals.gstrFieldName)
        Dim arParameters(13)

        arParameters(0) = csOilExport.StockAt00
        arParameters(1) = CInt(csOilExport.DailyOilExport)
        If csOilExport.BasedInvShipments = "" Then

            arParameters(2) = "-"

        Else

            arParameters(2) = csOilExport.BasedInvShipments

        End If
        arParameters(3) = csOilExport.MeteringUnit

        If csOilExport.Metering = "" Then

            arParameters(4) = System.DBNull.Value

        Else

            arParameters(4) = csOilExport.Metering

        End If
        arParameters(5) = csOilExport.MonthlyOilExport
        arParameters(6) = csOilExport.YearlyOilExport
        arParameters(7) = csOilExport.TotalCummOilExport
        arParameters(8) = csOilExport.StockAt24
        arParameters(9) = Math.Round(csOilExport.SpGrAPI, 1)
        arParameters(10) = Math.Round(csOilExport.Salinity, 1)
        arParameters(11) = Math.Round(csOilExport.RVP, 1)
        arParameters(12) = Math.Round(csOilExport.BSW, 1)

        csOilExport = Nothing

        Dim csExecute As DataAccessLayer.SqlHelper

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportExport")
        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportExport", _dtReportDate, arParameters(0), arParameters(1), arParameters(2), arParameters(3), arParameters(4), arParameters(5), arParameters(6), arParameters(7), arParameters(8), arParameters(9), arParameters(10), arParameters(11), arParameters(12))

        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportCompAvailability
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportCompAvailability()

        Dim fieldcomps As New BusinessLogicLayer.CompressorSet
        Dim arColumns(6) As String

        fieldcomps.FieldName = Globals.gstrFieldName

        Dim csExecute As DataAccessLayer.SqlHelper

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportCompAvailability")

        If fieldcomps.Count > 0 Then

            Dim J As System.Int32

            For J = 0 To fieldcomps.Count - 1

                Dim InComp As New BusinessLogicLayer.Compressor

                InComp.setProductionOn(_dtReportDate)

                If fieldcomps.Compressors(J, InComp) = True Then

                    Select Case Trim(InComp.Name)

                        Case "RKF X-101A"

                            arColumns(0) = InComp.Hours.ToString & " hrs " & InComp.Minutes.ToString & " min"

                        Case "RKF X-101B"

                            arColumns(1) = InComp.Hours.ToString & " hrs " & InComp.Minutes.ToString & " min"

                        Case "RKF X-101C"

                            arColumns(2) = InComp.Hours.ToString & " hrs " & InComp.Minutes.ToString & " min"

                        Case "RKF X-101D"

                            arColumns(3) = InComp.Hours.ToString & " hrs " & InComp.Minutes.ToString & " min"

                        Case "RKF X-101E"

                            arColumns(4) = InComp.Hours.ToString & " hrs " & InComp.Minutes.ToString & " min"

                        Case "RKF X-110"

                            arColumns(5) = InComp.Hours.ToString & " hrs " & InComp.Minutes.ToString & " min"

                    End Select

                End If
                InComp = Nothing

            Next

            csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportCompAvailability", _dtReportDate, arColumns(0), arColumns(1), arColumns(2), arColumns(3), arColumns(4), arColumns(5))

        End If

        fieldcomps = Nothing
        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportGasProduction
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportGasProduction()

        Dim csGasProd As New BusinessLogicLayer.GasProductionData(_dtReportDate, Globals.gintFieldSk)
        Dim arParameters(19)

        arParameters(0) = csGasProd.DailyGasProd
        arParameters(1) = csGasProd.DailyGasConsump
        arParameters(2) = csGasProd.DailyGasInj
        arParameters(3) = csGasProd.DailyGasFlare

        arParameters(4) = csGasProd.MonthlyGasProd
        arParameters(5) = csGasProd.MonthlyGasConsump
        arParameters(6) = csGasProd.MonthlyGasInj
        arParameters(7) = csGasProd.MonthlyGasFlare

        arParameters(8) = csGasProd.YearlyGasProd
        arParameters(9) = csGasProd.YearlyGasConsump
        arParameters(10) = csGasProd.YearlyGasInj
        arParameters(11) = csGasProd.YearlyGasFlare

        arParameters(12) = csGasProd.TotalCummGasProd
        arParameters(13) = csGasProd.TotalCummGasConsump
        arParameters(14) = csGasProd.TotalCummGasInj
        arParameters(15) = csGasProd.TotalCummGasFlare

        arParameters(16) = Math.Round(csGasProd.DailyPercenGasFlare, 2)
        arParameters(17) = csGasProd.AverageInjManifPress
        arParameters(18) = Math.Round(csGasProd.FlareMRA, 2)

        csGasProd = Nothing

        Dim csExecute As DataAccessLayer.SqlHelper

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportGasProduction", _dtReportDate, arParameters(0), arParameters(1), arParameters(2), arParameters(3), arParameters(4), arParameters(5), arParameters(6), arParameters(7), arParameters(8), arParameters(9), arParameters(10), arParameters(11), arParameters(12), arParameters(13), arParameters(14), arParameters(15), arParameters(16), arParameters(17), arParameters(18))

        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportTankSummary
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportTankSummary()

        Dim csExecute As DataAccessLayer.SqlHelper
        Dim FieldTanks As New BusinessLogicLayer.TankSet

        Dim stTankName As System.String
        Dim arStocks00(4) As System.String
        Dim arDailyOilExports(4) As System.String
        Dim arStocks24(4) As System.String
        Dim arStatus(4) As System.String

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportTankSummary")

        FieldTanks.FieldName = Globals.gstrFieldName
        If FieldTanks.Count > 0 Then

            stTankName = ""
            arStocks00(0) = "Stock at 00:00 bbls"
            arDailyOilExports(0) = "Daily Oil Export bbls"
            arStocks24(0) = "Stock at 24:00 bbls"
            arStatus(0) = "Status"

            Dim I As System.Int32

            For I = 0 To FieldTanks.Count - 1

                Dim InTank As New BusinessLogicLayer.Tank

                If FieldTanks.Tanks(I, InTank) = True Then

                    InTank.setProductionOn(_dtReportDate)
                    stTankName = Trim(InTank.Name)
                    Select Case stTankName

                        Case "RFK T 205"

                            arStocks00(1) = InTank.Stock00.ToString
                            arDailyOilExports(1) = InTank.OilExport.ToString
                            arStocks24(1) = InTank.Stock24.ToString
                            arStatus(1) = InTank.Status

                        Case "RKF T 206"

                            arStocks00(2) = InTank.Stock00.ToString
                            arDailyOilExports(2) = InTank.OilExport.ToString
                            arStocks24(2) = InTank.Stock24.ToString
                            arStatus(2) = InTank.Status

                        Case "RKF T 207"

                            arStocks00(3) = InTank.Stock00.ToString
                            arDailyOilExports(3) = InTank.OilExport.ToString
                            arStocks24(3) = InTank.Stock24.ToString
                            arStatus(3) = InTank.Status

                    End Select

                End If

                InTank = Nothing

            Next


        End If

        FieldTanks = Nothing

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportTankSummary", _dtReportDate, 1, arStocks00(0), arStocks00(1), arStocks00(2), arStocks00(3))
        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportTankSummary", _dtReportDate, 2, arDailyOilExports(0), arDailyOilExports(1), arDailyOilExports(2), arDailyOilExports(3))
        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportTankSummary", _dtReportDate, 3, arStocks24(0), arStocks24(1), arStocks24(2), arStocks24(3))
        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportTankSummary", _dtReportDate, 4, arStatus(0), arStatus(1), arStatus(2), arStatus(3))

        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportWellStatusSumInjection
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportWellStatusSummaryInjection()

        Dim csExecute As DataAccessLayer.SqlHelper
        Dim arParameters(6)
        Dim csWellInj As New BusinessLogicLayer.GasInjectionData(_dtReportDate.Date, Globals.gintFieldSk)
        Dim drRow As DataRow

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportWellStatusSumInjection")

        If csWellInj.DetailedInjection.Rows.Count > 0 Then

            For Each drRow In csWellInj.DetailedInjection.Rows

                arParameters(0) = CInt(drRow.Item("WellID"))
                arParameters(1) = CStr(drRow.Item("Well"))
                If Not drRow.Item("Status") Is System.DBNull.Value Then
                    arParameters(2) = CStr(drRow.Item("Status"))
                Else
                    arParameters(2) = System.DBNull.Value
                End If
                If Not drRow.Item("FCV%") Is System.DBNull.Value Then
                    arParameters(3) = Math.Round(CDbl(drRow.Item("FCV%")), 3)
                Else
                    arParameters(3) = System.DBNull.Value
                End If
                If Not drRow.Item("Hours") Is System.DBNull.Value Then
                    arParameters(4) = CInt(drRow.Item("Hours"))
                Else
                    arParameters(4) = System.DBNull.Value
                End If
                If Not drRow.Item("Avj Inj Rate MMsfc") Is System.DBNull.Value Then
                    arParameters(5) = Math.Round(CDbl(drRow.Item("Avj Inj Rate MMsfc")), 1)
                Else
                    arParameters(5) = System.DBNull.Value
                End If
                csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportWellStatusSumInjection", _dtReportDate.Date, arParameters(0), arParameters(1), arParameters(2), arParameters(3), arParameters(4), arParameters(5))

            Next

        End If
        csWellInj = Nothing
        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportWellStatusSumProduction
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportWellStatusSummaryProduction()

        Dim csExecute As DataAccessLayer.SqlHelper
        Dim arParameters(6)
        Dim csWellProd As New BusinessLogicLayer.OilProductionData(_dtReportDate.Date, Globals.gintFieldSk)
        Dim drRow As DataRow

        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportWellStatusSumProduction")

        If csWellProd.DetailedProduction.Rows.Count > 0 Then

            For Each drRow In csWellProd.DetailedProduction.Rows

                arParameters(0) = CInt(drRow.Item("WellID"))
                arParameters(1) = CStr(drRow.Item("Well"))
                If Not drRow.Item("Status") Is System.DBNull.Value Then
                    arParameters(2) = CStr(drRow.Item("Status"))
                Else
                    arParameters(2) = System.DBNull.Value
                End If
                If Not drRow.Item("Choke") Is System.DBNull.Value Then
                    arParameters(3) = Math.Round(CDbl(drRow.Item("Choke")), 3)
                Else
                    arParameters(3) = System.DBNull.Value
                End If
                If Not drRow.Item("Hours") Is System.DBNull.Value Then
                    arParameters(4) = CInt(drRow.Item("Hours"))
                Else
                    arParameters(4) = System.DBNull.Value
                End If
                If Not drRow.Item("Allocated Oil Rate") Is System.DBNull.Value Then
                    arParameters(5) = CInt(drRow.Item("Allocated Oil Rate"))
                Else
                    arParameters(5) = System.DBNull.Value
                End If

                csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportWellStatusSumProduction", _dtReportDate.Date, arParameters(0), arParameters(1), arParameters(2), arParameters(3), arParameters(4), arParameters(5))

            Next

        End If
        csWellProd = Nothing
        csExecute = Nothing

    End Sub
    '-------------------------------------------------------------------------------------------------------
    'Fill tblReportWellTestSummary
    '-------------------------------------------------------------------------------------------------------
    Public Sub SetReportWellTestSummary()

        Dim csExecute As DataAccessLayer.SqlHelper
        Dim arParameters(10)
        Dim csWellTest As New BusinessLogicLayer.WellTestData(_dtReportDate.Date, Globals.gintFieldSk)
        Dim drRow As DataRow
        Dim i As System.Int32


        csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_DeleteReportWellTestSummary")
        If csWellTest.SummaryForDaily.Rows.Count > 0 Then

            i = 1
            For Each drRow In csWellTest.SummaryForDaily.Rows

                arParameters(0) = i
                i = i + 1
                arParameters(1) = drRow.Item("test_date")
                arParameters(2) = CStr(drRow.Item("comp_name"))
                If Not drRow.Item("Choke") Is System.DBNull.Value Then
                    arParameters(3) = CInt(drRow.Item("choke"))
                Else
                    arParameters(3) = System.DBNull.Value
                End If
                If Not drRow.Item("fthp") Is System.DBNull.Value Then
                    arParameters(4) = CInt(drRow.Item("fthp"))
                Else
                    arParameters(4) = System.DBNull.Value
                End If
                If Not drRow.Item("oil_rate") Is System.DBNull.Value Then
                    arParameters(5) = CInt(drRow.Item("oil_rate"))
                Else
                    arParameters(5) = System.DBNull.Value
                End If
                If Not drRow.Item("water_rate") Is System.DBNull.Value Then
                    arParameters(6) = CInt(drRow.Item("water_rate"))
                Else
                    arParameters(6) = System.DBNull.Value
                End If
                If Not drRow.Item("gas_rate") Is System.DBNull.Value Then
                    arParameters(7) = CInt(drRow.Item("gas_rate"))
                Else
                    arParameters(7) = System.DBNull.Value
                End If
                If Not drRow.Item("gor1") Is System.DBNull.Value Then
                    arParameters(8) = CInt(drRow.Item("gor1"))
                Else
                    arParameters(8) = System.DBNull.Value
                End If
                If Not drRow.Item("duration") Is System.DBNull.Value Then
                    arParameters(9) = CInt(drRow.Item("duration"))
                Else
                    arParameters(9) = System.DBNull.Value
                End If

                csExecute.ExecuteScalar(strConnectionString, "SP_ReportTables_UpdateReportWellTestSummary", _dtReportDate.Date, arParameters(0), arParameters(1), arParameters(2), arParameters(3), arParameters(4), arParameters(5), arParameters(6), arParameters(7), arParameters(8), arParameters(9))

            Next

        End If

        csWellTest = Nothing
        csExecute = Nothing

    End Sub

End Class