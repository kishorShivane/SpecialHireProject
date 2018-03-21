using EbusDataProvider;
using SpecialHire.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace SpecialHire.Utilities
{
    public class EmailHelper
    {
        public CompanyConfigurationInfo ConfigurationSettings
        {
            get
            {
                return (HttpContext.Current.Session["CONFIGURATION"] != null) ? (CompanyConfigurationInfo)(CompanyConfigurationInfo)HttpContext.Current.Session["CONFIGURATION"] : new CompanyConfigurationInfo();
            }
        }

        public void SendQuotationConfirmation(BookingQuoteInfoModel bookingQuoteInfoModal, CompanyConfigurationInfo ConfigurationSettings)
        {
            try
            {
                if (bookingQuoteInfoModal.EmailAddress != null && bookingQuoteInfoModal.EmailAddress != string.Empty )
                {
                    MailAddress mailfrom = null;
                    SmtpClient smtp = null;
                    if (ConfigurationManager.AppSettings["Gmail"].ToString() == "true")
                    {
                        smtp = new SmtpClient(ConfigurationManager.AppSettings["GmailSMTP"], Convert.ToInt32(ConfigurationManager.AppSettings["GmailSMTPPort"]));
                        smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["GmailUserName"], ConfigurationManager.AppSettings["GmailPassword"]);
                        smtp.EnableSsl = true;
                        mailfrom = new MailAddress(ConfigurationManager.AppSettings["GmailUserName"]);
                    }
                    else {
                        smtp = new SmtpClient(ConfigurationSettings.EmailSMTP, Convert.ToInt32(ConfigurationSettings.EmailPort));
                        smtp.Credentials = new NetworkCredential(ConfigurationSettings.EmailUserName, ConfigurationSettings.EmailPassword);
                        smtp.EnableSsl = false;
                        mailfrom = new MailAddress(ConfigurationSettings.EmailUserName);
                    }

                    MailAddress mailto = new MailAddress(bookingQuoteInfoModal.EmailAddress);
                    MailMessage newmsg = new MailMessage(mailfrom, mailto);

                    newmsg.Subject = ConfigurationManager.AppSettings["QuotationEmailSubject"].Replace("##QuotationNumber##", bookingQuoteInfoModal.AlternateID);
                    newmsg.IsBodyHtml = true;
                    newmsg.Body = GetQuotationEmailBody(bookingQuoteInfoModal, ConfigurationSettings);

                    //For File Attachment, more file can also be attached
                    Attachment att = new Attachment(ConfigurationManager.AppSettings["QuotationPDF"] + bookingQuoteInfoModal.QuotationFileName);
                    newmsg.Attachments.Add(att);

                    smtp.Send(newmsg);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, ConfigurationManager.AppSettings["ExceptionLogPath"]);
            }
        }

        public void SendInvoiceConfirmation(BookingQuoteInfoModel bookingQuoteInfoModal, CompanyConfigurationInfo ConfigurationSettings)
        {
            try
            {
                if (bookingQuoteInfoModal.EmailAddress != null && bookingQuoteInfoModal.EmailAddress != string.Empty)
                {
                    MailAddress mailfrom = null;
                    SmtpClient smtp = null;
                    if (ConfigurationManager.AppSettings["Gmail"].ToString() == "true")
                    {
                        smtp = new SmtpClient(ConfigurationManager.AppSettings["GmailSMTP"], Convert.ToInt32(ConfigurationManager.AppSettings["GmailSMTPPort"]));
                        //smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["GmailUserName"], ConfigurationManager.AppSettings["GmailPassword"]);
                        smtp.EnableSsl = true;
                        mailfrom = new MailAddress(ConfigurationManager.AppSettings["GmailUserName"]);
                    }
                    else {
                        smtp = new SmtpClient(ConfigurationSettings.EmailSMTP, Convert.ToInt32(ConfigurationSettings.EmailPort));
                        smtp.Credentials = new NetworkCredential(ConfigurationSettings.EmailUserName, ConfigurationSettings.EmailPassword);
                        smtp.EnableSsl = false;
                        mailfrom = new MailAddress(ConfigurationSettings.EmailUserName);
                    }

                    MailAddress mailto = new MailAddress(bookingQuoteInfoModal.EmailAddress);
                    MailMessage newmsg = new MailMessage(mailfrom, mailto);

                    newmsg.Subject = ConfigurationManager.AppSettings["InvoiceEmailSubject"].Replace("##InvoiceNumber##", bookingQuoteInfoModal.BookingInfo.AlternateID.ToString()); ;
                    newmsg.IsBodyHtml = true;
                    newmsg.Body = GetInvoiceEmailBody(bookingQuoteInfoModal, ConfigurationSettings);

                    //For File Attachment, more file can also be attached
                    Attachment att = new Attachment(ConfigurationManager.AppSettings["InvoicePDF"] + bookingQuoteInfoModal.BookingInfo.InvoiceFileName);
                    newmsg.Attachments.Add(att);

                    smtp.Send(newmsg);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, ConfigurationManager.AppSettings["ExceptionLogPath"]);
            }
        }

        public string GetQuotationEmailBody(BookingQuoteInfoModel bookingQuoteInfoModal, CompanyConfigurationInfo ConfigurationSettings)
        {
            //string body = "<html><body style='color:black; font-size:15px;'><font face='Helvetica, Arial, sans-serif'><div style='padding:30px; margin-top:30px;'><p>Dear ##Username##,</p>Good Day, Thank you for booking with eBus Special Hire.<br>Please find the attachment for the Quotation write/call back to us for any clarification<br><br>Thank you,<br>Ebus Supplies C C<br>33 Judges Avenue,<br>Tel: 011 476 5400<br>Fax: 086 554 2482<br></div></body></html>";
            string body = ConfigurationSettings.QuotationEmailTemplate;
            body = body.Replace("##Username##", bookingQuoteInfoModal.FirstName);
            return body;
        }

        public string GetInvoiceEmailBody(BookingQuoteInfoModel bookingQuoteInfoModal, CompanyConfigurationInfo ConfigurationSettings)
        {
            //string body = "<html><body style='color:black; font-size:15px;'><font face='Helvetica, Arial, sans-serif'><div style='padding:30px; margin-top:30px;'><p>Dear ##Username##,</p>Good Day, Thank you for booking with eBus Special Hire.<br>Please find the attachment for the Invoice write/call back to us for any clarification<br><br>Thank you,<br>Ebus Supplies C C<br>33 Judges Avenue,<br>Tel: 011 476 5400<br>Fax: 086 554 2482<br><p></p></div></body></html>";
            string body = ConfigurationSettings.InvoiceEmailTemplate;
            body = body.Replace("##Username##", bookingQuoteInfoModal.FirstName);
            return body;
        }
    }
}