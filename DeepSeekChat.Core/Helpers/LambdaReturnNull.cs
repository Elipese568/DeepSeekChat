using System;

namespace DeepSeekChat.Core.Helpers;

public static class LambdaReturnDefault
{
    public static T ReturnDefault<T>(Action single)
    {
        single();
        return default;
    }
}
