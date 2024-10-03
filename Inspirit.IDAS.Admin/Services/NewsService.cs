using Inspirit.IDAS.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inspirit.IDAS.Admin
{
    public class NewsService
    {
        IDASDbContext _dbContext;
        public NewsService(IDASDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ActionResult<NewsDataTableResponse>> NewsList(DataTableRequest request)
        {
            NewsDataTableResponse response = new NewsDataTableResponse();
            try
            {
                var lst = _dbContext.News.AsQueryable();
                int cnt = _dbContext.News.Count();
                var flt = lst;

                response.recordsTotal = cnt;
                response.recordsFiltered = flt.Count();



                var data = (from s in flt
                            select new NewsData
                            {
                                Id = s.Id,
                                Content = s.Content,
                                CreatedDate = s.CreatedDate,
                                Name = s.BlogName

                            }).ToList();


                response.data = data;

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<News> View(Guid id)
        {
            News data = new News();
            try
            {
                data = _dbContext.News
                    .FirstOrDefault(d => d.Id == id);
            }
            catch (Exception ex) { }
            return data;
        }

        public async Task<NewsCrudResponse> Insert(News data)
        {
            NewsCrudResponse response = new NewsCrudResponse();
            try
            {

                data.Id = Guid.NewGuid();
                data.CreatedDate = DateTime.Now;
                if (data.Content.Contains("<img"))
                {
                    string contentfilter = string.Empty;

                    string regularExpressionPattern2 = @"<div (.*?)</div>";
                    Regex regex2 = new Regex(regularExpressionPattern2, RegexOptions.Singleline);
                    MatchCollection collection2 = regex2.Matches(data.Content);
                    List<Match> m2 = collection2.ToList();
                    foreach (var con2 in m2)
                    {
                        var source = Regex.Replace(con2.ToString(), "(style=.+?\")", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        source = source + "";
                        contentfilter = data.Content.Replace(con2.ToString(), source);
                        data.Content = contentfilter;
                    }





                    string regularExpressionPattern1 = @"<img (.*?)>";
                    Regex regex = new Regex(regularExpressionPattern1, RegexOptions.Singleline);
                    MatchCollection collection = regex.Matches(data.Content);
                    List<Match> m = collection.ToList();
                    foreach (var con in m)
                    {
                        var source = string.Empty;
                        if (con.ToString().Contains("style"))
                        {
                            source = Regex.Replace(con.ToString(), "(style=.+?\">)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                          
                            source = source + "style=\"height: auto; width: 100 %; border: 0; display: block;\"";
                            data.Content.Replace(con.ToString(), source);
                        }
                        else
                        {
                            string contentbig = data.Content;
                            string cont = con.ToString().TrimEnd('>');
                            string s = cont.ToString();
                            source = s + "style=\"height: auto; width: 100% !important; border: 0; display: block;\"";
                            data.Content = contentbig.Replace(s, source);
                        }
                    }
                }
                _dbContext.News.Add(data);
                _dbContext.SaveChanges();
                response.isSuccess = true;
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<NewsCrudResponse> Update(News data)
        {
            NewsCrudResponse response = new NewsCrudResponse();
            try
            {
                if (data.Content.Contains("<img"))
                {
                    string contentfilter = string.Empty;

                    string regularExpressionPattern2 = @"<div (.*?)</div>";
                    Regex regex2 = new Regex(regularExpressionPattern2, RegexOptions.Singleline);
                    MatchCollection collection2 = regex2.Matches(data.Content);
                    List<Match> m2 = collection2.ToList();
                    foreach (var con2 in m2)
                    {
                        var source = Regex.Replace(con2.ToString(), "(style=.+?\")", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        source = source + "";
                        contentfilter = data.Content.Replace(con2.ToString(), source);
                        data.Content = contentfilter;
                    }





                    string regularExpressionPattern1 = @"<img (.*?)>";
                    Regex regex = new Regex(regularExpressionPattern1, RegexOptions.Singleline);
                    MatchCollection collection = regex.Matches(data.Content);
                    List<Match> m = collection.ToList();
                    foreach (var con in m)
                    {
                        var source = string.Empty;
                        if (con.ToString().Contains("style"))
                        {
                            source = Regex.Replace(con.ToString(), "(style=.+?\">)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                          
                            source = source + "style=\"height: auto; width: 100 %; border: 0; display: block;\"";
                            data.Content.Replace(con.ToString(), source);
                        }
                        else
                        {
                            string contentbig = data.Content;
                            string cont = con.ToString().TrimEnd('>');
                             string s = cont.ToString();
                             source = s + "style=\"height: auto; width: 100% !important; border: 0; display: block;\"";
                            data.Content = contentbig.Replace(s, source);
                        }
                    }
                }

                _dbContext.News.Update(data);
                _dbContext.SaveChanges();
                response.isSuccess = true;
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<NewsCrudResponse> Delete(Guid Id)
        {
            NewsCrudResponse response = new NewsCrudResponse();
            try
            {
                var data = _dbContext.News.Where(t => t.Id == Id).FirstOrDefault();
                _dbContext.News.Remove(data);
                _dbContext.SaveChanges();
                response.isSuccess = true;
                response.Message = "";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

    }
    public class NewsDataTableResponse : DataTableResponse<NewsData>
    {

    }
    public class NewsData
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
    }
    public class NewsCrudResponse
    {
        public string Message { get; set; }
        public bool isSuccess { get; set; }
    }

}
