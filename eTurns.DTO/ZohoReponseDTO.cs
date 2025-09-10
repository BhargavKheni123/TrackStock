using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTurns.DTO
{
    public class ZohoReponseDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public long ID { get; set; }
        public bool IsStarted { get; set; }
        public Nullable<System.DateTime> TimeStarted { get; set; }
        public bool IsCompleted { get; set; }
        public Nullable<System.DateTime> TimeCompleted { get; set; }
        public bool IsException { get; set; }
        public Nullable<System.DateTime> TimeException { get; set; }
        public string ErrorException { get; set; }
        public System.DateTime Created { get; set; }
        public System.DateTime Updated { get; set; }
        public long? EnterpriseID { get; set; }
        public long? CompanyID { get; set; }
        public long? RoomID { get; set; }
        public long? RoleId { get; set; }
        public long? UserId { get; set; }
        public string RespData { get; set; }
        public SubscriptionInfo subscriptionInfo { get; set; }
        public Subscription subscription { get; set; }
        public bool isValid { get; set; }
    }
    public class BillingAddress
    {
        public string zip { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string attention { get; set; }
        public string street2 { get; set; }
        public string state { get; set; }
        public string fax { get; set; }
    }
    public class Contactperson
    {
        public string zcrm_contact_id { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string email { get; set; }
        public string contactperson_id { get; set; }
    }
    public class ZohoCustomer
    {
        public string website { get; set; }
        public string zcrm_account_id { get; set; }
        public List<object> custom_fields { get; set; }
        public string channel_reference_id { get; set; }
        public string last_name { get; set; }
        public BillingAddress billing_address { get; set; }
        public string ip_address { get; set; }
        public string display_name { get; set; }
        public int payment_terms { get; set; }
        public string channel_customer_id { get; set; }
        public string zcrm_contact_id { get; set; }
        public string company_name { get; set; }
        public CustomFieldHash custom_field_hash { get; set; }
        public string salutation { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public string customer_id { get; set; }
        public string first_name { get; set; }
        public string email { get; set; }
        public string payment_terms_label { get; set; }
    }
    public class CustomFieldHash
    {
    }
    public class Data
    {
        public Subscription subscription { get; set; }
    }
    public class PaymentGateway
    {
        public string payment_gateway { get; set; }
        public string gateway_name { get; set; }
    }
    public class Plan
    {
        public int setup_fee { get; set; }
        public int quantity { get; set; }
        public string tax_name { get; set; }
        public string setup_fee_tax_percentage { get; set; }
        public string plan_code { get; set; }
        public string description { get; set; }
        public int discount { get; set; }
        public List<object> item_custom_fields { get; set; }
        public string tax_id { get; set; }
        public string setup_fee_tax_id { get; set; }
        public int total { get; set; }
        public string unit { get; set; }
        public string setup_fee_tax_name { get; set; }
        public string tax_type { get; set; }
        public int price { get; set; }
        public string name { get; set; }
        public int tax_percentage { get; set; }
        public string setup_fee_tax_type { get; set; }
        public string pricing_scheme { get; set; }
        public string plan_id { get; set; }
    }
    public class SubscriptionInfo
    {
        public DateTime created_time { get; set; }
        public string event_id { get; set; }
        public string event_type { get; set; }
        public Data data { get; set; }
        public string event_time_formatted { get; set; }
        public string event_source { get; set; }
        public string event_time { get; set; }
    }
    public class ShippingAddress
    {
        public string zip { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string attention { get; set; }
        public string street2 { get; set; }
        public string state { get; set; }
        public string fax { get; set; }
    }
    public class Subscription
    {
        public string invoice_notes { get; set; }
        public string next_shipment_day { get; set; }
        public DateTime updated_time { get; set; }
        public List<object> notes { get; set; }
        public string zcrm_potential_id { get; set; }
        public string reference_id { get; set; }
        public int trial_extended_count { get; set; }
        public List<object> addons { get; set; }
        public string next_billing_at { get; set; }
        public List<object> taxes { get; set; }
        public string coupon_duration { get; set; }
        public string subscription_id { get; set; }
        public string last_modified_by_id { get; set; }
        public bool end_of_term { get; set; }
        public string product_id { get; set; }
        public Plan plan { get; set; }
        public string pricebook_id { get; set; }
        public DateTime created_time { get; set; }
        public string shipping_interval_unit { get; set; }
        public int exchange_rate { get; set; }
        public bool is_inclusive_tax { get; set; }
        public int orders_remaining { get; set; }
        public List<object> custom_fields { get; set; }
        public List<object> auto_apply_credits { get; set; }
        public string product_name { get; set; }
        public string activated_at { get; set; }
        public bool is_metered_billing { get; set; }
        public string name { get; set; }
        public string zcrm_potential_name { get; set; }
        public int interval { get; set; }
        public string created_by_id { get; set; }
        public string crm_owner_id { get; set; }
        public string status { get; set; }
        public List<object> items_associated { get; set; }
        public string billing_mode { get; set; }
        public string channel_reference_id { get; set; }
        public string created_at { get; set; }
        public int shipping_interval { get; set; }
        public int payment_terms { get; set; }
        public string currency_code { get; set; }
        public bool can_prorate { get; set; }
        public int trial_remaining_days { get; set; }
        public string expires_at { get; set; }
        public string interval_unit { get; set; }
        public string end_of_term_scheduled_date { get; set; }
        public bool can_add_bank_account { get; set; }
        public CustomFieldHash custom_field_hash { get; set; }
        public bool is_advance_invoice_present { get; set; }
        public string tax_rounding { get; set; }
        public string start_date { get; set; }
        public string next_shipment_at { get; set; }
        public List<PaymentGateway> payment_gateways { get; set; }
        public int amount { get; set; }
        public int remaining_billing_cycles { get; set; }
        public string subscription_number { get; set; }
        public string trial_starts_at { get; set; }
        public string currency_symbol { get; set; }
        public string current_term_starts_at { get; set; }
        public string current_term_ends_at { get; set; }
        public int total_orders { get; set; }
        public string salesperson_name { get; set; }
        public List<object> pending_addons { get; set; }
        public string salesperson_id { get; set; }
        public List<Contactperson> contactpersons { get; set; }
        public bool auto_collect { get; set; }
        public string channel_source { get; set; }
        public int sub_total { get; set; }
        public string trial_ends_at { get; set; }
        public string created_date { get; set; }
        public bool allow_partial_payments { get; set; }
        public int orders_created { get; set; }
        public string customer_id { get; set; }
        public string payment_terms_label { get; set; }
        public ZohoCustomer customer { get; set; }
    }

    public enum ModuleList
    {
        Requisitions = 66,
        WorkOrders = 67,
        Orders = 69
    }
}
