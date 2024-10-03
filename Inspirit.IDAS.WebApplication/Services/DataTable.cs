using Inspirit.IDAS.Data.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication
{
    public class DataTableRequest
    {
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
        public long totalTime { get; set; }
        public List<T> data { get; set; } = new List<T>();

        // krishna start

        public string CustomerRefNum { get; set; }
        public string SearchInput { get; set; }
        public Int64 RefNum { get; set; }
        // krishna end


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

    public class ContactScore
    {
        public string GetContactScore(List<ContactDetail> details)
        {
            int cmonths = 0;
            int wmonths = 0;
            string score = "DIRTY";
            try
            {
                var list = details.OrderByDescending(t => t.createdonDate).ToList();

                if (list.Count >= 2)
                {
                    var cellnumber = list.Find(m => m.type.ToUpper() == "CELL");

                    var worknumber = list.Find(m => m.type.ToUpper() == "WORK" || m.type.ToUpper() == "HOME");

                    if (cellnumber != null && DateTime.Today > cellnumber.createdonDate.Value)
                    {
                        cmonths = (((DateTime.Today.Year - cellnumber.createdonDate.Value.Year) * 12) + (DateTime.Today.Month - cellnumber.createdonDate.Value.Month));
                    }
                    if (worknumber != null && DateTime.Today > worknumber.createdonDate.Value)
                    {
                        wmonths = (((DateTime.Today.Year - worknumber.createdonDate.Value.Year) * 12) + (DateTime.Today.Month - worknumber.createdonDate.Value.Month));
                    }

                    if (cmonths != 0 && wmonths != 0)
                    {
                        if (cmonths < 6 && wmonths < 3)
                        {
                            score = "PLATINUM";
                        }
                        else if (cmonths < 12 && wmonths < 6)
                        {
                            score = "GOLD";
                        }
                        else if (cmonths < 24 && wmonths < 12)
                        {
                            score = "SILVER";
                        }
                        else if (cmonths < 48 || wmonths < 48)
                        {
                            score = "BRONZE";
                        }
                        else
                            score = "COPPER";
                    }
                    else if (cmonths != 0 || wmonths != 0)
                    {
                        if (cmonths < 48 || wmonths < 48)
                        {
                            score = "BRONZE";
                        }
                        else
                            score = "COPPER";
                    }


                }

                else if (list.Count == 1)
                {
                    var contact = list.FirstOrDefault();
                    if (contact.createdonDate < DateTime.Today)
                    {
                        int months = DateTime.Today.Month - contact.createdonDate.Value.Month;
                        if (months < 48)
                        {
                            score = "BRONZE";
                        }
                        else
                            score = "COPPER";
                    }
                }
            }catch(Exception ex)
            {

            }

            return score;
        }
    }


}
