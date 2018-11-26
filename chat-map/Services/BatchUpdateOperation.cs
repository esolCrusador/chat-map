using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMap.Services
{
    public class BatchUpdateOperation
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string NewValue { get; set; }
    }
}
