using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class DataTableRequest
    {
        public Guid Id { get; set; }
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public Search search { get; set; }
        public List<OrderColumn> order { get; set; }
        public List<Column> columns { get; set; }
    
    }



    public class DataTableResponse<T>
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<T> data { get; set; } = new List<T>();

    }

    public class OrderColumn
    {
        public string column { get; set; }
        public string dir { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public string orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }
    public class displaynumber
    {
        public string GenerateNumber(int number, string type, int totalLenght)
        {
            //PRO0000001 type PRO number 1
            //PRO0000001
            string Inputnumber = number.ToString();
            string Output = type.ToUpper() + Inputnumber;
            while (Output.Length != totalLenght)
            {
                Inputnumber = "0" + Inputnumber;
                Output = type + Inputnumber;
            }
            return Output;
        }
    }
}
