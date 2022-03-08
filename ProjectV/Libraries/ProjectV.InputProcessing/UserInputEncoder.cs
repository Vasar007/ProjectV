using System;
using Acolyte.Assertions;

namespace ProjectV.IO.Input
{
    public static class UserInputEncoder
    {
        public static string Encode(string input)
        {
            input.ThrowIfNull(nameof(input));
            return input.Replace(Environment.NewLine, string.Empty);
        }
    }
}
