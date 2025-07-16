using System;

namespace DeepSeekChat.Core.AI;

public class ApiKeyCredential : IReadonlyConvertable<ApiKeyCredential>
{
    private class ApiKeyCredentialReadonly : Readonly<ApiKeyCredential>
    {
        public ApiKeyCredentialReadonly(ApiKeyCredential value) : base(value)
        {
            ApiKey = value.ApiKey ?? throw new ArgumentNullException(nameof(value), "API key cannot be null.");
        }

        public readonly string ApiKey;

        public override object GetValue(string memberName)
        {
            if(memberName == null) throw new MissingMemberException(nameof(memberName), "Member name cannot be null.");
            if (memberName == nameof(ApiKey))
            {
                return ApiKey;
            }
            throw new MissingMemberException(memberName, "Member not found in readonly ApiKeyCredential.");
        }
    }

    public static readonly ReadonlyProperty<string, ApiKeyCredential> ApiKeyProperty = ReadonlyProperty<string, ApiKeyCredential>.Create(nameof(ApiKey));

    /// <summary>
    /// The API key used for authentication.
    /// </summary>
    public string ApiKey { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyCredential"/> class.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    public ApiKeyCredential(string apiKey)
    {
        ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public Readonly<ApiKeyCredential> AsReadonly()
    {
        return new ApiKeyCredentialReadonly(this);
    }
}
