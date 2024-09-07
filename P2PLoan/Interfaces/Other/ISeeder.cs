using System;
using System.Threading.Tasks;

namespace P2PLoan.Interfaces;

public interface ISeeder
{
    Task up();
    Task down();

    string Description();
}
