using Par.Api.Enums;

namespace Par.Api.Extensions
{
    public static class BoxSizeExtensions
    {
        public static double GetFixedWeight(this BoxSize size)
        {
            return size switch
            {
                BoxSize.Large => 100,
                BoxSize.Medium => 50,
                BoxSize.Small => 35,
                _ => 0
            };
        }
    }
}