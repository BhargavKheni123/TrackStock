using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class APIRequestInfo
    {
        public string ApiQueryURL { get; set; }
        public string AccessToken { get; set; }
        public string ZohoOrganizationID { get; set; }
        public string RequstBodyJSON { get; set; }
    }

    public class ZOHOAPIPathConfigDTO
    {
        public long ID { get; set; }
        public string ZohoAPISection { get; set; }
        public string ZohoAPIModuleName { get; set; }
        public string ZohoAPIOperation { get; set; }
        public string ZohoAPIPath { get; set; }
        public string RequestType { get; set; }
        public DateTime Created { get; set; }
    }
    public class ZohoDevAppSettingDTO
    {
        public long ID { get; set; }
        public string ZohoOrganizationName { get; set; }
        public string ZohoOrganizationID { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string UserID { get; set; }
        public string RedirectUri { get; set; }
        public string TokenRequestURL { get; set; }
    }
    public class ZohoTokenDetailDTO
    {
        public Int64 ID { get; set; }
        public string ZohoGrantCode { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public Int64 CreatedBy { get; set; }
        public Int64 LastUpdatedBy { get; set; }
        public System.String AddedFrom { get; set; }
        public System.String EditedFrom { get; set; }
    }
    public class ZohoTokenInfo
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string error_description { get; set; }
        public string error { get; set; }
    }


    //public class BillingAddress
    //{
    //    public string attention { get; set; }
    //    public string street { get; set; }
    //    public string city { get; set; }
    //    public string state { get; set; }
    //    public int zip { get; set; }
    //    public string country { get; set; }
    //    public string state_code { get; set; }
    //    public int fax { get; set; }
    //}

    public class ZohoGetCustomer
    {
        public string display_name { get; set; }
        public string company_name { get; set; }
        public string customer_id { get; set; }
        public string contact_name { get; set; }
        public string contact_id { get; set; }
        public string status { get; set; }
        public string customer_sub_type { get; set; }
        public string currency_id { get; set; }
        public bool is_client_review_asked { get; set; }
        public bool is_client_review_settings_enabled { get; set; }
        public string source { get; set; }
        public bool payment_reminder_enabled { get; set; }
        public string language_code { get; set; }
        public string portal_status { get; set; }
        public string owner_id { get; set; }
        public string language_code_formatted { get; set; }
        public bool is_added_in_portal { get; set; }
        public bool can_invite { get; set; }
        public string billing_day { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public int price_precision { get; set; }
        public double unused_credits { get; set; }
        public double outstanding_receivable_amount { get; set; }
        public double outstanding { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string salutation { get; set; }
        public string contact_salutation { get; set; }
        public string ip_address { get; set; }
        public string twitter { get; set; }
        public string facebook { get; set; }
        public string department { get; set; }
        public string designation { get; set; }
        public string skype { get; set; }
        public string fax { get; set; }
        public bool is_portal_invitation_accepted { get; set; }
        public string website { get; set; }
        public string pricebook_id { get; set; }
        public string zcrm_account_id { get; set; }
        public string zcrm_contact_id { get; set; }
        public bool is_sms_enabled { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public string cf_amazon_state { get; set; }
        public string cf_amazon_state_unformatted { get; set; }
        public string cf_amazon_callback_uri { get; set; }
        public string cf_amazon_callback_uri_unformatted { get; set; }
        public CustomerCustomFieldHash custom_field_hash { get; set; }
        public int payment_terms { get; set; }
        public string payment_terms_label { get; set; }
        public bool is_gapps_customer { get; set; }
        public double unused_credits_receivable_amount { get; set; }
        public double unused_credits_receivable_amount_bcy { get; set; }
        public double unused_credits_payable_amount { get; set; }
        public double unused_credits_payable_amount_bcy { get; set; }
        public bool is_linked_with_zohocrm { get; set; }
        public string photo_url { get; set; }
        public string category { get; set; }
        public bool customer_consolidation_preference { get; set; }
        public bool customer_consolidation_applicable { get; set; }
        public string channel_customer_id { get; set; }
        public string channel_source { get; set; }
        public EncryptionKeyMap encryption_key_map { get; set; }
        public ZohoAddress billing_address { get; set; }
        public ZohoAddress shipping_address { get; set; }
        public bool ach_supported { get; set; }
        public string primary_contactperson_id { get; set; }
        public bool is_sms_enabled_for_primary_cp { get; set; }
        public List<object> addresses { get; set; }
        public bool can_add_card { get; set; }
        public bool can_add_bank_account { get; set; }
        public string notes { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public DefaultTemplates default_templates { get; set; }
        public List<object> documents { get; set; }
    }

    public class CustomField
    {
        public string field_id { get; set; }
        public string customfield_id { get; set; }
        public bool show_in_store { get; set; }
        public bool show_in_portal { get; set; }
        public bool is_active { get; set; }
        public int index { get; set; }
        public string label { get; set; }
        public bool show_on_pdf { get; set; }
        public bool edit_on_portal { get; set; }
        public bool edit_on_store { get; set; }
        public string api_name { get; set; }
        public bool show_in_all_pdf { get; set; }
        public string value_formatted { get; set; }
        public string search_entity { get; set; }
        public string data_type { get; set; }
        public string placeholder { get; set; }
        public string value { get; set; }
        public bool is_dependent_field { get; set; }
    }

    public class CustomerCustomFieldHash
    {
        public string cf_amazon_state { get; set; }
        public string cf_amazon_state_unformatted { get; set; }
        public string cf_amazon_callback_uri { get; set; }
        public string cf_amazon_callback_uri_unformatted { get; set; }
    }

    public class DefaultTemplates
    {
        public string invoice_template_id { get; set; }
        public string creditnote_template_id { get; set; }
    }

    public class EncryptionKeyMap
    {
        public string modulus { get; set; }
        public string exponent { get; set; }
    }

    //public class Document
    //{
    //    public bool can_show_in_portal { get; set; }
    //    public string file_name { get; set; }
    //    public string file_type { get; set; }
    //    public int file_size { get; set; }
    //    public string file_size_formatted { get; set; }
    //    public string document_id { get; set; }
    //    public int attachment_order { get; set; }
    //}

    public class GetZohoCustomerDTO
    {
        public int code { get; set; }
        public string message { get; set; }
        public ZohoGetCustomer customer { get; set; }
    }

    public class ZohoAddress
    {
        public string address_id { get; set; }
        public string street { get; set; }
        public string address { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string fax { get; set; }
        public string state_code { get; set; }
        public string phone { get; set; }
        public string attention { get; set; }
    }

    //public class Tag
    //{
    //    public string tag_option_id { get; set; }
    //    public bool is_tag_mandatory { get; set; }
    //    public string tag_name { get; set; }
    //    public string tag_id { get; set; }
    //    public string tag_option_name { get; set; }
    //}


}
