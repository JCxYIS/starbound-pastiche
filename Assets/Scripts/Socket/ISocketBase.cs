using System;

interface ISocketBase : IDisposable
{
    void Send(string message);
}