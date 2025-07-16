using System;

namespace DeepSeekChat.Core.Network.Body;

public readonly struct ResponseBodyInfo
{
    public string RawString { get; }
    public int StatusCode { get; }

    public object ResultDto { get; }
    public Type DtoType { get; }

    public ResponseBodyInfo(string rawString, int statusCode, object resultDto)
    {
        RawString = rawString;
        StatusCode = statusCode;
        ResultDto = resultDto;
        DtoType = resultDto.GetType();
    }

    public bool IsError => StatusCode != 200;
}
