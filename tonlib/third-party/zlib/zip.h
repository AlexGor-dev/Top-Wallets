#pragma once

#define ZLIB_INTERNAL
#include <Windows.h>
#include "zlib.h"

z_stream* ZEXPORT InflaterCreate()
{
    z_stream* stream = (z_stream*)LocalAlloc(0, sizeof(z_stream));
    memset(stream, 0, sizeof(z_stream));
    int res = inflateInit2(stream, -MAX_WBITS);
    if (res != 0)
    {
        LocalFree(stream);
        return (z_stream*)res;
    }
    return stream;
}

void ZEXPORT InflaterDispose(z_stream* stream)
{
    LocalFree(stream);
}

int ZEXPORT InflaterInflate(z_stream* stream, Bytef* dest, uInt* destLen, Bytef* source, uInt* sourceLen)
{
    stream->next_in = source;
    stream->avail_in = *sourceLen;
    stream->next_out = dest;
    stream->avail_out = *destLen;
    int res = inflate(stream, Z_NO_FLUSH);
    if(res == Z_STREAM_END)
        inflateEnd(stream);
    *sourceLen = stream->avail_in;
    *destLen = stream->avail_out;
    return res;
}

z_stream* ZEXPORT DeflaterCreate(int level = Z_DEFAULT_COMPRESSION)
{
    z_stream* stream = (z_stream*)LocalAlloc(0, sizeof(z_stream));
    memset(stream, 0, sizeof(z_stream));
    int res = deflateInit2(stream, level, Z_DEFLATED, -MAX_WBITS, 8, Z_DEFAULT_STRATEGY);
    if (res != 0)
    {
        LocalFree(stream);
        return (z_stream*)res;
    }
    return stream;
}

void ZEXPORT DeflaterDispose(z_stream* stream)
{
    LocalFree(stream);
}

int ZEXPORT DeflaterDeflate(z_stream* stream, Bytef* dest, uInt* destLen, Bytef* source, uInt* sourceLen)
{
    stream->next_in = source;
    stream->avail_in = *sourceLen;
    stream->next_out = dest;
    stream->avail_out = *destLen;
    int res = deflate(stream, Z_FINISH);
    if (res == Z_STREAM_END)
        deflateEnd(stream);
    *sourceLen = stream->avail_in;
    *destLen = stream->avail_out;
    return res;
}



