using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DeepSeekChat.Core.Network.Json.Naming;

/// <summary>
/// Camel-case naming mode base.
/// </summary>
public partial class CamelCaseBase : NamingPolicy
{
    /// <summary>
    /// Word pattern
    /// </summary>
    public readonly struct CamelCasePattern
    {
        /// <summary>
        /// The first character of word.
        /// </summary>
        public readonly char FirstCharacter { get; }

        /// <summary>
        /// The content before first character
        /// </summary>
        public readonly string Body { get; }

        /// <summary>
        /// Initializer for <see cref="CamelCasePattern"/>.
        /// </summary>
        /// <param name="firstCharacter">The first character of word.</param>
        /// <param name="body">The content before first character</param>
        public CamelCasePattern(char firstCharacter, string body)
        {
            FirstCharacter = firstCharacter;
            Body = body;
        }

        /// <summary>
        /// Get word of current pattern
        /// </summary>
        /// <returns>A camel-case naming word.</returns>
        public override string ToString()
        {
            return string.Concat(FirstCharacter, Body);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is CamelCasePattern pattern &&
                   FirstCharacter == pattern.FirstCharacter &&
                   Body == pattern.Body;
        }

        public static bool operator ==(CamelCasePattern left, CamelCasePattern right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CamelCasePattern left, CamelCasePattern right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstCharacter, Body);
        }
    }

    public override string GetTransformedName(string rawName)
    {
        string[] patterns = GetParttensOfString().Matches(rawName).Select(x => x.Value).ToArray();
        StringBuilder sb = new();
        List<CamelCasePattern> resultPatterns = [];

        foreach (string pattern in patterns)
        {
            CamelCasePattern structured = new(GetFirstCharacter(pattern[0], false), pattern[1..]);
            resultPatterns.Add(structured);
        }

        foreach (var pattern in resultPatterns)
        {
            if(pattern.FirstCharacter == '_')
            {
                sb.Append(pattern.Body);
            }
            else
            {
                sb.Append(pattern);
            }

            if(!pattern.Equals(resultPatterns.Last()) && !pattern.ToString().EndsWith('_'))
            {
                sb.Append('_');
            }
        }
        return sb.ToString();
    }
    public override string GetUntransformedName(string rawName)
    {
        string[] patterns = rawName.Split('_');
        StringBuilder sb = new();
        List<CamelCasePattern> resultPatterns = new();

        foreach (string pattern in patterns)
        {
            CamelCasePattern structured = new(GetFirstCharacter(pattern[0], true), pattern[1..]);
            resultPatterns.Add(structured);
        }

        sb.AppendJoin('_', resultPatterns);
        return sb.ToString();
    }

    /// <summary>
    /// When a class drived, get word first character.
    /// </summary>
    /// <param name="character">A character</param>
    /// <param name="reverse">A flag, indicates whether the opposite should be done.</param>
    /// <returns>If it is false, get chararacter for after renamed; otherwise, before renamed.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual char GetFirstCharacter(char character, bool reverse) => throw new InvalidOperationException();
    [GeneratedRegex("[A-Z]+(?![a-z])|[A-Z][a-z]*|\\d+|[^a-zA-Z0-9]+")]
    private static partial Regex GetParttensOfString();
}
