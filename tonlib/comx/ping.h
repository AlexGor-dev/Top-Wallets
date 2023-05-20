#pragma once

#include "headers.h"
#include <WS2tcpip.h>
#include <Windows.h>
#include <iphlpapi.h>
#include <IcmpAPI.h>

#pragma comment(lib, "Iphlpapi.lib")
#pragma comment(lib, "Ws2_32.lib")

int static ping(IN_ADDR dest_ip, int timeout)
{
    int res = 1;
    HANDLE icmp_handle = IcmpCreateFile();
    if (icmp_handle != INVALID_HANDLE_VALUE)
    {
        constexpr WORD payload_size = 1;
        unsigned char payload[payload_size]{};

        constexpr DWORD reply_buf_size = sizeof(ICMP_ECHO_REPLY32) + payload_size + 16;
        unsigned char reply_buf[reply_buf_size]{};

        DWORD reply_count = IcmpSendEcho(icmp_handle, dest_ip.S_un.S_addr, payload, payload_size, NULL, reply_buf, reply_buf_size, timeout);

        if (reply_count == 0)
        {
            //auto e = GetLastError();
            //DWORD buf_size = 1000;
            //WCHAR buf[1000];
            //GetIpErrorString(e, buf, &buf_size);
            //// Some documented error codes from IcmpSendEcho docs.
            //switch (e)
            //{
            //case ERROR_INSUFFICIENT_BUFFER:
            //    break;
            //case ERROR_INVALID_PARAMETER:
            //    break;
            //case ERROR_NOT_ENOUGH_MEMORY:
            //    break;
            //case ERROR_NOT_SUPPORTED:
            //    break;
            //case IP_BUF_TOO_SMALL:
            //    break;
            //}
        }
        else
        {
            res = 0;
        }
        IcmpCloseHandle(icmp_handle);
    }
    return res;
}
int static pingIp(const char* ip, int timeout)
{
    IN_ADDR dest_ip{};
    if (1 == InetPtonA(AF_INET, ip, &dest_ip))
        return ping(dest_ip, timeout);
    return 1;
}