using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Runtime;
//using Microsoft.SqlServer.Dts.Runtime.Wrapper;
namespace Inspirit.IDAS.ETLApplication
{
    public class ETLPackages
    {
        public string RunPackages()
        {
            string pkgLocation;
            Package pkg;
            Application app;
            DTSExecResult pkgResults;
            try
            {
                pkgLocation = @"C:\Users\sridhar.kn\Desktop\IDAS\01Telephone.dtsx";
                app = new Application();
                pkg = app.LoadPackage(pkgLocation, null);
                pkgResults = pkg.Execute();
                return "Success";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
