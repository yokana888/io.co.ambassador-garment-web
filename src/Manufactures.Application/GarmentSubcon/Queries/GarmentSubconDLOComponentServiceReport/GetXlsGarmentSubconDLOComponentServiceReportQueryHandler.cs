﻿using ExtCore.Data.Abstractions;
using Infrastructure.Domain.Queries;
using Infrastructure.External.DanLirisClient.Microservice.HttpClientService;
using Manufactures.Domain.GarmentSubcon.SubconDeliveryLetterOuts.Repositories;
using Manufactures.Domain.GarmentSubcon.ServiceSubconCuttings.Repositories;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Data;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Manufactures.Application.GarmentSubcon.Queries.GarmentSubconDLOComponentServiceReport
{
    public class GetXlsGarmentSubconDLOComponentServiceReportQueryHandler : IQueryHandler<GetXlsGarmentSubconDLOComponentServiceReportQuery, MemoryStream>
    {
        protected readonly IHttpClientService _http;
        private readonly IStorage _storage;

        private readonly IGarmentSubconDeliveryLetterOutRepository garmentSubconDeliveryLetterOutRepository;
        private readonly IGarmentSubconDeliveryLetterOutItemRepository garmentSubconDeliveryLetterOutItemRepository;

        private readonly IGarmentServiceSubconCuttingRepository garmentServiceSubconCuttingRepository;
        private readonly IGarmentServiceSubconCuttingItemRepository garmentServiceSubconCuttingItemRepository;
        private readonly IGarmentServiceSubconCuttingDetailRepository garmentServiceSubconCuttingDetailRepository;
        private readonly IGarmentServiceSubconCuttingSizeRepository garmentServiceSubconCuttingSizeRepository;


        public GetXlsGarmentSubconDLOComponentServiceReportQueryHandler(IStorage storage, IServiceProvider serviceProvider)
        {
            _storage = storage;

            garmentSubconDeliveryLetterOutRepository = storage.GetRepository<IGarmentSubconDeliveryLetterOutRepository>();
            garmentSubconDeliveryLetterOutItemRepository = storage.GetRepository<IGarmentSubconDeliveryLetterOutItemRepository>();

            garmentServiceSubconCuttingRepository = storage.GetRepository<IGarmentServiceSubconCuttingRepository>();
            garmentServiceSubconCuttingItemRepository = storage.GetRepository<IGarmentServiceSubconCuttingItemRepository>();
            garmentServiceSubconCuttingDetailRepository = storage.GetRepository<IGarmentServiceSubconCuttingDetailRepository>();
            garmentServiceSubconCuttingSizeRepository = storage.GetRepository<IGarmentServiceSubconCuttingSizeRepository>();

            _http = serviceProvider.GetService<IHttpClientService>();
        }

        class monitoringViewTemp
        {
            public string DLType { get; internal set; }
            public string DLNo { get; internal set; }
            public DateTimeOffset DLDate { get; internal set; }
            public string ContractNo { get; internal set; }
            public string ContractType { get; internal set; }
            public string SubConCategory { get; internal set; }
            public string SubConNo { get; internal set; }
            public DateTimeOffset SubConDate { get; internal set; }
            public string SubConType { get; internal set; }
            public string UnitName { get; internal set; }
            public string BuyerCode { get; internal set; }
            public string BuyerName { get; internal set; }
            public string ComodityCode { get; internal set; }
            public string ComoditytName { get; internal set; }
            public string RONo { get; internal set; }
            public string ProductCode { get; internal set; }
            public string ProductName { get; internal set; }
            public string Size { get; internal set; }
            public string Colour { get; internal set; }
            public double Quantity { get; internal set; }
            public string UomUnit { get; internal set; }
        }

        public async Task<MemoryStream> Handle(GetXlsGarmentSubconDLOComponentServiceReportQuery request, CancellationToken cancellationToken)
        {
             var Query = (from a in garmentSubconDeliveryLetterOutRepository.Query
                          join b in garmentSubconDeliveryLetterOutItemRepository.Query on a.Identity equals b.SubconDeliveryLetterOutId
                          join c in garmentServiceSubconCuttingRepository.Query on b.SubconId equals c.Identity
                          join d in garmentServiceSubconCuttingItemRepository.Query on c.Identity equals d.ServiceSubconCuttingId
                          join e in garmentServiceSubconCuttingDetailRepository.Query on d.Identity equals e.ServiceSubconCuttingItemId
                          join f in garmentServiceSubconCuttingSizeRepository.Query on e.Identity equals f.ServiceSubconCuttingDetailId
                          where a.Deleted == false && b.Deleted == false && c.Deleted == false
                          && d.Deleted == false && e.Deleted == false && f.Deleted == false
                          && a.DLDate.AddHours(7).Date >= request.dateFrom
                          && a.DLDate.AddHours(7).Date <= request.dateTo.Date
                          && a.ContractType == "SUBCON JASA" && a.SubconCategory == "SUBCON JASA KOMPONEN"

                          select new monitoringViewTemp
                          {
                              DLType = a.DLType,
                              DLNo = a.DLNo,
                              DLDate = a.DLDate,
                              ContractNo= a.EPONo,
                              ContractType = a.ContractType,
                              SubConCategory = a.SubconCategory,
                              SubConNo = b.SubconNo,
                              SubConDate = c.SubconDate,
                              SubConType = c.SubconType,
                              UnitName = c.UnitName,
                              BuyerCode = c.BuyerCode,
                              BuyerName = c.BuyerName,
                              ComodityCode = d.ComodityCode,
                              ComoditytName = d.ComodityName,
                              RONo = d.RONo,
                              ProductCode = f.ProductCode,
                              ProductName = f.ProductName,
                              Size = f.SizeName,
                              Colour = f.Color,
                              Quantity = f.Quantity,
                              UomUnit = f.UomUnit
                          }).GroupBy(x => new {
                              x.DLType,
                              x.DLNo,
                              x.DLDate,
                              x.ContractNo,
                              x.ContractType,
                              x.SubConCategory,
                              x.SubConNo,
                              x.SubConDate,
                              x.SubConType,
                              x.UnitName,
                              x.BuyerCode,
                              x.BuyerName,
                              x.ComodityCode,
                              x.ComoditytName,
                              x.RONo,
                              x.ProductCode,
                              x.ProductName,
                              x.Size,
                              x.Colour,
                              x.Quantity,
                              x.UomUnit
                          }, (key, group) => new
                          {
                              dlType = key.DLType,
                              dlNo = key.DLNo,
                              dlDate = key.DLDate,
                              contractNo = key.ContractNo,
                              contractType = key.ContractType,
                              subConCategory = key.SubConCategory,
                              subConNo = key.SubConNo,
                              subConDate = key.SubConDate,
                              subConType = key.SubConType,
                              unitName = key.UnitName,
                              buyerCode = key.BuyerCode,
                              buyerName = key.BuyerName,
                              comodityCode = key.ComodityCode,
                              comoditytName = key.ComoditytName,
                              rONo = key.RONo,
                              productCode = key.ProductCode,
                              productName = key.ProductName,
                              size = key.Size,
                              colour = key.Colour,
                              quantity = group.Sum(x => x.Quantity),
                              uomUnit = key.UomUnit
                          }).ToList().OrderBy(x => x.dlNo).ThenBy(x => x.subConNo);


            GarmentSubconDLOComponentServiceReportListViewModel listViewModel = new GarmentSubconDLOComponentServiceReportListViewModel();
            List<GarmentSubconDLOComponentServiceReportDto> rekapcomponentservice = new List<GarmentSubconDLOComponentServiceReportDto>();

            foreach (var i in Query)
            {
                GarmentSubconDLOComponentServiceReportDto report = new GarmentSubconDLOComponentServiceReportDto
                {
                    DLType = i.dlType,
                    DLNo = i.dlNo,
                    DLDate = i.dlDate,
                    ContractNo = i.contractNo,
                    ContractType = i.contractType,
                    SubConCategory = i.subConCategory,
                    SubConNo = i.subConNo,
                    SubConDate = i.subConDate,
                    SubConType = i.subConType,
                    UnitName = i.unitName,
                    BuyerCode = i.buyerCode,
                    BuyerName = i.buyerName,
                    ComodityCode = i.comodityCode,
                    ComoditytName = i.comoditytName,
                    RONo = i.rONo,
                    ProductCode = i.productCode,
                    ProductName = i.productName,
                    Size = i.size,
                    Colour = i.colour,
                    Quantity = i.quantity,
                    UomUnit = i.uomUnit
                };

                rekapcomponentservice.Add(report);
            }

            listViewModel.garmentSubconDLOComponentServiceReportDto = rekapcomponentservice;

            DataTable reportDataTable = new DataTable();

            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Jenis SJ SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Jenis SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kategori SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No SJ SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tgl SJ SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No PO External", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tgl SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Tipe SubCon", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Nama Unit", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kode Buyer", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Nama Buyer", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kode Komoditi", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Nama Komoditi", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "No RO", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Kode Barang", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Nama Barang", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Size", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Colour", DataType = typeof(string) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Quantity", DataType = typeof(double) });
            reportDataTable.Columns.Add(new DataColumn() { ColumnName = "Satuan", DataType = typeof(string) });

            var index = 1;
            int idx = 1;
            var rCount = 0;
            Dictionary<string, string> Rowcount = new Dictionary<string, string>();

            if (listViewModel.garmentSubconDLOComponentServiceReportDto.Count() == 0)
            {
                reportDataTable.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 0, "");
            }
            else
            {                
                foreach (var item in listViewModel.garmentSubconDLOComponentServiceReportDto)
                {
                    idx++;
                    if (!Rowcount.ContainsKey(item.DLNo))
                    {
                        rCount = 0;
                        var index1 = idx;
                        Rowcount.Add(item.DLNo, index1.ToString());
                    }
                    else
                    {
                        rCount += 1;
                        Rowcount[item.DLNo] = Rowcount[item.DLNo] + "-" + rCount.ToString();
                        var val = Rowcount[item.DLNo].Split("-");
                        if ((val).Length > 0)
                        {
                            Rowcount[item.DLNo] = val[0] + "-" + rCount.ToString();
                        }
                    }

                    string dlDate = item.DLDate == null ? "-" : item.DLDate.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
                    string scDate = item.SubConDate == null ? "-" : item.SubConDate.ToOffset(new TimeSpan(7, 0, 0)).ToString("dd MMM yyyy", new CultureInfo("id-ID"));
            
                    reportDataTable.Rows.Add(item.DLType, item.ContractType, item.SubConCategory, item.DLNo, dlDate, item.ContractNo, item.SubConNo, scDate,
                                             item.ContractType, item.UnitName, item.BuyerCode, item.BuyerName, item.ComodityCode, item.ComoditytName,
                                             item.RONo, item.ProductCode, item.ProductName, item.Size, item.Colour, item.Quantity, item.UomUnit);
                }
            }

            using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Sheet 1");
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells["A4"].LoadFromDataTable(reportDataTable, true);

                var countdata = Query.Count();

                worksheet.Cells["A" + 1 + ":K" + 4 + ""].Style.Font.Bold = true;
                worksheet.Cells["A1"].Value = "LAPORAN SURAT JALAN SUBCON | JASA KOMPONEN";
                worksheet.Cells["A2"].Value = "Periode " + request.dateFrom.ToString("dd-MM-yyyy") + " S/D " + request.dateTo.ToString("dd-MM-yyyy");
                worksheet.Cells["A" + 1 + ":U" + 1 + ""].Merge = true;
                worksheet.Cells["A" + 2 + ":U" + 2 + ""].Merge = true;
                worksheet.Cells["A" + 1 + ":U" + 4 + ""].Style.Font.Bold = true;
                
                if (countdata > 0)
                {
                    worksheet.Cells["U" + 5 + ":U" + (4 + countdata) + ""].Merge = true;
                    worksheet.Cells["U" + 5 + ":U" + (4 + countdata) + ""].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"A{(5 + countdata)}:S{(5 + countdata)}"].Merge = true;
                    worksheet.Cells[$"A{(5 + countdata)}:T{(5 + countdata)}"].Style.Font.Bold = true;
                    //ADD SUMMARY OF QUANTITY
                    worksheet.Cells[$"A{(5 + countdata)}"].Value = "TOTAL SURAT JALAN SUBCON | JASA KOMPONEN :";
                    worksheet.Cells[$"T{(5 + countdata)}"].Formula = "SUM(" + worksheet.Cells["T" + 5 + ":T" + (4 + countdata) + ""].Address + ")";
                    worksheet.Cells[$"U{(5 + countdata)}"].Value = "PCS";
                    worksheet.Calculate();
                }

                //
                foreach (var a in Rowcount)
                {
                    var UnitrowNum = a.Value.Split("-");
                    int rowNum2 = 1;
                    int rowNum1 = Convert.ToInt32(UnitrowNum[0]);
                    if (UnitrowNum.Length > 1)
                    {
                        rowNum2 = Convert.ToInt32(rowNum1) + Convert.ToInt32(UnitrowNum[1]);
                    }
                    else
                    {
                        rowNum2 = Convert.ToInt32(rowNum1);
                    }

                    worksheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"A{(rowNum1 + 3)}:A{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"B{(rowNum1 + 3)}:B{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{(rowNum1 + 3)}:C{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"D{(rowNum1 + 3)}:D{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"E{(rowNum1 + 3)}:E{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"E{(rowNum1 + 3)}:E{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"E{(rowNum1 + 3)}:E{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"F{(rowNum1 + 3)}:F{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"F{(rowNum1 + 3)}:F{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"F{(rowNum1 + 3)}:F{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"G{(rowNum1 + 3)}:G{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"G{(rowNum1 + 3)}:G{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"G{(rowNum1 + 3)}:G{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"H{(rowNum1 + 3)}:H{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"H{(rowNum1 + 3)}:H{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"H{(rowNum1 + 3)}:H{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"I{(rowNum1 + 3)}:I{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"I{(rowNum1 + 3)}:I{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"I{(rowNum1 + 3)}:I{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"J{(rowNum1 + 3)}:J{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"J{(rowNum1 + 3)}:J{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"J{(rowNum1 + 3)}:J{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"K{(rowNum1 + 3)}:K{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"K{(rowNum1 + 3)}:K{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"K{(rowNum1 + 3)}:K{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"L{(rowNum1 + 3)}:L{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"L{(rowNum1 + 3)}:L{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"L{(rowNum1 + 3)}:L{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"M{(rowNum1 + 3)}:M{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"M{(rowNum1 + 3)}:M{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"M{(rowNum1 + 3)}:M{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"N{(rowNum1 + 3)}:N{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"N{(rowNum1 + 3)}:N{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"N{(rowNum1 + 3)}:N{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"O{(rowNum1 + 3)}:O{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"O{(rowNum1 + 3)}:O{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"O{(rowNum1 + 3)}:O{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"P{(rowNum1 + 3)}:P{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"P{(rowNum1 + 3)}:P{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"P{(rowNum1 + 3)}:P{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"Q{(rowNum1 + 3)}:Q{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"Q{(rowNum1 + 3)}:Q{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"Q{(rowNum1 + 3)}:Q{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"R{(rowNum1 + 3)}:R{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"R{(rowNum1 + 3)}:R{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"R{(rowNum1 + 3)}:R{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                    worksheet.Cells[$"S{(rowNum1 + 3)}:S{(rowNum2) + 3}"].Merge = true;
                    worksheet.Cells[$"S{(rowNum1 + 3)}:S{(rowNum2) + 3}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"S{(rowNum1 + 3)}:S{(rowNum2) + 3}"].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
				return stream;

            }
        }
    }
}

