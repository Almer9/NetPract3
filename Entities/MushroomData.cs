using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTEST
{
    public class Mushroom
    {
        [LoadColumn(0)]
        public string? edibility { get; set; }
        [LoadColumn(1)]
        public string capShape { get; set; }
        [LoadColumn(2)]
        public string capSurface { get; set; }
        [LoadColumn(3)]
        public string capColor { get; set; }
        [LoadColumn(4)]
        public string bruises { get; set; }
        [LoadColumn(5)]
        public string odor { get; set; }
        [LoadColumn(6)]
        public string gillAttachment { get; set; }
        [LoadColumn(7)]
        public string gillSpacing { get; set; }
        [LoadColumn(8)]
        public string gillSize { get; set; }
        [LoadColumn(9)]
        public string gillColor { get; set; }
        [LoadColumn(10)]
        public string stalkShape { get; set; }
        [LoadColumn(11)]
        public string stalkRoot { get; set; }
        [LoadColumn(12)]
        public string stalkSurfaceAboveRing { get; set; }
        [LoadColumn(13)]
        public string stalkSurfaceBelowRing { get; set; }
        [LoadColumn(14)]
        public string stalkColorAboveRing { get; set; }
        [LoadColumn(15)]
        public string stalkColorBelowRing { get; set; }
        [LoadColumn(16)]
        public string veilType { get; set; }
        [LoadColumn(17)]
        public string veilColor { get; set; }
        [LoadColumn(18)]
        public string ringNumber { get; set; }
        [LoadColumn(19)]
        public string ringType { get; set; }
        [LoadColumn(20)]
        public string sporePrintColor { get; set; }
        [LoadColumn(21)]
        public string population { get; set; }
        [LoadColumn(22)]
        public string habitat { get; set; }
    }
    public class MushroomPrediction
    {
        [ColumnName("PredictedLabel")]
        public string? edibility;
    }
}
