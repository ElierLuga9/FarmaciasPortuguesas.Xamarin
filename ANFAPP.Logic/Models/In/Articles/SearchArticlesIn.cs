using System;

namespace ANFAPP.Logic.Models.In.Articles
{
    public class SearchArticlesIn
    {
        public string SEARCHSTRING { get; set; }
        public int QTY { get; set; }
        public int CATID { get; set; }
        public int RINICIAL { get; set; }
    }
}
