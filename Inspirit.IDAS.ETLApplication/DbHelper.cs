using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.Data.Raw;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.ETLApplication
{
    public class DbHelper
    {
        public static string  RawConnectionString {get;set; }

        public static string ProductionConnectionString { get; set; }

        public static string IDASConnectionString { get { return System.Configuration.ConfigurationManager.ConnectionStrings["IDASDBConnection"].ConnectionString; } }

        public static RawDbContext GetRawDbContext()
        {
            return new RawDbContext(RawConnectionString);
        }

        public static ProductionDbContext GetProductionDbContext()
        {
            return new ProductionDbContext(ProductionConnectionString);
        }

       

    }
}
