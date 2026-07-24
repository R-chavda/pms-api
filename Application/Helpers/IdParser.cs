using static Domain.Wrappers.AppException;

namespace Application.Helpers
{
    public static class IdParser
    {
        public static long ParseToLong(string keyId)
        {
            if(string.IsNullOrEmpty(keyId))
            {
                throw new BadRequestException("keyId is required");
            }

            if(!long.TryParse(keyId, out var id))
            {
                throw new BadRequestException($"Invalid keyId format {keyId}");
            }
            return id;
        }
    }
}