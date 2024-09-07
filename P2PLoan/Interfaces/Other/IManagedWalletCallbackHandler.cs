using System;

namespace P2PLoan.Interfaces;

public interface IManagedWalletCallbackHandler
{
    void Subscribe(IMonnifyService processor);

}
