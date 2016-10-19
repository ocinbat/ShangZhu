using ShangZhu.Dtos;

namespace ShangZhu.Responses
{
    internal class GetApplicationSettingResponse
    {
        public ApplicationSettingDto Setting { get; set; }

        public bool HasError { get; set; }
    }
}