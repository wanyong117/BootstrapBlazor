﻿using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Magicodes.ExporterAndImporter.Excel;
using OfficeOpenXml.Table;
using System.ComponentModel;

namespace b03sqlite.Data;

[ExcelImporter(IsLabelingError = true)]
[ExcelExporter(Name = "导入商品中间表", TableStyle = TableStyles.Light10, AutoFitAllColumn = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true)]
public class WeatherForecast
{
    [Column(IsIdentity = true)]
    [DisplayName("序号")]
    public int ID { get; set; }

    [DisplayName("日期")]
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }


    [DisplayName("未审批/价格隐藏")]
    public bool? HidePrice { get => hidePrice ?? false; set => hidePrice = value; }
    bool? hidePrice = true;

    [DisplayName("显示库存")]
    public bool? ShowStock { get; set; }

    [DisplayName("显示库存2")]
    public bool ShowStock2 { get; set; }

}