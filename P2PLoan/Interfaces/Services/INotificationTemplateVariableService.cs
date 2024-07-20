using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;
using P2PLoan.Services;

namespace P2PLoan.Services;

public interface INotificationTemplateVariableService
{
    Task CreateNotificationTemplateVariableAsync();
    Task GetNotificationTemplateVariableByIdAsync();
    Task GetAllNotificationTemplateVariableAsync();
    Task UpdateNotificationTemplateVariableAsync();
    Task DeleteNotificationTemplateVariableAsync();

}