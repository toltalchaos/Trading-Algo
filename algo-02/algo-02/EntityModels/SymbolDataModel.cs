using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace algo_02.EntityModels
{
    public class SymbolDataModel
    {

    }
    public class MetaData
    {

    }
    public class TimeSeries
    {

    }
    public class DayAndTime
    {
        public string Symbol { get; set; }

        public decimal? Open { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Close { get; set; }

        public int? Volume { get; set; }

        public DateTime? DataTime { get; set; }
    }

}

