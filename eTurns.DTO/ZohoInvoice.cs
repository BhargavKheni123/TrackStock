using System;
using System.Collections.Generic;

namespace eTurns.DTO
{
    public class GetZohoInvoiceDTO
    {
        public int code { get; set; }
        public string message { get; set; }
        public ZohoInvoice invoice { get; set; }
    }

    public class ZohoInvoice
    {
        public string invoice_id { get; set; }
        public string unbilled_charges_id { get; set; }
        public double exchange_rate { get; set; }
        public string number { get; set; }
        public string invoice_number { get; set; }
        public string invoice_date { get; set; }
        public string date { get; set; }
        public string due_date { get; set; }
        public string reference_number { get; set; }
        public string zcrm_potential_id { get; set; }
        public string payment_expected_date { get; set; }
        public bool can_send_invoice_sms { get; set; }
        public bool stop_reminder_until_payment_expected_date { get; set; }
        public string status { get; set; }
        public bool can_edit_items { get; set; }
        public bool can_skip_payment_info { get; set; }
        public bool inprocess_transaction_present { get; set; }
        public bool ach_payment_initiated { get; set; }
        public bool allow_partial_payments { get; set; }
        public string transaction_type { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public List<object> customer_custom_fields { get; set; }
        public ZohoInvoiceCustomerCustomFieldHash customer_custom_field_hash { get; set; }
        public string email { get; set; }
        public string pricebook_id { get; set; }
        public string currency_id { get; set; }
        public string currency_code { get; set; }
        public string currency_symbol { get; set; }
        public bool is_inclusive_tax { get; set; }
        public string tax_rounding { get; set; }
        public bool is_viewed_by_client { get; set; }
        public string client_viewed_time { get; set; }
        public bool is_viewed_in_mail { get; set; }
        public string mail_first_viewed_time { get; set; }
        public string mail_last_viewed_time { get; set; }
        public List<InvoiceItem> invoice_items { get; set; }
        public List<object> taxes { get; set; }
        public double shipping_charge { get; set; }
        public double adjustment { get; set; }
        public double sub_total { get; set; }
        public double tax_total { get; set; }
        public double discount_total { get; set; }
        public bool payment_reminder_enabled { get; set; }
        public bool auto_reminders_configured { get; set; }
        public double total { get; set; }
        public double discount_percent { get; set; }
        public double bcy_shipping_charge { get; set; }
        public double bcy_adjustment { get; set; }
        public string adjustment_description { get; set; }
        public double bcy_sub_total { get; set; }
        public double bcy_discount_total { get; set; }
        public double bcy_tax_total { get; set; }
        public double bcy_total { get; set; }
        public double payment_made { get; set; }
        public List<object> coupons { get; set; }
        public double unused_credits_receivable_amount { get; set; }
        public double credits_applied { get; set; }
        public List<object> credits { get; set; }
        public int price_precision { get; set; }
        public double balance { get; set; }
        public double write_off_amount { get; set; }
        public List<Payment> payments { get; set; }
        public string salesperson_id { get; set; }
        public string salesperson_name { get; set; }
        public string submitter_id { get; set; }
        public string approver_id { get; set; }
        public List<object> custom_fields { get; set; }
        public ZohoInvoiceCustomFieldHash custom_field_hash { get; set; }
        public DateTime created_time { get; set; }
        public string updated_time { get; set; }
        public string created_date { get; set; }
        public string created_by_id { get; set; }
        public string last_modified_by_id { get; set; }
        public string invoice_url { get; set; }
        public string reference_id { get; set; }
        public ZohoInvoiceAddress billing_address { get; set; }
        public ZohoInvoiceAddress shipping_address { get; set; }
        public List<ZohoInvoiceSubscription> subscriptions { get; set; }
        public string template_type { get; set; }
        public string page_width { get; set; }
        public string template_id { get; set; }
        public string template_name { get; set; }
        public string notes { get; set; }
        public string terms { get; set; }
        public int payment_terms { get; set; }
        public string payment_terms_label { get; set; }
        public List<Contactperson> contactpersons { get; set; }
        public List<PaymentGateway> payment_gateways { get; set; }
        public List<object> documents { get; set; }
        public bool can_send_in_mail { get; set; }
    }

    public class InvoiceItem
    {
        public string item_id { get; set; }
        public string product_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string unit { get; set; }
        public string code { get; set; }
        public string account_id { get; set; }
        public string account_name { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public double discount_amount { get; set; }
        public double item_total { get; set; }
        public string tax_name { get; set; }
        public string tax_type { get; set; }
        public int tax_percentage { get; set; }
        public string tax_id { get; set; }
        //public List<object> tags { get; set; }
        public List<object> item_custom_fields { get; set; }
    }

    //public class PaymentGateway
    //{
    //    public string payment_gateway { get; set; }
    //    public string gateway_name { get; set; }
    //}

    public class Payment
    {
        public string payment_id { get; set; }
        public string payment_mode { get; set; }
        public string card_type { get; set; }
        public string invoice_payment_id { get; set; }
        public double amount_refunded { get; set; }
        public string gateway_transaction_id { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string reference_number { get; set; }
        public double amount { get; set; }
        public double bank_charges { get; set; }
        public double exchange_rate { get; set; }
        public string last_four_digits { get; set; }
        public string settlement_status { get; set; }
    }

    public class ZohoInvoiceSubscription
    {
        public string subscription_id { get; set; }
    }

    public class ZohoInvoiceAddress: BillingAddress
    {
        public string address { get; set; }
    }

    public class ZohoInvoiceCustomerCustomFieldHash
    {
    }

    public class ZohoInvoiceCustomFieldHash
    {
    }

}
