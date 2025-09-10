using System.Collections.Generic;

namespace eTurns.DTO
{
    public class ArticleInfoDTO
    {
        public List<ArticleList> articleList { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
    }

    public class ArticleList
    {
        public string stationCode { get; set; }
        public string articleId { get; set; }
        public string articleName { get; set; }
        public string nfcUrl { get; set; }
        public ArticleData data { get; set; }
        public string generateDate { get; set; }
        public string lastModified { get; set; }
        public List<string> assignedLabel { get; set; }
    }

    public class ArticleData
    {
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Default_Order_Qty { get; set; }
        public string UOM { get; set; }
        public string Loc { get; set; }
        public string BARCODE { get; set; }
        public object DISPLAY_PAGE_1 { get; set; }
        public object DISPLAY_PAGE_2 { get; set; }
        public object DISPLAY_PAGE_3 { get; set; }
        public object DISPLAY_PAGE_4 { get; set; }
        public object DISPLAY_PAGE_5 { get; set; }
        public object DISPLAY_PAGE_6 { get; set; }
        public object DISPLAY_PAGE_7 { get; set; }
        public object NFC_DATA { get; set; }
        public object SKU { get; set; }
        public string ON_ORDER { get; set; }
        public string Shipped { get; set; }
        public string BackOrdered { get; set; }
        public string Exception { get; set; }
    }

    public class Article
    {
        //public string stationCode { get; set; }
        public string articleId { get; set; }
        //public string articleName { get; set; }
        public ArticleData data { get; set; }
    }

    
    public class SolumArticle
    {
        public List<Article> dataList { get; set; }
    }

    //public class SolumArticlePostResponse
    //{
    //    public string responseCode { get; set; }
    //    public string responseMessage { get; set; }
    //}
    public class LabelListDTO
    {
        public List<labelList> labelsList { get; set; }
    }
    public class labelList
    {
        public string labelCode { get; set; }
        public List<ArticleListDTO> articleList { get; set; }
    }

    public class ArticleListDTO
    {
        public string articleId { get; set; }
    }

    public class SolumnLabelsDTO
    {
        public LabelListDTO EntireList { get; set; }
        public string ExistingLabels { get; set; }
        public string NonExistingLabels { get; set; }
        public int? AddCount { get; set; }
        public bool? ISBomItem { get; set; }
    }

    public class SolumnLabelAssignDTO
    {
        public string[] articleIdList { get; set; }
        public string labelCode { get; set; }
    }

    public class LabelVerification
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
    }
}
