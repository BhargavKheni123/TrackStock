using System;

namespace eTurns.DTO.LabelPrinting
{
    public class LabelTemplateMasterDTO
    {
        public Int64 ID { get; set; }

        public Int64 TemplateID { get; set; }

        public Int64? CompanyID { get; set; }

        public String TemplateName { get; set; }

        public String LabelSize { get; set; }

        public Int32 NoOfLabelPerSheet { get; set; }

        public Int32 NoOfColumns { get; set; }

        public Double PageWidth { get; set; }

        public Double PageHeight { get; set; }

        public Double LabelWidth { get; set; }

        public Double LabelHeight { get; set; }

        public Double PageMarginLeft { get; set; }

        public Double PageMarginRight { get; set; }

        public Double PageMarginTop { get; set; }

        public Double PageMarginBottom { get; set; }

        public Double LabelSpacingHorizontal { get; set; }

        public Double LabelSpacingVerticle { get; set; }

        public Double LabelPaddingLeft { get; set; }

        public Double LabelPaddingRight { get; set; }

        public Double LabelPaddingTop { get; set; }

        public Double LabelPaddingBottom { get; set; }

        public Int32 LabelType { get; set; }

        public string RoomName { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public string TemplateNameWithSize { get; set; }

        public int TotalRecords { get; set; }
    }


}


