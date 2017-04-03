using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System.Threading.Tasks;

namespace Derby
{
    class OpticalStuff : BaseScript
    {
        public OpticalStuff()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            Function.Call(Hash.NETWORK_OVERRIDE_CLOCK_TIME, 0, 0, 0);
            World.Weather = Weather.ExtraSunny;

            Screen.Hud.IsRadarVisible = false;

            await Task.FromResult(0);
        }
    }
}
