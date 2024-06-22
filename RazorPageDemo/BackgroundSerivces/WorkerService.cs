using System.Collections.Concurrent;
using System.Net.Mail;
using System.Net;
using ViewModel;
using DataModel.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using RazorPage.Hubs;

public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly CustomerViewModel _customerViewModel = new CustomerViewModel();
    private readonly IHubContext<SignalRServer> _signalRHub;
    private DateTime _startDate;
    private DateTime _endDate;
    public WorkerService(IConfiguration configuration, ILogger<WorkerService> logger, 
        IHubContext<SignalRServer> signalRHub)
    {
        _configuration = configuration;
        _logger = logger;
        _signalRHub = signalRHub;
    }

    public async Task ProcessData(DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExecuteAsync started.");

        string MailServer = _configuration["EmailSettings:MailServer"];
        string FromEmail = _configuration["EmailSettings:FromEmail"];
        string Password = _configuration["EmailSettings:Password"];
        int Port = int.Parse(_configuration["EmailSettings:MailPort"]);
        string Subject = "Verify mail";

        _logger.LogInformation("Email configuration loaded. MailServer: {MailServer}, FromEmail: {FromEmail}, Port: {Port}", 
            MailServer, FromEmail, Port);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking for registered users and emails to send.");

                // Simulating a delay for the periodic task
                await Task.Delay(TimeSpan.FromMinutes(0.3), stoppingToken);

                List<Customer> registerUser = await _customerViewModel.GetRegisterCustomer();
                _logger.LogInformation("Retrieved {UserCount} registered users.", registerUser.Count);

                //if (_startDate != null && _endDate != null)
                //{
                //    List<RoomViewModel> roomViewModels = await _customerViewModel.GetAvailableRooms(_startDate, _endDate);
                //    await _signalRHub.Clients.All.SendAsync("LoadRoom");
                //    _logger.LogInformation("Load room");
                //}
                foreach (var user in registerUser)
                {
                    if (user.ExpiredCode == null)
                    {
                        try
                        {
                            using var client = new SmtpClient(MailServer, Port)
                            {
                                Credentials = new NetworkCredential(FromEmail, Password),
                                EnableSsl = true,
                            };

                            var mailMessage = new MailMessage(FromEmail, user.EmailAddress, Subject, user.EmailVerifyCode.ToString())
                            {
                                IsBodyHtml = true
                            };
                            await client.SendMailAsync(mailMessage);
                            _logger.LogInformation("Email sent to: {Email}", user.EmailAddress);
                        }
                        catch (SmtpException smtpEx)
                        {
                            _logger.LogError(smtpEx, "SMTP error occurred while sending email to: {Email}", user.EmailAddress);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "General error occurred while sending email to: {Email}", user.EmailAddress);
                        }
                        user.ExpiredCode = DateTime.Now.AddMinutes(3);
                        await _customerViewModel.UpdateCustomer(user);
                    }

                    if(user.ExpiredCode < DateTime.Now)
                    {
                        _customerViewModel.DeleteCustomer(user);
                        _logger.LogInformation("Deleted user: {Email}", user.EmailAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ExecuteAsync.");
            }
        }
        _logger.LogInformation("ExecuteAsync stopping.");
    }
}