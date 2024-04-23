using MediatR;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Contracts
{
    public class QuizzesQueryParams : IParsable<QuizzesQueryParams>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }

        public static QuizzesQueryParams Parse(string s, IFormatProvider? provider)
        {
            var parts = s.Split(',');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid format");
            }

            if (!int.TryParse(parts[0], out var offset) || !int.TryParse(parts[1], out var limit))
            {
                throw new FormatException("Invalid format");
            }

            return new QuizzesQueryParams
            {
                Offset = offset,
                Limit = limit
            };
        }

        public static bool TryParse(string? s, IFormatProvider? provider, out QuizzesQueryParams result)
        {
            result = null;
            if (s == null)
            {
                return false;
            }

            var parts = s.Split(',');
            if (parts.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(parts[0], out var offset) || !int.TryParse(parts[1], out var limit))
            {
                return false;
            }

            result = new QuizzesQueryParams
            {
                Offset = offset,
                Limit = limit
            };

            return true;
        }
    }
}