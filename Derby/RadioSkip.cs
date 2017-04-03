using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace Derby
{
    class RadioSkip : BaseScript
    {
        public RadioSkip()
        {
            //Tick += OnTick;
        }

        private async Task OnTick()
        {
            if (Game.IsControlJustReleased(1, Control.InteractionMenu))
            {
                Function.Call(Hash.SKIP_RADIO_FORWARD, string.Empty);
            }

            await Task.FromResult(0);
        }
    }
}
