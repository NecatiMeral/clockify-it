using Volo.Abp.Threading;

namespace Sg.ClockifyIt;

public static class ClockifyItDtoExtensions
{
    private static readonly OneTimeRunner s_oneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        s_oneTimeRunner.Run(() =>
        {
            /* You can add extension properties to DTOs
             * defined in the depended modules.
             *
             * Example:
             *
             * ObjectExtensionManager.Instance
             *   .AddOrUpdateProperty<IdentityRoleDto, string>("Title");
             *
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Object-Extensions
             */
        });
    }
}
