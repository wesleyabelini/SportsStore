using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using System.Net.Mail;
using System.Net;

namespace SportsStore.Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetail)
        {
            using(var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder().AppendLine("A new order has been submitted")
                    .AppendLine("---").AppendLine("Items:");

                foreach(var line in cart.Lines)
                {
                    var subtotal = line.Product.Price * line.Quantity;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue()).AppendLine("---").AppendLine("Ship to:")
                    .AppendLine(shippingDetail.Name).AppendLine(shippingDetail.Line1).AppendLine(shippingDetail.Line2 ?? "")
                    .AppendLine(shippingDetail.Line3 ?? "").AppendLine(shippingDetail.City).AppendLine(shippingDetail.State ?? "")
                    .AppendLine(shippingDetail.Country).AppendLine(shippingDetail.Zip).AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingDetail.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddres,
                    "New order submitted!", body.ToString());

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            }
        }
    }
}

public class EmailSettings
{
    public string MailToAddres = "";
    public string MailFromAddress = "";
    public bool UseSsl = true;
    public string Username = "";
    public string Password = "";
    public string ServerName = "";
    public int ServerPort = 0;
    public bool WriteAsFile = false;
    public string FileLocation = "";
}
