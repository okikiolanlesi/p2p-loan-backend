using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;
using P2PLoan.Services;

namespace P2PLoan.Services;

public interface INotificationTemplateService
{
    Task CreateNotificationAsync();
    Task GetByIdAsync();
    Task GetAllNotificationAsync();
    Task UpdateNotificationAsync();

}