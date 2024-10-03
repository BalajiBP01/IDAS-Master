using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Inspirit.IDAS.Data;
using Inspirit.IDAS.WebApplication.Model;

namespace Inspirit.IDAS.WebApplication
{
    public class NewsService
    {
        IDASDbContext _context;
        public NewsService(IDASDbContext context)
        {
            _context = context;
        }

        public List<NewsBlog> GetNewsData()
        {
            List<NewsBlog> newslst = new List<NewsBlog>();
            List<News> lst = new List<News>();
            try
            {
                var keywords = _context.Keywords.Select(t=>t.Name).ToList();
                lst = _context.News.ToList();
                foreach (var nw in lst)
                {
                    string contentfinal = string.Empty;
                    NewsBlog news = new NewsBlog();
                    int index = nw.Content.IndexOf("&lt;endsummary&gt;");
                    string contentremove = nw.Content.Substring(index, nw.Content.Length - index);
                    contentfinal = nw.Content.Replace(contentremove, "");
                    if (contentfinal.Contains("&lt;endsummary&gt;"))
                    {
                        news.Content = contentfinal.Replace("&lt;/summary&gt;", "");
                    }
                    if (contentfinal.Contains("&lt;startsummary&gt;"))
                    {
                        news.Content = contentfinal.Replace("&lt;startsummary&gt;", "");
                    }
                    news.date = nw.CreatedDate;
                    news.Id = nw.Id;
                    news.keywords = keywords;
                    newslst.Add(news);
                }
                newslst = newslst.OrderByDescending(t => t.date).ToList();
            }
            catch (Exception ex)
            {

            }
            return newslst;
        }

        public NewsBlog GetBlogDetail(Guid Id)
        {
            News newsdet = new News();
            NewsBlog blog = new NewsBlog();
            newsdet = _context.News.Where(t => t.Id == Id).FirstOrDefault();
            blog.Id = newsdet.Id;
            
            if (newsdet.Content.Contains("&lt;endsummary&gt;"))
            {
                newsdet.Content= newsdet.Content.Replace("&lt;endsummary&gt;", "");
            }
            if (newsdet.Content.Contains("&lt;startsummary&gt;"))
            {
                newsdet.Content= newsdet.Content.Replace("&lt;startsummary&gt;", "");
            }
            blog.keywords = _context.Keywords.Select(t=>t.Name).ToList();
            blog.Content = newsdet.Content;
            blog.date = newsdet.CreatedDate;
            return blog;
        }

        public List<NewsBlog> GetKeywords(string key)
        {
            List<NewsBlog> newsdet = new List<NewsBlog>();
            var blogs = _context.News.Where(t => t.Content.Contains(key)).ToList();
            foreach (var nw in blogs) {
                NewsBlog news = new NewsBlog();
                news.Id = nw.Id;
                if (nw.Content.Contains("&lt;endsummary&gt;"))
                {
                    nw.Content = nw.Content.Replace("&lt;endsummary&gt;", "");
                }
                if (nw.Content.Contains("&lt;startsummary&gt;"))
                {
                    nw.Content = nw.Content.Replace("&lt;startsummary&gt;", "");
                }
                news.Content = nw.Content;
                news.date = nw.CreatedDate;
                news.keywords = _context.Keywords.Select(t=>t.Name).ToList();
                newsdet.Add(news);
            }
            newsdet = newsdet.OrderByDescending(t => t.date).ToList();
            return newsdet;
        }

    }
}
