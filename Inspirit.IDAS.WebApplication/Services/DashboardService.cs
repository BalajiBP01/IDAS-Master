using Inspirit.IDAS.Data;
using Inspirit.IDAS.Data.Production;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class DashboardService
    {

        ProductionDbContext _productionContext;
        IDASDbContext _IDAScontext;
        public DashboardService(ProductionDbContext productionContext,  IDASDbContext IDAScontext)
        {
            _productionContext = productionContext;
            _IDAScontext = IDAScontext;
        }
       
        public List<DashboardVm> GetDashboardData()
        {
            List<DashboardVm> count = new List<DashboardVm>();
            var lst = _productionContext.Dashboards.ToList();
            foreach(var data in lst)
            {
                var dash = new DashboardVm();
                dash.InsertCount = data.InsertCount;
                dash.TableName = data.TableName;
                dash.TotalCount = data.TotalCount;
                dash.UpdateCount = data.UpdateCount;
                dash.YearToDateUpdate = data.YearToDateUpdate;

                dash.insertper = (data.InsertCount * 100) / data.TotalCount;
                dash.updateper = (data.UpdateCount*100) / data.TotalCount;
                dash.yeartodateper = (data.YearToDateUpdate *100) / data.TotalCount;
                count.Add(dash);
            }
            return count;
        }
        
    }
    public class DashboardVm
    {
        public long InsertCount { get; set; }

        public long UpdateCount { get; set; }
        public long YearToDateUpdate { get; set; }
        public string TableName { get; set; }
        public long TotalCount { get; set; }
        public decimal insertper { get; set; }
        public decimal updateper { get; set; }
        public decimal yeartodateper { get; set; }
    }
}
