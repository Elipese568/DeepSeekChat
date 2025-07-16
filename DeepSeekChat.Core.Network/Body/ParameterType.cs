using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Core.Network.Body;

/// <summary>
/// Types of parameters that can be added to requests
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// A <see cref="Parameter"/> that will added to the QueryString for GET, DELETE, OPTIONS and HEAD requests; and form for POST and PUT requests.
    /// </summary>
    /// <remarks>
    /// See <see cref="GetOrPostParameter"/>.
    /// </remarks>
    GetOrPost,

    /// <summary>
    /// A <see cref="Parameter"/> that will be added to part of the url by replacing a <c>{placeholder}</c> within the absolute path.
    /// </summary>
    /// <remarks>
    /// See <see cref="UrlSegmentParameter"/>.
    /// </remarks>
    UrlSegment,

    /// <summary>
    /// A <see cref="Parameter"/> that will be added as a request header
    /// </summary>
    /// <remarks>
    /// See <see cref="HeaderParameter"/>.
    /// </remarks>
    HttpHeader,

    /// <summary>
    /// A <see cref="Parameter"/> that will be added to the request body
    /// </summary>
    /// <remarks>
    /// See <see cref="BodyParameter"/>.
    /// </remarks>
    RequestBody,

    /// <summary>
    /// A <see cref="Parameter"/> that will be added to the query string
    /// </summary>
    /// <remarks>
    /// See <see cref="QueryParameter"/>.
    /// </remarks>
    QueryString
}
