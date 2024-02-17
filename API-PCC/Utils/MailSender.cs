﻿using API_PCC.Models;
using MimeKit;
using MailKit.Net.Smtp;
using static API_PCC.Controllers.UserController;

namespace API_PCC.Utils
{
    public class MailSender
    {

        public async void sendOtpMail (TblRegistrationOtpmodel data)
        {

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("lbarasona@vertere-gs.com", "lbarasona@vertere-gs.com"));
                message.To.Add(new MailboxAddress("learijohnb@gmail.com", data.Email));
                message.Subject = "OTP";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = @"<!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
                    <title></title>
                </head>
                <style>
                    @font-face {
                    font-family: 'Montserrat-Reg';
                    src: 
                    url('{{ config('app.url') }}/assets/fonts/Montserrat/Montserrat-Regular.ttf');
                    }
                    @font-face {
                        font-family: 'Montserrat-SemiBold';
                        src: url('{{ config('app.url') }}/assets/fonts/Montserrat/Montserrat-SemiBold.ttf');
                    }
                    body{
                        display: flex;
                        flex-direction: column;
                        font-family: 'Montserrat-Reg';
                    }
                    h3{
                        width: 400px;
                        text-align: center;
                        margin:20px auto;
                    }
                    h2{
                        width: 400px;
                        text-align: center;
                        margin:20px auto;
                    }
                    p{
                        width: 400px;
                        margin:10px auto;
                    }
                </style>
                <body>
                    <h3>OTP</h3>
                    <p>We received a request to generate OTP for your account. If you did not initiate this request, please ignore this email.</p>
                    <p>Here is your OTP: <h2>" + data.Otp + "</h2></p>" +
                    "<p>If you have any issues with resetting your password or need further assistance, please contact our support team at <b>(support email here)</b>.</p>" +
                "</body> " +
                "</html>";
                message.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("lbarasona@vertere-gs.com", "LerVer123456#");
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async void sendForgotPasswordMail(JWTokenModel data)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(data.Email);
                string email = System.Convert.ToBase64String(plainTextBytes);

                var emailsend = "" + email;
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("lbarasona@vertere-gs.com", "lbarasona@vertere-gs.com"));
                message.To.Add(new MailboxAddress("", data.Email));
                message.Subject = "Reset Password";
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = @"<!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <meta http-equiv=""X-UA-Compatible"" content=""ie=edge"">
                    <title></title>
                </head>
                <style>
                    @font-face {
                    font-family: 'Montserrat-Reg';
                    src: 
                    url('{{ config('app.url') }}/assets/fonts/Montserrat/Montserrat-Regular.ttf');
                    }
                    @font-face {
                        font-family: 'Montserrat-SemiBold';
                        src: url('{{ config('app.url') }}/assets/fonts/Montserrat/Montserrat-SemiBold.ttf');
                    }
                    body{
                        display: flex;
                        flex-direction: column;
                        font-family: 'Montserrat-Reg';
                    }
                    .img-container {
                        width: 200px;
                        margin:0 auto;
                    }
                    h3{
                        width: 400px;
                        text-align: center;
                        margin:20px auto;
                    }
                    p{
                        width: 400px;
                        margin:10px auto;
                    }
                </style>
                <body>
                    <h3>Reset Password</h3>
                    <p>We received a request to reset the password for your account. If you did not initiate this request, please ignore this email.</p>
                    <p>To reset your password, please click the following link:<a href=" + emailsend + ">" + emailsend + "</a>. This link will be valid for the next 24 hours.</p>" +
                    "<p>If you have any issues with resetting your password or need further assistance, please contact our support team at <b>(support email here)</b>.</p>" +
                "</body> " +
                "</html>";
                message.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("lbarasona@vertere-gs.com", "LerVer123456#");
                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
